using LibGit2Sharp;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    /// <summary>
    /// The mode to be used when resetting the branch head 
    /// and, optionally, the index and the working tree.
    /// </summary>
    public enum GitResetMode
    {
        /// <summary>
        /// Resets the index and working tree. Any changes to tracked 
        /// files in the working tree since the latest or specified commit are discarded.
        /// </summary>
        Hard = ResetMode.Hard,
        /// <summary>
        /// Resets the index but not the working tree (i.e., the changed files are 
        /// preserved but not marked for commit).
        /// </summary>
        Soft = ResetMode.Soft,
        /// <summary>
        /// Resets the index and working tree. Any changes to tracked files in the 
        /// working tree since the latest or specified commit are discarded.
        /// </summary>
        Mixed = ResetMode.Mixed
    }
}