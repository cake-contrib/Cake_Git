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
var solutions           = GetFiles("./**/Cake.Git.sln");
var solutionPaths       = solutions.Select(solution => solution.GetDirectory());
var releaseNotes        = ParseReleaseNotes("./ReleaseNotes.md");
var version             = releaseNotes.Version.ToString();
var binDir              = MakeAbsolute(Directory("./src/Cake.Git/bin/" + configuration));
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
                                Owners                  = new[] {assemblyInfo.Company},
                                Description             = assemblyInfo.Description,
                                Summary                 = "Cake AddIn that extends Cake with Git SCM features",
                                ProjectUrl              = new Uri("https://github.com/WCOMAB/Cake_Git/"),
                                IconUrl                 = new Uri("http://cdn.rawgit.com/WCOMAB/nugetpackages/master/Chocolatey/icons/wcom.png"),
                                LicenseUrl              = new Uri("https://github.com/WCOMAB/Cake_Git/blob/master/LICENSE"),
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

DotNetCoreMSBuildSettings msBuildSettings = new DotNetCoreMSBuildSettings()
                                            .WithProperty("Version", semVersion)
                                            .WithProperty("AssemblyVersion", version)
                                            .WithProperty("FileVersion", version);

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
        DotNetCoreRestore(solution.FullPath, new DotNetCoreRestoreSettings {
                Verbosity = DotNetCoreVerbosity.Minimal,
                Sources = new [] { "https://api.nuget.org/v3/index.json", "https://dotnet.myget.org/F/dotnet-core/api/v3/index.json" },
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
        DotNetCoreBuild(solution.FullPath, new DotNetCoreBuildSettings {
            Configuration = configuration,
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
        Framework = "net45",
        MSBuildSettings = msBuildSettings,
        OutputDirectory = artifactsRoot + "/net45"
    });

    DotNetCorePublish("./src/Cake.Git", new DotNetCorePublishSettings
    {
        Framework = "netstandard1.6",
        MSBuildSettings = msBuildSettings,
        OutputDirectory = artifactsRoot + "/netstandard1.6"
    });
});

Task("Create-NuGet-Package")
    .IsDependentOn("Publish-Artifacts")
    .Does(() =>
{
    var native = GetFiles(artifactsRoot.FullPath + "/net45/lib/**/*");
    var cakeGit = GetFiles(artifactsRoot.FullPath + "/**/Cake.Git.dll");
    var libGit = GetFiles(artifactsRoot.FullPath + "/**/LibGit2Sharp*");
    var coreNative = GetFiles(artifactsRoot.FullPath + "/netstandard1.6/lib/**/*") - GetFiles(artifactsRoot.FullPath + "/netstandard1.6/lib/**/x86/*");

    nuGetPackSettings.Files =  (native + libGit + cakeGit)
                                    .Where(file=>!file.FullPath.Contains("Cake.Core.") && !file.FullPath.Contains("/runtimes/"))
                                    .Select(file=>file.FullPath.Substring(artifactsRoot.FullPath.Length+1))
                                    .Select(file=>new NuSpecContent {Source = file, Target = "lib/" + file})
                                    .Union(
                                        coreNative
                                            .Select(file=>file.FullPath.Substring(artifactsRoot.FullPath.Length+1))
                                            .Select(file=>new NuSpecContent {Source = file, Target = "lib/netstandard1.6/" + new FilePath(file).GetFilename().FullPath})
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
    var addinDir = MakeAbsolute(Directory("./tools/Addins/Cake.Git"));
    if (DirectoryExists(addinDir))
    {
        DeleteDirectory(addinDir, true);
    }
    Unzip(package, addinDir);

    Action executeTests = ()=> {
        CakeExecuteScript("./test.cake",
            new CakeSettings{ Arguments = new Dictionary<string, string>{{"target", target == "Default" ? "Default-Tests" : "Local-Tests"}}}
        );
        DotNetCoreExecute(
            "./tools/Cake.CoreCLR/Cake.dll",
            string.Concat("test.cake --verbosity=diagnostic --target=", target == "Default" ? "Default-Tests" : "Local-Tests")
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
