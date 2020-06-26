using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;

namespace Cake.Git
{
    partial class GitAliases
    {
        /// <summary>
        /// Gets a list of all branches from the repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitBranches("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Branches")]
        public static ICollection<GitBranch> GitBranches(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (repositoryDirectoryPath == null)
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));

            return
                context.UseRepository(
                    repositoryDirectoryPath,
                    repository =>
                        repository
                            .Branches
                            .Select(branch => new GitBranch(repository, branch))
                            .ToList());
        }
    }
}