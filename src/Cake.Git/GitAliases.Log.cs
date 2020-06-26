using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Get last commit
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitLogTip("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns>Last commit in the repository passed in <paramref name="repositoryDirectoryPath"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Log")]
        public static GitCommit GitLogTip(
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

            return context.UseRepository(
                repositoryDirectoryPath,
                repository => new GitCommit(repository.Head.Tip)
                );
        }

        /// <summary>
        /// Get commit log.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitLog("c:/temp/cake", 5);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="count">Number of commits to fetch.</param>
        /// <returns>Commits in the repository passed in <paramref name="repositoryDirectoryPath"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Log")]
        public static ICollection<GitCommit> GitLog(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            int count
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

            return context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.Commits
                    .Take(count)
                    .Select(commit => new GitCommit(commit))
                    .ToList()
                );
        }

        /// <summary>
        /// Get commit from certain commit id up to current.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitLog("c:/temp/cake", "since commit id");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="sinceCommitId">Commit id to start fetching from.</param>
        /// <returns>Commits in the repository passed in <paramref name="repositoryDirectoryPath"/> which have <paramref name="sinceCommitId"/> as parent.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Log")]
        public static ICollection<GitCommit> GitLog(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string sinceCommitId
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

            if (string.IsNullOrWhiteSpace(sinceCommitId))
            {
                throw new ArgumentNullException(nameof(sinceCommitId));
            }

            return context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    return repository.Commits
                        .QueryBy(new CommitFilter
                        {
                            IncludeReachableFrom = repository.Head,
                            ExcludeReachableFrom = repository.Lookup<Commit>(sinceCommitId).Parents
                        })
                        .Select(commit => new GitCommit(commit))
                        .ToList();
                }
                );
        }

        /// <summary>
        /// Get commits after a certain tag
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitLogTag("c:/temp/cake", "since tag");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="sinceTag">Tag to start fetching from.</param>
        /// <returns>Commits since the tag passed with <paramref name="sinceTag"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Log")]
        public static ICollection<GitCommit> GitLogTag(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string sinceTag
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

            if (string.IsNullOrWhiteSpace(sinceTag))
            {
                throw new ArgumentNullException(nameof(sinceTag));
            }

            return context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    return repository.Commits
                        .QueryBy(new CommitFilter
                        {
                            IncludeReachableFrom = repository.Head,
                            ExcludeReachableFrom = repository.Tags[sinceTag].Target.Sha
                        })
                        .Select(commit => new GitCommit(commit))
                        .ToList();
                }
                );
        }

        /// <summary>
        /// Get specific commit.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitLogLookup("c:/temp/cake", "commit id");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="commitId">Commit id to lookup.</param>
        /// <returns>Commits with ID <paramref name="commitId"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Log")]
        public static GitCommit GitLogLookup(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string commitId
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

            if (string.IsNullOrWhiteSpace(commitId))
            {
                throw new ArgumentNullException(nameof(commitId));
            }

            return context.UseRepository(
                repositoryDirectoryPath,
                repository =>new GitCommit(repository.Lookup<Commit>(commitId))
                );
        }
    }
}
