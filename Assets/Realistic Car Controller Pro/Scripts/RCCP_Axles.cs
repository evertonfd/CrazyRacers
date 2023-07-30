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
/// Axles manager. All axles must be connected to this manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Axles")]
public class RCCP_Axles : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    public RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  All axles attached to the vehicle.
    private List<RCCP_Axle> _axles = new List<RCCP_Axle>();
    public List<RCCP_Axle> Axles {

        get {

            if (!CarController)
                return null;

            if (_axles == null) {

                _axles.Clear();

                foreach (RCCP_Axle axle in CarController.GetComponentsInChildren<RCCP_Axle>()) {

                    if (!_axles.Contains(axle))
                        _axles.Add(axle);

                }

            }

            return _axles;

        }

    }

#if UNITY_EDITOR
    [HideInInspector] public bool completeSetup = false;
    [HideInInspector] public bool checkedSetup = false;
#endif

    private void OnEnable() {

        if (CarController)
            CarController.AxleManager = this;
        else
            enabled = false;

    }

    /// <summary>
    /// Creating two new axles if no any axle found.
    /// </summary>
    private void Reset() {

        if (Axles.Count == 0) {

            GameObject subject = new GameObject("RCCP_Axle_Front");
            subject.transform.SetParent(transform, false);
            RCCP_Axle axle_F = subject.AddComponent<RCCP_Axle>();
            axle_F.gameObject.name = "RCCP_Axle_Front";
            axle_F.isSteer = true;
            axle_F.isBrake = true;

            GameObject subject_R = new GameObject("RCCP_Axle_Rear");
            subject_R.transform.SetParent(transform, false);
            RCCP_Axle axle_R = subject_R.AddComponent<RCCP_Axle>();
            axle_R.gameObject.name = "RCCP_Axle_Rear";
            axle_R.isBrake = true;
            axle_R.isHandbrake = true;
            axle_R.brakeMultiplier = .65f;

        }

    }

}
