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

[CustomEditor(typeof(RCCP_Gearbox))]
public class RCCP_GearboxEditor : Editor {

    RCCP_Gearbox prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Gearbox)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Multiplies the received power from the engine --> clutch by x ratio, and transmits it to the differential. Higher ratios = faster accelerations, lower top speeds, lower ratios = slower accelerations, higher top speeds.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gearRatios"), new GUIContent("Gear Ratios", "Faster accelerations and slower top speeds on higher ratios. Slower accelerations and higher top speeds on lower ratios."));

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gearRPMs"), new GUIContent("Gear RPMs", "Maximum gear rpms related to maximum engine RPM."));
        float[] gearSpeeds = prop.GearRPMs;
        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGear"), new GUIContent("Current Gear", "Current gear as index. Add +1 for real gear count."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gearInput"), new GUIContent("Input", "Any gear is in use now?"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("reverseGearEngaged"), new GUIContent("Reverse Gear Engaged", "Reverse gear is in use now. First gear ratio will be used for reverse gear."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralGearEngaged"), new GUIContent("Neutral Gear Engaged", "Neutral gear is in use now."));

        EditorGUILayout.Space();

        if (BehaviorSelected())
            GUI.color = Color.red;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftingTime"), new GUIContent("Shifting Delay", "Delay timer for shifting between gears."));

        GUI.color = guiColor;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftingNow"), new GUIContent("Shifting Now", "Shifting now?"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dontShiftTimer"), new GUIContent("Dont Shift Timer", "Won't shift gears instantly."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticTransmission"), new GUIContent("Automatic Transmission", "Shifts up & down automatically at certain speeds and engine rpm."));

        if (prop.automaticTransmission) {

            EditorGUI.indentLevel++;

            if (BehaviorSelected())
                GUI.color = Color.red;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftThreshold"), new GUIContent("Shift Threshold", "Shifts soon on lower values. Shifts late on higher values."));

            GUI.color = guiColor;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftUpRPM"), new GUIContent("Shift Up RPM", "Target rpm to shift up."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftDownRPM"), new GUIContent("Shift Down RPM", "Target rpm to shift down."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedTorqueAsNM"), new GUIContent("Received Torque As NM", "Received torque from the clutch or engine."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("producedTorqueAsNM"), new GUIContent("Produced Torque As NM", "Produced torque for target component as NM."));

        GUI.skin = null;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent"), new GUIContent("Output Event", "Produced torque will be transfered to this component."));
        GUI.skin = skin;

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Add Output To Differential"))
                AddListener();

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

            if (prop.GetComponentInParent<RCCP_CarController>(true).checkComponents) {

                prop.GetComponentInParent<RCCP_CarController>(true).checkComponents = false;

                if (errorMessages.Count > 0) {

                    if (EditorUtility.DisplayDialog("Errors found", errorMessages.Count + " Errors found!", "Cancel", "Check"))
                        Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

                } else {

                    Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;
                    Debug.Log("No errors found");

                }

            }

        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("1 Gear Preset")) {

            prop.InitGears(1);
            EditorUtility.SetDirty(prop);

        }

        if (GUILayout.Button("2 Gears Preset")) {

            prop.InitGears(2);
            EditorUtility.SetDirty(prop);

        }

        if (GUILayout.Button("3 Gears Preset")) {

            prop.InitGears(3);
            EditorUtility.SetDirty(prop);

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("4 Gears Preset")) {

            prop.InitGears(4);
            EditorUtility.SetDirty(prop);

        }

        if (GUILayout.Button("5 Gears Preset")) {

            prop.InitGears(5);
            EditorUtility.SetDirty(prop);

        }

        if (GUILayout.Button("6 Gears Preset")) {

            prop.InitGears(6);
            EditorUtility.SetDirty(prop);

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("7 Gears Preset")) {

            prop.InitGears(7);
            EditorUtility.SetDirty(prop);

        }

        if (GUILayout.Button("8 Gears Preset")) {

            prop.InitGears(8);
            EditorUtility.SetDirty(prop);

        }

        EditorGUILayout.EndHorizontal();

        if (BehaviorSelected())
            EditorGUILayout.HelpBox("Settings with red labels will be overridden by the selected behavior in RCCP_Settings", MessageType.None);

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (prop.currentGear < 0)
            prop.currentGear = 0;

        if (prop.shiftingTime < 0)
            prop.shiftingTime = 0f;

        if (prop.shiftUpRPM < 0)
            prop.shiftUpRPM = 0f;

        if (prop.shiftDownRPM < 0)
            prop.shiftDownRPM = 0f;

        if (prop.gearRatios == null || prop.gearRatios.Length < 1)
            prop.gearRatios = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };

        for (int i = 0; i < prop.gearRatios.Length; i++) {

            if (prop.gearRatios[i] < .01f)
                prop.gearRatios[i] = .01f;

        }

        bool completeSetup = true;
        errorMessages.Clear();

        if (prop.outputEvent == null)
            errorMessages.Add("Output event not selected");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() < 1)
            errorMessages.Add("Output event not selected");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() > 0 && prop.outputEvent.GetPersistentMethodName(0) == "")
            errorMessages.Add("Output event created, but object or method not selected");

        if (prop.GetComponentInParent<RCCP_CarController>(true).Engine) {

            if (prop.shiftUpRPM > prop.GetComponentInParent<RCCP_CarController>(true).Engine.maxEngineRPM)
                errorMessages.Add("Shift up rpm can't be higher than the engine max rpm");

            if (prop.shiftDownRPM < prop.GetComponentInParent<RCCP_CarController>(true).Engine.minEngineRPM)
                errorMessages.Add("Shift down rpm can't be lower than the engine min rpm");

        }

        if (prop.shiftDownRPM >= prop.shiftUpRPM)
            errorMessages.Add("Shift down rpm can't be higher than the shift up rpm");

        if (prop.shiftUpRPM <= prop.shiftDownRPM)
            errorMessages.Add("Shift up rpm can't be lower than the shift down rpm");

        if (errorMessages.Count > 0)
            completeSetup = false;

        prop.completeSetup = completeSetup;

        if (!completeSetup)
            EditorGUILayout.HelpBox("Errors found!", MessageType.Error, true);

        GUI.color = Color.red;

        for (int i = 0; i < errorMessages.Count; i++) {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(errorMessages[i]);
            EditorGUILayout.EndVertical();

        }

        GUI.color = guiColor;

    }

    private void AddListener() {

        if (prop.GetComponentInParent<RCCP_CarController>(true).Differential == null) {

            Debug.LogError("Differential not found. Event is not added.");
            return;

        }

        prop.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.GetComponentInParent<RCCP_CarController>(true).Differential,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.GetComponentInParent<RCCP_CarController>(true).Differential, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.outputEvent, methodDelegate);

    }

    private bool BehaviorSelected() {

        bool state = RCCP_Settings.Instance.overrideBehavior;

        if(prop.GetComponentInParent<RCCP_CarController>(true).ineffectiveBehavior)
            state = false;

        return state;

    }

}
