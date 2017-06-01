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
        /// Removes from the staging area all the modifications of a collection of file since the latest commit (addition, updation or removal).
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitUnstage("c:/temp/cake", filePaths);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="filePaths">Path to file(s) to unstage.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Unstage")]
        public static void GitUnstage(
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
                repository =>
                {
                    Commands.Unstage(
                        repository,
                        filePaths.ToRelativePathStrings(context, repositoryDirectoryPath)
                    );
                    repository.Index.Write();
                });
        }

        /// <summary>
        /// Removes from the staging area all the modifications all files the latest commit (addition, updation or removal).
        /// </summary>
        /// <example>
        /// <code>
        ///     GitUnstageAll("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Unstage")]
        public static void GitUnstageAll(
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
                repository =>
                {
                    Commands.Unstage(repository, "*");
                    repository.Index.Write();
                });
        }
    }
}