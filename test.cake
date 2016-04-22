#r "./src/Cake.Git/bin/Release/LibGit2Sharp.dll"
#r "./src/Cake.Git/bin/Release/Cake.Git.dll"
using System.Security.Cryptography;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target          = Argument<string>("target", "Default-Tests");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var testRepo                = MakeAbsolute(Directory("./TestRepo"));
var testInitalRepo          = MakeAbsolute(Directory("./TestRepo/InitalRepo"));
var testCloneRepo           = MakeAbsolute(Directory("./TestRepo/CloneRepo"));
var isLocalBuild            = !AppVeyor.IsRunningOnAppVeyor;
var releaseNotes            = ParseReleaseNotes("./ReleaseNotes.md");
var version                 = releaseNotes.Version.ToString();
var semVersion              = isLocalBuild
                                ? version
                                : string.Concat(version, "-build-", AppVeyor.Environment.Build.Number.ToString("0000"));
FilePath[]  testFiles       = null,
            testDeleteFiles = null,
            testModifyFiles = null;
GitCommit   initalCommit    = null,
            removeCommit    = null,
            modifiedCommit  = null;



///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    var buildStartMessage = string.Format(
                            "Testing version {0} of {1} ({2}).",
                            version,
                            "Cake Git Addin",
                            semVersion
                            );

    Information(buildStartMessage);
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");

    try
    {
        Information("Trying to clean up test repo {0}", testRepo);
        if (DirectoryExists(testRepo))
        {
            ForceDeleteDirectory(testRepo.FullPath);
        }
        if (DirectoryExists(testRepo))
        {
            Warning("Failed to clean {0}", testRepo);
        }
        else
        {
            Information("Successfully cleaned test repo {0}", testRepo);
        }
    }
    catch(Exception ex)
    {
        Error("Failed to clean up test reop {0}", testRepo);
    }
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////


Task("Clean")
    .Does(() =>
{
    // Clean up if repo exists
    if (DirectoryExists(testRepo))
    {
        ForceDeleteDirectory(testRepo.FullPath);
    }
});

Task("Git-Init")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Creating repository...");
    CreateDirectory(testRepo);
    CreateDirectory(testInitalRepo);
    var repo = GitInit(testInitalRepo);
    if (!DirectoryExists(repo))
    {
        throw new DirectoryNotFoundException (string.Format("Failed to create repository {0}",
            repo
            ));
    }
    Information("Repository created {0}.", repo);
});

Task("Create-Test-Files")
    .IsDependentOn("Git-Init")
    .Does(() =>
{
    Information("Creating test files...");
    testFiles = Enumerable
                    .Range(1, 10)
                    .Select(
                        index=> {
                            FilePath filePath = testInitalRepo.CombineWithFilePath(string.Format("{0}.{1:000}",
                                Guid.NewGuid(),
                                index
                                ));
                            Information("Creating file {0}...", filePath);
                            CreateRandomDataFile(Context, filePath);
                            Information("File {0} created.", filePath);

                            return filePath;
                        }
                        )
                    .ToArray();
});

Task("Git-Init-Add")
    .IsDependentOn("Create-Test-Files")
    .Does(() =>
{
    Information("Adding test files...");
    GitAdd(testInitalRepo, testFiles);
});

Task("Git-Init-Commit")
    .IsDependentOn("Git-Init-Add")
    .Does(() =>
{
    Information("Committing files...");
    initalCommit = GitCommit(testInitalRepo, "John Doe", "john@doe.com", "Inital commit");
    Information("Commit created:\r\n{0}", initalCommit);
});

Task("Git-Init-Diff")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    var diffFiles = GitDiff(testInitalRepo);
    foreach(var diffFile in diffFiles)
    {
        Information("{0}", diffFile);
    }
});

Task("Git-Log-TipToCommitId")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    var commits = GitLog(testInitalRepo, initalCommit.Sha);
    foreach(var commit in commits)
    {
        Information("{0}", commit);
    }
});

Task("Git-Log-Tip")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    var commit = GitLogTip(testInitalRepo);
    Information("{0}", commit);
});

Task("Git-Log-TipToRange")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    var commits = GitLog(testInitalRepo, byte.MaxValue);
    foreach(var commit in commits)
    {
        Information("{0}", commit);
    }
});

Task("Git-Log-Lookup")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    var commit = GitLogLookup(testInitalRepo, initalCommit.Sha);
    Information("{0}", commit);
});

