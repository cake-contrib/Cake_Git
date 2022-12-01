#tool nuget:?package=NuGet.CommandLine&version=6.0.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");

///////////////////////////////////////////////////////////////////////////////
// GITHUB ACTION Grouping
///////////////////////////////////////////////////////////////////////////////

if (BuildSystem.GitHubActions.IsRunningOnGitHubActions)
{
    TaskSetup(context => System.Console.WriteLine($"::group::{context.Task.Name.Quote()}"));
    TaskTeardown(context => System.Console.WriteLine("::endgroup::"));
}


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    var isMainBranch        = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
    var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
    var releaseNotes        = ParseReleaseNotes("./ReleaseNotes.md");
    var version             = releaseNotes.Version.ToString();
    var semVersion          = isLocalBuild || (AppVeyor.Environment.Repository.Tag.IsTag
                                                && !string.IsNullOrWhiteSpace(AppVeyor.Environment.Repository.Tag.Name))
                                    ? version
                                    : string.Concat(version, "-build-", AppVeyor.Environment.Build.Number.ToString("0000"));

    var shouldPublish       = ctx.IsRunningOnWindows()
                                && AppVeyor.IsRunningOnAppVeyor
                                && AppVeyor.Environment.Repository.Tag.IsTag
                                && StringComparer.OrdinalIgnoreCase.Equals(version, AppVeyor.Environment.Repository.Tag.Name?.TrimStart('v'));

    if (!isLocalBuild)
    {
        AppVeyor.UpdateBuildVersion(semVersion);
    }

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


    var configuration   = Argument<string>("configuration", "Release");


    // Executed BEFORE the first task.
    Information("Building {0} version {1} of {2} ({3}), IsMainBranch: {4}, Publish: {5}.",
                configuration,
                version,
                assemblyInfo.Product,
                semVersion,
                isMainBranch,
                shouldPublish);

    var artifactsRoot = MakeAbsolute(Directory("./artifacts/"));
    var nugetRoot     = MakeAbsolute(Directory("./nuget/"));

    return new BuildData(
        isMainBranch,
        shouldPublish,
        isLocalBuild,
        configuration,
        target,
        new[]{ "net6.0", "net7.0" },
        new DotNetMSBuildSettings()
                            .WithProperty("Version", semVersion)
                            .WithProperty("AssemblyVersion", version)
                            .WithProperty("FileVersion", version)
                            .WithProperty("PackageReleaseNotes", string.Concat("\"", string.Concat(releaseNotes.Notes.ToArray()), "\""))
                            .WithProperty("ContinuousIntegrationBuild", AppVeyor.IsRunningOnAppVeyor ? "true" : "false")
                            .WithProperty("EmbedUntrackedSources", "true"),
        new NuGetPackSettings {
                                Id                      = assemblyInfo.Product,
                                Version                 = assemblyInfo.InformationalVersion,
                                Title                   = assemblyInfo.Title,
                                Authors                 = new[] {assemblyInfo.Company},
                                Owners                  = new[] {assemblyInfo.Company, "cake-contrib"},
                                Description             = assemblyInfo.Description,
                                Summary                 = "Cake AddIn that extends Cake with Git SCM features",
                                ProjectUrl              = new Uri("https://github.com/cake-contrib/Cake_Git/"),
                                Repository              = new NuGetRepository { Type = "git", Url = "https://github.com/cake-contrib/Cake_Git.git" },
                                Icon                    = "images/icon.png",
                                IconUrl                 = new Uri("https://cdn.jsdelivr.net/gh/cake-contrib/graphics/png/addin/cake-contrib-addin-medium.png"),
                                License                 = new NuSpecLicense { Type = "expression", Value = "MIT" },
                                Copyright               = assemblyInfo.Copyright,
                                ReleaseNotes            = releaseNotes.Notes.ToArray(),
                                Tags                    = new [] { "cake", "script", "build", "git", "cake-addin", "cake-build" },
                                RequireLicenseAcceptance= false,
                                Symbols                 = false,
                                NoPackageAnalysis       = true,
                                Files                   = Array.Empty<NuSpecContent>(), // is set dynamically
                                BasePath                = artifactsRoot,
                                OutputDirectory         = nugetRoot
                            },
        assemblyInfo,
        new BuildPaths(
            artifactsRoot,
            nugetRoot,
            nugetRoot.CombineWithFilePath("Cake.Git." + semVersion + ".nupkg"),
            GetFiles("./**/*.sln")
                .ToArray(),
            MakeAbsolute(Directory("./tools/Addins/Cake.Git/Cake.Git"))));
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
    .Does<BuildData>(static (context, data) =>
{
    // Clean solution directories.
    foreach(var path in data.BuildPaths.SolutionPaths)
    {
        context.Information("Cleaning {0}", path);
        context.CleanDirectories(path + "/**/bin/" + data.Configuration);
        context.CleanDirectories(path + "/**/obj/" + data.Configuration);
    }

    context.Information("Cleaning {0}", data.BuildPaths.NuGetRoot);
    context.CleanDirectory(data.BuildPaths.NuGetRoot);

    context.Information("Cleaning {0}", data.BuildPaths.ArtifactsRoot);
    context.CleanDirectory(data.BuildPaths.ArtifactsRoot);
});

