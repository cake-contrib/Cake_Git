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

        internal GitCommit(Commit commit)
        {
            Sha = commit.Sha;
            Author = new GitSignature(commit.Author.Email, commit.Author.Name, commit.Author.When);
            Committer = new GitSignature(commit.Committer.Email, commit.Committer.Name, commit.Committer.When);
            Message = commit.Message;
            MessageShort = commit.MessageShort;
        }
    }
}