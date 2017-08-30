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
        /// Gets a list of all tags from the repo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="repositoryDirectoryPath"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("AllTags")]
        public static List<Tag> GitTags(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (repositoryDirectoryPath == null)
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));

            var retval =
                context.UseRepository(repositoryDirectoryPath, repo => SortedTags(repo.Tags, t => t));

            return retval;
        }

        private static List<T> SortedTags<T>(IEnumerable<Tag> tags, Func<Tag, T> selector)
        {
            return tags.OrderBy(t => t.CanonicalName, StringComparer.Ordinal).Select(selector).ToList();
        }

    }
}