#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;

namespace Editor
{
    public static class BackendBuilder
    {
        private static readonly string KotlinProjectPath = Path.Combine("..", "collektive-backend");
        private static readonly string GradleExecutable = Path.Combine(
            KotlinProjectPath,
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "gradlew.bat" : "gradlew"
        );
        private const string GradleTask = "build";

        private static readonly string SourceSoPath = Path.Combine(
            KotlinProjectPath,
            "lib",
            "build",
            "bin",
            "native",
            "releaseShared",
            "libcollektive_backend.so"
        );

        private static readonly string UnitySoPath = Path.Combine(
            "..",
            "Collektive.Unity",
            "Runtime",
            "Plugins",
            "libcollektive_backend.so"
        );

        [MenuItem("Tools/Native/Rebuild backend")]
        public static void RebuildNativeLibrary()
        {
            try
            {
                if (!RunGradleBuild())
                {
                    Debug.LogError("BackendBuilder: Gradle/Kotlin build failed. Aborting.");
                    return;
                }
                CopySoIntoUnity();
                ConfigurePluginImporter();
                Debug.Log("BackendBuilder: Native library rebuilt and configured successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"BackendBuilder: Exception while rebuilding native library: {ex}");
            }
        }

        private static bool RunGradleBuild()
        {
            var workingDir = Path.GetFullPath(KotlinProjectPath);
            if (!Directory.Exists(workingDir))
            {
                Debug.LogError($"BackendBuilder: Kotlin project directory not found: {workingDir}");
                return false;
            }
            var gradlePath = GradleExecutable;
            var startInfo = new ProcessStartInfo
            {
                FileName = gradlePath,
                Arguments = GradleTask,
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            Debug.Log(
                $"BackendBuilder: Running Gradle in '{workingDir}' → {gradlePath} {GradleTask}"
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
            var success = process.ExitCode == 0;
            Debug.Log(
                $"BackendBuilder: Gradle build finished with exit code {process.ExitCode} (success = {success})"
            );
            return success;
        }

        private static void CopySoIntoUnity()
        {
            var source = Path.GetFullPath(SourceSoPath);
            var destination = Path.GetFullPath(UnitySoPath);
            if (!File.Exists(source))
                throw new FileNotFoundException(
                    $"BackendBuilder: .so file not found at expected path: {source}"
                );
            var destDir = Path.GetDirectoryName(destination)!;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            File.Copy(source, destination, overwrite: true);
            Debug.Log($"BackendBuilder: Copied .so from '{source}' to '{destination}'");
            AssetDatabase.Refresh();
        }

        private static void ConfigurePluginImporter()
        {
            var importer = AssetImporter.GetAtPath(UnitySoPath) as PluginImporter;
            if (importer == null)
            {
                Debug.LogError(
                    $"BackendBuilder: Could not get PluginImporter for asset at '{UnitySoPath}'"
                );
                return;
            }
            importer.ClearSettings();
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(true);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
            importer.SetPlatformData(BuildTarget.StandaloneLinux64, "CPU", "x86_64");
            importer.SaveAndReimport();
            Debug.Log(
                "BackendBuilder: PluginImporter configuration updated for libcollektive_backend.so"
            );
        }
    }

    public sealed class BackendBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("NativeLibBuildPreprocessor: Rebuilding native lib before build");
            BackendBuilder.RebuildNativeLibrary();
        }
    }
}

#endif
