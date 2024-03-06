# How to release a new version of Cake.Git

* Create release branch `release/vMajor.Minor.Patch` from `develop` (e.g. `git branch release/v3.1.4 develop`)
* Go to the branch (e.g. `git checkout release/v3.1.4`)
* Update the [`ReleaseNotes.md`](ReleaseNotes.md) (the build version number comes from parsing this file)
* Merge the release branch into the `master` branch (e.g. `git merge master`)
* Tag the updated `master` branch with the version (e.g. `git tag v3.1.4`)
* Push the `master` branch with the tag to GitHub (e.g. `git push origin master --tags`)
* The [AppVeyor build](https://ci.appveyor.com/project/cakecontrib/cake-git/history) will publish the NuGet package automatically
* Merge the `master` branch into into `develop` to bring the updated [`ReleaseNotes.md`](ReleaseNotes.md)
* Create and publish a new [release](https://github.com/cake-contrib/Cake_Git/releases) on GitHub
* Close the [milestone](https://github.com/cake-contrib/Cake_Git/milestones) on GitHub
* Publish a notification of the release on the [Cake-Contrib](https://twitter.com/cakecontrib) Twitter account ([see example](https://twitter.com/cakecontrib/status/1603138374994468864))
