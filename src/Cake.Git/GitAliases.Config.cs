using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git.Extensions;

namespace Cake.Git
{
    public static partial class GitAliases
    {
        /// <summary>
        /// Gets the specified configuration value.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// var configValue = GitConfigGet&lt;string&gt;("user.email");
        /// </code>
        /// </example>
        /// 
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="key">The configuration key.</param>
        ///
        /// <typeparam name="T">The expected type of configuration.</typeparam>
        ///
        /// <returns>The value of the specified configuration key.</returns>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException">Configuration value not found.</exception>
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
                repository =>
                {
                    var config = repository.Config.Get<T>(key) 
                         ?? throw new KeyNotFoundException("Configuration not found.");
                    
                    return config.Value;
                });
        }
        
        /// <summary>
        /// Gets the specified configuration value. If the specified value is not found it will return the specified
        /// default value.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// var configValue = GitConfigGet("user.email", "bob@example.com");
        /// </code>
        /// </example>
        /// 
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to use if the configuration isn't present.</param>
        ///
        /// <typeparam name="T">The expected type of configuration.</typeparam>
        ///
        /// <returns>The value of the specified configuration key.</returns>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Configuration")]
        public static T GitConfigGet<T>(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string key,
            T defaultValue)
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
                repository => repository.Config.GetValueOrDefault<T>(key));
        }
        
        /// <summary>
        /// Gets the specified configuration value. If the specified value is not found it will return the specified
        /// default value.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// GitConfigSet("user.email", "bob@example.com");
        /// </code>
        /// </example>
        /// 
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="newValue">The new value.</param>
        ///
        /// <typeparam name="T">The type of configuration.</typeparam>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Configuration")]
        public static void GitConfigSet<T>(
            this ICakeContext context,
            DirectoryPath repositoryDirectoryPath,
            string key,
            T newValue)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath is null)
            {
                throw new ArgumentException(nameof(repositoryDirectoryPath));
            }

            context.UseRepository(repositoryDirectoryPath, repository => repository.Config.Set(key, newValue));
        }
        
        /// <summary>
        /// Unsets the specified local configuration key.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// GitConfigUnsetLocal("user.email", "bob@example.com");
        /// </code>
        /// </example>
        /// 
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="key">The configuration key.</param>
        ///
        /// <returns>Whether the key was unset.</returns>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Configuration")]
        public static void GitConfigUnsetLocal(
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

            context.UseRepository(repositoryDirectoryPath, repository => repository.Config.Unset(key));
        }

        /// <summary>
        /// Returns whether a configuration value exists with the specifed key.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// if (GetConfigurationExists&lt;string&gt;("user.email")
        /// {
        ///     //..
        /// }
        /// </code>
        /// </example>
        /// 
        /// <param name="context">The context.</param>
        /// <param name="repositoryDirectoryPath">Repository path.</param>
        /// <param name="key">The configuration key.</param>
        ///
        /// <typeparam name="T">The expected configuration type.</typeparam>
        ///
        /// <returns>Whether a configuration exists with the specified key.</returns>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Configuration")]
        public static bool GitConfigExist<T>(this ICakeContext context, DirectoryPath repositoryDirectoryPath, string key)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (repositoryDirectoryPath is null)
            {
                throw new ArgumentException(nameof(repositoryDirectoryPath));
            }

            return context.UseRepository(repositoryDirectoryPath, repository => repository.Config.Get<T>(key) != null);
        }
    }
}