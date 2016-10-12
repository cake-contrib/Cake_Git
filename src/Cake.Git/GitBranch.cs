using LibGit2Sharp;
// ReSharper disable MemberCanBePrivate.Global

namespace Cake.Git
{
    /// <summary>
    /// Representation of a Git branch.
    /// </summary>
    public sealed class GitBranch
    {
        /// <summary>
        /// Gets the full name of the branch.
        /// </summary>
        /// <value>The full name of the branch.</value>
        public string CanonicalName { get; }

        /// <summary>
        /// Gets the human-friendly name of the branch.
        /// </summary>
        /// <value>The human-friendly name of the branch.</value>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets the commit this branch points to.
        /// </summary>
        /// <value>The commit this branch points to.</value>
        public GitCommit Tip { get; }

        /// <summary>
        /// Gets a value indicating whether this branch is remote.
        /// </summary>
        /// <value>The a value indicating whether this branch is remote.</value>
        public bool IsRemote { get; }

        /// <summary>
        /// Gets the remote name for this branch.
        /// </summary>
        /// <value>The remote name for this branch.</value>
        public string RemoteName { get; }

        /// <summary>
        /// Gets or sets the remotes.
        /// </summary>
        public System.Collections.Generic.List<GitRemote> Remotes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitBranch"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GitBranch(Repository repository)
        {
            CanonicalName = repository.Head.CanonicalName;
            FriendlyName = repository.Head.FriendlyName;
            Tip = new GitCommit(repository.Head.Tip);
            IsRemote = repository.Head.IsRemote;
            RemoteName = repository.Head.RemoteName;
            Remotes = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(repository.Network.Remotes, remote => new GitRemote(remote.Name, remote.PushUrl, remote.Url)));
        }

        /// <summary>
        /// Generates a string representation of <see cref="GitBranch"/>
        /// </summary>
        /// <returns><see cref="GitBranch"/> as string</returns>
        public override string ToString()
        {
            return $"Canonical name: {CanonicalName}, Friendly name: {FriendlyName}, Tip: ({Tip}), IsRemote: ({IsRemote}), RemoteName: ({RemoteName}), Remotes: [{System.String.Join(", ", Remotes)}]";
        }
    }
}