Task("Restore")
    .Does<BuildData>(static (context, data) =>
{
    // Restore all NuGet packages.
    foreach(var solution in data.BuildPaths.Solutions)
    {
        context.Information("Restoring {0}...", solution);
        context.DotNetRestore(
            solution.FullPath,
            new DotNetRestoreSettings {
                Verbosity = DotNetVerbosity.Minimal,
                MSBuildSettings = data.MSBuildSettings
        });
    }
});

Task("SolutionInfo")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does<BuildData>(static (context, data) =>
{
    var file = "./src/SolutionInfo.cs";
    context.CreateAssemblyInfo(file, data.AssemblyInfoSettings);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("SolutionInfo")
    .Does<BuildData>(static (context, data) =>
{
    // Build all solutions.
    foreach(var solution in data.BuildPaths.Solutions)
    {
        context.Information("Building {0}", solution);
        context.DotNetBuild(
            solution.FullPath,
            new DotNetBuildSettings {
                    Configuration = data.Configuration,
                    NoRestore = true,
                    MSBuildSettings = data.MSBuildSettings
                });
    }
});

Task("Publish-Artifacts")
    .IsDependentOn("Build")
    .DoesForEach<BuildData, string>(
        static (data, context) => {
            context.EnsureDirectoryExists(data.BuildPaths.ArtifactsRoot);
            return data.TargetFrameworks;
        },
        static (data, targetFramework, context) =>
{
    context.DotNetPublish("./src/Cake.Git", new DotNetPublishSettings
    {
        NoRestore = true,
        Framework = targetFramework,
        MSBuildSettings = data.MSBuildSettings,
        OutputDirectory = data.BuildPaths.ArtifactsRoot.Combine(targetFramework)
    });
});


Task("Create-NuGet-Package")
    .IsDependentOn("Publish-Artifacts")
    .Does<BuildData>(static (context, data) =>
{
    var cakeGit = context.GetFiles(data.BuildPaths.ArtifactsRoot.FullPath + "/**/Cake.Git.dll");
    var cakeGitDoc = context.GetFiles(data.BuildPaths.ArtifactsRoot.FullPath + "/**/Cake.Git.xml");
    var libGit = context.GetFiles(data.BuildPaths.ArtifactsRoot.FullPath + "/**/LibGit2Sharp*");
    var unmanaged = context.GetFiles(data.BuildPaths.ArtifactsRoot.FullPath + "/net6.0/runtimes/**/*");

    data.NuGetPackSettings.Files =  (libGit + cakeGit + cakeGitDoc)
                                    .Select(file=>file.FullPath.Substring(data.BuildPaths.ArtifactsRoot.FullPath.Length+1))
                                    .Select(file=>new NuSpecContent {Source = file, Target = "lib/" + file})
                                    // add unmanaged dlls to the "right" place in the nuget
                                    .Union(unmanaged
                                        .Select(file=>file.FullPath.Substring(data.BuildPaths.ArtifactsRoot.FullPath.Length+1))
                                        .Select(file=>new NuSpecContent {Source = file, Target = "/" + file.Substring(7)}))
                                    // cake scripting needs the unmanaged dlls to be in the "wrong" place for some reason..
                                    .Union(unmanaged
                                        .Where(file=>file.FullPath.Contains("/linux-x64/") || file.FullPath.Contains("/win-x64/") || file.FullPath.Contains("/osx/"))
                                        .SelectMany(file => data.TargetFrameworks.Select(tfm =>
                                            new NuSpecContent {
                                                Source = file.FullPath.Substring(data.BuildPaths.ArtifactsRoot.FullPath.Length+1),
                                                Target = $"lib/{tfm}/{file.GetFilename()}"
                                            })))
                                    // add the icon
                                    .Union(new []
                                    {
                                        new NuSpecContent
                                        {
                                            Source = context.MakeAbsolute(context.File("./asset/cake-contrib-addin-medium.png")).FullPath,
                                            Target = "images/icon.png",
                                        },
                                    })
                                    .ToArray();

    context.EnsureDirectoryExists(data.BuildPaths.NuGetRoot);

    context.NuGetPack(data.NuGetPackSettings);
});

Task("Test")
    .IsDependentOn("Create-NuGet-Package")
    .WithCriteria<BuildData>(data => StringComparer.OrdinalIgnoreCase.Equals(data.Configuration, "Release"))
    .DoesForEach<BuildData, TestCase>(
        static (data, context) => {
            if (context.DirectoryExists(data.BuildPaths.AddinDir))
            {
                context.DeleteDirectory(data.BuildPaths.AddinDir, new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                });
            }

            context.Unzip(data.BuildPaths.Package, data.BuildPaths.AddinDir);

            var cakeSettings = new CakeSettings {
                Arguments = new Dictionary<string, string> {
                                { "target", data.Target == "Default" ? "Default-Tests" : "Local-Tests"}
                            }
            };

            return data
                    .TargetFrameworks
                    .Select(targetFramework => new TestCase(targetFramework, cakeSettings))
                    .ToArray();
        },
        static (data, testCase, context) =>
{
    context.Information("Testing {0}", testCase.TargetFramework);

    context.CakeExecuteScript(
        testCase.FilePath,
        testCase.CakeSettings
    );
});

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test")
    .WithCriteria<BuildData>(static data => !data.IsLocalBuild)
    .Does<BuildData>(data =>
{
    // Upload Artifact
    AppVeyor.UploadArtifact(data.BuildPaths.Package);
});

Task("Push-NuGet-Packages")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test")
    .WithCriteria<BuildData>(static data => data.ShouldPushNuGetPackages())
    .Does<BuildData>((context, data) =>
{
   context.DotNetNuGetPush(
                data.BuildPaths.Package,
                new DotNetNuGetPushSettings
                {
                    Source = data.NuGetSource,
                    ApiKey = data.NuGetApiKey
                }
        );
});

Task("Upload-GitHubActions-Artifacts")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test")
    .WithCriteria(() => GitHubActions.IsRunningOnGitHubActions)
    .Does<BuildData>(async data =>
{
    // Upload Artifacts
    var suffix = EnvironmentVariable("matrix-os")
        ?? GitHubActions.Environment.Runner.OS;
    Information("Uploading artifacts for {0}...", suffix);
    await GitHubActions.Commands.UploadArtifact(data.BuildPaths.Package, string.Concat("NuGet-", suffix));
    await GitHubActions.Commands.UploadArtifact(data.BuildPaths.ArtifactsRoot, string.Concat("Artifacts-", suffix));
});


Task("Default")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test");

Task("Local-Tests")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("Test");

Task("AppVeyor")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Push-NuGet-Packages");

Task("GitHubActions")
    .IsDependentOn("Upload-GitHubActions-Artifacts");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);


