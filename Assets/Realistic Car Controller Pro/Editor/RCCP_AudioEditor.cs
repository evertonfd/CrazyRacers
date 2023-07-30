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

[CustomEditor(typeof(RCCP_Audio))]
public class RCCP_AudioEditor : Editor {

    RCCP_Audio prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Audio)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Audio system for engine, brake, crashes, transmission, and other stuff.", MessageType.Info, true);

        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create 1 Engine Sound")) {

            prop.engineSounds = new RCCP_Audio.EngineSound[1];
            prop.engineSounds[0] = new RCCP_Audio.EngineSound();

            prop.engineSounds[0].minRPM = 0f;
            prop.engineSounds[0].maxRPM = 8000f;
            prop.engineSounds[0].maxVolume = .9f;
            prop.engineSounds[0].minPitch = .85f;
            prop.engineSounds[0].maxPitch = 1.65f;

        }

        if (GUILayout.Button("Create 2 Engine Sounds")) {

            prop.engineSounds = new RCCP_Audio.EngineSound[2];
            prop.engineSounds[0] = new RCCP_Audio.EngineSound();
            prop.engineSounds[1] = new RCCP_Audio.EngineSound();

            prop.engineSounds[0].minRPM = 0f;
            prop.engineSounds[0].maxRPM = 4000f;
            prop.engineSounds[0].maxVolume = .8f;
            prop.engineSounds[0].minPitch = .85f;
            prop.engineSounds[0].maxPitch = 1.65f;

            prop.engineSounds[1].minRPM = 3000f;
            prop.engineSounds[1].maxRPM = 8000f;
            prop.engineSounds[1].maxVolume = .9f;
            prop.engineSounds[1].minPitch = .85f;
            prop.engineSounds[1].maxPitch = 1.65f;

        }

        if (GUILayout.Button("Create 3 Engine Sounds")) {

            prop.engineSounds = new RCCP_Audio.EngineSound[3];
            prop.engineSounds[0] = new RCCP_Audio.EngineSound();
            prop.engineSounds[1] = new RCCP_Audio.EngineSound();
            prop.engineSounds[2] = new RCCP_Audio.EngineSound();

            prop.engineSounds[0].minRPM = 0f;
            prop.engineSounds[0].maxRPM = 3000f;
            prop.engineSounds[0].maxVolume = .75f;
            prop.engineSounds[0].minPitch = .85f;
            prop.engineSounds[0].maxPitch = 1.65f;

            prop.engineSounds[1].minRPM = 2000f;
            prop.engineSounds[1].maxRPM = 6000f;
            prop.engineSounds[1].maxVolume = .85f;
            prop.engineSounds[1].minPitch = .85f;
            prop.engineSounds[1].maxPitch = 1.65f;

            prop.engineSounds[2].minRPM = 6000f;
            prop.engineSounds[2].maxRPM = 8000f;
            prop.engineSounds[2].maxVolume = .9f;
            prop.engineSounds[2].minPitch = .85f;
            prop.engineSounds[2].maxPitch = 1.65f;

        }

        EditorGUILayout.EndHorizontal();

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
