using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;

namespace Cake.Git
{
    partial class GitAliases
    {
        /// <summary>
        /// Gets a list of all tags from the repository.
        /// </summary>
        /// <remarks>
        /// If you need to access the targets of the tags use <see cref="GitTags(ICakeContext, DirectoryPath, bool)"/>
        /// to make sure targerts are loaded.
        /// </remarks>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns>List of all tags from the repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tags")]
        public static List<Tag> GitTags(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (repositoryDirectoryPath == null)
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));

            return context.GitTags(repositoryDirectoryPath, false);
        }

        /// <summary>
        /// Gets a list of all tags from the repository with the option to load targets of the tags.
        /// </summary>
        /// <remarks>
        /// If you need to access the targets of the tags, set <paramref name="loadTargets"/> to <see langword="true"/>.
        /// This will make sure that the targets are loaded before the <see cref="Repository"/> is disposed.
        /// Otherwise, accessing a tag's target will throw an exception.
        /// </remarks>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="loadTargets">A value indicating whether targets of the tags should be loaded.</param>
        /// <returns>List of all tags from the repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tags")]
        public static List<Tag> GitTags(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            bool loadTargets)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (repositoryDirectoryPath == null)
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));

            var retval =
                context.UseRepository(
                    repositoryDirectoryPath,
                    repo => SortedTags(
                        repo.Tags,
                        t =>
                        {
                            if (loadTargets)
                            {
                                _ = t.PeeledTarget;
                            }

                            return t;
                        }));

            return retval;
        }

        private static List<T> SortedTags<T>(IEnumerable<Tag> tags, Func<Tag, T> selector)
        {
            return tags.OrderBy(t => t.CanonicalName, StringComparer.Ordinal).Select(selector).ToList();
        }

    }
}