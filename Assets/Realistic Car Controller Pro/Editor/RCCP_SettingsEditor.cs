//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RCCP_Settings))]
public class RCC_SettingsEditor : Editor {

    RCCP_Settings RCCSettingsAsset;

    public bool UseNewInputSystem {

        get {

            bool _bool = RCCP_Settings.Instance.useNewInputSystem;
            return _bool;

        }

        set {

            bool _bool = RCCP_Settings.Instance.useNewInputSystem;

            if (_bool == value)
                return;

            RCCP_Settings.Instance.useNewInputSystem = value;
            RCCP_SetScriptingSymbol.SetEnabled("BCG_NEWINPUTSYSTEM", value);

        }

    }

    Color originalGUIColor;
    Vector2 scrollPos;
    PhysicMaterial[] physicMaterials;

    bool foldGeneralSettings = false;
    bool foldBehaviorSettings = false;
    bool foldControllerSettings = false;
    bool foldUISettings = false;
    bool foldWheelPhysics = false;
    bool foldTagsAndLayers = false;
    bool foldResourcesSettings = false;
    bool foldOptimization = false;

    private void OnEnable() {

        foldGeneralSettings = RCCP_Settings.Instance.foldGeneralSettings;
        foldBehaviorSettings = RCCP_Settings.Instance.foldBehaviorSettings;
        foldControllerSettings = RCCP_Settings.Instance.foldControllerSettings;
        foldUISettings = RCCP_Settings.Instance.foldUISettings;
        foldWheelPhysics = RCCP_Settings.Instance.foldWheelPhysics;
        foldTagsAndLayers = RCCP_Settings.Instance.foldTagsAndLayers;
        foldResourcesSettings = RCCP_Settings.Instance.resourcesSettings;
        foldOptimization = RCCP_Settings.Instance.foldOptimization;

    }

    private void OnDestroy() {

        RCCP_Settings.Instance.foldGeneralSettings = foldGeneralSettings;
        RCCP_Settings.Instance.foldBehaviorSettings = foldBehaviorSettings;
        RCCP_Settings.Instance.foldControllerSettings = foldControllerSettings;
        RCCP_Settings.Instance.foldUISettings = foldUISettings;
        RCCP_Settings.Instance.foldWheelPhysics = foldWheelPhysics;
        RCCP_Settings.Instance.foldTagsAndLayers = foldTagsAndLayers;
        RCCP_Settings.Instance.resourcesSettings = foldResourcesSettings;
        RCCP_Settings.Instance.foldOptimization = foldOptimization;

    }

