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

[CustomEditor(typeof(RCCP_Axle))]
public class RCCP_AxleEditor : Editor {

    RCCP_Axle prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Axle)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Transmits the received power from the differential and share to the wheels (if differential is connected to this axle). Steering, braking, traction, and all wheel related processes are managed by this axle. Has two connected wheels.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftWheelModel"), new GUIContent("Left Wheel Model", "Left wheel model."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightWheelModel"), new GUIContent("Right Wheel Model", "Right wheel model."));
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftWheelCollider"), new GUIContent("Left WheelCollider", "Left wheelcollider."));

        if (prop.leftWheelCollider) {

            if (!EditorApplication.isPlaying && prop.leftWheelModel)
                prop.leftWheelCollider.wheelModel = prop.leftWheelModel;

            if (GUILayout.Button("Edit"))
                Selection.activeObject = prop.leftWheelCollider;

        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightWheelCollider"), new GUIContent("Right WheelCollider", "Right wheelcollider."));

        if (prop.rightWheelCollider) {

            if (!EditorApplication.isPlaying && prop.rightWheelModel)
                prop.rightWheelCollider.wheelModel = prop.rightWheelModel;

            if (GUILayout.Button("Edit"))
                Selection.activeObject = prop.rightWheelCollider;

        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoAlignWheelColliders"), new GUIContent("Auto Align WheelColliders", "Will align wheelcolliders automatically if the wheel model is selected."));

        EditorGUILayout.Space();

        if (BehaviorSelected())
            GUI.color = Color.red;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("antirollForce"), new GUIContent("AntiRoll Force", "Force amount to be used for antiroll."));

        GUI.color = guiColor;

        EditorGUILayout.Space();

        if (prop.isPower)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("powerMultiplier"), new GUIContent("Power Multiplier", "Power multiplier."));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isSteer"), new GUIContent("Is Steer", "Is steering?"));

        if (prop.isSteer) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSteerAngle"), new GUIContent("Maximum Steer Angle", "Maximum steering angle as degree."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerSpeed"), new GUIContent("Steering Speed", "Steering speed."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerMultiplier"), new GUIContent("Steer Multiplier", "Steer multiplier."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isBrake"), new GUIContent("Is Brake", "Is braking?"));

        if (prop.isBrake) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxBrakeTorque"), new GUIContent("Maximum Brake Torque", "Maximum brake torque as NM."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeMultiplier"), new GUIContent("Brake Multiplier", "Brake multiplier."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isHandbrake"), new GUIContent("Is Handbrake", "Is handbrake?"));

        if (prop.isHandbrake) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeMultiplier"), new GUIContent("Handbrake Multiplier", "Handbrake multiplier."));
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (prop.autoAlignWheelColliders && !EditorApplication.isPlaying) {

                if (prop.leftWheelCollider)
                    prop.leftWheelCollider.AlignWheel();

                if (prop.rightWheelCollider)
                    prop.rightWheelCollider.AlignWheel();

            }

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_Axles>(true).gameObject;

            if (GUILayout.Button("Remove"))
                RemoveAxle();

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

        if (prop.antirollForce < 0)
            prop.antirollForce = 0f;

        if (prop.maxSteerAngle < 0)
            prop.maxSteerAngle = 0f;

        if (prop.maxBrakeTorque < 0)
            prop.maxBrakeTorque = 0f;

        bool completeSetup = true;
        errorMessages.Clear();

        if (prop.leftWheelCollider == null)
            errorMessages.Add("Left wheelcollider not found");

        if (prop.rightWheelCollider == null)
            errorMessages.Add("Right wheelcollider not found");

        //if (prop.isSteer && prop.steeringCurve == null)
        //    errorMessages.Add("Steering Curve not configured");

        if (prop.leftWheelCollider && prop.leftWheelCollider.transform.localPosition == Vector3.zero)
            errorMessages.Add("Left wheelcollider is not configured");

        if (prop.rightWheelCollider && prop.rightWheelCollider.transform.localPosition == Vector3.zero)
            errorMessages.Add("Right wheelcollider is not configured");

        if (prop.leftWheelCollider && prop.leftWheelCollider.wheelModel == null)
            errorMessages.Add("Left wheelcollider is missing wheel model");

        if (prop.rightWheelCollider && prop.rightWheelCollider.wheelModel == null)
            errorMessages.Add("Right wheelcollider is missing wheel model");

        if (prop.leftWheelCollider && prop.leftWheelCollider.WheelCollider.suspensionDistance < .05f)
            errorMessages.Add("Left wheelcollider is missing suspension distance");

        if (prop.rightWheelCollider && prop.rightWheelCollider.WheelCollider.suspensionDistance < .05f)
            errorMessages.Add("Right wheelcollider is missing suspension distance");

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

    public void RemoveAxle() {
         
        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(prop.gameObject);
        
        if (isPrefab) {

            bool unpackPrefab = EditorUtility.DisplayDialog("Can't Remove The Object", "This vehicle is connected to a prefab. In order to delete a gameobject, you must ubpack the prefab. Would you like to unpack the prefab completely?", "Unpack", "Don't Unpack");

            if (unpackPrefab)
                PrefabUtility.UnpackPrefabInstance(PrefabUtility.GetOutermostPrefabInstanceRoot(prop.gameObject), PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            return;

        } else {

            DestroyImmediate(prop.gameObject);

        }

    }

}
