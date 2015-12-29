using LibGit2Sharp;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    /// <summary>
    ///  The status of what happened as a result of a merge.
    /// </summary>
    public enum GitMergeStatus
    {
        /// <summary>
        /// Repository was up to date
        /// </summary>
        UpToDate = MergeStatus.UpToDate,

        /// <summary>
        /// Repository was fast forward merged
        /// </summary>
        FastForward = MergeStatus.FastForward,

        /// <summary>
        /// Repository was not fast forward merged
        /// </summary>
        NonFastForward = MergeStatus.NonFastForward,

        /// <summary>
        /// Repository encountered conflicts during merge
        /// </summary>
        Conflicts = MergeStatus.Conflicts
    }
}