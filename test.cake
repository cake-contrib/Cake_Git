#r "./tools/Addins/Cake.Git/Cake.Git/lib/net46/Cake.Git.dll"
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
var testUser                = "John Doe";
var testUserEmail           = "john@doe.com";
FilePath[]  testFiles       = null,
            testDeleteFiles = null,
            testModifyFiles = null;
GitCommit   initalCommit    = null,
            removeCommit    = null,
            modifiedCommit  = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
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

Teardown(ctx =>
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
        Error("Failed to clean up test reop {0}\r\n{1}", testRepo, ex);
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

Task("Git-IsValidRepository-LocalRepo")
    .IsDependentOn("Git-Init")
    .Does(() =>
{
    Information("Checking if local repository is recognized as a valid repository...");
    if (!GitIsValidRepository(testInitalRepo)) {
        throw new Exception(string.Format("Initialized repository at '{0}' is not a valid Git repository.",
            testInitalRepo));
    }
});

Task("Git-IsValidRepository-TempDirectory")
    .Does(() =>
{
    Information("Checking if temp directory is not reported as a valid repository...");
    var tempPath = System.IO.Path.GetTempPath();
    if (GitIsValidRepository(tempPath)) {
        throw new Exception(string.Format("Path at '{0}' should not be a valid Git repository.",
            tempPath));
    }
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

Task("Create-Untracked-Test-Files")
    .Does(() =>
{
    Information("Creating untracked test files...");
    Enumerable
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
    initalCommit = GitCommit(testInitalRepo, testUser, testUserEmail, "Inital commit");
    Information("Commit created:\r\n{0}", initalCommit);
});

Task("Git-HasUncommitedChanges-Dirty")
    .IsDependentOn("Git-Init-Add")
    .Does(() =>
{
    Information("Checking if repository has uncommited changes...");
    if (!GitHasUncommitedChanges(testInitalRepo)) 
    {
        throw new Exception("Repository doesn't report uncommited changes.");
    };
});

Task("Git-HasUncommitedChanges-Clean")
    .IsDependentOn("Git-Init-Commit")
    .Does(() =>
{
    Information("Checking if repository has uncommited changes...");
    if (GitHasUncommitedChanges(testInitalRepo)) 
    {
        throw new Exception("Repository reports uncommited changes after commiting all files.");
    };
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
    removeCommit = GitCommit(testInitalRepo, testUser, testUserEmail, "Remove commit");
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
            Information("Modifying file {0}...", modifyFilePath);
            CreateRandomDataFile(Context, modifyFilePath);
            return modifyFilePath;
            })
        .ToArray();
});

Task("Modify-More-Test-Files")
    .IsDependentOn("Git-Init-Commit")
    .IsDependentOn("Git-Remove-Commit")
    .Does(() =>
{
    testModifyFiles = testFiles
        .Except(testDeleteFiles)
        .Select(modifyFilePath => {
            Information("Modifying file {0}...", modifyFilePath);
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

Task("Git-Modify-AddAll")
    .IsDependentOn("Modify-More-Test-Files")
    .Does(() =>
{
    Information("Adding modified test files...");
    GitAddAll(testInitalRepo);
});

Task("Git-Modify-Unstage")
    .IsDependentOn("Git-Modify-Add")
    .Does(() =>
{
    Information("Unstaging added test files...");
    if (!GitHasStagedChanges(testInitalRepo)) 
    {
        throw new Exception("Repository doesn't report indexed changes.");
    };
    GitUnstageAll(testInitalRepo);
    if (GitHasStagedChanges(testInitalRepo)) 
    {
        throw new Exception("Repository does report indexed changes.");
    };
    GitAdd(testInitalRepo, testModifyFiles);
});

Task("Git-Modify-Commit")
    .IsDependentOn("Git-Modify-Add")
    .Does(() =>
{
    Information("Committing modified files...");
    modifiedCommit = GitCommit(testInitalRepo, testUser, testUserEmail, "Modified commit");
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
    if (DirectoryExists(testCloneRepo))
    {
        ForceDeleteDirectory(testCloneRepo.FullPath);
    }
    var repo = GitClone(sourceUrl, testCloneRepo);
    Information("Cloned {0}.", repo);
});

Task("Git-Clone-WithSettings")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var sourceUrl = "https://github.com/WCOMAB/CakeGitTestRepo.git";
    Information("Cloning bare repo {0}...", sourceUrl);
    if (DirectoryExists(testCloneRepo))
    {
        ForceDeleteDirectory(testCloneRepo.FullPath);
    }
    var repo = GitClone(sourceUrl, testCloneRepo, new GitCloneSettings {
        IsBare = true
    });
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

Task("Git-Find-Root-From-Path")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    var deepFolder = testInitalRepo.Combine("test-folder/deep");
    CreateDirectory(deepFolder);
    var rootFolder = GitFindRootFromPath(deepFolder);
    Information("Git root folder found: {0}", rootFolder);
    if (rootFolder.FullPath != testInitalRepo.FullPath)
        throw new Exception(string.Format("Wrong git root found (actual: {0}, expected: {1})", rootFolder, testInitalRepo));
});

Task("Git-Tag-Apply-Objectish")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    GitTag(testInitalRepo, "test-tag-objectish", modifiedCommit.Sha);
});

Task("Git-Annotated-Tag-Apply-Objectish")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    GitTag(testInitalRepo, "test-annotated-tag-objectish", modifiedCommit.Sha, testUser, testUserEmail, "Test annotated tag. (objectish)");
});

