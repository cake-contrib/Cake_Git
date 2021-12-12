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
                    // ReSharper disable once ConvertToConstant.Local
                    var logMessage = "";
                    var remote = repository.Network.Remotes[remoteName];
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repository, remote.Name, refSpecs, null, logMessage);
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
                    // ReSharper disable once ConvertToConstant.Local
                    var logMessage = "";
                    var remote = repository.Network.Remotes[remoteName];
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repository, remote.Name, refSpecs, new FetchOptions
                    {
                        TagFetchMode = TagFetchMode.All
                    }, logMessage);
                });
        }
    }
}