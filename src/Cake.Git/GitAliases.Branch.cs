using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;

namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Gets the current branch.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Branch")]
        public static GitBranch GitBranchCurrent(this ICakeContext context, DirectoryPath repositoryDirectoryPath)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            return context.UseRepository(
                repositoryDirectoryPath,
                repository => new GitBranch(repository.Head)
                );
        }
    }
}
