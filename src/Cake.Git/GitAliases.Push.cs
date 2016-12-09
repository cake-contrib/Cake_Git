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
        /// <example>
        /// <code>
        ///     GitPush("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
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
        /// <example>
        /// <code>
        ///     GitPush("c:/temp/cake", "username", "password");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
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
        /// <example>
        /// <code>
        ///     GitPush("c:/temp/cake", "username", "password", "branch");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <param name="branchName">Name of branch to push.</param>
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

        /// <summary>
        /// Push a tag to a remote unauthenticated.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitPushRef("c:/temp/cake", "remote", "refSpec");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="remote">The <see cref="T:LibGit2Sharp.Remote"/> to push to.</param>
        /// <param name="pushRefSpec">The pushRefSpec to push.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Push")]
        public static void GitPushRef(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string remote, string pushRefSpec)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (remote == null)
            {
                throw new ArgumentNullException(nameof(remote));
            }

            if (string.IsNullOrWhiteSpace(remote))
            {
                throw new ArgumentException("Remote cannot be empty", nameof(remote));
            }

            if (pushRefSpec == null)
            {
                throw new ArgumentNullException(nameof(pushRefSpec));
            }

            if (string.IsNullOrWhiteSpace(pushRefSpec))
            {
                throw new ArgumentException("Pushrefspec cannot be empty", nameof(pushRefSpec));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.Network.Push(repository.Network.Remotes[remote],
                repository.Tags[pushRefSpec].CanonicalName)
                );
        }

        /// <summary>
        /// Push a tag to a remote authenticated.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitPushRef("c:/temp/cake", "username", "password", "remote", "refSpec");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <param name="remote">The <see cref="T:LibGit2Sharp.Remote"/> to push to.</param>
        /// <param name="pushRefSpec">The pushRefSpec to push.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Push")]
        public static void GitPushRef(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string username,
            string password,
            string remote, string pushRefSpec)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (remote == null)
            {
                throw new ArgumentNullException(nameof(remote));
            }

            if (string.IsNullOrWhiteSpace(remote))
            {
                throw new ArgumentException("Remote cannot be empty", nameof(remote));
            }

            if (pushRefSpec == null)
            {
                throw new ArgumentNullException(nameof(pushRefSpec));
            }

            if (string.IsNullOrWhiteSpace(pushRefSpec))
            {
                throw new ArgumentException("Pushrefspec cannot be empty", nameof(pushRefSpec));
            }

            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var options = new PushOptions
            {
                CredentialsProvider = (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials
                    {
                        Username = username,
                        Password = password
                    }
            };

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.Network.Push(repository.Network.Remotes[remote], 
                repository.Tags[pushRefSpec].CanonicalName, 
                options)
                );
        }
    }
}
