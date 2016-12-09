using System;
using System.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using LibGit2Sharp;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Init using default options.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitInit("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="initDirectoryPath">The path to the working folder when initializing a standard ".git" repository.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="DirectoryNotFoundException">If initDirectoryPath doesn't exist.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Init")]
        public static DirectoryPath GitInit(
            this ICakeContext context,
            DirectoryPath initDirectoryPath
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (initDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(initDirectoryPath));
            }

            var workFullDirectoryPath = initDirectoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(workFullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find workDirectoryPath: {workFullDirectoryPath}");
            }

            return Repository.Init(workFullDirectoryPath.FullPath);
        }
    }
}
