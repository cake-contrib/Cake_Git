using LibGit2Sharp;

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
        /// Initializes a new instance of the <see cref="GitBranch"/> class.
        /// </summary>
        /// <param name="branch">The branch.</param>
        public GitBranch(Branch branch)
        {
            CanonicalName = branch.CanonicalName;
            FriendlyName = branch.FriendlyName;
            Tip = new GitCommit(branch.Tip);
        }

        /// <summary>
        /// Generates a string representation of <see cref="GitBranch"/>
        /// </summary>
        /// <returns><see cref="GitBranch"/> as string</returns>
        public override string ToString()
        {
            return $"Canonical name: {CanonicalName}, Friendly name: {FriendlyName}, Tip: ({Tip})";
        }
    }
}
