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

/// <summary>
/// Used to render dash and needle.
/// </summary>
public class RCCP_UI_Dash : MonoBehaviour {

    private Camera dashCamera;      //  Actual dashboard camera.
    public GameObject needle;       //  Needle of the RPM.
    public float needleRotationMultiplier = 1f;     //  Needle rotation multiplier.
    private float startingAngle = 0f;       //  Start angle of the needle.

    private void Awake() {

        //  Getting dash camera and starting angle.
        dashCamera = GetComponentInChildren<Camera>();
        startingAngle = needle.transform.localEulerAngles.z;

    }

    private void Update() {

        //  If no dash camera found, return.
        if (!dashCamera)
            return;

        //  If no player vehicle found, return.
        if (!RCCP_SceneManager.Instance.activePlayerVehicle)
            return;

        //  Assigning rotation of the needle based on player's vehicle engine RPM.
        needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, startingAngle + RCCP_SceneManager.Instance.activePlayerVehicle.engineRPM * -needleRotationMultiplier);

        //  Make sure z rotation of the camera is always set to 0.
        dashCamera.transform.rotation = Quaternion.Euler(dashCamera.transform.eulerAngles.x, dashCamera.transform.eulerAngles.y, 0f);

    }

}
