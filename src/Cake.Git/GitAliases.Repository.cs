using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    public static partial class GitAliases
    { 
        /// <summary>
        /// Checks if a specific directory is a valid Git repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitIsValidRepository("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to the repository to check.</param>
        /// <returns>True if the path is part of a valid Git Repository.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If path doesn't exist.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Repository")]
        public static bool GitIsValidRepository(this ICakeContext context, DirectoryPath path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!context.FileSystem.Exist(path))
            {
                throw new RepositoryNotFoundException($"Path '{path}' doesn't exists.");
            }

            return Repository.IsValid(path.FullPath);
        }

        /// <summary>
        /// Checks if a repository contains uncommited changes.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitHasUncommitedChanges("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to the repository to check.</param>
        /// <returns>True if the Git repository contains uncommited changes.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If path doesn't exist.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Repository")]
        public static bool GitHasUncommitedChanges(this ICakeContext context, DirectoryPath path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!context.FileSystem.Exist(path))
            {
                throw new RepositoryNotFoundException($"Path '{path}' doesn't exists.");
            }

            return context.UseRepository(
                path,
                repository => repository.RetrieveStatus().IsDirty);
        }

        /// <summary>
        /// Checks if a repository contains staged changes.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitHasStagedChanges("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to the repository to check.</param>
        /// <returns>True if the Git repository contains staged changes.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If path doesn't exist.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Repository")]
        public static bool GitHasStagedChanges(this ICakeContext context, DirectoryPath path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!context.FileSystem.Exist(path))
            {
                throw new RepositoryNotFoundException($"Path '{path}' doesn't exists.");
            }

            return context.UseRepository(
                path,
                repository => repository.RetrieveStatus().Staged.Any());
        }

        /// <summary>
        /// Checks if a repository contains untracked files.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitHasStagedChanges("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to the repository to check.</param>
        /// <returns>True if the Git repository contains staged changes.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If path doesn't exist.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Repository")]
        public static bool GitHasUntrackedFiles(this ICakeContext context, DirectoryPath path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!context.FileSystem.Exist(path))
            {
                throw new RepositoryNotFoundException($"Path '{path}' doesn't exists.");
            }
            return context.UseRepository(
                path,
                repository => repository.RetrieveStatus().Untracked.Any());
        }

        /// <summary>
        /// Finding git root path from subtree.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitFindRootFromPath("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to probe the repository.</param>
        /// <returns>The path to the repository root.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If git root path not found.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Repository")]
        public static DirectoryPath GitFindRootFromPath(this ICakeContext context, DirectoryPath path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!context.FileSystem.Exist(path))
            {
                throw new RepositoryNotFoundException($"Path '{path}' doesn't exists.");
            }

            var fsPath = path;
            do
            {
                if (Repository.IsValid(fsPath.FullPath))
                    return fsPath;
                var parentDir = GetParent(context, fsPath);
                if (!context.FileSystem.Exist(parentDir) || GetParent(context, parentDir) == null)
                    throw new RepositoryNotFoundException($"Path '{path}' doesn't point at a valid Git repository or workdir.");
                fsPath = parentDir;
            }
            while (true);
        }
        
        // replace with DirectoryPath.GetParent(), once https://github.com/cake-build/cake/pull/3349/files
        // ReSharper disable once InconsistentNaming
        private static DirectoryPath GetParent(ICakeContext context, DirectoryPath path)
        {
            path = path.MakeAbsolute(context.Environment); // to be sure. It's no use splitting the segments of "."

            if (path.Segments.Length == 1)
            {
                // one segment on Windows is e.g. "C:/" 
                // on all other systems one segment is e.g "/home"
                if (context.Environment.Platform.Family == PlatformFamily.Windows) 
                {
                    // no more parents
                    return null;
                }

                // root ("/") is not really a segment for Cake, 
                // so we return that directly.
                return new DirectoryPath("/");
            }
            
            if(path.Segments.Length == 0) 
            {
                return null;
            }

            var segments = path.Segments.Take(path.Segments.Length - 1);
            return new DirectoryPath(string.Join(path.Separator.ToString(), segments));
        }
    }
}