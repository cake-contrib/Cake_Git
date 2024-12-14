### New in 5.0.0 (Released 2024/12/14)

* Update to Cake 5.0 (@pascalberger)
* Multi-Target .NET 8 & 9 (@pascalberger)
* Update to LibGit2Sharp 0.30.0 (@pascalberger)
* Add GitRemotes() and GitRemote() aliases (@ap0llo)

### New in 4.0.0 (Released 2024/03/07)

* Update to Cake 4.0 (@pascalberger)
* Multi-Target .NET 6, 7 & 8 (@pascalberger)
* Update to LibGit2Sharp 0.29.0 (@pascalberger)
* Introduce Cake.Frosting.Git addin for using with Cake Frosting (@pascalberger)
* Document steps to release new versions of Cake.Git (@augustoproiete)

### New in 3.0.0 (Released 2022/12/14)

* Update Cake.Git to target Cake.Core 3.0.0 and net7.0 TFM

### New in 2.0.0 (Released 2021/12/13)

* Update Cake.Git to target Cake v2.0.0
* Fix infinite loop in GitFindRootFromPath
* Added another alias for git fetch
* Update Cake.Git NuGet package icon to the new Cake-Contrib logo for Cake addin
* Added an alias GitShortenSha
* Update LibGit2Sharp to 0.27.0-preview-0102
* update LibGit2Sharp.NativeBinaries to 2.0.312

### New in 1.1.0 (Released 2021/08/09)

* Update LibGit2Sharp to 0.27.0-preview-0102
* Add Basic Support for git Configuration Commands

### New in 1.0.1 (Released 2021/03/12)

* Add cake-addin tag to NuGet package

### New in 1.0.0 (Released 2021/02/09)

* Update Cake.Git to target Cake v1.0.0
* Issue with accessing Tag properties
* Exception TargetInvocationException in GitTags(..)

### New in 0.22.0 (Released 2020/06/26)

* Use SPDX expression for license of NuGet package
* Update documentation for GitLog aliases
* Add XML comments to NuGet package
* Remove transitive dependency from project file
* Add missing alias category for GitCreateBranch
* Add repository metadata to NuGet package
* Add missing null reference checks
* New alias "GitBranches" for retrieving all branches of a repository.
* Update to LibGit2Sharp 0.26.2
* Added support for listing commits since a tag
* Inconsistency between GitClone aliases

### New in 0.21.0 (Released 2019/07/05)

* Update to LibGit2Sharp 0.26.0

### New in 0.20.0 (Released 2019/06/22)

* Bumped Cake.Core to 0.33.0
* GitCreateBranch returns GitBranch
* Fix GitLog
* Unify tags documentation

### New in 0.19.0 (Released 2018/08/23)

* Ported to .NET Standard 2.0
* Updated to LibGit2Sharp 0.25.2

### New in 0.18.0 (Released 2018/07/04)

* Bumped Cake.Core to 0.28.1
* Add git clean command

### New in 0.17.0 (Released 2018/03/01)

* Bumped Cake.Core to 0.26.0

### New in 0.16.1 (Released 2017/10/09)

* Updated nuget icon and repo url
* Add framework lib/net46 moniker
* Fix warnings in build script

### New in 0.16.0 (Released 2017/09/14)

* Bumped Cake.Core to 0.22.0

### New in 0.15.0 (Released 2017/06/01)

* Added GitHasStagedChanges alias
* Included license in nuget package

### New in 0.14.0 (Released 2017/03/22)

* Only fetch merge commit for non fast forward, fixes GitPull issue
* Changed to LibGit Portable

### New in 0.13.0 (Released 2017/02/21)

* Added GitIsValidRepository Alias
* Added GitHasUncommitedChanges alias
* Fixed link in Nuspec

### New in 0.12.0 (Released 2017/01/09)

* Fixed null issue in GitPull alias

### New in 0.11.0 (Released 2016/12/09)

* Updated documentation

### New in 0.10.0 (Released 2016/10/16)

* Added GitCloneSettings support
* Updated documentation

### New in 0.9.0 (Released 2016/10/12)

* Added remotes to GitBranch

### New in 0.8.0 (Released 2016/08/16)

* Added GitPushRef alias, which adds possibility to push tags

### New in 0.7.0 (Released 2016/08/04)

* Added GitTag alias
* Updated to Cake.Core 0.15.2

### New in 0.6.0 (Released 2016/07/11)

* Updated to Cake.Core 0.14.0

### New in 0.5.0 (Released 2016/07/01)

* Added Checkout full repo

### New in 0.4.0 (Released 2016/06/22)

* Added GitReset alias

### New in 0.3.0 (Released 2016/06/03)

* Added GitBranchCurrent alias

### New in 0.2.0 (Released 2016/04/22)

* Added GittDiff alias
* Fixed documentatio typos
* Fixed GitLog bugg
* Added Git Commit / Author ToString() overrides
* Removed unused attributes
* Added test.cake script
* Added GitDescribe alias
* Added GitFindRootFromPath
* Updated LibGit to 0.23.0 prerelease to support Mono (OSX & Linux)

### New in 0.1.0 (Released 2016/04/18)

* Inital Release
* Updated LibGit to 0.22 stable

### New in 0.0.3 (Released 2016/01/31)

* Added more Git push/pull/log

### New in 0.0.2 (Released 2015/12/14)

* Added more Git operations

### New in 0.0.1 (Released 2015/12/12)

* First experimental release of Cake.Git