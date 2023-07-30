//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;
using System;

[CustomEditor(typeof(RCCP_CarController))]
public class RCCP_CarControllerEditor : Editor {

    RCCP_CarController prop;
    GUISkin skin;
    Color guiColor;
    List<string> errorMessages = new List<string>();

    static bool addAllComponents;
    bool statsEnabled;
    bool thumbnailsEnabled;

    RCCP_Engine engine;
    RCCP_Clutch clutch;
    RCCP_Gearbox gearbox;
    RCCP_Differential differential;
    RCCP_Axles axles;
    RCCP_Input inputs;
    RCCP_AeroDynamics aero;
    RCCP_Audio audio;
    RCCP_Lights lights;
    RCCP_Stability stability;
    RCCP_Damage damage;
    RCCP_Particles particles;
    RCCP_OtherAddons otherAddons;

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Add Main Controller To Vehicle", false, -85)]
    static void CreateBehavior() {

        if (!Selection.activeGameObject.GetComponentInParent<RCCP_CarController>()) {

            bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

            if (isPrefab) {

                bool isModelPrefab = PrefabUtility.IsPartOfModelPrefab(Selection.activeGameObject);
                bool unpackPrefab = EditorUtility.DisplayDialog("Unpack Prefab", "This gameobject is connected to a " + (isModelPrefab ? "model" : "") + " prefab. Would you like to unpack the prefab completely? If you don't unpack it, you won't be able to move, reorder, or delete any children instance of the prefab.", "Unpack", "Don't Unpack");

                if (unpackPrefab)
                    PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            }

            bool foundRigids = false;

            if (Selection.activeGameObject.GetComponentInChildren<Rigidbody>())
                foundRigids = true;

            if (foundRigids) {

                bool removeRigids = EditorUtility.DisplayDialog("Rigidbodies Found", "Additional rigidbodies found in your vehicle. Additional rigidbodies will affect vehicle behavior directly.", "Remove Them", "Leave Them");

                if (removeRigids) {

                    foreach (Rigidbody rigidbody in Selection.activeGameObject.GetComponentsInChildren<Rigidbody>())
                        DestroyImmediate(rigidbody);

                }

            }

            bool foundWheelColliders = false;

            if (Selection.activeGameObject.GetComponentInChildren<WheelCollider>())
                foundWheelColliders = true;

            if (foundWheelColliders) {

                bool removeWheelColliders = EditorUtility.DisplayDialog("WheelColliders Found", "Additional wheelcolliders found in your vehicle.", "Remove Them", "Leave Them");

                if (removeWheelColliders) {

                    foreach (WheelCollider wheelcollider in Selection.activeGameObject.GetComponentsInChildren<WheelCollider>())
                        DestroyImmediate(wheelcollider);

                }

            }

            bool fixPivot = EditorUtility.DisplayDialog("Fix Pivot Position Of The Vehicle", "Would you like to fix pivot position of the vehicle? If your vehicle has correct pivot position, select no.", "Fix", "No");

            if (fixPivot) {

                GameObject pivot = new GameObject(Selection.activeGameObject.name);
                pivot.transform.position = RCCP_GetBounds.GetBoundsCenter(Selection.activeGameObject.transform);
                pivot.transform.rotation = Selection.activeGameObject.transform.rotation;

                pivot.AddComponent<RCCP_CarController>();

                Rigidbody rigid = pivot.GetComponent<Rigidbody>();
                rigid.mass = 1350f;
                rigid.drag = .01f;
                rigid.angularDrag = .25f;
                rigid.interpolation = RigidbodyInterpolation.Interpolate;
                rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;

                Selection.activeGameObject.transform.SetParent(pivot.transform);
                Selection.activeGameObject = pivot;

            } else {

                GameObject selectedVehicle = Selection.activeGameObject;

                selectedVehicle.AddComponent<RCCP_CarController>();

                Rigidbody rigid = selectedVehicle.GetComponent<Rigidbody>();
                rigid.mass = 1350f;
                rigid.drag = .01f;
                rigid.angularDrag = .25f;
                rigid.interpolation = RigidbodyInterpolation.Interpolate;
                rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;

                Selection.activeGameObject = selectedVehicle;

            }

            int answer = EditorUtility.DisplayDialogComplex("Adding Components", "Would you like to add all components (engine, clutch, gearbox, differential, and axle) automatically?", "Yes", "No", "");

            if (answer == 0)
                addAllComponents = true;

        } else {

            EditorUtility.DisplayDialog("Your Gameobject Already Has Realistic Car Controller Pro", "Your Gameobject Already Has Realistic Car Controller Pro", "Close");

        }

    }

