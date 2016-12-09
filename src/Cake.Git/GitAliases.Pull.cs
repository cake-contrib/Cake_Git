using System;
using System.IO;
using System.Runtime.CompilerServices;
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
        /// Pull unauthenticated using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitPull("c:/temp/cake", "name", "email");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="mergerName">The name of the merger.</param>
        /// <param name="mergerEmail">The email of the merger.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Pull")]
        public static GitMergeResult GitPull(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string mergerName,
            string mergerEmail
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

            var workFullDirectoryPath = repositoryDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            return new GitMergeResult(
                context.UseRepository(
                    repositoryDirectoryPath,
                    repository =>
                        Commands.Pull(
                            repository,
                            new Signature(
                                mergerName,
                                mergerEmail,
                                DateTimeOffset.Now
                                ),
                            new PullOptions()
                            )
                    )
                );
        }

        /// <summary>
        /// Pull authenticating using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitPull("c:/temp/cake", "name", "email", "username", "password", "remote");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="mergerName">The name of the merger.</param>
        /// <param name="mergerEmail">The email of the merger.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <param name="remoteName">Name of remote to pull from.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Pull")]
        public static GitMergeResult GitPull(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string mergerName,
            string mergerEmail,
            string username,
            string password,
            string remoteName
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

            var repositoryFullDirectoryPath = repositoryDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(repositoryFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find repositoryDirectoryPath: {repositoryFullDirectoryPath}");
            }

            return new GitMergeResult(
                context.UseRepository(
                    repositoryDirectoryPath,
                    repository =>
                    {
                        var remote = repository.Network.Remotes[remoteName];
                        if (remote == null)
                        {
                            throw new NotFoundException("Remote named {0} not found", remoteName);
                        }

                        Commands.Fetch(
                            repository,
                            remote.Name,
                            new string[0],
                            new FetchOptions
                            {
                                CredentialsProvider =
                                    (url, usernameFromUrl, types) =>
                                        new UsernamePasswordCredentials
                                        {
                                            Username = username,
                                            Password = password
                                        }
                            },
                            null
                            );

                        return repository.MergeFetchedRefs(
                            new Signature(
                                mergerName,
                                mergerEmail,
                                DateTimeOffset.Now
                                ),
                            new MergeOptions()
                            );
                    }
                    )
                );
        }
    }
}