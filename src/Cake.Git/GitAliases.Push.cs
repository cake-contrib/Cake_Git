using System;
using System.IO;
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
        /// Push all branches unauthenticated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Push")]
        public static void GitPush(
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

            var workFullDirectoryPath = repositoryDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                    repository.Network.Push(
                        repository.Branches
                        )
                );
        }

        /// <summary>
        /// Push all branches authenticated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Push")]
        public static void GitPush(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string username,
            string password
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

            context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                    repository.Network.Push(
                        repository.Branches,
                        new PushOptions
                        {
                            CredentialsProvider =
                                (url, usernameFromUrl, types) =>
                                    new UsernamePasswordCredentials
                                    {
                                        Username = username,
                                        Password = password
                                    }
                        }
                        )
                );
        }

        /// <summary>
        /// Push specific branch authenticated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <param name="branchName">Name of branch to push.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Push")]
        public static void GitPush(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string username,
            string password,
            string branchName
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

            context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    var branch = repository.Branches[branchName];
                    if (branch == null)
                    {
                        throw new NotFoundException("Failed to find branch {0}", branchName);
                    }

                    repository.Network.Push(
                        branch,
                        new PushOptions
                        {
                            CredentialsProvider =
                                (url, usernameFromUrl, types) =>
                                    new UsernamePasswordCredentials
                                    {
                                        Username = username,
                                        Password = password
                                    }
                        }
                        );
                }
                );
        }
    }
}