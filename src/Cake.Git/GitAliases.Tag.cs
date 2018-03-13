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

            if (String.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException(nameof(tagName));
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

            if (String.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException(nameof(tagName));
            }

            if (String.IsNullOrWhiteSpace(objectish))
            {
                throw new ArgumentException(nameof(objectish));
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

            if (String.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException(nameof(tagName));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
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

            if (String.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException(nameof(tagName));
            }

            if (String.IsNullOrWhiteSpace(objectish))
            {
                throw new ArgumentException(nameof(objectish));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            context.UseRepository(
                repositoryDirectoryPath,
                repository => repository.ApplyTag(tagName, objectish, new Signature(name, email, DateTime.Now), message)
                );
        }

    }
}