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

[CustomEditor(typeof(RCCP_Clutch))]
public class RCCP_ClutchEditor : Editor {

    RCCP_Clutch prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Clutch)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Connecter between engine and the gearbox. Transmits the received power from the engine to the gearbox or not.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("clutchInput"), new GUIContent("Input", "Current clutch input."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("clutchInertia"), new GUIContent("Inertia", "Faster reactions on lower values, slower reactions on higher values."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticClutch"), new GUIContent("Automatic Clutch", "Adjusts clutch input automatically based on vehicle speed - engine rpm relation."));

        if (prop.automaticClutch)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engageRPM"), new GUIContent("Engage RPM", "Clutch will be pressed if engine rpm is lower than this value."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedTorqueAsNM"), new GUIContent("Received Torque As NM", "Received torque from the engine as NM."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("producedTorqueAsNM"), new GUIContent("Produced Torque As NM", "Produced torque for target component as NM."));

        GUI.skin = null;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent"), new GUIContent("Output Event", "Produced torque will be transfered to this component."));
        GUI.skin = skin;

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Add Output To Gearbox"))
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

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (prop.engageRPM < 0)
            prop.engageRPM = 0f;

        bool completeSetup = true;
        errorMessages.Clear();

        if (prop.outputEvent == null)
            errorMessages.Add("Output event not selected");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() < 1)
            errorMessages.Add("Output event not selected");

        if (prop.outputEvent != null && prop.outputEvent.GetPersistentEventCount() > 0 && prop.outputEvent.GetPersistentMethodName(0) == "")
            errorMessages.Add("Output event created, but object or method not selected");

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

        if (prop.GetComponentInParent<RCCP_CarController>(true).Gearbox == null) {

            Debug.LogError("Gearbox not found. Event is not added.");
            return;

        }

        prop.outputEvent = new RCCP_Event_Output();

        var targetinfo = UnityEvent.GetValidMethodInfo(prop.GetComponentInParent<RCCP_CarController>(true).Gearbox,
"ReceiveOutput", new Type[] { typeof(RCCP_Output) });

        var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<RCCP_Output>), prop.GetComponentInParent<RCCP_CarController>(true).Gearbox, targetinfo) as UnityAction<RCCP_Output>;
        UnityEventTools.AddPersistentListener(prop.outputEvent, methodDelegate);

    }

}
