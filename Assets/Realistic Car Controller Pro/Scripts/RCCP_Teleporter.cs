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
/// Teleports the vehicle in zone to the target spawn point.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Teleporter")]
public class RCCP_Teleporter : MonoBehaviour {

    public Transform spawnPoint;        //  Target spawn point.

    private void OnTriggerEnter(Collider col) {

        //  If trigger enabled for other collider, return.
        if (col.isTrigger)
            return;

        //  Getting car controller.
        RCCP_CarController carController = col.gameObject.GetComponentInParent<RCCP_CarController>();

        //  If no car controller found, return.
        if (!carController)
            return;

        //  Transport the vehicle.
        RCCP.Transport(carController, spawnPoint.position, spawnPoint.rotation);

    }

}
