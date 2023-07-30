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

[CustomEditor(typeof(RCCP_AeroDynamics))]
public class RCCP_AeroDynamicsEditor : Editor {

    RCCP_AeroDynamics prop;
    GUISkin skin;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_AeroDynamics)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Manages the dynamics of the vehicle.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"), new GUIContent("COM", "Centre of mass. Must be placed correctly. You can google it for vehicles to see which locations are suitable."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("downForce"), new GUIContent("Downforce", "Downforce will be applied to the vehicle related with vehicle speed."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReset"), new GUIContent("Auto Reset", "Auto resets the vehicle if upside down."));

        if (prop.autoReset)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoResetTime"), new GUIContent("Auto Reset Timer", "Auto reset timer limit. Vehicle will reset itself within this time."));

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

        }

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

}
