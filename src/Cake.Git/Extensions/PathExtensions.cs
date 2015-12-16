using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Git.Extensions
{
    internal static class PathExtensions
    {
        internal static Path MakeRelativePath(this Path path, ICakeEnvironment environment, DirectoryPath rootDirectoryPath)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (rootDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(rootDirectoryPath));
            }

            return (path as FilePath)?.MakeRelativePath(environment, rootDirectoryPath)
                   ??
                   (path as DirectoryPath)?.MakeRelativePath(environment, rootDirectoryPath) as Path;
        }

        internal static FilePath MakeRelativePath(this FilePath filePath, ICakeEnvironment environment, DirectoryPath rootDirectoryPath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (rootDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(rootDirectoryPath));
            }
            
            var fileUri = new Uri(filePath.MakeAbsolute(environment).FullPath);
            var rootUri = new Uri($"{rootDirectoryPath.MakeAbsolute(environment).FullPath}/");

            var relativeUri = rootUri.MakeRelativeUri(fileUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            var relativeFilePath = new FilePath(relativePath);

            return relativeFilePath;
        }

        internal static DirectoryPath MakeRelativePath(this DirectoryPath directoryPath, ICakeEnvironment environment, DirectoryPath rootDirectoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (rootDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(rootDirectoryPath));
            }

            var dirUri = new Uri(directoryPath.MakeAbsolute(environment).FullPath);
            var rootUri = new Uri($"{rootDirectoryPath.MakeAbsolute(environment).FullPath}/");

            var relativeUri = rootUri.MakeRelativeUri(dirUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            var relativeDirectoryPath = new DirectoryPath(relativePath);

            return relativeDirectoryPath;
        }

        internal static string[] ToRelativePathStrings(this IEnumerable<FilePath> filePaths, ICakeContext context, DirectoryPath repositoryDirectoryPath)
        {
            return filePaths
                .Select(filePath => filePath.MakeRelativePath(context.Environment, repositoryDirectoryPath).FullPath)
                .ToArray();
        }
    }
}
