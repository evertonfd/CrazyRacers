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
/// Exhaust manager. All exhausts must be connected to this manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Exhausts")]
public class RCCP_Exhausts : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    public RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  All exhausts attached to the vehicle.
    public RCCP_Exhaust[] _exhausts;
    public RCCP_Exhaust[] Exhaust {

        get {

            _exhausts = CarController.GetComponentsInChildren<RCCP_Exhaust>();

            return _exhausts;

        }

    }

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.Exhausts = this;
        else
            enabled = false;

    }

}
