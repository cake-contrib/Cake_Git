﻿using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using LibGit2Sharp;
using System;
using System.Runtime.InteropServices;
using Cake.Core.Diagnostics;
using LogLevel = LibGit2Sharp.LogLevel;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Git
{
    // ReSharper disable once PublicMembersMustHaveComments
    public static partial class GitAliases
    {
        /// <summary>
        /// Sets the <see cref="GlobalSettings.NativeLibraryPath"/> in which the
        /// libGit libraries reside to the native libraries that are included
        /// with Cake.Git.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("LibGit2Sharp")]
        public static void GitSetLibGit2NativeLibraryPathToIncluded(
            this ICakeContext context
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var dllPath = typeof(GitAliases).Assembly.Location;
            var runtimesDir = 
                new FilePath(dllPath) // the dll
                .GetDirectory() // the tfm folder
                .GetParent() // the "lib" folder
                .GetParent() // root of the package
                .Combine("runtimes");

            var os = context.Environment.Platform.Family switch
            {
                PlatformFamily.Windows => "win",
                PlatformFamily.OSX => "osx",
                _ => "linux"
            };
            
            var processorArchitecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            var rid = $"{os}-{processorArchitecture}";
            var nativeDllPath = runtimesDir.Combine(rid).Combine("native");
            context.Log.Verbose("Calculated the Cake.Git packaged libGit2 dlls to be in {0}", nativeDllPath);

            if (!context.FileSystem.GetDirectory(nativeDllPath).Exists)
            {
                throw new CakeException(
                    $"Could not set libGit2 NativeLibraryPath: No libGit2 library was packed for runtime-ID '{rid}'.");
            }
            
            GlobalSettings.NativeLibraryPath = nativeDllPath.FullPath;
        }

        /// <summary>
        /// Routes the LibGit2Sharp logging into the Cake log.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [CakeMethodAlias]
        [CakeAliasCategory("LibGit2Sharp")]
        public static void GitEnableLogging(this ICakeContext context)
        {
            GlobalSettings.LogConfiguration = new LogConfiguration(
                LogLevel.Trace,
                (level, txt) =>
                {
                    if (level == LogLevel.None)
                    {
                        return;
                    }
                    
                    var cakeLogLevel = level switch
                    {
                        LogLevel.Trace => Cake.Core.Diagnostics.LogLevel.Debug,
                        LogLevel.Debug => Cake.Core.Diagnostics.LogLevel.Verbose,
                        LogLevel.Info => Cake.Core.Diagnostics.LogLevel.Information,
                        LogLevel.Warning => Cake.Core.Diagnostics.LogLevel.Warning,
                        LogLevel.Error => Cake.Core.Diagnostics.LogLevel.Error,
                        LogLevel.Fatal => Cake.Core.Diagnostics.LogLevel.Fatal,
                        _ => Core.Diagnostics.LogLevel.Debug
                    };
                    
                    context.Log.Write(Verbosity.Normal, cakeLogLevel, "LibGit2Sharp: {0}", txt);
                });
        }
    }
}