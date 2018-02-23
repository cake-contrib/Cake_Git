using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;
using LibGit2Sharp;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Applys tagName to repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitTag("c:/temp/cake", "tag name");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="tagName">The tag name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tag")]
        public static void GitTag(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string tagName
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository =>repository.ApplyTag(tagName)
                );
        }

        /// <summary>
        /// Applys tagName to repository.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitTag("c:/temp/cake", "tag name", "objectish");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="objectish">The revparse spec for the target object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tag")]
        public static void GitTag(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string tagName,
            string objectish
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            if (objectish == null)
            {
                throw new ArgumentNullException(nameof(objectish));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.ApplyTag(tagName, objectish)
                );
        }
        /// <summary>
        /// Applys tagName to repository as annotated tag.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitTag("c:/temp/cake", "tag name", "user", "user@mail.me", "tag message");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="name">Name of committer.</param>
        /// <param name="email">Email of committer.</param>
        /// <param name="message">Commit message.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tag")]
        public static void GitTag(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string tagName,
            string name,
            string email,
            string message
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.ApplyTag(tagName, new Signature(name, email, DateTime.Now), message)
                );
        }
        /// <summary>
        /// Applys tagName to repository as annotated tag.
        /// </summary>
        /// <example>
        /// <code>
        ///     GitTag("c:/temp/cake", "tag name", "objectish", "user", "user@mail.me", "tag message");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="objectish">The revparse spec for the target object.</param>
        /// <param name="name">Name of committer.</param>
        /// <param name="email">Email of committer.</param>
        /// <param name="message">Commit message.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Tag")]
        public static void GitTag(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string tagName,
            string objectish,
            string name,
            string email,
            string message
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            if (objectish == null)
            {
                throw new ArgumentNullException(nameof(objectish));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.ApplyTag(tagName, objectish, new Signature(name, email, DateTime.Now), message)
                );
        }

    }
}