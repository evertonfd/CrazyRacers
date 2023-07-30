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

[CustomEditor(typeof(RCCP_Differential))]
public class RCCP_DifferentialEditor : Editor {

    RCCP_Differential prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Differential)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Transmits the received power from the engine --> clutch --> gearbox to the axle. Open differential = RPM difference between both wheels will decide to which wheel needs more traction or not. Limited = almost same with open with slip limitation. Higher percents = more close to the locked system. Locked = both wheels will have the same traction.", MessageType.Info, true);

        if (BehaviorSelected())
            GUI.color = Color.red;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("differentialType"), new GUIContent("Differential Type", "Differential types. Types are explained above."));

        GUI.color = guiColor;

        if (prop.differentialType == RCCP_Differential.DifferentialType.Limited)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("limitedSlipRatio"), new GUIContent("Limited Slip Ratio", "Limited slip ratio."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("finalDriveRatio"), new GUIContent("Final Drive Ratio", "Final drive ratio will be multiplied by received torque from the gearbox."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("connectedAxle"), new GUIContent("Connected Axle", "An axle must be connected to this differential at least."), true);

        EditorGUILayout.Space();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedTorqueAsNM"), new GUIContent("Received Torque As NM", "Received torque from the gearbox as NM."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("producedTorqueAsNM"), new GUIContent("Produced Torque As NM", "Produced torque as NM."));
        GUI.enabled = true;

        if (prop.connectedAxle == null) {

            if (prop.GetComponentInParent<RCCP_CarController>(true).AxleManager != null) {

                for (int i = 0; i < prop.GetComponentInParent<RCCP_CarController>(true).AxleManager.Axles.Count; i++) {

                    if (GUILayout.Button("Connect to " + prop.GetComponentInParent<RCCP_CarController>(true).AxleManager.Axles[i].gameObject.name))
                        prop.connectedAxle = prop.GetComponentInParent<RCCP_CarController>(true).AxleManager.Axles[i];

                }

            }

        } else {

            GUI.color = Color.red;

            if (GUILayout.Button("Remove connection to " + prop.connectedAxle.gameObject.name)) {

                bool decision = EditorUtility.DisplayDialog("Remove connection to " + prop.connectedAxle.gameObject.name, "Are you sure want to remove connection to the " + prop.connectedAxle.gameObject.name + "?", "Yes", "No");

                if (decision)
                    prop.connectedAxle = null;

            }

            GUI.color = guiColor;

        }

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

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

        if (BehaviorSelected())
            EditorGUILayout.HelpBox("Settings with red labels will be overridden by the selected behavior in RCCP_Settings", MessageType.None);

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (prop.finalDriveRatio < .01)
            prop.finalDriveRatio = .01f;

        bool completeSetup = true;
        errorMessages.Clear();

        if (prop.connectedAxle == null)
            errorMessages.Add("Output axle not selected");

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

    private bool BehaviorSelected() {

        bool state = RCCP_Settings.Instance.overrideBehavior;

        if (prop.GetComponentInParent<RCCP_CarController>(true).ineffectiveBehavior)
            state = false;

        return state;

    }

}
