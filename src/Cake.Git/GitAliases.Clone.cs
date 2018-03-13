using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using LibGit2Sharp;
using System;
using System.IO;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Clone unauthenticated using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitClone("https://github.com/cake-build/cake.git", "c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository.</param>
        /// <param name="workDirectoryPath">Local path to clone into.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException">If any of the arguments are null.</exception>
        /// <exception cref="DirectoryNotFoundException">If parent directory doesnt exist.</exception>
        /// <exception cref="IOException">If workDirectoryPath already exists.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        public static DirectoryPath GitClone(
            this ICakeContext context,
            string sourceUrl,
            DirectoryPath workDirectoryPath
            )
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
            var parentFullDirectoryPath = workFullDirectoryPath.Combine("../").Collapse();

            if (!context.FileSystem.Exist(parentFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find {nameof(parentFullDirectoryPath)}: {parentFullDirectoryPath}");
            }

            if (context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new IOException($"{nameof(workFullDirectoryPath)} already exists: {workFullDirectoryPath}");
            }

            context.FileSystem.GetDirectory(workFullDirectoryPath).Create();

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath);
        }

        /// <summary>
        /// Clone unauthenticated using specific settings.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitClone("https://github.com/cake-build/cake.git", "c:/temp/cake", 
        ///         new GitCloneSettings{ BranchName = "main" });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository.</param>
        /// <param name="workDirectoryPath">Local path to clone into.</param>
        /// <param name="cloneSettings">The clone settings.</param>
        /// <returns>
        /// The path to the created repository.
        /// </returns>
        /// <exception cref="ArgumentNullException">If any of the arguments are null.</exception>
        /// <exception cref="DirectoryNotFoundException">If parent directory doesnt exist.</exception>
        /// <exception cref="IOException">If workDirectoryPath already exists.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        public static DirectoryPath GitClone(
            this ICakeContext context,
            string sourceUrl,
            DirectoryPath workDirectoryPath,
            GitCloneSettings cloneSettings
            )
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

            cloneSettings = cloneSettings ?? new GitCloneSettings();

            var workFullDirectoryPath = workDirectoryPath.MakeAbsolute(context.Environment);
            var parentFullDirectoryPath = workFullDirectoryPath.Combine("../").Collapse();

            if (!context.FileSystem.Exist(parentFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find {nameof(parentFullDirectoryPath)}: {parentFullDirectoryPath}");
            }

            if (context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new IOException($"{nameof(workFullDirectoryPath)} already exists: {workFullDirectoryPath}");
            }

            context.FileSystem.GetDirectory(workFullDirectoryPath).Create();

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath, cloneSettings.ToCloneOptions());
        }

        /// <summary>
        /// Clone authenticated using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitClone("https://github.com/cake-build/cake.git", 
        ///         "c:/temp/cake", 
        ///         "username", 
        ///         "password");
        /// </code>
        /// </example>
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
        public static DirectoryPath GitClone(
            this ICakeContext context,
            string sourceUrl,
            DirectoryPath workDirectoryPath,
            string username,
            string password
            )
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
                    (url, user, cred) => new UsernamePasswordCredentials { Username = username, Password = password }
            };

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath, options);
        }

        /// <summary>
        /// Clone authenticated using specific settings.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitClone("https://github.com/cake-build/cake.git", 
        ///         "c:/temp/cake", 
        ///         "username", 
        ///         "password",
        ///         new GitCloneSettings{ BranchName = "main" });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="sourceUrl">URI for the remote repository.</param>
        /// <param name="workDirectoryPath">Local path to clone into.</param>
        /// <param name="username">Username used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <param name="cloneSettings">The clone settings.</param>
        /// <returns>
        /// The path to the created repository.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">Failed to find workDirectoryPath: {workFullDirectoryPath}</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Clone")]
        public static DirectoryPath GitClone(
            this ICakeContext context,
            string sourceUrl,
            DirectoryPath workDirectoryPath,
            string username,
            string password,
            GitCloneSettings cloneSettings
            )
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

            cloneSettings = cloneSettings ?? new GitCloneSettings();

            var workFullDirectoryPath = workDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            var options = cloneSettings.ToCloneOptions();
            options.CredentialsProvider =
                (url, user, cred) => new UsernamePasswordCredentials { Username = username, Password = password };

            return Repository.Clone(sourceUrl, workFullDirectoryPath.FullPath, options);
        }
    }
}