Task("Git-Tag-Apply")
    .IsDependentOn("Git-Tag-Apply-Objectish")
    .Does(() =>
{
    GitTag(testInitalRepo, "test-tag");
});

Task("Git-Annotated-Tag-Apply")
    .IsDependentOn("Git-Modify-Commit")
    .Does(() =>
{
    GitTag(testInitalRepo, "test-annotated-tag", testUser, testUserEmail, "Test annotated tag.");
});

Task("Git-AllTags")
    .IsDependentOn("Git-Tag")    
    .Does(()=>
    {
        var tags = GitTags(testInitalRepo);
        if(tags.Count(t=>t.FriendlyName == "test-tag") < 1)
            throw new Exception("test-tag not found");
        if(tags.Count(t=>t.FriendlyName == "test-tag-objectish") < 1)
            throw new Exception("test-tag not found");
    }
    );

Task("Git-AllTags-Annotated")
    .IsDependentOn("Git-Tag-Annotated")    
    .Does(()=>
    {
        var tags = GitTags(testInitalRepo);
        if(tags.Count(t=>t.FriendlyName == "test-annotated-tag") < 1)
            throw new Exception("test-annotated-tag not found");
        if(tags.Count(t=>t.FriendlyName == "test-annotated-tag-objectish") < 1)
            throw new Exception("test-annotated-tag-objectish not found");
    }
    );

Task("Git-Describe-Generic")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tag = GitDescribe(testInitalRepo);
    Information("Describe returned: [{0}]", tag);
    if (!tag.Contains("test-tag"))
        throw new Exception("Wrong described tag: " + tag);
});

Task("Git-Describe-Tags")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tag = GitDescribe(testInitalRepo, GitDescribeStrategy.Tags);
    Information("Describe returned: [{0}]", tag);
    if (!tag.Contains("test-tag"))
        throw new Exception("Wrong described tag: " + tag);
});

Task("Git-Describe-Long")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, true, GitDescribeStrategy.Tags);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, false, GitDescribeStrategy.Tags);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Abbrev")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Commit")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, modifiedCommit.Sha, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, modifiedCommit.Sha, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Commit-NoTag")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, initalCommit.Sha, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, initalCommit.Sha, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!string.IsNullOrEmpty(tagAlwaysLong))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!string.IsNullOrEmpty(tagNotAlwaysLong))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Master")
    .IsDependentOn("Git-Tag")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, "master", true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, "master", false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-tag"))
        throw new Exception("Wrong describe");
    if (!tagNotAlwaysLong.Equals("test-tag"))
        throw new Exception("Wrong describe");
});

Task("Git-Describe-Generic-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tag = GitDescribe(testInitalRepo);
    Information("Describe returned: [{0}]", tag);
    if (!tag.Contains("test-annotated-tag"))
        throw new Exception("Wrong described tag: " + tag);
});

Task("Git-Describe-Tags-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tag = GitDescribe(testInitalRepo, GitDescribeStrategy.Tags);
    Information("Describe returned: [{0}]", tag);
    if (!tag.Contains("test-annotated-tag"))
        throw new Exception("Wrong described tag: " + tag);
});

Task("Git-Describe-Long-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, true, GitDescribeStrategy.Tags);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, false, GitDescribeStrategy.Tags);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-annotated-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-annotated-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Abbrev-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-annotated-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-annotated-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Commit-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, modifiedCommit.Sha, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, modifiedCommit.Sha, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-annotated-tag"))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!tagNotAlwaysLong.Equals("test-annotated-tag"))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Commit-NoTag-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, initalCommit.Sha, true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, initalCommit.Sha, false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!string.IsNullOrEmpty(tagAlwaysLong))
        throw new Exception("Wrong described tagAlwaysLong: " + tagAlwaysLong);
    if (!string.IsNullOrEmpty(tagNotAlwaysLong))
        throw new Exception("Wrong described tagNotAlwaysLong: " + tagNotAlwaysLong);
});

