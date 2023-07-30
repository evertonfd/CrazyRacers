//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class RCCP_WelcomeWindow : EditorWindow {

    public class ToolBar {

        public string title;
        public UnityEngine.Events.UnityAction Draw;

        /// <summary>
        /// Create New Toolbar
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="onDraw">Method to draw when toolbar is selected</param>
        public ToolBar(string title, UnityEngine.Events.UnityAction onDraw) {

            this.title = title;
            this.Draw = onDraw;

        }

        public static implicit operator string(ToolBar tool) {
            return tool.title;
        }

    }

    /// <summary>
    /// Index of selected toolbar.
    /// </summary>
    public int toolBarIndex = 0;

    /// <summary>
    /// List of Toolbars
    /// </summary>
    public ToolBar[] toolBars = new ToolBar[]{

        new ToolBar("Welcome", WelcomePageContent),
        new ToolBar("Demos", DemosPageContent),
        new ToolBar("Updates", UpdatePageContent),
        new ToolBar("Addons", Addons),
        new ToolBar("DOCS", Documentations)

    };

    public static Texture2D bannerTexture = null;

    private GUISkin skin;

    private const int windowWidth = 600;
    private const int windowHeight = 600;

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Welcome Window", false, 100)]
    public static void OpenWindow() {

        GetWindow<RCCP_WelcomeWindow>(true);

    }

    private void OnEnable() {

        titleContent = new GUIContent("Realistic Car Controller Pro");
        maxSize = new Vector2(windowWidth, windowHeight);
        minSize = maxSize;

        InitStyle();

    }

    private void InitStyle() {

        if (!skin)
            skin = Resources.Load("RCCP_Gui") as GUISkin;

        bannerTexture = (Texture2D)Resources.Load("Editor Icons/RCCP_Banner", typeof(Texture2D));

    }

    private void OnGUI() {

        GUI.skin = skin;
        
        DrawHeader();
        DrawMenuButtons();
        DrawToolBar();
        DrawFooter();

        if (!EditorApplication.isPlaying)
            Repaint();

    }

    private void DrawHeader() {

        GUILayout.Label(bannerTexture, GUILayout.Height(120));

    }

    private void DrawMenuButtons() {

        GUILayout.Space(-10);
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, ToolbarNames());
        
    }

    #region ToolBars

    public static void WelcomePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Welcome!");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Thank you for purchasing and using Realistic Car Controller Pro. Please read the documentations before use. Also check out the online documentations for updated info. Have fun :)");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Demo Scenes To Build Settings"))
            AddDemoScenesToBuildSettings();

        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("If you want to add Photon PUN2 scenes, import and install Photon PUN2 & integration first. Then click again to add those scenes to your Build Settings.", MessageType.Info, true);
        EditorGUILayout.HelpBox("If you want to add Enter / Exit scenes, import BCG Shared Assets to your project first. Then click again to add those scenes to your Build Settings.", MessageType.Info, true);
        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();

        //GUILayout.FlexibleSpace();

        GUI.color = Color.red;

        if (GUILayout.Button("Delete all demo contents from the project")) {

            if (EditorUtility.DisplayDialog("Warning", "You are about to delete all demo contents such as vehicle models, vehicle prefabs, vehicle textures, all scenes, scene models, scene prefabs, scene textures!", "Delete", "Cancel"))
                DeleteDemoContent();

        }
        
        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

    }

    public static void UpdatePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Updates");

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>Installed Version: </b>" + RCCP_Version.version.ToString());
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>1</b>- Always backup your project before updating RCCP or any asset in your project!");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>2</b>- If you have own assets such as prefabs, audioclips, models, scripts in RealisticCarControllerV3 folder, keep your own asset outside from RealisticCarControllerV3 folder.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>3</b>- Delete RealisticCarControllerV3 folder, and import latest version to your project.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (GUILayout.Button("Check Updates"))
            Application.OpenURL(RCCP_AssetPaths.assetStorePath);

        GUILayout.Space(6);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void DemosPageContent() {

        EditorGUILayout.BeginVertical("window");

        GUILayout.Label("Demo Scenes");

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        bool photonInstalled = false;

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
        photonInstalled = true;
#endif

        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("All scenes must be in your Build Settings to run AIO demo.", MessageType.Warning, true);
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        if (GUILayout.Button("RCCP City AIO"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_AIO, OpenSceneMode.Single);

        if (GUILayout.Button("RCCP City"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_City, OpenSceneMode.Single);

        if (GUILayout.Button("RCCP City Car Selection"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_CarSelection, OpenSceneMode.Single);

        if (GUILayout.Button("RCCP Blank API"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_APIBlank, OpenSceneMode.Single);

        if (GUILayout.Button("RCCP Blank"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_BlankMobile, OpenSceneMode.Single);

        if (GUILayout.Button("RCCP Damage"))
            EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_Damage, OpenSceneMode.Single);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        if (BCGInstalled) {

            if (GUILayout.Button("RCCP City Enter-Exit FPS"))
                EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_CityFPS, OpenSceneMode.Single);

            if (GUILayout.Button("RCCP City Enter-Exit TPS"))
                EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_CityTPS, OpenSceneMode.Single);

        } else {

            EditorGUILayout.HelpBox("If you want to add enter exit scenes, you have to import latest BCG Shared Assets to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import BCG Shared Assets"))
                AssetDatabase.ImportPackage(RCCP_AssetPaths.BCGSharedAssetsPath, true);

        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");

        if (photonInstalled) {

            if (GUILayout.Button("RCCP Lobby Photon"))
                EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_PUN2Lobby, OpenSceneMode.Single);

            if (GUILayout.Button("RCCP Blank Photon"))
                EditorSceneManager.OpenScene(RCCP_AssetPaths.demo_PUN2City, OpenSceneMode.Single);

        } else {

            EditorGUILayout.HelpBox("If you want to add Photon PUN2 scenes, you have to import latest Photon PUN2 to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import Photon PUN2"))
                Application.OpenURL(RCCP_AssetPaths.photonPUN2);

        }

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void Addons() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Addons");
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>Photon PUN2</b>");

        bool photonInstalled = false;

#if PHOTON_UNITY_NETWORKING
        photonInstalled = true;
#endif

        bool photonAndRCCInstalled = false;

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
        photonAndRCCInstalled = true;
#endif

        if (!photonAndRCCInstalled) {

            if (!photonInstalled) {

                EditorGUILayout.HelpBox("You have to import latest Photon PUN2 to your project first.", MessageType.Warning);

                if (GUILayout.Button("Download and import Photon PUN2"))
                    Application.OpenURL(RCCP_AssetPaths.photonPUN2);

            } else {

                EditorGUILayout.HelpBox("Found Photon PUN2, You can import integration package and open Photon demo scenes now.", MessageType.Info);

                if (GUILayout.Button("Import Photon PUN2 Integration"))
                    AssetDatabase.ImportPackage(RCCP_AssetPaths.PUN2AssetsPath, true);

            }

        } else if (photonInstalled) {

            EditorGUILayout.HelpBox("Installed Photon PUN2 with RCCP, You can open Photon demo scenes now.", MessageType.Info);
            EditorGUILayout.HelpBox("If you want to remove Photon PUN2 integration from the project, delete the ''Photon PUN2'' folder inside the Addons/Installed folder. After that, you need to remove ''RCCP_PHOTON'' scripting define symbol in your player settings. In order to do that, go to Edit --> Project Settings --> Player Settings --> Other Settings, and remove the scripting symbol from the list.", MessageType.Warning, true);

        }

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
        if (photonInstalled) {

            EditorGUILayout.LabelField("Photon PUN2 Version: " + System.Reflection.Assembly.GetAssembly(typeof(ExitGames.Client.Photon.PhotonPeer)).GetName().Version.ToString(), EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(6);

        }
#endif

        EditorGUILayout.EndVertical();

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>BCG Shared Assets (Enter / Exit)</b>");

        if (!BCGInstalled) {

            EditorGUILayout.HelpBox("You have to import latest BCG Shared Assets to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import BCG Shared Assets"))
                AssetDatabase.ImportPackage(RCCP_AssetPaths.BCGSharedAssetsPath, true);

        } else {

            EditorGUILayout.HelpBox("Found BCG Shared Assets, You can open Enter / Exit demo scenes now.", MessageType.Info);
            EditorGUILayout.HelpBox("If you want to remove BCG Shared Assets integration from the project, delete the ''BoneCracker Games Shared Assets'' folder. After that, you need to remove ''BCG_ENTEREXIT'' scripting define symbol in your player settings. In order to do that, go to Edit --> Project Settings --> Player Settings --> Other Settings, and remove the scripting symbol from the list.", MessageType.Warning, true);

#if BCG_ENTEREXIT
            EditorGUILayout.LabelField("BCG Shared Assets Version: " + BCG_Version.version, EditorStyles.centeredGreyMiniLabel);
#endif
            GUILayout.Space(6);

        }

        EditorGUILayout.EndVertical();

        //EditorGUILayout.Separator();

        //EditorGUILayout.BeginVertical("box");

        //GUILayout.Label("<b>Logitech</b>");

        //EditorGUILayout.BeginHorizontal();

        //if (GUILayout.Button("Download and import Logitech SDK"))
        //    Application.OpenURL(RCCP_AssetPaths.logitech);

        //if (GUILayout.Button("Import Logitech Integration"))
        //    AssetDatabase.ImportPackage(RCCP_AssetPaths.LogiAssetsPath, true);

        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>ProFlares</b>");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Download and import ProFlares"))
            Application.OpenURL(RCCP_AssetPaths.proFlares);

        if (GUILayout.Button("Import ProFlares Integration"))
            AssetDatabase.ImportPackage(RCCP_AssetPaths.ProFlareAssetsPath, true);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void Documentations() {

        EditorGUILayout.BeginVertical("window");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.HelpBox("Offline documentations can be found in the documentations folder.", MessageType.Info);

        if (GUILayout.Button("Online Documentations"))
            Application.OpenURL(RCCP_AssetPaths.documentations);

        if (GUILayout.Button("Youtube Tutorial Videos"))
            Application.OpenURL(RCCP_AssetPaths.YTVideos);

        if (GUILayout.Button("Other Assets"))
            Application.OpenURL(RCCP_AssetPaths.otherAssets);

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    #endregion

    private string[] ToolbarNames() {

        string[] names = new string[toolBars.Length];

        for (int i = 0; i < toolBars.Length; i++)
            names[i] = toolBars[i];

        return names;

    }

    private void DrawToolBar() {
        
        GUILayout.BeginArea(new Rect(4, 140, 592, 540));

        toolBars[toolBarIndex].Draw();

        GUILayout.EndArea();

        GUILayout.FlexibleSpace();

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Car Controller Pro " + RCCP_Version.version, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Ekrem Bugra Ozdoganlar", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

    private static void ImportPackage(string package) {

        try {
            AssetDatabase.ImportPackage(package, true);
        } catch (Exception) {
            Debug.LogError("Failed to import package: " + package);
            throw;
        }

    }

    private static void DeleteDemoContent() {

        Debug.LogWarning("Deleting demo contents...");

        foreach (var item in RCCP_AssetPaths.demoAssetPaths)
            FileUtil.DeleteFileOrDirectory(item);

        AssetDatabase.Refresh();

        Debug.LogWarning("Deleted demo contents!");
        EditorUtility.DisplayDialog("Deleted Demo Contents", "All demo contents have been deleted!", "Ok");

    }

    private static void AddDemoScenesToBuildSettings() {

        List<string> demoScenePaths = new List<string>();

        demoScenePaths.Add(RCCP_AssetPaths.demo_AIO);
        demoScenePaths.Add(RCCP_AssetPaths.demo_City);
        demoScenePaths.Add(RCCP_AssetPaths.demo_CarSelection);
        demoScenePaths.Add(RCCP_AssetPaths.demo_APIBlank);
        demoScenePaths.Add(RCCP_AssetPaths.demo_BlankMobile);
        demoScenePaths.Add(RCCP_AssetPaths.demo_Damage);

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        if (BCGInstalled) {

            demoScenePaths.Add(RCCP_AssetPaths.demo_CityFPS);
            demoScenePaths.Add(RCCP_AssetPaths.demo_CityTPS);

        }

        bool photonAndRCCInstalled = false;

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
        photonAndRCCInstalled = true;
#endif

        if (photonAndRCCInstalled) {

            demoScenePaths.Add(RCCP_AssetPaths.demo_PUN2Lobby);
            demoScenePaths.Add(RCCP_AssetPaths.demo_PUN2City);

        }

        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

        foreach (string path in demoScenePaths) {

            if (!string.IsNullOrEmpty(path))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(path, true));

        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        EditorUtility.DisplayDialog("Demo Scenes", "All demo scenes have been added to the Build Settings. For Photon and Enter / Exit scenes, you have to import and intregrate them first (Addons).", "Ok");

    }

}