    private void AddAllComponents() {

        addAllComponents = false;

        AddEngine();
        AddClutch();
        AddGearbox();
        AddDifferential();
        AddAxles();
        AddEngineToClutchListener();
        AddClutchToGearboxListener();
        AddGearboxToDifferentialListener();
        AddDifferetialToAxle();

    }

    private void AddEngineToClutchListener() {

        prop.Engine.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.Clutch,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.Clutch, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.Engine.outputEvent, methodDelegate);

    }

    private void AddClutchToGearboxListener() {

        prop.Clutch.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.Gearbox,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.Gearbox, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.Clutch.outputEvent, methodDelegate);

    }

    private void AddGearboxToDifferentialListener() {

        prop.Gearbox.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.Differential,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.Differential, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.Gearbox.outputEvent, methodDelegate);

    }

    private void AddDifferetialToAxle() {

        prop.Differential.connectedAxle = prop.RearAxle;

    }

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

        if (!EditorApplication.isPlaying)
            ReOrderComponents();

    }

    private void ReOrderComponents() {

        prop = (RCCP_CarController)target;

        int index = 0;

        if (prop.Engine) {

            prop.Engine.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Clutch) {

            prop.Clutch.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Gearbox) {

            prop.Gearbox.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Differential) {

            prop.Differential.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.AxleManager) {

            prop.AxleManager.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Inputs) {

            prop.Inputs.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.AeroDynamics) {

            prop.AeroDynamics.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Stability) {

            prop.Stability.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Audio) {

            prop.Audio.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Lights) {

            prop.Lights.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Damage) {

            prop.Damage.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Particles) {

            prop.Particles.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.OtherAddonsManager) {

            prop.OtherAddonsManager.transform.SetSiblingIndex(index);
            index++;

        }

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_CarController)target;
        serializedObject.Update();
        GUI.skin = skin;
        guiColor = GUI.color;

        RCCP_SceneManager sm = RCCP_SceneManager.Instance;

        if (addAllComponents)
            AddAllComponents();

        if (!EditorApplication.isPlaying) {

            prop.checkComponents = false;
            prop.componentsTaken = false;

        }

        CheckMissingAxleManager();

        engine = prop.Engine;
        clutch = prop.Clutch;
        gearbox = prop.Gearbox;
        differential = prop.Differential;
        axles = prop.AxleManager;
        inputs = prop.Inputs;
        aero = prop.AeroDynamics;
        audio = prop.Audio;
        stability = prop.Stability;
        lights = prop.Lights;
        damage = prop.Damage;
        particles = prop.Particles;
        otherAddons = prop.OtherAddonsManager;

        if (EditorUtility.IsPersistent(prop))
            EditorGUILayout.HelpBox("Double click the prefab to edit settings. Some editor features are disabled in this mode.", MessageType.Warning);

        if (Screen.width < 500)
            EditorGUILayout.HelpBox("Increase width of your inspector panel to see all buttons.", MessageType.Warning);

        GUILayout.Label("Drivetrain");

        EditorGUILayout.BeginHorizontal();

        DrivetrainButtons();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        GUILayout.Label("Addons");

        EditorGUILayout.BeginHorizontal();

        AddonButtons();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();

        AddonButtons2();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ineffectiveBehavior"), new GUIContent("Ineffective Behavior", "Selected behavior in RCCP_Settings won't affect this vehicle if this option is enabled."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canControl"), new GUIContent("Can Control", "Is this vehicle controllable now? RCCP_Inputs attached to the vehicle will receive inputs when enabled."));

        EditorGUILayout.Space();

        statsEnabled = EditorGUILayout.BeginToggleGroup(new GUIContent("Runtime Stats", "Will be updated at runtime."), statsEnabled);

        if (statsEnabled) {

            if (!EditorApplication.isPlaying)
                EditorGUILayout.HelpBox("Stats will be updated at runtime", MessageType.Info);

            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRPM"), new GUIContent("Engine RPM"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minEngineRPM"), new GUIContent("Minimum Engine RPM"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEngineRPM"), new GUIContent("Maximum Engine RPM"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGear"), new GUIContent("Current Gear"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGearRatio"), new GUIContent("Current Gear Ratio"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lastGearRatio"), new GUIContent("Last Gear Ratio"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("differentialRatio"), new GUIContent("Differential Ratio"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Physically Speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelRPM2Speed"), new GUIContent("Wheel RPM 2 Speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tractionWheelRPM2EngineRPM"), new GUIContent("Wheel RPM 2 Engine RPM"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetWheelSpeedForCurrentGear"), new GUIContent("Target Wheel Speed For Current Gear"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSpeed"), new GUIContent("Maximum Speed"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("producedEngineTorque"), new GUIContent("Produced Engine Torque as NM"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("producedGearboxTorque"), new GUIContent("Produced Gearbox Torque as NM"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("producedDifferentialTorque"), new GUIContent("Produced Differential Torque as NM"));
            EditorGUILayout.Space();

            if (prop.poweredAxles != null)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("poweredAxles"), new GUIContent("Power Axles"), true);

            if (prop.brakedAxles != null)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("brakedAxles"), new GUIContent("Brake Axles"), true);

            if (prop.steeredAxles != null)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("steeredAxles"), new GUIContent("Steer Axles"), true);

            if (prop.handbrakedAxles != null)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakedAxles"), new GUIContent("Handbrake Axles"), true);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("direction"), new GUIContent("Direction"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftingNow"), new GUIContent("Shifting Now"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reversingNow"), new GUIContent("Reversing Now"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerAngle"), new GUIContent("Steer Angle"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("throttleInput_V"), new GUIContent("Vehicle Throttle Input"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeInput_V"), new GUIContent("Vehicle Brake Input"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerInput_V"), new GUIContent("Vehicle Steer Input"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeInput_V"), new GUIContent("Vehicle Handbrake Input"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clutchInput_V"), new GUIContent("Vehicle clutch Input"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nosInput_V"), new GUIContent("Nos Input"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lowBeamLights"), new GUIContent("Low Beam Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("highBeamLights"), new GUIContent("High Beam Lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorsLeftLights"), new GUIContent("Indicator Lights L"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorsRightLights"), new GUIContent("Indicator Lights R"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorsAllLights"), new GUIContent("Indicator Lights All"));

            GUI.enabled = true;

        }

        EditorGUILayout.EndToggleGroup();

        if (GUI.changed && !EditorApplication.isPlaying) {

            if (RCCP_Settings.Instance.setLayers)
                SetLayers();

            EditorUtility.SetDirty(prop);

        }

        serializedObject.ApplyModifiedProperties();

        if (!EditorApplication.isPlaying)
            CheckSetup();

        if (!EditorApplication.isPlaying)
            Repaint();

    }

    private void SetLayers() {

        prop.transform.gameObject.layer = LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPLayer);

        var children = prop.transform.GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
            child.gameObject.layer = LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPLayer);

        foreach (RCCP_WheelCollider item in prop.gameObject.GetComponentsInChildren<RCCP_WheelCollider>(true))
            item.gameObject.layer = LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPWheelColliderLayer);

        foreach (RCCP_DetachablePart item in prop.gameObject.GetComponentsInChildren<RCCP_DetachablePart>(true))
            item.gameObject.layer = LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPDetachablePartLayer);

    }

    private void DrivetrainButtons() {

        EditorGUILayout.BeginHorizontal();

        EngineButton();
        GUILayout.Space(1f);
        ClutchButton();
        GUILayout.Space(1f);
        GearboxButton();
        GUILayout.Space(1f);
        DifferentialButton();
        GUILayout.Space(1f);
        AxlesButton();

        EditorGUILayout.EndHorizontal();

    }

    private void AddonButtons() {

        EditorGUILayout.BeginHorizontal();

        InputsButton();
        GUILayout.Space(1f);
        AeroButton();
        GUILayout.Space(1f);
        StabilityButton();
        GUILayout.Space(1f);
        AudioButton();

        EditorGUILayout.EndHorizontal();

    }

    private void AddonButtons2() {

        EditorGUILayout.BeginHorizontal();

        LightsButton();
        GUILayout.Space(1f);
        DamageButton();
        GUILayout.Space(1f);
        ParticlesButton();
        GUILayout.Space(1f);
        OtherAddonsButton();

        EditorGUILayout.EndHorizontal();

    }

    private void EngineButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (engine)
            engine.enabled = EditorGUILayout.ToggleLeft("", engine.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Engine"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Engine") as Texture, GUILayout.Width(70f), GUILayout.Height(50f), GUILayout.ExpandWidth(true))) {

            if (engine)
                Selection.activeObject = prop.Engine.gameObject;
            else
                AddEngine();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (engine)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (engine) {

                bool completeSetup = prop.Engine.completeSetup;

                if (!completeSetup)
                    GUI.color = Color.red;

                if (engine.checkedSetup)
                    EditorGUILayout.LabelField(completeSetup ? "OK" : "Not OK", GUILayout.Width(50f));

                GUI.color = guiColor;

                if (GUILayout.Button("Check", GUILayout.Width(50f), GUILayout.ExpandWidth(true))) {

                    prop.checkComponents = true;
                    engine.checkedSetup = true;

                    if (engine)
                        Selection.activeObject = engine.gameObject;

                }

            }

            GUI.color = Color.red;

            if (engine) {

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveEngine();

            }

            GUI.color = guiColor;

        }

        EditorGUILayout.EndVertical();

    }

    private void ClutchButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (clutch)
            clutch.enabled = EditorGUILayout.ToggleLeft("", clutch.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Clutch"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Clutch") as Texture, GUILayout.Width(70f), GUILayout.Height(50f), GUILayout.ExpandWidth(true))) {

            if (clutch)
                Selection.activeObject = prop.Clutch.gameObject;
            else
                AddClutch();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (clutch)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (clutch) {

                bool completeSetup = prop.Clutch.completeSetup;

                if (!completeSetup)
                    GUI.color = Color.red;

                if (clutch.checkedSetup)
                    EditorGUILayout.LabelField(completeSetup ? "OK" : "Not OK", GUILayout.Width(50f));

                GUI.color = guiColor;

                if (GUILayout.Button("Check", GUILayout.Width(50f), GUILayout.ExpandWidth(true))) {

                    prop.checkComponents = true;
                    clutch.checkedSetup = true;

                    if (clutch)
                        Selection.activeObject = clutch.gameObject;

                }

            }

            GUI.color = Color.red;

            if (clutch) {

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveClutch();

            }

            GUI.color = guiColor;

        }

        EditorGUILayout.EndVertical();

    }

    private void GearboxButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (gearbox)
            gearbox.enabled = EditorGUILayout.ToggleLeft("", gearbox.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Gearbox"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Gearbox") as Texture, GUILayout.Width(70f), GUILayout.Height(50f), GUILayout.ExpandWidth(true))) {

            if (gearbox)
                Selection.activeObject = prop.Gearbox.gameObject;
            else
                AddGearbox();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (gearbox)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (gearbox) {

                bool completeSetup = prop.Gearbox.completeSetup;

                if (!completeSetup)
                    GUI.color = Color.red;

                if (gearbox.checkedSetup)
                    EditorGUILayout.LabelField(completeSetup ? "OK" : "Not OK", GUILayout.Width(50f));

                GUI.color = guiColor;

                if (GUILayout.Button("Check", GUILayout.Width(50f), GUILayout.ExpandWidth(true))) {

                    prop.checkComponents = true;
                    gearbox.checkedSetup = true;

                    if (gearbox)
                        Selection.activeObject = gearbox.gameObject;

                }

            }

            GUI.color = Color.red;

            if (gearbox) {

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveGearbox();

            }

            GUI.color = guiColor;

        }

        EditorGUILayout.EndVertical();

    }

    private void DifferentialButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (differential)
            differential.enabled = EditorGUILayout.ToggleLeft("", differential.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Differential"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Differential") as Texture, GUILayout.Width(70f), GUILayout.Height(50f), GUILayout.ExpandWidth(true))) {

            if (differential)
                Selection.activeObject = prop.Differential.gameObject;
            else
                AddDifferential();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (differential)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (differential) {

                bool completeSetup = prop.Differential.completeSetup;

                if (!completeSetup)
                    GUI.color = Color.red;

                if (differential.checkedSetup)
                    EditorGUILayout.LabelField(completeSetup ? "OK" : "Not OK", GUILayout.Width(50f));

                GUI.color = guiColor;

                if (GUILayout.Button("Check", GUILayout.Width(50f), GUILayout.ExpandWidth(true))) {

                    prop.checkComponents = true;
                    differential.checkedSetup = true;

                    if (differential)
                        Selection.activeObject = differential.gameObject;

                }

            }

            GUI.color = Color.red;

            if (differential) {

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveDifferential();

            }

            GUI.color = guiColor;

        }

        EditorGUILayout.EndVertical();

    }

    private void AxlesButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (differential)
            differential.enabled = EditorGUILayout.ToggleLeft("", differential.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Axles"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Axle") as Texture, GUILayout.Width(70f), GUILayout.Height(50f), GUILayout.ExpandWidth(true))) {

            if (axles)
                Selection.activeObject = prop.AxleManager.gameObject;
            else
                AddAxles();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (axles)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (axles) {

                bool completeSetup = prop.AxleManager.completeSetup;

                if (!completeSetup)
                    GUI.color = Color.red;

                if (axles.checkedSetup)
                    EditorGUILayout.LabelField(completeSetup ? "OK" : "Not OK", GUILayout.Width(50f));

                GUI.color = guiColor;

                if (GUILayout.Button("Check", GUILayout.Width(50f), GUILayout.ExpandWidth(true))) {

                    prop.checkComponents = true;
                    axles.checkedSetup = true;

                    if (axles)
                        Selection.activeObject = axles.gameObject;

                }

            }

            GUI.color = Color.red;

            if (axles) {

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveAxle();

            }

            GUI.color = guiColor;

        }

        EditorGUILayout.EndVertical();

    }

    private void InputsButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (inputs)
            inputs.enabled = EditorGUILayout.ToggleLeft("", inputs.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Inputs"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Inputs") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (inputs)
                Selection.activeObject = prop.Inputs.gameObject;
            else
                AddInputs();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (inputs)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (inputs) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveInputs();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void AudioButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (audio)
            audio.enabled = EditorGUILayout.ToggleLeft("", audio.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Audio"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Audio") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (audio)
                Selection.activeObject = prop.Audio.gameObject;
            else
                AddAudio();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (audio)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (audio) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveAudio();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void AeroButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (aero)
            aero.enabled = EditorGUILayout.ToggleLeft("", aero.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Dynamics"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Aero") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (aero)
                Selection.activeObject = prop.AeroDynamics.gameObject;
            else
                AddAero();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (aero)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (aero) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveAero();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void StabilityButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (stability)
            stability.enabled = EditorGUILayout.ToggleLeft("", stability.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Stability"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Stability") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (stability)
                Selection.activeObject = prop.Stability.gameObject;
            else
                AddStability();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (stability)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (stability) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveStability();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void LightsButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (lights)
            lights.enabled = EditorGUILayout.ToggleLeft("", lights.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Lights"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Light") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (lights)
                Selection.activeObject = prop.Lights.gameObject;
            else
                AddLights();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (lights)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (lights) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveLights();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void DamageButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (damage)
            damage.enabled = EditorGUILayout.ToggleLeft("", damage.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Damage"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Damage") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (damage)
                Selection.activeObject = prop.Damage.gameObject;
            else
                AddDamage();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (damage)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (damage) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveDamage();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void ParticlesButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (particles)
            particles.enabled = EditorGUILayout.ToggleLeft("", particles.enabled, GUILayout.Width(15f));

        GUILayout.Label(("Particles"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_Particles") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (particles)
                Selection.activeObject = prop.Particles.gameObject;
            else
                AddParticles();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (particles)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (particles) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveParticles();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void OtherAddonsButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        GUILayout.Label(("Other Addons"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

        if (GUILayout.Button(Resources.Load("Editor Icons/Icon_OtherAddons") as Texture, GUILayout.Width(70f), GUILayout.Height(35f), GUILayout.ExpandWidth(true))) {

            if (otherAddons)
                Selection.activeObject = prop.OtherAddonsManager.gameObject;
            else
                AddOtherAddons();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (otherAddons)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (otherAddons) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove", GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    RemoveOtherAddons();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void AddEngine() {

        if (EditorUtility.IsPersistent(prop))
            return;
        
        GameObject subject = new GameObject("RCCP_Engine");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.AddComponent<RCCP_Engine>();

    }

    private void AddClutch() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Clutch");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(1);
        subject.gameObject.AddComponent<RCCP_Clutch>();

    }

    private void AddGearbox() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Gearbox");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(2);
        subject.gameObject.AddComponent<RCCP_Gearbox>();

    }

    private void AddDifferential() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Differential");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(3);
        subject.gameObject.AddComponent<RCCP_Differential>();

    }

    private void AddAxles() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Axles");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(4);
        subject.gameObject.AddComponent<RCCP_Axles>();

    }

    private void AddAxle() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Axle_New");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(4);
        RCCP_Axle axle = subject.gameObject.AddComponent<RCCP_Axle>();
        axle.gameObject.name = "RCCP_Axle_New";
        axle.isBrake = true;
        axle.isHandbrake = true;

    }

    private void AddInputs() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Inputs");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(5);
        subject.gameObject.AddComponent<RCCP_Input>();

    }

    private void AddAero() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Aero");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(6);
        subject.gameObject.AddComponent<RCCP_AeroDynamics>();

    }

    private void AddAudio() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Audio");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(7);
        subject.gameObject.AddComponent<RCCP_Audio>();

    }

    private void AddStability() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Stability");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(8);
        subject.gameObject.AddComponent<RCCP_Stability>();

    }

    private void AddLights() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Lights");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(9);
        subject.gameObject.AddComponent<RCCP_Lights>();

    }

    private void AddDamage() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Damage");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(10);
        subject.gameObject.AddComponent<RCCP_Damage>();

    }

    private void AddParticles() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Particles");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(11);
        subject.gameObject.AddComponent<RCCP_Particles>();

    }

    private void AddOtherAddons() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_OtherAddons");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(12);
        subject.gameObject.AddComponent<RCCP_OtherAddons>();

    }

    private void RemoveEngine() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(engine.gameObject);

    }

    private void RemoveClutch() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(clutch.gameObject);

    }

    private void RemoveGearbox() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(gearbox.gameObject);

    }

    private void RemoveDifferential() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(differential.gameObject);

    }

    private void RemoveAxle() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(axles.gameObject);

    }

    private void RemoveInputs() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(inputs.gameObject);

    }

    private void RemoveAero() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(aero.gameObject);

    }

    private void RemoveAudio() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(audio.gameObject);

    }

    private void RemoveStability() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(stability.gameObject);

    }

    private void RemoveLights() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(lights.gameObject);

    }

    private void RemoveDamage() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(damage.gameObject);

    }

    private void RemoveParticles() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(particles.gameObject);

    }

    private void RemoveOtherAddons() {

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(otherAddons.gameObject);

    }

    private void CheckSetup() {

        errorMessages.Clear();

        Collider colliderFound = null;
        Collider[] allColliders = prop.gameObject.GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < allColliders.Length; i++) {

            Collider col = allColliders[i];

            if (!(col is WheelCollider))
                colliderFound = col;

        }

        if (colliderFound == null)
            errorMessages.Add("Body collider not found");

        if (prop.AxleManager != null) {

            bool steerAxlefound = false;

            List<RCCP_Axle> allAxles = prop.AxleManager.Axles;

            if (allAxles != null && allAxles.Count < 2)
                errorMessages.Add("Two axles needed at least. Vehicle has " + allAxles.Count.ToString() + " axle currently.");

            if (allAxles != null) {

                for (int i = 0; i < allAxles.Count; i++) {

                    if (allAxles[i] != null && allAxles[i].isSteer)
                        steerAxlefound = true;

                }

            }

            if (!steerAxlefound)
                errorMessages.Add("Steer axle not found");

        }

        bool missingWheelModelsFound = false;

        if (prop.AxleManager != null) {

            for (int i = 0; i < prop.AxleManager.Axles.Count; i++) {

                if (prop.AxleManager.Axles[i].leftWheelModel == null || prop.AxleManager.Axles[i].rightWheelModel == null)
                    missingWheelModelsFound = true;

            }

        }

        if (missingWheelModelsFound)
            errorMessages.Add("Missing Wheel Models Found On Axle");

        GUI.color = Color.red;

        for (int i = 0; i < errorMessages.Count; i++) {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(errorMessages[i]);
            EditorGUILayout.EndVertical();

        }

        GUI.color = guiColor;

    }

    private void CheckMissingAxleManager() {

        bool axleFound = false;

        RCCP_Axle[] foundAxles = prop.GetComponentsInChildren<RCCP_Axle>(true);

        if (foundAxles.Length >= 1)
            axleFound = true;

        if (axleFound) {

            bool axleManagerFound = false;

            RCCP_Axles foundAxleManager = prop.GetComponentInChildren<RCCP_Axles>(true);

            if (foundAxleManager != null)
                axleManagerFound = true;

            if (!axleManagerFound) {

                GameObject newAxleManager = new GameObject("RCCP_Axles");
                newAxleManager.transform.SetParent(prop.transform, false);
                newAxleManager.AddComponent<RCCP_Axles>();
                Debug.Log("Found missing axle manager on " + prop.transform.name + ". Adding it...");

            } else {

                for (int i = 0; i < foundAxles.Length; i++) {

                    if (foundAxles[i].transform.parent != foundAxleManager.transform)
                        foundAxles[i].transform.SetParent(foundAxleManager.transform, false);

                }

            }

        }

    }

}
