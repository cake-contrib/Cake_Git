using System;
using System.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using LibGit2Sharp;

namespace Cake.Git
{
    /// <summary>
    /// Contains functionality for working with GIT using LibGit2 &amp;
    /// LibGit2Sharp
    /// </summary>
    [CakeAliasCategory("Git")]
    // ReSharper disable UnusedMember.Global
    public static class GitAliases
    {
        /// <summary>
        /// Clone using default options.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository</param>
        /// <param name="workDirectoryPath">Local path to clone into</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        [CakeNamespaceImport("LibGit2Sharp")]
        public static DirectoryPath GitClone(this ICakeContext context, string sourceUrl,
            DirectoryPath workDirectoryPath)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                throw new ArgumentNullException(nameof(sourceUrl));
            }

            if (workDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(workDirectoryPath));
            }

            var workFullDirectoryPath = workDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath);
        }

        /// <summary>
        /// Clone using specified options.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository</param>
        /// <param name="workDirectoryPath">Local path to clone into</param>
        /// <param name="options"><see cref="T:LibGit2Sharp.CloneOptions"/> controlling clone behavior</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>The path to the created repository.</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        [CakeNamespaceImport("LibGit2Sharp")]
        public static DirectoryPath GitClone(this ICakeContext context, string sourceUrl,
            DirectoryPath workDirectoryPath, CloneOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                throw new ArgumentNullException(nameof(sourceUrl));
            }

            if (workDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(workDirectoryPath));
            }

            var workFullDirectoryPath = workDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath, options);
        }

        /// <summary>
        /// Clone using specified options.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository.</param>
        /// <param name="workDirectoryPath">Local path to clone into.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>The path to the created repository.</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        [CakeNamespaceImport("LibGit2Sharp")]
        public static DirectoryPath GitClone(this ICakeContext context, string sourceUrl,
            DirectoryPath workDirectoryPath, string username, string password)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                throw new ArgumentNullException(nameof(sourceUrl));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (workDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(workDirectoryPath));
            }

            var workFullDirectoryPath = workDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            var options = new CloneOptions
            {
                CredentialsProvider =
                    (url, user, cred) => new UsernamePasswordCredentials {Username = username, Password = password}
            };

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath, options);
        }
    }
}