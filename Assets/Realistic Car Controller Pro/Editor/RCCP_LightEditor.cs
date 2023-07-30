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

[CustomEditor(typeof(RCCP_Light))]
public class RCCP_LightEditor : Editor {

    RCCP_Light prop;
    GUISkin skin;
    private Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Light)target;
        serializedObject.Update();
        GUI.skin = skin;

        DrawDefaultInspector();
        CheckMisconfig();

        if (!EditorUtility.IsPersistent(prop)) {

            if (GUILayout.Button("Duplicate To Other Side")) {

                GameObject duplicated = Instantiate(prop.gameObject, prop.GetComponentInParent<RCCP_CarController>(true).Lights.transform);

                duplicated.transform.name = prop.transform.name + "_D";
                duplicated.transform.localPosition = new Vector3(-duplicated.transform.localPosition.x, duplicated.transform.localPosition.y, duplicated.transform.localPosition.z);
                duplicated.transform.localRotation = prop.transform.localRotation;

                if (duplicated.GetComponent<RCCP_Light>().lightType == RCCP_Light.LightType.IndicatorLeftLight)
                    duplicated.GetComponent<RCCP_Light>().lightType = RCCP_Light.LightType.IndicatorRightLight;

                if (duplicated.GetComponent<RCCP_Light>().lightType == RCCP_Light.LightType.IndicatorRightLight)
                    duplicated.GetComponent<RCCP_Light>().lightType = RCCP_Light.LightType.IndicatorLeftLight;

                Selection.activeObject = duplicated;

            }

            if (GUILayout.Button("Create LightBox")) {

                if (prop.transform.Find(RCCP_Settings.Instance.lightBox.name))
                    return;

                MeshRenderer lightBoxRenderer = Instantiate(RCCP_Settings.Instance.lightBox, prop.transform).GetComponent<MeshRenderer>();
                lightBoxRenderer.transform.name = RCCP_Settings.Instance.lightBox.name;
                lightBoxRenderer.transform.rotation = prop.transform.root.transform.rotation;
                prop.emissiveRenderer = lightBoxRenderer;
                prop.emissiveMaterialIndex = 0;

            }

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).Lights.gameObject;

            if (prop.GetComponentInParent<RCCP_CarController>(true).checkComponents)
                Selection.activeObject = prop.GetComponentInParent<RCCP_CarController>(true).gameObject;

        }

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMisconfig() {

        if (!prop.gameObject.activeInHierarchy)
            return;

        Vector3 relativePos = prop.GetComponentInParent<RCCP_CarController>(true).transform.InverseTransformPoint(prop.transform.position);

        if (relativePos.z > 0f) {

            if (Mathf.Abs(prop.transform.localRotation.y) > .5f) {

                GUI.color = Color.red;
                EditorGUILayout.HelpBox("Lights is facing to wrong direction!", MessageType.Error);
                GUI.color = guiColor;

                GUI.color = Color.green;

                if (GUILayout.Button("Fix Rotation"))
                    prop.transform.localRotation = Quaternion.identity;

                GUI.color = guiColor;

            }

        } else {

            if (Mathf.Abs(prop.transform.localRotation.y) < .5f) {

                GUI.color = Color.red;
                EditorGUILayout.HelpBox("Lights is facing to wrong direction!", MessageType.Error);
                GUI.color = guiColor;

                GUI.color = Color.green;

                if (GUILayout.Button("Fix Rotation"))
                    prop.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                GUI.color = guiColor;

            }

        }

    }

}
