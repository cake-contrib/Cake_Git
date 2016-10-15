using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    /// <summary>
    /// Class GitAliases.
    /// </summary>
    public static partial class GitAliases
    {
        /// <summary>
        /// Gets the current branch.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns>GitBranch.</returns>
        /// <exception cref="ArgumentNullException">context
        /// or
        /// repositoryDirectoryPath</exception>
        /// <example>
        ///   <code>
        /// DirectoryPath repositoryDirectoryPath = DirectoryPath.FromString(".");
        /// GitBranch currentBranch = ((ICakeContext)cakeContext).GitBranchCurrent(repositoryDirectoryPath);
        ///   </code>
        /// </example>
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
                repository => new GitBranch(repository)
                );
        }
    }
}
