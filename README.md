# Cake Git

[![NuGet](https://img.shields.io/nuget/v/Cake.Git.svg)](https://www.nuget.org/packages/Cake.Git)

Cake AddIn that extends Cake with Git features using LibGit2 and LibGit2Sharp.

| Build server                | Platform                | Status                                                                                                                                                                       |
|-----------------------------|-------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AppVeyor                    | Windows                 | [![Build status](https://ci.appveyor.com/api/projects/status/mycuknigvm2418ht/branch/develop?svg=true)](https://ci.appveyor.com/project/cakecontrib/cake-git/branch/develop) |
| GitHub Actions              | Windows / Linux / macOS | [![Build status](https://github.com/cake-contrib/Cake_Git/actions/workflows/build.yml/badge.svg)](https://github.com/cake-contrib/Cake_Git/actions/workflows/build.yml)      |

## Documentation

Documentation is available at [cakebuild.net/dsl/git](http://cakebuild.net/dsl/git).

## Example usage

```cake
#addin nuget:?package=Cake.Git

var lastCommit = GitLogTip("PATH TO REPOSITORY");

Information(@"Last commit {0}
    Short message: {1}
    Author:        {2}
    Authored:      {3:yyyy-MM-dd HH:mm:ss}
    Committer:     {4}
    Committed:     {5:yyyy-MM-dd HH:mm:ss}",
    lastCommit.Sha,
    lastCommit.MessageShort,
    lastCommit.Author.Name,
    lastCommit.Author.When,
    lastCommit.Committer.Name,
    lastCommit.Committer.When
    );
```

### Example output

```bash
Last commit fb5b9805e543d8d1715886f78c273dc45b51a928
    Short message: Added Travis test folding
    Author:        Mattias Karlsson
    Authored:      2016-08-16 085836
    Committer:     Mattias Karlsson
    Committed:     2016-08-16 085836
```

## Discussion

For questions and to discuss ideas & feature requests, use the [GitHub discussions on the Cake GitHub repository](https://github.com/cake-build/cake/discussions), under the [Extension Q&A](https://github.com/cake-build/cake/discussions/categories/extension-q-a) category.

[![Join in the discussion on the Cake repository](https://img.shields.io/badge/GitHub-Discussions-green?logo=github)](https://github.com/cake-build/cake/discussions)
