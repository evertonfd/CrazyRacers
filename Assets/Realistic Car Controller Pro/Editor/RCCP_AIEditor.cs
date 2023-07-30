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

[CustomEditor(typeof(RCCP_AI))]
public class RCCP_AIEditor : Editor {

    RCCP_AI prop;
    GUISkin skin;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_AI)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("AI inputs will be used to control the vehicle. Even in RCCP_Inputs attached to the vehicle, vehicle won't receive player inputs.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("navigationMode"), new GUIContent("Navigation Mode", "Navigation modes."));

        if (prop.navigationMode == RCCP_AI.NavigationMode.FollowWaypoints) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypointsContainer"), new GUIContent("Waypoints Container", "Waypoints container."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextWaypointPassDistance"), new GUIContent("Next Waypoint Pass Distance", "Distance for passing to the next waypoint."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentWaypointIndex"), new GUIContent("Current Waypoint Index", "Current index of the waypoint."));
            EditorGUI.indentLevel--;

        }

        if (prop.navigationMode == RCCP_AI.NavigationMode.ChaseTarget || prop.navigationMode == RCCP_AI.NavigationMode.FollowTarget) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTag"), new GUIContent("Target tag", "Target tag."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startChaseDistance"), new GUIContent("Start Chase Distance", "Vehicle will start to chase within this distance."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stopFollowDistance"), new GUIContent("Stop Follow Distance", "Vehicle will start to follow within this distance."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("useRaycasts"), new GUIContent("Use Raycasts", "Raycasts will be used to avoid obstacles on the path. Has impact on performance as well."));

        if (prop.useRaycasts) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastLength"), new GUIContent("Raycast Length", "Length of the raycasts."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastAngle"), new GUIContent("Raycast Angle", "Angle as degree of the raycasts."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("obstacleLayers"), new GUIContent("Obstacle Layers", "These layers will be used to detect obstacles."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("limitSpeed"), new GUIContent("Limit Speed", "Limits the maximum speed."));

        if(prop.limitSpeed)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSpeed"), new GUIContent("Maximum Speed", "Maximum limited speed."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothedSteer"), new GUIContent("Smoothed Steering", "Smoothed steering."));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("totalWaypointsPassed"), new GUIContent("Total Waypoints Passed", "How many waypoints passed."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lap"), new GUIContent("Total Laps", "How many laps passed."));
       
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stopAfterLap"), new GUIContent("Stop After Lap", "Vehicle should stop after target lap."));

        if (prop.stopAfterLap)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stopLap"), new GUIContent("Stop Lap", "Vehicle should stop after target lap."));

        if (GUILayout.Button("Back"))
            Selection.activeObject = prop.GetComponentInParent<RCCP_OtherAddons>(true).gameObject;

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

}
