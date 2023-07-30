//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;

public class RCCP_PhotonInitLoad : MonoBehaviour {

    [InitializeOnLoadMethod]
    static void InitOnLoad() {

        EditorApplication.delayCall += EditorUpdate;

    }

    public static void EditorUpdate() {

        bool hasKey = false;

#if RCCP_PHOTON
        hasKey = true;
#endif

        if (!hasKey) {

            EditorUtility.DisplayDialog("Photon PUN 2 For Realistic Car Controller Pro", "Be sure you have imported latest Photon PUN 2 to your project. Pass in your AppID to Photon, and run the RCCP_Scene_Blank_Photon demo scene. You can find more detailed info in documentations.", "Close");

        }

        RCCP_SetScriptingSymbol.SetEnabled("RCCP_PHOTON", true);

    }

}
