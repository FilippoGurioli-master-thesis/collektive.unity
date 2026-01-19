#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;

namespace Collektive.Unity.Editor
{
    public static class GenerateProto
    {
        [MenuItem("Tools/Proto/Generate")]
        public static void Generate()
        {
            var workingDir = Path.GetFullPath("..");
            var filename = "npx";
            var outputDir = Path.Combine(workingDir, "Collektive.Unity", "Runtime", "Generated");
            var baseDir = Path.Combine(
                workingDir,
                "collektive-backend",
                "lib",
                "src",
                "commonMain",
                "proto"
            );
            foreach (var file in Directory.EnumerateFiles(baseDir, "*.proto"))
            {
                var args =
                    $"protoc -I={baseDir} --csharp_out={outputDir} {Path.Combine(baseDir, Path.GetFileName(file))}";
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
                Log($"Running {args} in '{workingDir}'");
                using var process = new Process { StartInfo = startInfo };
                process.OutputDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.Log($"[proto] {e.Data}");
                };
                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.LogError($"[proto] {e.Data}");
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                var success = process.ExitCode == 0;
                Log(
                    $"Proto build finished with exit code {process.ExitCode} (success = {success})"
                );
            }
            Log("Generation finished");
        }

        public static void Log(string message)
        {
            Debug.Log($"Generate Proto: {message}");
        }
    }

    public sealed class GenerateProtoPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            GenerateProto.Log("Generating cs from proto files");
            GenerateProto.Generate();
        }
    }
}

#endif
