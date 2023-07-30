using UnityEngine;
using UnityEditor;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class RCC_CheckBeforePlay {

    // register an event handler when the class is initialized
    static RCC_CheckBeforePlay() {

        EditorApplication.playModeStateChanged += LogPlayModeState;

    }

    private static void LogPlayModeState(PlayModeStateChange state) {

        if (state != PlayModeStateChange.ExitingEditMode)
            return;

        if (EditorPrefs.GetBool("RCCP_IgnorePlatformWarnings", false) == false) {

            int i = -1;

            if (!RCCP_Settings.Instance.mobileControllerEnabled && (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)) {

                i = EditorUtility.DisplayDialogComplex("Mobile Controller.", "Your target platform is mobile, but it's not enabled in RCC Settings yet.", "Enable it", "Ignore", "Ignore and don't warn me again");

                switch (i) {

                    case 0:
                        RCCP_Settings.Instance.mobileControllerEnabled = true;
                        break;

                    case 2:
                        EditorPrefs.SetBool("RCC_IgnorePlatformWarnings", true);
                        break;

                }


            }

            if (RCCP_Settings.Instance.mobileControllerEnabled && (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android && EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)) {

                i = EditorUtility.DisplayDialogComplex("Mobile Controller.", "Your target platform is not mobile, but it's still enabled in RCC Settings yet.", "Disable it", "Ignore", "Ignore and don't warn me again");

                switch (i) {

                    case 0:
                        RCCP_Settings.Instance.mobileControllerEnabled = false;
                        break;

                    case 2:
                        EditorPrefs.SetBool("RCC_IgnorePlatformWarnings", true);
                        break;

                }


            }

        }

    }

}