///////////////////////////////////////////////////////////////////////////////
// RECORDS
///////////////////////////////////////////////////////////////////////////////

public record BuildPaths(
    DirectoryPath ArtifactsRoot,
    DirectoryPath NuGetRoot,
    FilePath Package,
    ICollection<FilePath> Solutions,
    DirectoryPath AddinDir)
{
     public ICollection<DirectoryPath> SolutionPaths { get; } = Solutions
                                                        .Select(solution => solution.GetDirectory())
                                                        .ToArray();
}

public record BuildData(
    bool IsMainBranch,
    bool ShouldPublish,
    bool IsLocalBuild,
    string Configuration,
    string Target,
    ICollection<string> TargetFrameworks,
    DotNetMSBuildSettings MSBuildSettings,
    NuGetPackSettings NuGetPackSettings,
    AssemblyInfoSettings AssemblyInfoSettings,
    BuildPaths BuildPaths)
{
    public string NuGetSource { get; } = System.Environment.GetEnvironmentVariable("NUGET_SOURCE");
    public string NuGetApiKey { get; } = System.Environment.GetEnvironmentVariable("NUGET_API_KEY");
    public bool ShouldPushNuGetPackages() =>    IsMainBranch &&
                                                ShouldPublish &&
                                                !string.IsNullOrWhiteSpace(NuGetSource) &&
                                                !string.IsNullOrWhiteSpace(NuGetApiKey);
}

public record TestCase(
    string TargetFramework,
    CakeSettings CakeSettings)
{
    public FilePath FilePath { get; } = $"./test_{TargetFramework}.cake";
}