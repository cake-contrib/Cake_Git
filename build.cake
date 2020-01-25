///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var isPullRequest       = AppVeyor.Environment.PullRequest.IsPullRequest;
var solutions           = GetFiles("./**/*.sln");
var solutionPaths       = solutions.Select(solution => solution.GetDirectory());
var releaseNotes        = ParseReleaseNotes("./ReleaseNotes.md");
var version             = releaseNotes.Version.ToString();
var binDir              = MakeAbsolute(Directory("./src/Cake.Git/bin/" + configuration + "/net461"));
var nugetRoot           = "./nuget/";
var artifactsRoot       = MakeAbsolute(Directory("./artifacts/"));
var semVersion          = isLocalBuild
                                ? version
                                : string.Concat(version, "-build-", AppVeyor.Environment.Build.Number.ToString("0000"));

var assemblyInfo        = new AssemblyInfoSettings {
                                Title                   = "Cake.Git",
                                Description             = "Cake Git AddIn",
                               Product                 = "Cake.Git",
                                Company                 = "WCOM AB",
                                Version                 = version,
                                FileVersion             = version,
                                InformationalVersion    = semVersion,
                                Copyright               = string.Format("Copyright © WCOM AB {0}", DateTime.Now.Year),
                                CLSCompliant            = true
                            };
var nuGetPackSettings   = new NuGetPackSettings {
                                Id                      = assemblyInfo.Product,
                                Version                 = assemblyInfo.InformationalVersion,
                                Title                   = assemblyInfo.Title,
                                Authors                 = new[] {assemblyInfo.Company},
                                Owners                  = new[] {assemblyInfo.Company, "cake-contrib"},
                                Description             = assemblyInfo.Description,
                                Summary                 = "Cake AddIn that extends Cake with Git SCM features",
                                ProjectUrl              = new Uri("https://github.com/cake-contrib/Cake_Git/"),
                                Repository              = new NuGetRepository { Type = "git", Url = "https://github.com/cake-contrib/Cake_Git.git" },
                                IconUrl                 = new Uri("https://cdn.jsdelivr.net/gh/cake-contrib/graphics/png/cake-contrib-medium.png"),
                                License                 = new NuSpecLicense { Type = "expression", Value = "MIT" },
                                Copyright               = assemblyInfo.Copyright,
                                ReleaseNotes            = releaseNotes.Notes.ToArray(),
                                Tags                    = new [] {"Cake", "Script", "Build", "Git"},
                                RequireLicenseAcceptance= false,
                                Symbols                 = false,
                                NoPackageAnalysis       = true,
                                Files                   = new NuSpecContent[0], // is set dynamically
                                BasePath                = artifactsRoot,
                                OutputDirectory         = nugetRoot
                            };

var msBuildSettings     = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", semVersion)
                            .WithProperty("AssemblyVersion", version)
                            .WithProperty("FileVersion", version)
                            .WithProperty("PackageReleaseNotes", string.Concat("\"", string.Concat(releaseNotes.Notes.ToArray()), "\""));

Context.Tools.RegisterFile("./tools/nuget.exe");

if (!isLocalBuild)
{
    AppVeyor.UpdateBuildVersion(semVersion);
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    var buildStartMessage = string.Format(
                            "Building version {0} of {1} ({2}).",
                            version,
                            assemblyInfo.Product,
                            semVersion
                            );

    Information(buildStartMessage);

    if(!IsRunningOnWindows())
    {
        var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        // Use FrameworkPathOverride when not running on Windows.
        Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
        msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }
});

Teardown(ctx =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }

    Information("Cleaning {0}", nugetRoot);
    CleanDirectory(MakeAbsolute(Directory(nugetRoot)));

    Information("Cleaning {0}", artifactsRoot);
    CleanDirectory(artifactsRoot);
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        DotNetCoreRestore(
            solution.FullPath,
            new DotNetCoreRestoreSettings {
                Verbosity = DotNetCoreVerbosity.Minimal,
                MSBuildSettings = msBuildSettings
        });
    }
});

Task("SolutionInfo")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var file = "./src/SolutionInfo.cs";
    CreateAssemblyInfo(file, assemblyInfo);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("SolutionInfo")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        DotNetCoreBuild(
            solution.FullPath,
            new DotNetCoreBuildSettings {
                   Configuration = configuration,
                    NoRestore = true,
                    MSBuildSettings = msBuildSettings
                });
    }
});

