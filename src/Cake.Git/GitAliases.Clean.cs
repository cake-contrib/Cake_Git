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
        /// Remove untracked file(s) workspace.
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitClean("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param> 
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Clean")]
        public static void GitClean(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(repositoryDirectoryPath.FullPath))
            {
                throw new ArgumentException(nameof(repositoryDirectoryPath));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.RemoveUntrackedFiles()
                );
        }

        
    }
}
