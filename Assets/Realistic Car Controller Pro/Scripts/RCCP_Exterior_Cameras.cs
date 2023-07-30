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
/// Additional camera manager for hood and wheel cameras.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Exterior Cameras")]
public class RCCP_Exterior_Cameras : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    public RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  FPS / hood camera.
    public RCCP_HoodCamera _hoodCamera;
    public RCCP_HoodCamera HoodCamera {

        get {

            if (_hoodCamera == null)
                _hoodCamera = CarController.GetComponentInChildren<RCCP_HoodCamera>();

            return _hoodCamera;

        }

    }

    //  Wheel camera.
    public RCCP_WheelCamera _wheelCamera;
    public RCCP_WheelCamera WheelCamera {

        get {

            if (_wheelCamera == null)
                _wheelCamera = CarController.GetComponentInChildren<RCCP_WheelCamera>();

            return _wheelCamera;

        }

    }

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.ExteriorCameras = this;
        else
            enabled = false;

    }

}
