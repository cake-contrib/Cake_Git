using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
// ReSharper disable UnusedMember.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Describe current branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = GitDescribe(".");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <returns>Described commit using the most recent tag reachable from it.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        public static string GitDescribe(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath)
        {
            return GitDescribe(context,
                repositoryDirectoryPath,
                strategy: GitDescribeStrategy.All,
                renderLongFormat: false,
                minimumCommitIdAbbreviatedSize: null);
        }

        /// <summary>
        /// Describe current branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = GitDescribe(".", GitDescribeStrategy.Tags);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="strategy">Describing strategy.</param>
        /// <returns>Described commit using the most recent tag reachable from it.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        public static string GitDescribe(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            GitDescribeStrategy strategy)
        {
            return GitDescribe(context,
                repositoryDirectoryPath,
                renderLongFormat: false,
                strategy: strategy,
                minimumCommitIdAbbreviatedSize: null);
        }

        /// <summary>
        /// Describe current branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = GitDescribe(".", false, GitDescribeStrategy.Tags);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="renderLongFormat">
        ///   Always output the long format (the tag, the number of commits and the abbreviated commit name)
        ///   even when it matches a tag. This is useful when you want to see parts of the commit object name
        ///   in "describe" output, even when the commit in question happens to be a tagged version.
        ///   Instead of just emitting the tag name, it will describe such a commit as v1.2-0-gdeadbee
        ///   (0th commit since tag v1.2 that points at object deadbee...).
        /// </param>
        /// <param name="strategy">Describe strategy.</param>
        /// <returns>Described commit using the most recent tag reachable from it.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        public static string GitDescribe(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            bool renderLongFormat,
            GitDescribeStrategy strategy)
        {
            return GitDescribe(context,
                repositoryDirectoryPath,
                renderLongFormat: renderLongFormat,
                strategy: strategy,
                minimumCommitIdAbbreviatedSize: null);
        }

        /// <summary>
        /// Describe current branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = GitDescribe(".", false, GitDescribeStrategy.Tags, 0);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="renderLongFormat">
        ///   Always output the long format (the tag, the number of commits and the abbreviated commit name)
        ///   even when it matches a tag. This is useful when you want to see parts of the commit object name
        ///   in "describe" output, even when the commit in question happens to be a tagged version.
        ///   Instead of just emitting the tag name, it will describe such a commit as v1.2-0-gdeadbee
        ///   (0th commit since tag v1.2 that points at object deadbee...).
        /// </param>
        /// <param name="strategy">Describe strategy.</param>
        /// <param name="minimumCommitIdAbbreviatedSize">Number of minimum hexadecimal digits used to render a uniquely abbreviated commit id.</param>
        /// <returns>Described commit using the most recent tag reachable from it.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        public static string GitDescribe(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            bool renderLongFormat,
            GitDescribeStrategy strategy,
            int? minimumCommitIdAbbreviatedSize)
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
                repository => GitDescribeImpl(context,
                    repository,
                    repository.Head.Commits.First(),
                    renderLongFormat,
                    strategy,
                    minimumCommitIdAbbreviatedSize));
        }

        /// <summary>
        /// Describe specified commit-ish.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = GitDescribe(".", "master", false, GitDescribeStrategy.Tags, 0);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="commitishName">Commit-ish name.</param>
        /// <param name="renderLongFormat">
        ///   Always output the long format (the tag, the number of commits and the abbreviated commit name)
        ///   even when it matches a tag. This is useful when you want to see parts of the commit object name
        ///   in "describe" output, even when the commit in question happens to be a tagged version.
        ///   Instead of just emitting the tag name, it will describe such a commit as v1.2-0-gdeadbee
        ///   (0th commit since tag v1.2 that points at object deadbee...).
        /// </param>
        /// <param name="strategy">Describe strategy.</param>
        /// <param name="minimumCommitIdAbbreviatedSize">Number of minimum hexadecimal digits used to render a uniquely abbreviated commit id.</param>
        /// <returns>Described commit using the most recent tag reachable from it.</returns>
        /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        public static string GitDescribe(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string commitishName,
            bool renderLongFormat,
            GitDescribeStrategy strategy,
            int? minimumCommitIdAbbreviatedSize)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (commitishName == null)
            {
                throw new ArgumentNullException(nameof(commitishName));
            }

            return context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    var commit = repository.Lookup(commitishName, ObjectType.Commit) as Commit;
                    if (commit == null)
                        throw new NotFoundException("Commit not found: " + commitishName);
                    return GitDescribeImpl(context,
                        repository,
                        commit,
                        renderLongFormat,
                        strategy,
                        minimumCommitIdAbbreviatedSize);
                });
        }

        [CakeMethodAlias]
        [CakeAliasCategory("Describe")]
        private static string GitDescribeImpl(
            ICakeContext context,
            IRepository repository,
            Commit commit,
            bool renderLongFormat,
            GitDescribeStrategy strategy,
            int? minimumCommitIdAbbreviatedSize)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (commit == null)
            {
                throw new ArgumentNullException(nameof(commit));
            }

            try
            {
                var describeOptions = new DescribeOptions
                {
                    AlwaysRenderLongFormat = renderLongFormat,
                    Strategy = ConvertDescribeStrategy(strategy)
                };
                if (minimumCommitIdAbbreviatedSize.HasValue)
                    describeOptions.MinimumCommitIdAbbreviatedSize = minimumCommitIdAbbreviatedSize.Value;
                return repository.Describe(commit, describeOptions);
            }
            catch (Exception ex)
            {
                context.Log.Warning(Verbosity.Normal, "Describe failed, empty string returned:\n{0}", ex);
                return "";
            }
        }

        private static DescribeStrategy ConvertDescribeStrategy(GitDescribeStrategy source)
        {
            switch (source)
            {
                case GitDescribeStrategy.Default: return DescribeStrategy.Default;
                case GitDescribeStrategy.Tags: return DescribeStrategy.Tags;
                case GitDescribeStrategy.All: return DescribeStrategy.All;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, "Unknown git describe strategy");
            }
        }
    }
}