using LibGit2Sharp;

namespace Cake.Git
{
    /// <summary>
    /// Contains settings used by GitClone.
    /// </summary>
    public class GitCloneSettings
    {
        /// <summary>
        /// True will result in a bare clone, false a full clone.
        /// </summary>
        public bool IsBare { get; set; }

        /// <summary>
        /// If true, the origin's HEAD will be checked out. This only applies
        /// to non-bare repositories.
        /// </summary>
        public bool Checkout { get; set; } = true;

        /// <summary>
        /// The name of the branch to checkout. When unspecified the
        /// remote's default branch will be used instead.
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Recursively clone submodules.
        /// </summary>
        public bool RecurseSubmodules { get; set; }

        internal CloneOptions ToCloneOptions()
        {
            return new CloneOptions
            {
                IsBare = this.IsBare,
                Checkout = this.Checkout,
                BranchName = this.BranchName,
                RecurseSubmodules = this.RecurseSubmodules
            };
        }
    }
}