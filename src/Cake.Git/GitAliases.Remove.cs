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
        /// Remove file(s) from index.
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitRemove("c:/temp/cake", true, filePaths);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="filePaths">Path to file(s) to remove.</param>
        /// <param name="removeFromWorkingDirectory">True to remove the files from the working directory, False otherwise.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Remove")]
        public static void GitRemove(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            bool removeFromWorkingDirectory,
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
                repository => Commands.Remove(
                    repository,
                    filePaths.ToRelativePathStrings(context, repositoryDirectoryPath),
                    removeFromWorkingDirectory,
                    new ExplicitPathsOptions()
                    )
                );
        }

        
    }
}
