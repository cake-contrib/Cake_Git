using System;
using System.Linq;
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
        /// Checkout file(s) using supplied commit or branch spec.
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitCheckout("c:/temp/cake", "develop", filePaths);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="committishOrBranchSpec">A revparse spec for the commit or branch to checkout paths from.</param>
        /// <param name="filePaths">Path to files to checkout.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Checkout")]
        public static void GitCheckout(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string committishOrBranchSpec,
            params FilePath[] filePaths
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(committishOrBranchSpec))
            {
                throw new ArgumentNullException(nameof(committishOrBranchSpec));
            }

            if (repositoryDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            }

            if (filePaths.Any())
            {
                context.UseRepository(
                    repositoryDirectoryPath,
                    repository => repository.CheckoutPaths(
                        committishOrBranchSpec,
                        filePaths.ToRelativePathStrings(context, repositoryDirectoryPath),
                        new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force }
                        )
                    );
            }
            else
            {
                context.UseRepository(
                    repositoryDirectoryPath,
                    repository => Commands.Checkout(repository,
                        committishOrBranchSpec,
                        new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force }
                        )
                    );
            }
        }

        /// <summary>
        /// Checkout file.
        /// </summary>
        /// <example>
        /// <code>
        ///     var filePaths = new FilePath[] { ".\\test.txt" };
        ///     GitCheckout("c:/temp/cake", filePaths);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Path to repository.</param>
        /// <param name="filePaths">Path to files to remove.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Checkout")]
        public static void GitCheckout(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            params FilePath[] filePaths
            )
        {
            context.GitCheckout(repositoryDirectoryPath, "HEAD", filePaths);
        }
    }
}
