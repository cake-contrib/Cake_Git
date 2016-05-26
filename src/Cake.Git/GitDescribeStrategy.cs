namespace Cake.Git
{
    /// <summary>
    /// Specify the kind of committish which will be considered
    /// when trying to identify the closest reference to the described commit.
    /// </summary>
    public enum GitDescribeStrategy
    {
        /// <summary>
        /// Only annotated tags will be considered as reference points
        /// </summary>
        Default,

        /// <summary>
        /// Instead of using only the annotated tags, use any tag found
        /// in refs/tags namespace. This option enables matching
        /// a lightweight (non-annotated) tag.
        /// </summary>
        Tags,

        /// <summary>
        /// Instead of using only the annotated tags, use any ref found in
        /// refs/ namespace. This option enables matching any known branch,
        /// remote-tracking branch, or lightweight tag.
        /// </summary>
        All
    }
}
