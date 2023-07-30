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

[CustomEditor(typeof(RCCP_Lights))]
public class RCCP_LightsEditor : Editor {

    RCCP_Lights prop;
    List<string> errorMessages = new List<string>();
    GUISkin skin;
    private Color guiColor;

    RCCP_Light.LightType lightType;

    private void OnEnable() {

        guiColor = GUI.color;
        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Lights)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Main light manager of the vehicle. All lights are connected to this manager.", MessageType.Info, true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("lowBeamHeadlights"), new GUIContent("Low Beam Headlights", "Low beam headlights are on or off?"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("highBeamHeadlights"), new GUIContent("High Beam Headlights", "High beam headlights are on or off?"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorsAll"), new GUIContent("Indicators", "Indicators are set to on, or off?"), false);

        prop.lights.Clear();
        RCCP_Light[] allLights = prop.GetComponentsInChildren<RCCP_Light>(true);

        for (int i = 0; i < allLights.Length; i++) {

            if (!prop.lights.Contains(allLights[i]))
                prop.lights.Add(allLights[i]);

        }

        EditorGUILayout.Space();
        GUILayout.Label("Attached Lights", EditorStyles.boldLabel);

        if (prop.lights != null) {

            if (prop.lights.Count > 0) {

                for (int i = 0; i < prop.lights.Count; i++) {

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label(prop.lights[i].name, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Edit", GUILayout.Width(100f)))
                        Selection.activeObject = prop.lights[i].gameObject;

                    GUI.color = Color.red;

                    if (GUILayout.Button("X"))
                        DestroyImmediate(prop.lights[i].gameObject);

                    GUI.color = guiColor;
                    EditorGUILayout.EndHorizontal();

                }

            } else {

                EditorGUILayout.HelpBox("No lights found. You can create new lights below.", MessageType.Warning);

            }

        }

        if (!EditorUtility.IsPersistent(prop)) {

            EditorGUILayout.Space();

            GUILayout.Label("Create New Light", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            lightType = (RCCP_Light.LightType)EditorGUILayout.EnumPopup(lightType);

            GUI.color = Color.green;

            if (GUILayout.Button("Create Light"))
                CreateNewLight(lightType);

            GUI.color = guiColor;

            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.Space();

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

    private void CreateNewLight(RCCP_Light.LightType lightType) {

        switch (lightType) {

            case RCCP_Light.LightType.Headlight_LowBeam:

                GameObject newLightSource_Headlight = Instantiate(RCCP_Settings.Instance.headLights_Low, prop.transform, false);
                newLightSource_Headlight.transform.name = RCCP_Settings.Instance.headLights_Low.transform.name + "_D";
                newLightSource_Headlight.transform.localPosition = new Vector3(0f, 0f, 2.5f);
                Selection.activeObject = newLightSource_Headlight;

                break;

            case RCCP_Light.LightType.Headlight_HighBeam:

                GameObject newLightSource_Headlight_High = Instantiate(RCCP_Settings.Instance.headLights_High, prop.transform, false);
                newLightSource_Headlight_High.transform.name = RCCP_Settings.Instance.headLights_High.transform.name + "_D";
                newLightSource_Headlight_High.transform.localPosition = new Vector3(0f, 0f, 2.5f);
                Selection.activeObject = newLightSource_Headlight_High;

                break;

            case RCCP_Light.LightType.Brakelight:

                GameObject newLightSource_Brakelight = Instantiate(RCCP_Settings.Instance.brakeLights, prop.transform, false);
                newLightSource_Brakelight.transform.name = RCCP_Settings.Instance.brakeLights.transform.name + "_D";
                newLightSource_Brakelight.transform.localPosition = new Vector3(0f, 0f, -2.5f);
                Selection.activeObject = newLightSource_Brakelight;

                break;

            case RCCP_Light.LightType.Reverselight:

                GameObject newLightSource_Reverselight = Instantiate(RCCP_Settings.Instance.reverseLights, prop.transform, false);
                newLightSource_Reverselight.transform.name = RCCP_Settings.Instance.reverseLights.transform.name + "_D";
                newLightSource_Reverselight.transform.localPosition = new Vector3(0f, 0f, -2.5f);
                Selection.activeObject = newLightSource_Reverselight;

                break;

            case RCCP_Light.LightType.IndicatorLeftLight:

                GameObject newLightSource_IndicatorL = Instantiate(RCCP_Settings.Instance.indicatorLights_L, prop.transform, false);
                newLightSource_IndicatorL.transform.name = RCCP_Settings.Instance.indicatorLights_L.transform.name + "_D";
                newLightSource_IndicatorL.transform.localPosition = new Vector3(-.5f, 0f, -2.5f);
                Selection.activeObject = newLightSource_IndicatorL;

                break;

            case RCCP_Light.LightType.IndicatorRightLight:

                GameObject newLightSource_IndicatorR = Instantiate(RCCP_Settings.Instance.indicatorLights_R, prop.transform, false);
                newLightSource_IndicatorR.transform.name = RCCP_Settings.Instance.indicatorLights_R.transform.name + "_D";
                newLightSource_IndicatorR.transform.localPosition = new Vector3(.5f, 0f, -2.5f);
                Selection.activeObject = newLightSource_IndicatorR;

                break;

            case RCCP_Light.LightType.Taillight:

                GameObject newLightSource_Taillight = Instantiate(RCCP_Settings.Instance.tailLights, prop.transform, false);
                newLightSource_Taillight.transform.name = RCCP_Settings.Instance.tailLights.transform.name + "_D";
                newLightSource_Taillight.transform.localPosition = new Vector3(0f, 0f, -2.5f);
                Selection.activeObject = newLightSource_Taillight;

                break;

        }

    }

}
