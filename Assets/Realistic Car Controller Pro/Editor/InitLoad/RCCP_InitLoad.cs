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
using System;
using System.Collections;
using System.Collections.Generic;

public class RCCP_InitLoad : EditorWindow {

    [InitializeOnLoadMethod]
    static void InitOnLoad() {

        EditorApplication.delayCall += EditorUpdate;

    }

    public static void EditorUpdate() {

        bool hasKey = false;

#if BCG_RCCP
        hasKey = true;
#endif

        if (!hasKey) {

            EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing and using Realistic Car Controller Pro. Please read the documentations before use. Also check out the online documentations for updated info. Have fun :)", "Let's get started!");
            EditorUtility.DisplayDialog("Input System", "RCC Pro is using new input system as default. But you can switch to the old input system later if you want. Make sure your project has Input System installed through the Package Manager now. It should be installed if you have installed dependencies while importing the package. If you haven't installed dependencies, no worries. You can install Input System from the Package Manager (Window --> Package Manager). More info can be found in the documentations.", "Ok");
            RCCP_WelcomeWindow.OpenWindow();

        }

        bool newInputSystemKey = RCCP_Settings.Instance.useNewInputSystem;

        if (newInputSystemKey) {

#if !BCG_NEWINPUTSYSTEM

            RCCP_SetScriptingSymbol.SetEnabled("BCG_NEWINPUTSYSTEM", true);

#endif

        } else {

#if BCG_NEWINPUTSYSTEM

            RCCP_SetScriptingSymbol.SetEnabled("BCG_NEWINPUTSYSTEM", false);

#endif

        }

        RCCP_Installation.Check();
        RCCP_SetScriptingSymbol.SetEnabled("BCG_RCCP", true);

    }

}
