using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Add file to index.
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitAdd("c:/temp/cake", filePaths);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="filePaths">Path to file(s) to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Add")]
        public static void GitAdd(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            params FilePath[] filePaths
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

            context.UseRepository(
                repositoryDirectoryPath,
                repository => Commands.Stage(
                    repository,
                    filePaths.ToRelativePathStrings(context, repositoryDirectoryPath)
                    )
                );
        }

        /// <summary>
        /// Add all file changes to index.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitAddAll("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Add")]
        public static void GitAddAll(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath
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

            context.UseRepository(
                repositoryDirectoryPath,
                repository => Commands.Stage(repository, "*")
                );
        }
    }
}