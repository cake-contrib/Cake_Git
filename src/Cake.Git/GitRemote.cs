using LibGit2Sharp;
// ReSharper disable MemberCanBePrivate.Global

namespace Cake.Git
{
    /// <summary>
    /// Representation of a Git remote.
    /// </summary>
    public sealed class GitRemote
    {
        /// <summary>
        /// Gets the Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the push url.
        /// </summary>
        public string PushUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRemote"/> class.
        /// </summary>
        /// <param name="name">
        /// The remote name.
        /// </param>
        /// <param name="pushUrl">
        /// The push url.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public GitRemote(string name, string pushUrl, string url)
        {
            Name = name;
            Url = url;
            PushUrl = pushUrl;
        }

        /// <summary>
        /// Generates a string representation of <see cref="GitRemote"/>
        /// </summary>
        /// <returns><see cref="GitRemote"/> as string</returns>
        public override string ToString()
        {
            return $"( Name: {Name}, Url: {Url}, PushUrl: {PushUrl} )";
        }
    }
}