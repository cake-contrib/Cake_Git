using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Cake.Git
{
    /// <summary>
    /// Git Commit Author / Committer.
    /// </summary>
    public sealed class GitSignature
    {
        /// <summary>
        /// Email address of author / committer.
        /// </summary>
        public string Email { get; }
        /// <summary>
        /// Name of author / commiter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// When Committed / Authored
        /// </summary>
        public DateTimeOffset When { get; }

        /// <summary>
        /// Generates a string representation of the <see cref="GitSignature"/>
        /// </summary>
        /// <returns><see cref="GitSignature"/> as string</returns>
        public override string ToString()
        {
            return $"Email: {Email}, Name: {Name}, When: {When}";
        }

        internal GitSignature(string email, string name, DateTimeOffset when)
        {
            Email = email;
            Name = name;
            When = when;
        }
    }
}