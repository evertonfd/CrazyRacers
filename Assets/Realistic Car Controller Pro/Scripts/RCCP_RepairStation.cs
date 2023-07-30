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
/// Repairs the vehicle. Must be added to the box collider with trigger enabled.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Repair Station")]
public class RCCP_RepairStation : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {

        //  Getting car controller.
        RCCP_CarController carController = other.GetComponentInParent<RCCP_CarController>();

        //  If car controller not found, return.
        if (!carController)
            return;

        //  If vehicle has damage component, repair it.
        if (carController.Damage)
            carController.Damage.repairNow = true;

    }

}