Task("Git-Describe-Master-Annotated")
    .IsDependentOn("Git-Tag-Annotated")
    .Does(() =>
{
    var tagAlwaysLong = GitDescribe(testInitalRepo, "master", true, GitDescribeStrategy.Tags, 16);
    var tagNotAlwaysLong = GitDescribe(testInitalRepo, "master", false, GitDescribeStrategy.Tags, 16);
    Information("Describe returned long: [{0}], !long: [{1}]", tagAlwaysLong, tagNotAlwaysLong);
    if (!tagAlwaysLong.StartsWith("test-annotated-tag"))
        throw new Exception("Wrong describe");
    if (!tagNotAlwaysLong.Equals("test-annotated-tag"))
        throw new Exception("Wrong describe");
});

Task("Git-Current-Branch")
    .Does(() =>
{
    var branch = GitBranchCurrent(testInitalRepo);
    Information("Current branch: {0}", branch);
});

Task("Git-Create-Branch")
.Does(() => 
{
    var branchName = "Foo";
    GitCreateBranch(testInitalRepo, branchName, true);
    var branch = GitBranchCurrent(testInitalRepo);
    if (branch.FriendlyName != branchName)
     throw new Exception($"Incorrect Branch created. Expected {branchName} and got {branch.FriendlyName}");
});

Task("Git-Remote-Branch")
    .Does(() =>
{
    var branch = GitBranchCurrent(".");
    Information("Remote branch: {0}", branch);
});

Task("Git-Checkout")
    .Does(() =>
{
    GitCheckout(testInitalRepo, "master");
    var branch = GitBranchCurrent(testInitalRepo);
     if (branch.FriendlyName != "master")
        throw new Exception(string.Format("branch is incorrect. Expected master and got {0}", branch.FriendlyName));
});

Task("Git-Reset-Modify-Files")
  .Does(() =>
{
  testModifyFiles = testFiles
      .Select(modifyFilePath => {
          Information("Modifying file {0}...", modifyFilePath);
          CreateRandomDataFile(Context, modifyFilePath);
          return modifyFilePath;
          })
          .ToArray();
});

Task("Git-Reset-Stage-File")
  .IsDependentOn("Git-Reset-Modify-Files")
  .Does(() =>
{
    Information("Staging files...");
    GitAdd(testInitalRepo, testModifyFiles);
});

Task("Git-Reset-Hard")
  .IsDependentOn("Git-Reset-Stage-File")
  .Does(() =>
{
    Information("Performing hard reset on files...");
    GitReset(testInitalRepo, GitResetMode.Hard);
});

Task("Git-Clean")
  .IsDependentOn("Create-Untracked-Test-Files")
  .Does(() =>
{
    Information("Performing hard reset on files...");
    
    if (!GitHasUntrackedFiles(testInitalRepo))
        throw new InvalidOperationException("Repo contains no untracked files");

    GitClean(testInitalRepo);

    if (GitHasUntrackedFiles(testInitalRepo))
        throw new InvalidOperationException("Git clean is not working properly");
});




Task("Git-Tag")
    .IsDependentOn("Git-Tag-Apply")
    .IsDependentOn("Git-Tag-Apply-Objectish");

Task("Git-Tag-Annotated")
    .IsDependentOn("Git-Annotated-Tag-Apply")
    .IsDependentOn("Git-Annotated-Tag-Apply-Objectish");

Task("Git-Describe")
    .IsDependentOn("Git-Describe-Generic")
    .IsDependentOn("Git-Describe-Tags")
    .IsDependentOn("Git-Describe-Long")
    .IsDependentOn("Git-Describe-Abbrev")
    .IsDependentOn("Git-Describe-Commit")
    .IsDependentOn("Git-Describe-Commit-NoTag")
    .IsDependentOn("Git-Describe-Master");

Task("Git-Describe-Annotated")
    .IsDependentOn("Git-Describe-Generic-Annotated")
    .IsDependentOn("Git-Describe-Tags-Annotated")
    .IsDependentOn("Git-Describe-Long-Annotated")
    .IsDependentOn("Git-Describe-Abbrev-Annotated")
    .IsDependentOn("Git-Describe-Commit-Annotated")
    .IsDependentOn("Git-Describe-Commit-NoTag-Annotated")
    .IsDependentOn("Git-Describe-Master-Annotated");

