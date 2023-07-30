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

[CustomEditor(typeof(RCCP_WheelCollider))]
[CanEditMultipleObjects]
public class RCCP_WheelColliderEditor : Editor {

    RCCP_WheelCollider prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_WheelCollider)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("A circular object that revolves on an axle and is fixed below a vehicle or other object to enable it to move easily over the ground. Just kidding :)", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("connectedAxle"), new GUIContent("Connected Axle", "Connected to this axle. Axle will take control of this wheelcollider."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelModel"), new GUIContent("Wheel Model", "Visual model of the wheel. This wheelcollider will be aligned with this model."));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"), new GUIContent("Width", "Width of the wheel used to draw skidmarks."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("camber"), new GUIContent("Camber", "Camber angle of the wheel."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("caster"), new GUIContent("Caster", "Caster angle of the wheel."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"), new GUIContent("Offset", "Offset of the wheel."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("drawSkid"), new GUIContent("Draw Skidmarks", "Draws skidmarks on surface."));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deflatedRadiusMultiplier"), new GUIContent("Deflated Radius Multiplier", "Radius of the wheelcollider will be multiplied by this value if deflated."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deflatedStiffnessMultiplier"), new GUIContent("Deflated Stiffness Multiplier", "."));
        EditorGUILayout.Space();

        if (BehaviorSelected())
            GUI.color = Color.red;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("driftMode"), new GUIContent("Drift Mode", "Modifies curves of the forward and sideways friction to keep stable drifting."));

        GUI.color = guiColor;

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

            if (!EditorApplication.isPlaying && prop.connectedAxle && prop.connectedAxle.autoAlignWheelColliders)
                prop.AlignWheel();

        }

        if (BehaviorSelected())
            EditorGUILayout.HelpBox("Settings with red labels and frictions of the wheelcolliders will be overridden by the selected behavior in RCCP_Settings", MessageType.None);

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (prop.width < 0)
            prop.width = 0;

        bool completeSetup = true;
        errorMessages.Clear();

        if (!prop.wheelModel)
            errorMessages.Add("Wheel model not selected");

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
