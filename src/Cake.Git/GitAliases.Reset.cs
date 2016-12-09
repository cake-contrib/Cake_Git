using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Sets the current branch head (HEAD) to a specified commit, 
        /// optionally modifying index and working tree to match.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitReset("c:/temp/cake", GitResetMode.Hard, "commit id");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="mode">The effect that the reset action should have on the the index and working tree.</param>
        /// <param name="commitId">The id of the commit to which the current branch HEAD should be set.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Reset")]
        public static void GitReset(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            GitResetMode mode,
            string commitId)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            context.UseRepository(repositoryDirectoryPath,
                repository => repository.Reset((ResetMode) mode, repository.Lookup<Commit>(commitId)));
        }

        /// <summary>
        /// Resets the current branch head (HEAD) optionally modifying index and working tree to match.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitReset("c:/temp/cake", GitResetMode.Hard);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="mode">The effect that the reset action should have on the the index and working tree.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Reset")]
        public static void GitReset(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            GitResetMode mode)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            context.UseRepository(repositoryDirectoryPath,
                repository => repository.Reset((ResetMode)mode));
        }
    }
}