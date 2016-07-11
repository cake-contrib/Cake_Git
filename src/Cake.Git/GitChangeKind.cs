using LibGit2Sharp;
// ReSharper disable UnusedMember.Global

namespace Cake.Git
{
    /// <summary>
    /// The kind of changes that a Diff can report.
    /// </summary>
    public enum GitChangeKind
    {
        /// <summary>
        /// File is not modified.
        /// </summary>
        Unmodified  = ChangeKind.Unmodified,

        /// <summary>
        /// File is added.
        /// </summary>
        Added       = ChangeKind.Added,

        /// <summary>
        /// File is deleted.
        /// </summary>
        Deleted     = ChangeKind.Deleted,

        /// <summary>
        /// File is modified.
        /// </summary>
        Modified    = ChangeKind.Modified,

        /// <summary>
        /// File is renamed.
        /// </summary>
        Renamed     = ChangeKind.Renamed,

        /// <summary>
        /// File is copied.
        /// </summary>
        Copied      = ChangeKind.Copied,

        /// <summary>
        /// File is ignored.
        /// </summary>
        Ignored     = ChangeKind.Ignored,

        /// <summary>
        /// File is untracked.
        /// </summary>
        Untracked   = ChangeKind.Untracked,

        /// <summary>
        /// File type changed.
        /// </summary>
        TypeChanged = ChangeKind.TypeChanged,

        /// <summary>
        /// File is unreadable.
        /// </summary>
        Unreadable  = ChangeKind.Unreadable,

        /// <summary>
        /// File is conflicting.
        /// </summary>
        Conflicted  = ChangeKind.Conflicted
    }
}