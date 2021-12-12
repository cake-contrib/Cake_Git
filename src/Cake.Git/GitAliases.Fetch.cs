using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
using System;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Download objects and refs from another repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitFetch("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="remoteName">The name of the remote to fetch. Default is <c>origin</c></param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Fetch")]
        public static void GitFetch(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string remoteName = "origin"
            )
        {
            GitFetch(context, repositoryDirectoryPath, new GitFetchSettings
            {
                Remote = remoteName
            });
        }

        /// <summary>
        /// Download tags from another repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitFetchTags("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="remoteName">The name of the remote to fetch. Default is <c>origin</c></param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Fetch")]
        public static void GitFetchTags(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string remoteName = "origin"
            )
        {
            GitFetch(context, repositoryDirectoryPath, new GitFetchSettings
            {
                Remote = remoteName,
                TagFetchMode = TagFetchMode.All
            });
        }
        
        /// <summary>
        /// Download from another repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitFetchTags("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="fetchSettings">The settings on what to fetch and how.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Fetch")]
        public static void GitFetch(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            GitFetchSettings fetchSettings
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

            if (fetchSettings == null)
            {
                throw new ArgumentNullException(nameof(fetchSettings));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    var remote = repository.Network.Remotes[fetchSettings.Remote];
                    var refSpecs = remote.FetchRefSpecs
                        .Where(fetchSettings.RefSpecFilter)
                        .Select(x => x.Specification);
                    Commands.Fetch(repository, remote.Name, refSpecs, new FetchOptions
                    {
                        TagFetchMode = fetchSettings.TagFetchMode,
                        Prune = fetchSettings.Prune
                    }, string.Empty);
                });
        }
    }

    /// <summary>
    /// Settings for the fetch operation.
    /// </summary>
    public class GitFetchSettings
    {
        /// <summary>
        /// Gets or sets the name of the remote to fetch from.
        /// Default is <c>"origin"</c>.
        /// </summary>
        public string Remote { get; set; } = "origin";
        
        /// <summary>
        /// Gets or sets a filter which RefSpecs to fetch.
        /// Default is all.
        /// </summary>
        public Func<RefSpec, bool> RefSpecFilter { get; set; } = _ => true;

        /// <summary>
        /// Gets or sets how tags are being fetched.
        /// </summary>
        /// <seealso cref="FetchOptions.TagFetchMode"/>
        public TagFetchMode? TagFetchMode { get; set; }
        
        /// <summary>
        /// Gets or sets whether to prune during fetch.
        /// Default is <c>null</c>
        /// </summary>
        /// <seealso cref="FetchOptions.Prune"/>
        public bool? Prune { get; set; }
    }
}