using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Add file to index.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="filePaths">Path to file(s) to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Add")]
        public static void GitAdd(this ICakeContext context, DirectoryPath repositoryDirectoryPath, params FilePath[] filePaths)
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
                repository => Array.ForEach(
                    filePaths.ToRelativePathStrings(context, repositoryDirectoryPath),
                    filePath=>repository.Index.Add(filePath)
                    )
                );
        }

        
    }
}