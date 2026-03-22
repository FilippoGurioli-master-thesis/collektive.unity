#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;

namespace Collektive.Unity.Editor
{
    public static class BackendBuilder
    {
        private static readonly string KotlinProjectPath = Path.Combine("..", "collektive-backend");

        private static string GradleExecutable =>
            Path.Combine(KotlinProjectPath, IsWindows ? "gradlew.bat" : "gradlew");

        private const string GradleTask = "assemble";
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static bool IsMac => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        private static string LibFileName =>
            IsWindows ? "collektive_backend.dll"
            : IsMac ? "libcollektive_backend.dylib"
            : "libcollektive_backend.so";

        private static string KotlinTargetName =>
            IsWindows ? "windows"
            : IsMac
                ? (
                    RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                        ? "macosArm64"
                        : "macosX64"
                )
            : "linux";

        private static string SourceLibPath =>
            Path.Combine(
                KotlinProjectPath,
                "lib",
                "build",
                "bin",
                KotlinTargetName,
                "releaseShared",
                (IsWindows ? "" : "lib")
                    + "collektive_backend"
                    + (
                        IsWindows ? ".dll"
                        : IsMac ? ".dylib"
                        : ".so"
                    )
            );

        private static string UnityPluginPath =>
            Path.Combine("..", "Collektive.Unity", "Runtime", "Plugins", LibFileName);

        [MenuItem("Tools/Native/Rebuild backend")]
        public static void RebuildNativeLibrary()
        {
            if (!RunGradleBuild())
                return;
            CopyLibraryIntoUnity();
            ConfigurePluginImporter();
            AssetDatabase.Refresh();
            Debug.Log($"BackendBuilder: {LibFileName} rebuilt and configured.");
        }

        private static bool RunGradleBuild()
        {
            var workingDir = Path.GetFullPath(KotlinProjectPath);
            var filename = "/bin/bash";
            var gradleExecutable = Path.GetFullPath(GradleExecutable);
            var args =
                $"-c \"source ~/.sdkman/bin/sdkman-init.sh 2>/dev/null; {gradleExecutable} {GradleTask}\"";
            var startInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args,
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            Debug.Log(
                $"Running {startInfo.FileName} {startInfo.Arguments} in {startInfo.WorkingDirectory}"
            );
            using var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.Log($"[gradle] {e.Data}");
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.LogError($"[gradle] {e.Data}");
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            Debug.Log($"The process has returned {process.ExitCode}");
            return process.ExitCode == 0;
        }

        private static void CopyLibraryIntoUnity()
        {
            var source = Path.GetFullPath(SourceLibPath);
            var destination = Path.GetFullPath(UnityPluginPath);
            if (!File.Exists(source) && IsWindows)
                source = source.Replace("collektive_backend.dll", "libcollektive_backend.dll");
            if (!File.Exists(source))
                throw new FileNotFoundException($"Source binary not found at {source}");
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(source, destination, overwrite: true);
        }

        private static void ConfigurePluginImporter()
        {
            var packageName = "it.unibo.collektive.unity";
            var packageRelativePath = $"Packages/{packageName}/Runtime/Plugins/{LibFileName}";
            var importer = AssetImporter.GetAtPath(packageRelativePath) as PluginImporter;
            if (importer == null)
            {
                var absolutePath = Path.GetFullPath(UnityPluginPath);
                var unityPath = absolutePath.Replace('\\', '/');
                importer = AssetImporter.GetAtPath(unityPath) as PluginImporter;
            }
            if (importer == null)
            {
                Debug.LogError(
                    $"BackendBuilder: Could not find PluginImporter at {packageRelativePath}"
                );
                return;
            }
            importer.SaveAndReimport();
        }
    }
}
#endif