    public override void OnInspectorGUI() {

        RCCSettingsAsset = (RCCP_Settings)target;
        serializedObject.Update();

        originalGUIColor = GUI.color;
        EditorGUIUtility.labelWidth = 250;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RCCP Asset Settings Editor Window", EditorStyles.boldLabel);
        GUI.color = new Color(.75f, 1f, .75f);
        EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project for RCCP. Don't change directory of the ''Resources/RCCP Assets''.", EditorStyles.helpBox);
        GUI.color = originalGUIColor;
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        EditorGUILayout.Space();

        foldGeneralSettings = EditorGUILayout.Foldout(foldGeneralSettings, "General Settings");

        if (foldGeneralSettings) {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("General Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideFixedTimeStep"), new GUIContent("Override FixedTimeStep", "Overrides the fixed timestep. More accurate physics in lower values, more performance on higher values. Default is 0.02, which means 50 frames per second. Not recommended to change this value."));

            if (RCCSettingsAsset.overrideFixedTimeStep)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedTimeStep"), new GUIContent("Fixed Timestep", "Overrides the fixed timestep. More accurate physics in lower values, more performance on higher values. Default is 0.02, which means 50 frames per second. Not recommended to change this value."));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngularVelocity"), new GUIContent("Maximum Angular Velocity", "Maximum angular velocity limit of the vehicle. More stable angular movements on higher values, more speedy movements on lower values."));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideFPS"), new GUIContent("Override FPS", "Overrides maximum frames per second."));

            if (RCCSettingsAsset.overrideFPS)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFPS"), new GUIContent("Maximum FPS", "Overrides maximum frames per second."));

            EditorGUILayout.HelpBox("You can find all references to any mode. Open up ''RCCP_Settings.cs'' and right click to any mode. Hit ''Find references'' to find all modifications.", MessageType.Info);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useFixedWheelColliders"), new GUIContent("Use Fixed WheelColliders", "Improves stability by increasing mass of the WheelColliders."));

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldBehaviorSettings = EditorGUILayout.Foldout(foldBehaviorSettings, "Behavior Settings");

        if (foldBehaviorSettings) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Behavior Settings", EditorStyles.boldLabel);

            GUI.color = new Color(.75f, 1f, .75f);
            EditorGUILayout.HelpBox("Using behavior preset will override wheelcollider settings, chassis joint, antirolls, and other stuff. Using ''Custom'' mode will not override anything.", MessageType.Info);
            GUI.color = originalGUIColor;

            RCCSettingsAsset.overrideBehavior = EditorGUILayout.BeginToggleGroup("Override Behavior", RCCSettingsAsset.overrideBehavior);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("behaviorTypes"), new GUIContent("Behavior Types"), true);

            List<string> behaviorTypeStrings = new List<string>();

            GUI.color = new Color(.5f, 1f, 1f, 1f);

            for (int i = 0; i < RCCSettingsAsset.behaviorTypes.Length; i++)
                behaviorTypeStrings.Add(RCCSettingsAsset.behaviorTypes[i].behaviorName);

            RCCSettingsAsset.behaviorSelectedIndex = GUILayout.Toolbar(RCCSettingsAsset.behaviorSelectedIndex, behaviorTypeStrings.ToArray());

            EditorGUI.indentLevel--;
            GUI.color = originalGUIColor;

            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldControllerSettings = EditorGUILayout.Foldout(foldControllerSettings, "Controller Settings");

        if (foldControllerSettings) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Main Controller Settings", EditorStyles.boldLabel);

            UseNewInputSystem = EditorGUILayout.Toggle(new GUIContent("Use New Input System", "It will enable new input system."), UseNewInputSystem);
            
            EditorGUILayout.HelpBox("If you are using new input system, be sure your project has 'Input System' installed through the Package Manager. And be sure 'Active InputHandler' in your Player Settings (Edit --> Project Settings --> Player) is set to new or both.", MessageType.Info);

            EditorGUILayout.Space();

            GUILayout.Label("Mobile Controller Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("mobileControllerEnabled"), new GUIContent("Mobile Controller Enabled"));

            if (RCCSettingsAsset.mobileControllerEnabled) {

                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mobileController"), new GUIContent("Mobile Controller Type", "There are four controller types for mobile controller listed below."));
                EditorGUILayout.HelpBox("Mobile UI controller buttons will be used to receive player inputs through the RCCP_InputManager.", MessageType.Info);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gyroSensitivity"), new GUIContent("Gyro Sensitivity", "Gyro sensitivity."));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

            }

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldUISettings = EditorGUILayout.Foldout(foldUISettings, "UI Settings");

        if (foldUISettings) {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("UI Dashboard Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useTelemetry"), new GUIContent("Use Telemetry", "Enables telemetry to display vehicle stats."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useInputDebugger"), new GUIContent("Use Input Debugger", "Displays informer texts on important inputs."));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldWheelPhysics = EditorGUILayout.Foldout(foldWheelPhysics, "Wheel Physics Settings");

        if (foldWheelPhysics) {

            if (RCCP_GroundMaterials.Instance.frictions != null && RCCP_GroundMaterials.Instance.frictions.Length > 0) {

                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Ground Physic Materials", EditorStyles.boldLabel);

                physicMaterials = new PhysicMaterial[RCCP_GroundMaterials.Instance.frictions.Length];

                for (int i = 0; i < physicMaterials.Length; i++) {

                    physicMaterials[i] = RCCP_GroundMaterials.Instance.frictions[i].groundMaterial;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.ObjectField("Ground Physic Materials " + i, physicMaterials[i], typeof(PhysicMaterial), false);
                    EditorGUILayout.EndVertical();

                }

                EditorGUILayout.Space();

            }

            GUI.color = new Color(.5f, 1f, 1f, 1f);

            if (GUILayout.Button("Configure Ground Physic Materials"))
                Selection.activeObject = RCCP_GroundMaterials.Instance;

            GUI.color = originalGUIColor;

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldTagsAndLayers = EditorGUILayout.Foldout(foldTagsAndLayers, "Tags & Layers");

        if (foldTagsAndLayers) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Tags & Layers", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("setLayers"), new GUIContent("Set Layers Auto", "Sets layers of the vehicle automatically. Vehicle will be RCCP_Vehicle, wheels will be RCCP_WheelCollider, detachable parts will be RCCP_DetachablePart."), false);

            if (RCCSettingsAsset.setLayers) {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPLayer"), new GUIContent("Vehicle Layer"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPWheelColliderLayer"), new GUIContent("WheelCollider Layer"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPDetachablePartLayer"), new GUIContent("DetachablePart Layer"), false);
                GUI.color = new Color(.75f, 1f, .75f);
                EditorGUILayout.HelpBox("Be sure you have that tag and layer in your Tags & Layers", MessageType.Warning);
                EditorGUILayout.HelpBox("All vehicles powered by Realistic Car Controller Pro are using this layer. What does this layer do? It was used for masking wheel rays, light masks, and projector masks. Just create a new layer for vehicles from Edit --> Project Settings --> Tags & Layers, and select the layer here.", MessageType.Info);
                GUI.color = originalGUIColor;

            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldOptimization = EditorGUILayout.Foldout(foldOptimization, "Optimization");

        if (foldOptimization) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Optimization", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useHeadLightsAsVertexLights"), new GUIContent("Head Lights As Vertex Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useBrakeLightsAsVertexLights"), new GUIContent("Brake Lights As Vertex Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useReverseLightsAsVertexLights"), new GUIContent("Reverse Lights As Vertex Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useIndicatorLightsAsVertexLights"), new GUIContent("Indicator Lights As Vertex Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useOtherLightsAsVertexLights"), new GUIContent("Other Lights As Vertex Lights"));

            GUI.color = new Color(.75f, 1f, .75f);
            EditorGUILayout.HelpBox("Always use vertex lights for mobile platform. Even only one pixel light will drop your performance dramaticaly!", MessageType.Info);
            GUI.color = originalGUIColor;

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        foldResourcesSettings = EditorGUILayout.Foldout(foldResourcesSettings, "Resources");

        if (foldResourcesSettings) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Resources", EditorStyles.boldLabel);

            GUI.color = new Color(.75f, 1f, .75f);
            EditorGUILayout.HelpBox("These are the initial resources for the initialization. RCCP will use these resources to setup things.", MessageType.Info);
            GUI.color = originalGUIColor;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineLowClipOn"), new GUIContent("Engine Low On"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineLowClipOff"), new GUIContent("Engine Low Off"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineMedClipOn"), new GUIContent("Engine Med On"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineMedClipOff"), new GUIContent("Engine Med Off"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineHighClipOn"), new GUIContent("Engine High On"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineHighClipOff"), new GUIContent("Engine High Off"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights_Low"), new GUIContent("Head Lights Low"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights_High"), new GUIContent("Head Lights High"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeLights"), new GUIContent("Brake Lights"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tailLights"), new GUIContent("Tail Lights"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reverseLights"), new GUIContent("Reverse Lights"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorLights_L"), new GUIContent("Indicator Lights Left"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorLights_R"), new GUIContent("Indicator Lights Right"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightBox"), new GUIContent("Light Box"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skidmarksManager"), new GUIContent("Skidmarks Manager"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("exhaustGas"), new GUIContent("Exhaust Gas"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPMainCamera"), new GUIContent("RCCP Main Camera"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPHoodCamera"), new GUIContent("RCCP Hood Camera"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPCinematicCamera"), new GUIContent("RCCP Cinematic Camera"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPWheelCamera"), new GUIContent("RCCP Wheel Camera"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPFixedCamera"), new GUIContent("RCCP Fixed Camera"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPCanvas"), new GUIContent("RCCP UI Canvas"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCPTelemetry"), new GUIContent("RCCP Telemetry Canvas"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("contactParticles"), new GUIContent("Contact Particles"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scratchParticles"), new GUIContent("Scratch Particles"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelSparkleParticles"), new GUIContent("Wheel Sparkle Particles"), false);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioMixer"), new GUIContent("Main Audio Mixer"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineStartClip"), new GUIContent("Engine Start Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorClip"), new GUIContent("Indicator Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bumpClip"), new GUIContent("Bump Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("exhaustFlameClips"), new GUIContent("Exhaust Flame Clips"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gearClips"), new GUIContent("Gear Shifting Clips"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("crashClips"), new GUIContent("Crash Clips"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NOSClip"), new GUIContent("NOS Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turboClip"), new GUIContent("Turbo Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("blowoutClip"), new GUIContent("Blowout Clips"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reversingClip"), new GUIContent("Reverse Transmission Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windClip"), new GUIContent("Wind Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeClip"), new GUIContent("Brake Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelDeflateClip"), new GUIContent("Wheel Deflate Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelInflateClip"), new GUIContent("Wheel Inflate Clip"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelFlatClip"), new GUIContent("Wheel Flat Clip"), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("Configure Wheel Slip Sounds"))
                Selection.activeObject = RCCP_GroundMaterials.Instance;

            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.button);

        GUI.color = new Color(.5f, 1f, 1f, 1f);

        if (GUILayout.Button("Open PDF Documentation")) {

            string url = RCCP_AssetPaths.documentations;
            Application.OpenURL(url);

        }

        GUI.color = originalGUIColor;

        EditorGUILayout.LabelField("Realistic Car Controller Pro " + RCCP_Version.version + " \nBoneCracker Games", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        EditorGUILayout.LabelField("Created by Ekrem Bugra Ozdoganlar", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(RCCSettingsAsset);

    }

}
