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

[CustomEditor(typeof(RCCP_Engine))]
public class RCCP_EngineEditor : Editor {

    RCCP_Engine prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Engine)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Main power generator of the vehicle. Produces and transmits the generated power to the clutch.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("minEngineRPM"), new GUIContent("Minimum RPM", "Target idle engine rpm will be counted as minimum engine rpm."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEngineRPM"), new GUIContent("Maximum RPM", "Maximum engine rpm."));
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRPM"), new GUIContent("Current RPM", "Current engine rpm."));
        GUI.enabled = true;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoCreateNMCurve"), new GUIContent("Auto Create NM Curve", "Creates NM curve automatically. First keyframe indicates minimum engine rpm, and last keyfrake indicates maximum engine rpm."));

        if (prop.NMCurve == null)
            prop.NMCurve = new AnimationCurve(new Keyframe(prop.minEngineRPM, .8f), new Keyframe(prop.maxTorqueAtRPM, 1f), new Keyframe(prop.maxEngineRPM, .85f));

        if (prop.NMCurve != null && prop.NMCurve.keys.Length < 2)
            prop.NMCurve = new AnimationCurve(new Keyframe(prop.minEngineRPM, .8f), new Keyframe(prop.maxTorqueAtRPM, 1f), new Keyframe(prop.maxEngineRPM, .85f));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("NMCurve"), new GUIContent("NM Curve", "Torque curve based on RPM. First keyframe indicates minimum engine rpm, and last keyfrake indicates maximum engine rpm."));

        if (prop.autoCreateNMCurve) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTorqueAtRPM"), new GUIContent("Maximum Torque At RPM", "Top peak keyframe of the NM Curve. Engine will be most powerful at this RPM."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("turboCharged"), new GUIContent("TurboCharged", "."));

        if (prop.turboCharged) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTurboChargePsi"), new GUIContent("Maximum TurboCharge PSI", "Maximum TurboCharge PSI."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turboChargerCoEfficient"), new GUIContent("TurboCharged CoEfficient", "TurboCharged CoEfficient (Multiplier)."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRevLimiter"), new GUIContent("Rev Limiter", "Cuts the fuel input when engine rpm exceeds the maximum limit."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineFriction"), new GUIContent("Friction", "Engine friction factor."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineInertia"), new GUIContent("Inertia", "Fast reactions on lower values, slower reactions on higher values."));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRunning"), new GUIContent("Running", "Is engine running right now?"));
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineStarting"), new GUIContent("Starting", "Is engine starting right now?"));
        GUI.enabled = true;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumTorqueAsNM"), new GUIContent("Maximum Torque As NM", "Maximum producable torque as NM. Engine will give this torque at peak rpm."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("producedTorqueAsNM"), new GUIContent("Produced Torque As NM", "Current produced engine torque as NM."));

        GUI.skin = null;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent"), new GUIContent("Output Event", "Produced torque will be transfered to this component."));
        GUI.skin = skin;

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Add Output To Clutch"))
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

        if (prop.autoCreateNMCurve)
            prop.NMCurve = new AnimationCurve(new Keyframe(prop.minEngineRPM, .80f), new Keyframe(prop.maxTorqueAtRPM, 1f), new Keyframe(prop.maxEngineRPM, .85f));

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (prop.minEngineRPM < 0)
            prop.minEngineRPM = 0f;

        if (prop.engineFriction < 0)
            prop.engineFriction = 0f;

        if (prop.engineInertia < 0)
            prop.engineInertia = 0f;

        if (prop.maximumTorqueAsNM < 0)
            prop.maximumTorqueAsNM = 0f;

        bool completeSetup = true;
        errorMessages.Clear();

        if (prop.outputEvent == null)
            errorMessages.Add("Output event not selected");

        if (prop.NMCurve == null)
            errorMessages.Add("NM Curve not configured");

        if (prop.NMCurve != null && prop.NMCurve.keys[prop.NMCurve.keys.Length - 1].time != prop.maxEngineRPM)
            errorMessages.Add("Last key of the NM Curve doesn't point the max engine rpm");

        if (prop.NMCurve != null && prop.NMCurve.keys[0].time != prop.minEngineRPM)
            errorMessages.Add("First key of the NM Curve doesn't point the min engine rpm");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() < 1)
            errorMessages.Add("Output event not selected");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() > 0 && prop.outputEvent.GetPersistentMethodName(0) == "")
            errorMessages.Add("Output event created, but object or method not selected (ReceiveOutput). Engine should be transmiting the power to the clutch.");

        if (prop.NMCurve != null && prop.NMCurve.keys.Length < 2)
            errorMessages.Add("NM Curve not configured");

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

        if (prop.GetComponentInParent<RCCP_CarController>(true).Clutch == null) {

            Debug.LogError("Clutch not found. Event is not added.");
            return;

        }

        prop.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.GetComponentInParent<RCCP_CarController>(true).Clutch,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.GetComponentInParent<RCCP_CarController>(true).Clutch, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.outputEvent, methodDelegate);

    }

}
