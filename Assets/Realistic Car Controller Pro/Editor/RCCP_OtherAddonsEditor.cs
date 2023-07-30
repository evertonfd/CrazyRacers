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

[CustomEditor(typeof(RCCP_OtherAddons))]
public class RCCP_OtherAddonsEditor : Editor {

    RCCP_OtherAddons prop;
    GUISkin skin;
    Color guiColor;

    RCCP_Nos nos;
    RCCP_Visual_Dashboard dashboard;
    RCCP_Exterior_Cameras cameras;
    RCCP_Exhausts exhausts;
    RCCP_AI AI;
    RCCP_Recorder recorder;
    RCCP_TrailerAttacher trailAttacher;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");
        ReOrderComponents();

    }

    private void ReOrderComponents() {

        prop = (RCCP_OtherAddons)target;

        int index = 0;

        if (prop.Nos) {

            prop.Nos.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Dashboard) {

            prop.Dashboard.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.ExteriorCameras) {

            prop.ExteriorCameras.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Exhausts) {

            prop.Exhausts.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.AI) {

            prop.AI.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.Recorder) {

            prop.Recorder.transform.SetSiblingIndex(index);
            index++;

        }

        if (prop.TrailAttacher)
            prop.TrailAttacher.transform.SetSiblingIndex(index);

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_OtherAddons)target;
        serializedObject.Update();
        GUI.skin = skin;
        guiColor = GUI.color;

        prop.componentsTaken = false;

        nos = prop.Nos;
        dashboard = prop.Dashboard;
        cameras = prop.ExteriorCameras;
        exhausts = prop.Exhausts;
        AI = prop.AI;
        recorder = prop.Recorder;
        trailAttacher = prop.TrailAttacher;

        if (EditorUtility.IsPersistent(prop))
            EditorGUILayout.HelpBox("Double click the prefab to edit settings. Some editor features are disabled in this mode.", MessageType.Warning);

        if (Screen.width < 500)
            EditorGUILayout.HelpBox("Increase width of your inspector panel to see all buttons.", MessageType.Warning);

        EditorGUILayout.HelpBox("Other addons such as nos, exhausts, interior & exterior cameras, mirrors, etc...", MessageType.Info, true);

        EditorGUILayout.BeginHorizontal();

        NOSButton();
        GUILayout.Space(10f);
        InteriorButton();
        GUILayout.Space(10f);
        CamerasButton();
        GUILayout.Space(10f);
        ExhaustsButton();
        GUILayout.Space(10f);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        AIButton();
        GUILayout.Space(10f);
        RecorderButton();
        GUILayout.Space(10f);
        TrailerButton();
        GUILayout.Space(10f);

        EditorGUILayout.EndHorizontal();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

        }

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

        if (!EditorApplication.isPlaying)
            Repaint();

    }

    private void NOSButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (nos)
            nos.enabled = EditorGUILayout.ToggleLeft("", nos.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("NOS", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (nos)
                Selection.activeObject = prop.Nos.gameObject;
            else
                AddNOS();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (nos)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (nos) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveNOS();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void InteriorButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (dashboard)
            dashboard.enabled = EditorGUILayout.ToggleLeft("", dashboard.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("Interior", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (dashboard)
                Selection.activeObject = prop.Dashboard.gameObject;
            else
                AddDashboard();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (dashboard)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (dashboard) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveDashboard();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void CamerasButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (cameras)
            cameras.enabled = EditorGUILayout.ToggleLeft("", cameras.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("Cameras", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (cameras)
                Selection.activeObject = prop.ExteriorCameras.gameObject;
            else
                AddCameras();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (cameras)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (cameras) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveCameras();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void ExhaustsButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (exhausts)
            exhausts.enabled = EditorGUILayout.ToggleLeft("", exhausts.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("Exhausts", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (exhausts)
                Selection.activeObject = prop.Exhausts.gameObject;
            else
                AddExhausts();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (exhausts)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (exhausts) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveExhausts();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void AIButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (AI)
            AI.enabled = EditorGUILayout.ToggleLeft("", AI.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("AI", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (AI)
                Selection.activeObject = prop.AI.gameObject;
            else
                AddAI();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (AI)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (AI) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveAI();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void RecorderButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (recorder)
            recorder.enabled = EditorGUILayout.ToggleLeft("", recorder.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("Recorder", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (recorder)
                Selection.activeObject = prop.Recorder.gameObject;
            else
                AddRecorder();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (recorder)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (recorder) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveRecorder();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void TrailerButton() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (trailAttacher)
            trailAttacher.enabled = EditorGUILayout.ToggleLeft("", trailAttacher.enabled, GUILayout.Width(15f));

        if (GUILayout.Button("Trail Attacher", GUILayout.Width(50f), GUILayout.Height(20f), GUILayout.ExpandWidth(true))) {

            if (trailAttacher)
                Selection.activeObject = prop.TrailAttacher.gameObject;
            else
                AddTrailAttacher();

        }

        if (!EditorUtility.IsPersistent(prop)) {

            if (trailAttacher)
                GUILayout.Label(("Equipped"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(("Create"), GUILayout.Width(50f), GUILayout.ExpandWidth(true));

            if (trailAttacher) {

                GUI.color = Color.red;

                if (GUILayout.Button("Remove"))
                    RemoveTrailAttacher();

                GUI.color = guiColor;

            }

        }

        EditorGUILayout.EndVertical();

    }

    private void AddNOS() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_NOS");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_Nos>();

    }

    private void AddDashboard() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Dashboard");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_Visual_Dashboard>();

    }

    private void AddCameras() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_ExteriorCameras");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_Exterior_Cameras>();

    }

    private void AddExhausts() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Exhausts");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_Exhausts>();

    }

    private void AddAI() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_AI");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_AI>();

    }

    private void AddRecorder() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_Recorder");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_Recorder>();

    }

    private void AddTrailAttacher() {

        if (EditorUtility.IsPersistent(prop))
            return;

        GameObject subject = new GameObject("RCCP_TrailAttacher");
        subject.transform.SetParent(prop.transform, false);
        subject.transform.SetSiblingIndex(0);
        subject.gameObject.AddComponent<RCCP_TrailerAttacher>();

    }

    private void RemoveNOS() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(nos.gameObject);

    }

    private void RemoveDashboard() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(dashboard.gameObject);

    }

    private void RemoveCameras() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(cameras.gameObject);

    }

    private void RemoveExhausts() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(exhausts.gameObject);

    }

    private void RemoveAI() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(AI.gameObject);

    }

    private void RemoveRecorder() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(recorder.gameObject);

    }

    private void RemoveTrailAttacher() {

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject);

        if (isPrefab) {

            bool disconnectPrefabConnection = (EditorUtility.DisplayDialog("Unpacking Prefab", "This gameobject is connected to a prefab. In order to do remove this component, you'll need to unpack the prefab connection first. After removing the component, you can override your existing prefab with this gameobject.", "Disconnect", "Cancel"));

            if (!disconnectPrefabConnection)
                return;

            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        bool answer = (EditorUtility.DisplayDialog("Removing Component", "Are you sure want to remove this component? You can't undo this operation.", "Remove", "Cancel"));

        if (answer)
            DestroyImmediate(trailAttacher.gameObject);

    }

}
