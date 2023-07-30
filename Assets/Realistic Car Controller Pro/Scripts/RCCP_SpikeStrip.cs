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
/// Deflates the wheel. Must be added to the box collider with trigger enabled.
/// When a wheelcollider gets in the trigger zone, it'll be deflated.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Spikestrip")]
public class RCCP_SpikeStrip : MonoBehaviour {

    private BoxCollider col;        //  Trigger collider.

    private void Awake() {

        //  Getting trigger collider.
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;

    }

    /// <summary>
    /// On trigger stays.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other) {

        //  If component iss not enabled, return.
        if (!enabled)
            return;

        //  Getting car controller.
        RCCP_CarController carController = other.GetComponentInParent<RCCP_CarController>();

        //  If no car controller found, return.
        if (!carController)
            return;

        //  Checking positions of the all wheelcolliders of the vehicle. If one of them triggers, deflate it and mark vehicle as damaged.
        for (int i = 0; i < carController.AllWheelColliders.Length; i++) {

            if (col.bounds.Contains(carController.AllWheelColliders[i].transform.position)) {

                carController.AllWheelColliders[i].Deflate();

                if (carController.Damage) {

                    carController.Damage.deformingNow = true;
                    carController.Damage.deformed = false;

                    carController.Damage.repairNow = false;
                    carController.Damage.repaired = false;

                }

            }

        }

    }

}