Task("Publish-Artifacts")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (!DirectoryExists(artifactsRoot))
    {
        CreateDirectory(artifactsRoot);
    }

    DotNetCorePublish("./src/Cake.Git", new DotNetCorePublishSettings
    {
        NoRestore = true,
        Framework = "net461",
        MSBuildSettings = msBuildSettings,
        OutputDirectory = artifactsRoot + "/net461"
    });

    DotNetCorePublish("./src/Cake.Git", new DotNetCorePublishSettings
    {
        NoRestore = true,
        Framework = "netstandard2.0",
        MSBuildSettings = msBuildSettings,
        OutputDirectory = artifactsRoot + "/netstandard2.0"
    });
});


Task("Create-NuGet-Package")
    .IsDependentOn("Publish-Artifacts")
    .Does(() =>
{
    var native = GetFiles(artifactsRoot.FullPath + "/net461/lib/**/*");
    var cakeGit = GetFiles(artifactsRoot.FullPath + "/**/Cake.Git.dll");
    var cakeGitDoc = GetFiles(artifactsRoot.FullPath + "/**/Cake.Git.xml");
    var libGit = GetFiles(artifactsRoot.FullPath + "/**/LibGit2Sharp*");
    var coreNative = GetFiles(artifactsRoot.FullPath + "/netstandard2.0/runtimes/**/*")
                        - GetFiles(artifactsRoot.FullPath + "/netstandard2.0/runtimes/win-x86/**/*");

    nuGetPackSettings.Files =  (native + libGit + cakeGit + cakeGitDoc)
                                    .Where(file=>!file.FullPath.Contains("Cake.Core.") && !file.FullPath.Contains("/runtimes/"))
                                    .Select(file=>file.FullPath.Substring(artifactsRoot.FullPath.Length+1))
                                    .Select(file=>new NuSpecContent {Source = file, Target = "lib/" + file})
                                    .Union(
                                        coreNative
                                            .Where(file=>file.FullPath.Contains("/linux-x64/") || file.FullPath.Contains("/win-x64/") || file.FullPath.Contains("/osx/"))
                                            .Select(file=>new NuSpecContent {
                                                Source = file.FullPath.Substring(artifactsRoot.FullPath.Length+1),
                                                Target = "lib/netstandard2.0/" + file.GetFilename()
                                                })
                                        )
                                    .ToArray();

    if (!DirectoryExists(nugetRoot))
    {
        CreateDirectory(nugetRoot);
    }
    NuGetPack(nuGetPackSettings);
});

Task("Test")
    .IsDependentOn("Create-NuGet-Package")
    .WithCriteria(() => StringComparer.OrdinalIgnoreCase.Equals(configuration, "Release"))
    .Does(() =>
{
    var package = nugetRoot + "Cake.Git." + semVersion + ".nupkg";
    var addinDir = MakeAbsolute(Directory("./tools/Addins/Cake.Git/Cake.Git"));
    if (DirectoryExists(addinDir))
    {
        DeleteDirectory(addinDir, new DeleteDirectorySettings {
            Recursive = true,
            Force = true
        });
    }
    Unzip(package, addinDir);

    Action executeTests = ()=> {
        CakeExecuteScript("./testnet461.cake",
            new CakeSettings{
                Arguments = new Dictionary<string, string>{
                    {"target", target == "Default" ? "Default-Tests" : "Local-Tests"}}});

          DotNetCoreExecute(
            "./tools/Cake.CoreCLR/Cake.dll",
            string.Concat("testnetstandard2.cake --target=", target == "Default" ? "Default-Tests" : "Local-Tests")
            );
    };

    if (TravisCI.IsRunningOnTravisCI)
    {
        using(TravisCI.Fold("Execute-Tests"))
        {
            executeTests();
            return;
        }
    }

    executeTests();
});



Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    // Resolve the API key.
    var apiKey = EnvironmentVariable("MYGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    var source = EnvironmentVariable("MYGET_SOURCE");
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet source.");
    }

    // Get the path to the package.
    var package = nugetRoot + "Cake.Git." + semVersion + ".nupkg";

    // Push the package.
    NuGetPush(package, new NuGetPushSettings {
        Source = source,
        ApiKey = apiKey
    });
});


Task("Default")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test");

Task("Local-Tests")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test");

Task("AppVeyor")
    .IsDependentOn("Publish-MyGet");

Task("Travis")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
