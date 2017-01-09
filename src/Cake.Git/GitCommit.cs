using System;
using LibGit2Sharp;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Cake.Git
{
    /// <summary>
    /// Representation of a Git commit.
    /// </summary>
    public sealed class GitCommit
    {
        /// <summary>
        /// Commit Id for bare repository
        /// </summary>
        public const string RootCommit = "4b825dc642cb6eb9a060e54bf8d69288fbee4904";

        /// <summary>
        /// Sha Id of commit.
        /// </summary>
        public string Sha { get; }

        /// <summary>
        /// Commit author.
        /// </summary>
        public GitSignature Author { get; }

        /// <summary>
        /// Commit comitter.
        /// </summary>
        public GitSignature Committer { get; }

        /// <summary>
        /// Commit message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Commit short message.
        /// </summary>
        public string MessageShort { get; }

        /// <summary>
        /// Generates a string representation of <see cref="GitCommit"/>
        /// </summary>
        /// <returns><see cref="GitCommit"/> as string</returns>
        public override string ToString()
        {
            return $"Sha: {Sha}, Author: {{{Author}}}, Committer: {{{Committer}}}, Message: {Message}, MessageShort: {MessageShort}";
        }

        internal GitCommit(Commit commit)
        {
            if (commit == null)
            {
                throw new ArgumentException("Source commit can't be null.", nameof(commit));
            }

            Sha = commit.Sha;
            Author = new GitSignature(commit.Author.Email, commit.Author.Name, commit.Author.When);
            Committer = new GitSignature(commit.Committer.Email, commit.Committer.Name, commit.Committer.When);
            Message = commit.Message;
            MessageShort = commit.MessageShort;
        }
    }
}