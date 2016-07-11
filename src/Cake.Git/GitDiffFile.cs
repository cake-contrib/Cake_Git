using LibGit2Sharp;
// ReSharper disable MemberCanBePrivate.Global

namespace Cake.Git
{
    /// <summary>
    /// Represents a Git diff file entry
    /// </summary>
    public class GitDiffFile
    {
        /// <summary>
        /// The new path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The old path.
        /// </summary>
        public string OldPath { get; }

        /// <summary>
        /// The kind of change that has been done (added, deleted, modified ...).
        /// </summary>
        public GitChangeKind Status { get; }

        /// <summary>
        /// The file exists in the new side of the diff.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// The file exists in the old side of the diff.
        /// </summary>
        public bool OldExists { get; }

        internal GitDiffFile(TreeEntryChanges change)
        {
            Path = change.Path;
            OldPath = change.OldPath;
            Status = (GitChangeKind)change.Status;
            Exists = change.Exists;
            OldExists = change.OldExists;
        }

        /// <summary>
        /// Generates a string representation of the <see cref="GitDiffFile"/>
        /// </summary>
        /// <returns><see cref="GitDiffFile"/> as string</returns>
        public override string ToString()
        {
            return $"Path: {Path}, OldPath: {OldPath}, Status: {Status}, Exists: {Exists}, OldExists: {OldExists}";
        }
    }
}