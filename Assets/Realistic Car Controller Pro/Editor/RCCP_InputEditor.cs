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

[CustomEditor(typeof(RCCP_Input))]
public class RCCP_InputEditor : Editor {

    RCCP_Input prop;
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Input)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Receives player inputs from the RCCP_InputManager. All connected systems to the vehicle will be using player inputs if this is attached.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideInternalInputs"), new GUIContent("Override Internal Inputs", "Override internal inputs?"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideExternalInputs"), new GUIContent("Override External Inputs", "Override external inputs?"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("throttleInput"), new GUIContent("Throttle", "Throttle input."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("steerInput"), new GUIContent("Steer", "Steer input."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeInput"), new GUIContent("Brake", "Brake input."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeInput"), new GUIContent("Handbrake", "Handbrake input."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("clutchInput"), new GUIContent("Clutch", "Clutch input."));

        EditorGUILayout.Space();

        if (BehaviorSelected())
            GUI.color = Color.red;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringCurve"), new GUIContent("Steering Curve", "Steering curve based on speed of the vehicle. Multiplier will decrease based on speed of the vehicle."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringLimiter"), new GUIContent("Steering Limiter", "Limits the steering angle if vehicle is skidding."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("counterSteering"), new GUIContent("Counter Steering", "Applies counter steering if vehicle is skidding."));

        if (prop.counterSteering)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("counterSteerFactor"), new GUIContent("Counter Steering Factor", "Counter steer mutliplier."));

        GUI.color = guiColor;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReverse"), new GUIContent("Auto Reverse Gear", "Shifts to reverse gear if vehicle is completely stopped and player still pushes the brake button."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inverseThrottleBrakeOnReverse"), new GUIContent("Inverse Throttle - Brake On Reverse Gear", "Brake button will be throttle, throttle button will be brake while reversing."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cutThrottleWhenShifting"), new GUIContent("Cut Throttle While Shifting", "Cuts throttle input while shifting."));

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

        }

        if (BehaviorSelected())
            EditorGUILayout.HelpBox("Settings with red labels will be overridden by the selected behavior in RCCP_Settings", MessageType.None);

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private bool BehaviorSelected() {

        bool state = RCCP_Settings.Instance.overrideBehavior;

        if (prop.GetComponentInParent<RCCP_CarController>(true).ineffectiveBehavior)
            state = false;

        return state;

    }

}
