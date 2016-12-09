using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Get changed files from inital commit id up to current.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitDiff("c:/temp/cake");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Diff")]
        public static ICollection<GitDiffFile> GitDiff(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath
            )
        {
            return context.GitDiff(
                repositoryDirectoryPath,
                Git.GitCommit.RootCommit,
                null
                );
        }

        /// <summary>
        /// Get changed files from certain commit id up to current.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitDiff("c:/temp/cake", "since commit id");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="sinceCommitId">Commit id to start fetching from.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Diff")]
        public static ICollection<GitDiffFile> GitDiff(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string sinceCommitId
            )
        {
            return context.GitDiff(
                repositoryDirectoryPath,
                sinceCommitId,
                null
                );
        }

        /// <summary>
        /// Get changed files from certain commit id up to current.
        /// </summary>
        /// <example>
        /// <code>
        ///     var result = GitDiff("c:/temp/cake", "since commit id", "to commit id");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="sinceCommitId">Commit id to start fetching from.</param>
        /// <param name="toCommitId">The commit id to compare to.</param>
        /// <returns>The path to the created repository.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Diff")]
        public static ICollection<GitDiffFile> GitDiff(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string sinceCommitId,
            string toCommitId
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
                        var initalCommit = new List<KeyValuePair<Tree, Tree>>();
                        if (sinceCommitId == Git.GitCommit.RootCommit)
                        {
                            var firstCommit = repository.Commits.Select(commit => commit).FirstOrDefault();
                            if (firstCommit == null)
                            {
                                return new GitDiffFile[0];
                            }
                            initalCommit.Add(
                                new KeyValuePair<Tree, Tree>(
                                    repository.ObjectDatabase.CreateTree(new TreeDefinition()),
                                    firstCommit.Tree
                                    )
                            );
                            sinceCommitId = firstCommit.Sha;
                        }

                        if (!string.IsNullOrWhiteSpace(toCommitId))
                        {
                            var fromCommitTree = sinceCommitId == Git.GitCommit.RootCommit
                                ? repository.ObjectDatabase.CreateTree(new TreeDefinition())
                                : repository.Lookup<Commit>(sinceCommitId)?.Tree;
                            var toCommitTree = repository.Lookup<Commit>(toCommitId)?.Tree;
                            return (
                                from change in repository.Diff.Compare<TreeChanges>(fromCommitTree, toCommitTree)
                                select new GitDiffFile(change)
                                ).ToArray();
                        }

                        return (
                            from trees in initalCommit.Concat(
                                from commit in repository.Commits.QueryBy(new CommitFilter
                                {
                                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                                    IncludeReachableFrom = sinceCommitId,
                                    ExcludeReachableFrom = repository.Lookup<Commit>(sinceCommitId)?.Parents
                                }) as IEnumerable<Commit> ?? new Commit[0]
                                from parent in commit.Parents
                                select new KeyValuePair<Tree, Tree>(parent.Tree, commit.Tree)
                                )
                            from change in repository.Diff.Compare<TreeChanges>(trees.Key, trees.Value)
                            select new GitDiffFile(change)
                            ).ToArray();
                    }
                    );
            //}
            //return context.UseRepository(
            //    repositoryDirectoryPath,
            //    repository => (
            //        from commit in repository.Commits.QueryBy(new CommitFilter
            //        {
            //            SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
            //            IncludeReachableFrom = sinceCommitId,
            //            ExcludeReachableFrom = repository.Lookup<Commit>(sinceCommitId)?.Parents
            //        }) as IEnumerable<Commit> ?? new Commit[0]
            //        from parent in commit.Parents
            //        from change in repository.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree)
            //        select new GitDiffFile(change)
            //        ).ToList());
        }
    }
}
