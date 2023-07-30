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

[CustomEditor(typeof(RCCP_Particles))]
public class RCCP_ParticlesEditor : Editor {

    RCCP_Particles prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Particles)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Particles.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionFilter"), new GUIContent("Collision Filter", "Contact particles will be enabled on these layers."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contactSparklePrefab"), new GUIContent("Contact Sparkle Prefab", "Contact sparkle prefab will be used."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scratchSparklePrefab"), new GUIContent("Scratch Sparkle Prefab", "Scratch sparkle prefab will be used on scratches."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelSparklePrefab"), new GUIContent("Wheel Sparkle Prefab", "Wheel sparkle prefab will be used on flat wheels."));

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

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

}
