using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// 
        /// </summary>
        ///
        /// <param name="context"></param>
        /// <param name="repositoryDirectoryPath"></param>
        /// <param name="key"></param>
        ///
        /// <typeparam name="T"></typeparam>
        ///
        /// <returns></returns>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Configuration")]
        public static T GitConfigGet<T>(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string key)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath is null)
            {
                throw new ArgumentException(nameof(repositoryDirectoryPath));
            }
            
            return context.UseRepository(
                repositoryDirectoryPath, 
                repository => repository.Config.Get<T>(key).Value);
        }
    }
}