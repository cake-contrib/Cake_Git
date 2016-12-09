using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Commit using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitCommit("c:/temp/cake", "name", "email", "message");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="name">Name of committer.</param>
        /// <param name="email">Email of committer.</param>
        /// <param name="message">Commit message.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Commit")]
        public static GitCommit GitCommit(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string name,
            string email,
            string message
            )
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
                repository => new GitCommit(
                    repository.Commit(
                        message,
                        new Signature(name, email, DateTimeOffset.Now),
                        new Signature(name, email, DateTimeOffset.Now),
                        new CommitOptions()
                        )
                    )
                );
        }
    }
}