Task("Git-Log")
    .IsDependentOn("Git-Log-TipToCommitId")
    .IsDependentOn("Git-Log-Tip")
    .IsDependentOn("Git-Log-TipToRange")
    .IsDependentOn("Git-Log-Lookup");

Task("Git-Reset")
    .IsDependentOn("Git-Reset-Modify-Files")
    .IsDependentOn("Git-Reset-Stage-File")
    .IsDependentOn("Git-Reset-Hard");

Task("Git-IsValidRepository")
    .IsDependentOn("Git-IsValidRepository-LocalRepo")
    .IsDependentOn("Git-IsValidRepository-TempDirectory");

Task("Git-HasUncommitedChanges")
    .IsDependentOn("Git-HasUncommitedChanges-Dirty")
    .IsDependentOn("Git-HasUncommitedChanges-Clean");

Task("Default-Tests")
    .IsDependentOn("Git-Init")
    .IsDependentOn("Git-IsValidRepository-LocalRepo")
    .IsDependentOn("Git-IsValidRepository-TempDirectory")
    .IsDependentOn("Create-Test-Files")
    .IsDependentOn("Git-Init-Add")
    .IsDependentOn("Git-HasUncommitedChanges-Dirty")
    .IsDependentOn("Git-Init-Commit")
    .IsDependentOn("Git-HasUncommitedChanges-Clean")
    .IsDependentOn("Git-Init-Diff")
    .IsDependentOn("Git-Log")
    .IsDependentOn("Git-Remove")
    .IsDependentOn("Git-Remove-Commit")
    .IsDependentOn("Git-Remove-Diff")
    .IsDependentOn("Modify-Test-Files")
    .IsDependentOn("Git-Modify-Add")
    .IsDependentOn("Git-Modify-AddAll")
    .IsDependentOn("Git-Modify-Unstage")
    .IsDependentOn("Git-Modify-Commit")
    .IsDependentOn("Git-Modify-Diff")
    .IsDependentOn("Git-Clone")
    .IsDependentOn("Git-Clone-WithSettings")
    .IsDependentOn("Git-Diff")
    .IsDependentOn("Git-Find-Root-From-Path")
    .IsDependentOn("Git-Reset")
    .IsDependentOn("Git-Describe")
    .IsDependentOn("Git-Describe-Annotated")
    .IsDependentOn("Git-Current-Branch")
    .IsDependentOn("Git-Create-Branch")
    .IsDependentOn("Git-Remote-Branch")
    .IsDependentOn("Git-Checkout")
    .IsDependentOn("Git-AllTags")
    .IsDependentOn("Git-AllTags-Annotated")
    .IsDependentOn("Git-Clean");

Task("Local-Tests")
    .IsDependentOn("Git-Init")
    .IsDependentOn("Git-IsValidRepository-LocalRepo")
    .IsDependentOn("Git-IsValidRepository-TempDirectory")
    .IsDependentOn("Create-Test-Files")
    .IsDependentOn("Git-Init-Add")
    .IsDependentOn("Git-HasUncommitedChanges-Dirty")
    .IsDependentOn("Git-Init-Commit")
    .IsDependentOn("Git-HasUncommitedChanges-Clean")
    .IsDependentOn("Git-Init-Diff")
    .IsDependentOn("Git-Log")
    .IsDependentOn("Git-Remove")
    .IsDependentOn("Git-Remove-Commit")
    .IsDependentOn("Git-Remove-Diff")
    .IsDependentOn("Modify-Test-Files")
    .IsDependentOn("Git-Modify-Add")
    .IsDependentOn("Git-Modify-AddAll")
    .IsDependentOn("Git-Modify-Unstage")
    .IsDependentOn("Git-Modify-Commit")
    .IsDependentOn("Git-Modify-Diff")
    .IsDependentOn("Git-Diff")
    .IsDependentOn("Git-Find-Root-From-Path")
    .IsDependentOn("Git-Reset")
    .IsDependentOn("Git-Describe")
    .IsDependentOn("Git-Describe-Annotated")
    .IsDependentOn("Git-Current-Branch")
    .IsDependentOn("Git-Create-Branch")
    .IsDependentOn("Git-Remote-Branch")
    .IsDependentOn("Git-Checkout")
    .IsDependentOn("Git-AllTags")
    .IsDependentOn("Git-AllTags-Annotated")
    .IsDependentOn("Git-Clean");

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
