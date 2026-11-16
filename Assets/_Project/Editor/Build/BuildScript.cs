using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Addicted.Editor.Build
{
    /// <summary>
    /// Build do jogo. Usado pelo menu "Addicted/Build" e pela linha de comando (CI):
    /// Unity -batchmode -quit -projectPath . -executeMethod Addicted.Editor.Build.BuildScript.BuildWindows
    ///       [-buildVersion 0.1.0] [-buildPath Builds/Windows]
    /// </summary>
    public static class BuildScript
    {
        public const string CompanyName = "Jogos Absurdos";
        public const string ProductName = "Addicted";
        public const string DefaultVersion = "0.1.0";
        private const string DefaultWindowsPath = "Builds/Windows";
        private const string FirstScene = "Assets/_Project/Scenes/Escritorio.unity";

        [MenuItem("Addicted/Build/Windows (x64)")]
        public static void BuildWindowsFromMenu()
        {
            var report = Build(BuildTarget.StandaloneWindows64, DefaultWindowsPath, ReadArgument("-buildVersion"));
            if (report.summary.result == BuildResult.Succeeded)
                EditorUtility.RevealInFinder(report.summary.outputPath);
        }

        /// <summary>Ponto de entrada da linha de comando. Sai com código 1 se o build falhar.</summary>
        public static void BuildWindows()
        {
            var path = ReadArgument("-buildPath") ?? DefaultWindowsPath;
            var report = Build(BuildTarget.StandaloneWindows64, path, ReadArgument("-buildVersion"));
            if (Application.isBatchMode)
                EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        private static BuildReport Build(BuildTarget target, string folder, string version)
        {
            ApplyPlayerSettings(version);

            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled && File.Exists(s.path))
                .Select(s => s.path)
                .OrderBy(p => p == FirstScene ? 0 : 1)
                .ToArray();
            if (scenes.Length == 0 || scenes[0] != FirstScene)
                throw new BuildFailedException($"A cena inicial {FirstScene} precisa estar nas Build Settings.");

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                target = target,
                targetGroup = BuildPipeline.GetBuildTargetGroup(target),
                locationPathName = Path.Combine(folder, ProductName + ".exe"),
                options = BuildOptions.None,
            };

            Debug.Log($"[Build] {ProductName} {PlayerSettings.bundleVersion} → {options.locationPathName}");
            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            Debug.Log($"[Build] {summary.result}: {summary.totalErrors} erro(s), " +
                      $"{summary.totalSize / (1024f * 1024f):0.0} MB, {summary.totalTime.TotalSeconds:0} s");
            return report;
        }

        private static void ApplyPlayerSettings(string version)
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = string.IsNullOrWhiteSpace(version)
                ? (PlayerSettings.bundleVersion is { Length: > 0 } current && current != "1.0" ? current : DefaultVersion)
                : version.Trim().TrimStart('v');

            var standalone = NamedBuildTarget.Standalone;
            PlayerSettings.SetScriptingBackend(standalone, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(standalone, Il2CppCompilerConfiguration.Release);

            // Janela redimensionável em 1280x720, para dar para fechar e alternar com o editor.
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;

            AssetDatabase.SaveAssets();
        }

        private static string ReadArgument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
