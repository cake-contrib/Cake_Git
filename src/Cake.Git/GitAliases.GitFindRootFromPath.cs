using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using LibGit2Sharp;

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Finding git root path from subtree.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">Path to probe the repository.</param>
        /// <returns>The path to the repository root.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        /// <exception cref="RepositoryNotFoundException">If git root path not found.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("FindRootFromPath")]
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
                var parentDir = fsPath.Combine("../").Collapse();
                if (!context.FileSystem.Exist(parentDir))
                    throw new RepositoryNotFoundException($"Path '{path}' doesn't point at a valid Git repository or workdir.");
                fsPath = parentDir;
            }
            while (true);
        }
    }
}