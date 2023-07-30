//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Container for the brake zones. Editor only.
/// </summary>
[CustomEditor(typeof(RCCP_AIBrakeZonesContainer))]
public class RCCP_AIBZEditor : Editor {

    RCCP_AIBrakeZonesContainer bzScript;

    public override void OnInspectorGUI() {

        bzScript = (RCCP_AIBrakeZonesContainer)target;
        serializedObject.Update();

        if (GUILayout.Button("Delete Brake Zones")) {

            foreach (Transform t in bzScript.brakeZones)
                DestroyImmediate(t.gameObject);

            bzScript.brakeZones.Clear();
            EditorUtility.SetDirty(bzScript);

        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeZones"), new GUIContent("Brake Zones", "Brake Zones"), true);

        EditorGUILayout.HelpBox("Create BrakeZones By Shift + Left Mouse Button On Your Road", MessageType.Info);

        if (GUI.changed)
            EditorUtility.SetDirty(bzScript);

        serializedObject.ApplyModifiedProperties();

    }

    private void OnSceneGUI() {

        Event e = Event.current;
        bzScript = (RCCP_AIBrakeZonesContainer)target;

        if (e != null) {

            if (e.isMouse && e.shift && e.type == EventType.MouseDown) {

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, 5000.0f)) {

                    Vector3 newTilePosition = hit.point;

                    GameObject wp = new GameObject("Brake Zone " + bzScript.brakeZones.Count.ToString());

                    wp.transform.position = newTilePosition;
                    wp.transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    wp.AddComponent<RCCP_AIBrakeZone>();
                    BoxCollider bC = wp.AddComponent<BoxCollider>();
                    bC.isTrigger = true;
                    bC.size = new Vector3(10f, 3f, 10f);
                    wp.transform.SetParent(bzScript.transform);
                    GetBrakeZones();
                    Event.current.Use();

                }

            }

            if (bzScript)
                Selection.activeGameObject = bzScript.gameObject;

        }

        GetBrakeZones();

    }

    public void GetBrakeZones() {

        bzScript.brakeZones = new List<Transform>();

        Transform[] allTransforms = bzScript.transform.GetComponentsInChildren<Transform>();

        foreach (Transform t in allTransforms) {

            if (t != bzScript.transform)
                bzScript.brakeZones.Add(t);

        }

    }

}