Task("Git-Remove")
    .IsDependentOn("Git-Log")
    .Does(() =>
{
    Information("Removing files...");
    testDeleteFiles = testFiles.Take(5).ToArray();
    GitRemove(testInitalRepo, true, testDeleteFiles);
    var existingFiles = testDeleteFiles.Where(FileExists).ToList();
    if (existingFiles.Count > 0)
    {
        throw new Exception(string.Format("Files not removed:\r\n {0}",
            string.Join("\r\n", existingFiles)));
    }
    if (!testFiles.Except(testDeleteFiles).All(FileExists))
    {
        throw new Exception(string.Format("Wrong files removed:\r\n {0}",
            string.Join("\r\n", testFiles.Except(testDeleteFiles))));
    }
});

Task("Git-Remove-Commit")
    .IsDependentOn("Git-Remove")
    .Does(() =>
{
    Information("Committing removed files...");
    removeCommit = GitCommit(testInitalRepo, "John Doe", "john@doe.com", "Remove commit");
    Information("Commit created:\r\n{0}", removeCommit);
});

Task("Git-Remove-Diff")
    .IsDependentOn("Git-Remove-Commit")
    .Does(() =>
{
    var diffFiles = GitDiff(testInitalRepo, removeCommit.Sha);
    foreach(var diffFile in diffFiles)
    {
        Information("{0}", diffFile);
    }
});

Task("Modify-Test-Files")
    .IsDependentOn("Git-Init-Commit")
    .IsDependentOn("Git-Remove-Commit")
    .Does(() =>
{
    testModifyFiles = testFiles
        .Except(testDeleteFiles)
        .Select(modifyFilePath => {
            Information("Mofifying file {0}...", modifyFilePath);
            CreateRandomDataFile(Context, modifyFilePath);
            return modifyFilePath;
            })
        .ToArray();
});

Task("Git-Modify-Add")
    .IsDependentOn("Modify-Test-Files")
    .Does(() =>
{
    Information("Adding modified test files...");
    GitAdd(testInitalRepo, testModifyFiles);
});

Task("Git-Modify-Commit")
    .IsDependentOn("Git-Modify-Add")
    .Does(() =>
{
    Information("Committing modified files...");
    modifiedCommit = GitCommit(testInitalRepo, "John Doe", "john@doe.com", "Modified commit");
    Information("Commit created:\r\n{0}", modifiedCommit);
});

Task("Git-Modify-Diff")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    var diffFiles = GitDiff(testInitalRepo, modifiedCommit.Sha);
    foreach(var diffFile in diffFiles)
    {
        Information("{0}", diffFile);
    }
});

Task("Git-Clone")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var sourceUrl = "https://github.com/WCOMAB/CakeGitTestRepo.git";
    Information("Cloning repo {0}...", sourceUrl);
    var repo = GitClone(sourceUrl, testCloneRepo);
    Information("Cloned {0}.", repo);
});

Task("Git-Diff")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    var diffFiles = GitDiff("./", "ce4cb53b42c3ded28b9d582083ed640f19541e07", "b0e1770503d4d6d5beb61ad230001656d5dfed9b");
    foreach(var diffFile in diffFiles)
    {
        Information("{0}", diffFile);
    }
});


Task("Git-Log")
    .IsDependentOn("Git-Log-TipToCommitId")
    .IsDependentOn("Git-Log-Tip")
    .IsDependentOn("Git-Log-TipToRange")
    .IsDependentOn("Git-Log-Lookup");

Task("Default-Tests")
    .IsDependentOn("Git-Init")
    .IsDependentOn("Create-Test-Files")
    .IsDependentOn("Git-Init-Add")
    .IsDependentOn("Git-Init-Commit")
    .IsDependentOn("Git-Init-Diff")
    .IsDependentOn("Git-Log")
    .IsDependentOn("Git-Remove")
    .IsDependentOn("Git-Remove-Commit")
    .IsDependentOn("Git-Remove-Diff")
    .IsDependentOn("Modify-Test-Files")
    .IsDependentOn("Git-Modify-Add")
    .IsDependentOn("Git-Modify-Commit")
    .IsDependentOn("Git-Modify-Diff")
    .IsDependentOn("Git-Clone")
    .IsDependentOn("Git-Diff");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

public static void ForceDeleteDirectory(string path)
{
    var directory = new System.IO.DirectoryInfo(path) { Attributes = FileAttributes.Normal };

    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
    {
        info.Attributes = FileAttributes.Normal;
    }

    directory.Delete(true);
}

public static void CreateRandomDataFile(ICakeContext context, FilePath filePath)
{
    var file = context.FileSystem.GetFile(filePath);

    using (var rngCsp = new RNGCryptoServiceProvider())
    {
        var data = new byte[128];
        rngCsp.GetBytes(data);
        var base64Data = Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks);
        using(var stream = file.OpenWrite())
        {
            using(var writer = new System.IO.StreamWriter(stream, Encoding.ASCII))
            {
                writer.WriteLine(base64Data);
            }
        }
    }

}