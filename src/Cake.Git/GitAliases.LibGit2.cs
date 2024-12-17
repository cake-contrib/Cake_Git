using Cake.Core;
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
context.Log.Warning("Calculating the Cake.Git packaged libGit2 dlls path");
            var dllPath = typeof(GitAliases).Assembly.Location;
context.Log.Warning("dllPath is {0}", dllPath);
            var runtimesDir =
                new FilePath(dllPath) // the dll
                .GetDirectory() // the tfm folder
                .GetParent() // the "lib" folder
                .GetParent() // root of the package
                .Combine("runtimes");
context.Log.Warning("runtimesDir is {0}", runtimesDir);


            var os = context.Environment.Platform.Family switch
            {
                PlatformFamily.Windows => "win",
                PlatformFamily.OSX => "osx",
                _ => "linux"
            };
context.Log.Warning("OS is {0}", os);


            var processorArchitecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
context.Log.Warning("processorArchitecture is {0}", processorArchitecture);
            var rid = $"{os}-{processorArchitecture}";
context.Log.Warning("rid is {0}", rid);
            var nativeDllPath = runtimesDir.Combine(rid).Combine("native");
            context.Log.Verbose("Calculated the Cake.Git packaged libGit2 dlls to be in {0}", nativeDllPath);
context.Log.Warning("Calculated the Cake.Git packaged libGit2 dlls to be in {0}", nativeDllPath);
            if (!context.FileSystem.GetDirectory(nativeDllPath).Exists)
            {
                throw new CakeException(
                    $"Could not set libGit2 NativeLibraryPath: No libGit2 library was packed for runtime-ID '{rid}'.");
            }

            GlobalSettings.NativeLibraryPath = nativeDllPath.FullPath;
context.Log.Warning("GlobalSettings.NativeLibraryPath is now {0}", GlobalSettings.NativeLibraryPath);

// the next line will cause the first load of the native libgit2
context.Log.Warning("libgit2sharp Version: {0}", GlobalSettings.Version.ToString());
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