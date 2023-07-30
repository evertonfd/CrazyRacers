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

[CustomEditor(typeof(RCCP_Exterior_Cameras))]
public class RCCP_ExteriorCamerasEditor : Editor {

    RCCP_Exterior_Cameras prop;
    GUISkin skin;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Exterior_Cameras)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Exterior cameras attached to the vehicle as hood camera and wheel camera.", MessageType.Info, true);

        DrawDefaultInspector();

        if (!EditorUtility.IsPersistent(prop)) {

            RCCP_HoodCamera hoodCamera = prop.HoodCamera;
            RCCP_WheelCamera wheelCamera = prop.WheelCamera;

            if (hoodCamera == null) {

                if (GUILayout.Button("Add Hood / Cockpit Camera"))
                    CreateHoodCamera();

            }

            if (wheelCamera == null) {

                if (GUILayout.Button("Add Wheel Camera"))
                    CreateWheelCamera();

            }

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_OtherAddons>(true).gameObject;

        }

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CreateHoodCamera() {

        GameObject hoodCam = Instantiate(RCCP_Settings.Instance.RCCPHoodCamera, prop.transform.position, prop.transform.rotation);
        hoodCam.name = RCCP_Settings.Instance.RCCPHoodCamera.name;
        hoodCam.transform.SetParent(prop.transform, true);
        hoodCam.GetComponent<ConfigurableJoint>().connectedBody = prop.gameObject.GetComponent<Rigidbody>();
        hoodCam.GetComponent<ConfigurableJoint>().connectedMassScale = 0f;
        Selection.activeGameObject = hoodCam;

    }

    private void CreateWheelCamera() {

        GameObject wheelCam = Instantiate(RCCP_Settings.Instance.RCCPWheelCamera, prop.transform.position, prop.transform.rotation);
        wheelCam.name = RCCP_Settings.Instance.RCCPWheelCamera.name;
        wheelCam.transform.SetParent(prop.transform, true);
        Selection.activeGameObject = wheelCam;

    }

}
