using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;
using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Get the short version of a commit SHA.
        /// </summary>
        /// <example>
        /// <code>
        /// var commit = GitLogTip("path/to/repo");
        /// var shortSha = GitShortenSha("path/to/repo", commit);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="commit">The Commit whose Sha should be shortened.</param>
        /// <param name="minimalLength">The minimal length of the shortened SHA. The default is 7 (seven) characters.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Sha")]
        public static string GitShortenSha(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            GitCommit commit,
            int? minimalLength = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }
            
            if (commit == null)
            {
                throw new ArgumentNullException(nameof(commit));
            }

            string shortSha = null;
            
            context.UseRepository(
                repositoryDirectoryPath,
                repository =>
                {
                    var obj = repository.Lookup(commit.Sha, ObjectType.Commit);
                    shortSha = minimalLength.HasValue 
                        ? repository.ObjectDatabase.ShortenObjectId(obj, minimalLength.Value) 
                        : repository.ObjectDatabase.ShortenObjectId(obj);
                }
            );
            
            return shortSha;
        }


    }
}