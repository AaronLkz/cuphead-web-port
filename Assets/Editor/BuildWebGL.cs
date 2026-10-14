#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildWebGL
{
    private static readonly string[] Isla1Scenes = new string[]
    {
        "Assets/_CUPHEAD/Scenes/scene_start.unity",
        "Assets/_CUPHEAD/Scenes/Menus/scene_title.unity",
        "Assets/_CUPHEAD/Scenes/Menus/scene_slot_select.unity",
        "Assets/_CUPHEAD/Scenes/Cutscenes/scene_cutscene_intro.unity",
        "Assets/_CUPHEAD/Scenes/Houses/scene_level_house_elder_kettle.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_tutorial.unity",
        "Assets/_CUPHEAD/Scenes/Maps/scene_map_world_1.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_veggies.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_slime.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_frogs.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_flying_blimp.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_flower.unity",
        "Assets/_CUPHEAD/Scenes/Levels/Platforming/scene_level_platforming_1_1F.unity",
        "Assets/_CUPHEAD/Scenes/Levels/Platforming/scene_level_platforming_1_2F.unity",
        "Assets/_CUPHEAD/Scenes/Levels/scene_level_mausoleum.unity",
        "Assets/_CUPHEAD/Scenes/Shop/scene_shop.unity"
    };

    [MenuItem("Cuphead Web/Build Isla 1 WebGL")]
    public static void PerformBuild()
    {
        string buildPath = "build/WebGL";
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-customBuildPath" && i + 1 < args.Length)
            {
                buildPath = args[i + 1];
            }
        }

        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        Debug.Log("[BuildWebGL] Starting WebGL compilation for Isla 1 at: " + buildPath);

        // Configure build settings
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

        // Setup Scenes in Build Settings
        EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[Isla1Scenes.Length];
        for (int i = 0; i < Isla1Scenes.Length; i++)
        {
            buildScenes[i] = new EditorBuildSettingsScene(Isla1Scenes[i], true);
        }
        EditorBuildSettings.scenes = buildScenes;

        // Player Settings for WebGL (Disabled compression is required for GitHub Pages)
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.runInBackground = true;

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = Isla1Scenes;
        options.locationPathName = buildPath;
        options.target = BuildTarget.WebGL;
        options.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log("[BuildWebGL] Build process completed with result: " + report.summary.result);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError(string.Format("[BuildWebGL] Build FAILED! Total errors: {0}, Total warnings: {1}", 
                report.summary.totalErrors, report.summary.totalWarnings));
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log(string.Format("[BuildWebGL] Build SUCCEEDED! Total time: {0}, Size: {1} bytes", 
                report.summary.totalTime, report.summary.totalSize));
            EditorApplication.Exit(0);
        }
    }
}
#endif
