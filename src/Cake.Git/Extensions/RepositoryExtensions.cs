using System;
using System.IO;
using Cake.Core;
using Cake.Core.IO;
using LibGit2Sharp;

namespace Cake.Git.Extensions
{
    internal static class RepositoryExtensions
    {
        internal static void UseRepository(this ICakeContext context, DirectoryPath repositoryPath, Action<Repository> repositoryAction)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            if (repositoryAction == null)
            {
                throw new ArgumentNullException(nameof(repositoryAction));
            }

            var absoluteRepositoryPath = repositoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(absoluteRepositoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find {nameof(repositoryPath)}: {absoluteRepositoryPath}");
            }

            using (var repository = new Repository(absoluteRepositoryPath.FullPath))
            {
                repositoryAction(repository);
            }
        }

        internal static TResult UseRepository<TResult>(this ICakeContext context, DirectoryPath repositoryPath, Func<Repository, TResult> repositoryFunc)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            if (repositoryFunc == null)
            {
                throw new ArgumentNullException(nameof(repositoryFunc));
            }

            var absoluteRepositoryPath = repositoryPath.MakeAbsolute(context.Environment);

            if (!context.FileSystem.Exist(absoluteRepositoryPath))
            {
                throw new DirectoryNotFoundException($"Failed to find {nameof(repositoryPath)}: {absoluteRepositoryPath}");
            }

            using (var repository = new Repository(absoluteRepositoryPath.FullPath))
            {
                return repositoryFunc(repository);
            }
        }
    }
}
