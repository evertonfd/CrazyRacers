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
using UnityEditorInternal;

[CustomEditor(typeof(RCCP_Damage))]
public class RCCP_DamageEditor : Editor {

    RCCP_Damage prop;
    GUISkin skin;
    Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Damage)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Damage system.", MessageType.Info, true);

        DamageTab();

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

    private void DamageTab() {


        EditorGUILayout.HelpBox("Auto Install: All meshes, lights, parts, and wheels will be collected automatically at runtime. If you want to select specific objects, disable ''Auto Install'' and select specific objects. If you want to remove only few objects, you can use buttom buttons to get all.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticInstallation"), new GUIContent("Auto Install", "Auto Install: All meshes, lights, parts, and wheels will be collected automatically at runtime. If you want to select specific objects, disable ''Auto Install'' and select specific objects. If you want to remove only few objects, you can use buttom buttons to get all."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageFilter"), new GUIContent("Damage Filter", "LayerMask filter. Damage will be taken from the objects with these layers."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumDamage"), new GUIContent("Maximum Damage", "Maximum Vert Distance For Limiting Damage. 0 Value Will Disable The Limit."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("meshDeformation"), new GUIContent("Mesh Deformation", "Deforms selected meshes on collision."));

        if (prop.meshDeformation) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("deformationMode"), new GUIContent("Deformation Mode", "Fast and accurate modes."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("deformationRadius"), new GUIContent("Deformation Radius", "Verticies in this radius will be effected on collisions."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("deformationMultiplier"), new GUIContent("Deformation Multiplier", "Damage multiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recalculateNormals"), new GUIContent("Recalculate Normals", "Recalculate normals while deforming / restoring the mesh."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recalculateBounds"), new GUIContent("Recalculate Bounds", "Recalculate bounds while deforming / restoring the mesh."));

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelDamage"), new GUIContent("Wheel Damage", "Use wheel damage."));

        if (prop.wheelDamage) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelDamageRadius"), new GUIContent("Wheel Damage Radius", "Wheel damage radius."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelDamageMultiplier"), new GUIContent("Wheel Damage Multiplier", "Wheel damage multiplier."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelDetachment"), new GUIContent("Wheel Detachment", "Use wheel detachment."));

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("lightDamage"), new GUIContent("Light Damage", "Use light damage."));

        if (prop.lightDamage) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightDamageRadius"), new GUIContent("Light Damage Radius", "Light damage radius."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightDamageMultiplier"), new GUIContent("Light Damage Multiplier", "Light damage multiplier."));

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("partDamage"), new GUIContent("Part Damage", "Use part damage."));

        if (prop.partDamage) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("partDamageRadius"), new GUIContent("Part Damage Radius", "Part damage radius."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("partDamageMultiplier"), new GUIContent("Part Damage Multiplier", "Part damage multiplier."));

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        if (!prop.automaticInstallation) {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Mesh Filters", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            if (prop.meshFilters != null) {

                for (int i = 0; i < prop.meshFilters.Length; i++) {

                    if (prop.meshFilters[i]) {

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.ObjectField(prop.meshFilters[i], typeof(MeshFilter), false);

                        if (prop.meshFilters[i].sharedMesh == null) {

                            GUI.color = Color.red;
                            EditorGUILayout.HelpBox("Mesh is null!", MessageType.None);

                        }

                        if (prop.meshFilters[i].GetComponent<MeshRenderer>() == null) {

                            GUI.color = Color.red;
                            EditorGUILayout.HelpBox("No renderer found!", MessageType.None);

                        }

                        bool fixedRotation = 1 - Mathf.Abs(Quaternion.Dot(prop.meshFilters[i].transform.rotation, prop.transform.rotation)) < .01f;

                        if (!fixedRotation) {

                            GUI.color = Color.red;
                            EditorGUILayout.HelpBox("Axis is wrong!", MessageType.None);

                            if (GUILayout.Button("Fix Axis")) {

                                RCCP_FixAxisWindow fw = EditorWindow.GetWindow<RCCP_FixAxisWindow>(true);
                                fw.target = prop.meshFilters[i];
                                SceneView.lastActiveSceneView.Frame(new Bounds(prop.meshFilters[i].transform.position, Vector3.one), false);
                                Selection.activeGameObject = prop.meshFilters[i].gameObject;

                            }

                        }

                        GUI.color = guiColor;
                        GUI.color = Color.red;

                        if (GUILayout.Button("X", GUILayout.Width(25f))) {

                            List<MeshFilter> meshes = new List<MeshFilter>();

                            for (int k = 0; k < prop.meshFilters.Length; k++)
                                meshes.Add(prop.meshFilters[k]);

                            meshes.RemoveAt(i);

                            prop.meshFilters = meshes.ToArray();
                            EditorUtility.SetDirty(prop);

                        }

                        GUI.color = guiColor;
                        EditorGUILayout.EndHorizontal();

                    }

                }

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            //
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Wheels", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            if (prop.wheels != null) {

                for (int i = 0; i < prop.wheels.Length; i++) {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(prop.wheels[i], typeof(RCCP_WheelCollider), false);
                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(25f))) {

                        List<RCCP_WheelCollider> wheels = new List<RCCP_WheelCollider>();

                        for (int k = 0; k < prop.wheels.Length; k++)
                            wheels.Add(prop.wheels[k]);

                        wheels.RemoveAt(i);

                        prop.wheels = wheels.ToArray();
                        EditorUtility.SetDirty(prop);

                    }

                    GUI.color = guiColor;
                    EditorGUILayout.EndHorizontal();

                }

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            //
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Lights", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            if (prop.lights != null) {

                for (int i = 0; i < prop.lights.Length; i++) {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(prop.lights[i], typeof(RCCP_Light), false);
                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(25f))) {

                        List<RCCP_Light> lights = new List<RCCP_Light>();

                        for (int k = 0; k < prop.lights.Length; k++)
                            lights.Add(prop.lights[k]);

                        lights.RemoveAt(i);

                        prop.lights = lights.ToArray();
                        EditorUtility.SetDirty(prop);

                    }

                    GUI.color = guiColor;
                    EditorGUILayout.EndHorizontal();

                }

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            //
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Parts", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            if (prop.parts != null) {

                for (int i = 0; i < prop.parts.Length; i++) {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(prop.parts[i], typeof(RCCP_DetachablePart), false);
                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(25f))) {

                        List<RCCP_DetachablePart> parts = new List<RCCP_DetachablePart>();

                        for (int k = 0; k < prop.parts.Length; k++)
                            parts.Add(prop.parts[k]);

                        parts.RemoveAt(i);

                        prop.parts = parts.ToArray();
                        EditorUtility.SetDirty(prop);

                    }

                    GUI.color = guiColor;
                    EditorGUILayout.EndHorizontal();

                }

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            ///////////////////////

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Get Meshes"))
                prop.GetMeshesEditor();

            if (GUILayout.Button("Get Lights"))
                prop.lights = prop.GetComponentInParent<RCCP_CarController>(true).gameObject.GetComponentsInChildren<RCCP_Light>();

            if (GUILayout.Button("Get Parts"))
                prop.parts = prop.GetComponentInParent<RCCP_CarController>(true).gameObject.GetComponentsInChildren<RCCP_DetachablePart>();

            if (GUILayout.Button("Get Wheels"))
                prop.wheels = prop.GetComponentInParent<RCCP_CarController>(true).gameObject.GetComponentsInChildren<RCCP_WheelCollider>();

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clean Empty Elements"))
                CleanEmptyElements();

        }

        if (prop.repaired) {

            GUILayout.Button("Repaired");

        } else {

            GUI.color = Color.green;

            if (GUILayout.Button("Repair Now"))
                prop.repairNow = true;

            GUI.color = guiColor;

        }



        EditorGUILayout.Space();

    }

    private void CleanEmptyElements() {

        List<MeshFilter> meshFilterList = new List<MeshFilter>();

        for (int i = 0; i < prop.meshFilters.Length; i++) {

            if (prop.meshFilters[i] != null)
                meshFilterList.Add(prop.meshFilters[i]);

        }

        prop.meshFilters = meshFilterList.ToArray();

        List<RCCP_Light> lightList = new List<RCCP_Light>();

        for (int i = 0; i < prop.lights.Length; i++) {

            if (prop.lights[i] != null)
                lightList.Add(prop.lights[i]);

        }

        prop.lights = lightList.ToArray();

        List<RCCP_DetachablePart> partList = new List<RCCP_DetachablePart>();

        for (int i = 0; i < prop.parts.Length; i++) {

            if (prop.parts[i] != null)
                partList.Add(prop.parts[i]);

        }

        prop.parts = partList.ToArray();

        List<RCCP_WheelCollider> wheelsList = new List<RCCP_WheelCollider>();

        for (int i = 0; i < prop.wheels.Length; i++) {

            if (prop.wheels[i] != null)
                wheelsList.Add(prop.wheels[i]);

        }

        prop.wheels = wheelsList.ToArray();

    }

}
