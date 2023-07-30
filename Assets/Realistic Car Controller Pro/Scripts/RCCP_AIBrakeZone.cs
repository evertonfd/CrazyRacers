//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Brake Zones are meant to be used for slowing AI vehicles. If you have a sharp turn on your scene, you can simply use one of these Brake Zones. It has a target speed. AI will adapt its speed to this target speed while in this Brake Zone. It's simple.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/AI/RCCP AI Brake Zone")]
public class RCCP_AIBrakeZone : MonoBehaviour {

    private BoxCollider trigger;        //  Trigger.
    public float targetSpeed = 50;      //  Target speed.

    private void Awake() {

        //  Getting trigger.
        trigger = GetComponent<BoxCollider>();

    }

    private void OnTriggerStay(Collider other) {

        if (!enabled)
            return;

        //  Getting main car controller.
        RCCP_CarController carController = other.GetComponentInParent<RCCP_CarController>();

        //  If object is not a vehicle, return.
        if (!carController)
            return;

        //  If object is a vehicle, but missing component root, return.
        if (!carController.OtherAddonsManager)
            return;

        //  Of object is vehicle, but missing AI controller, return.
        if (!carController.OtherAddonsManager.AI)
            return;

        //  If trigger bounds contains car controller position, call "Entered Brake Zone" method in the AI script.
        if (trigger.bounds.Contains(carController.transform.position))
            carController.OtherAddonsManager.AI.EnteredBrakeZone(this);

    }

    private void OnTriggerExit(Collider other) {

        if (!enabled)
            return;

        //  Getting main car controller.
        RCCP_CarController carController = other.GetComponentInParent<RCCP_CarController>();

        //  If object is not a vehicle, return.
        if (!carController)
            return;

        //  If object is a vehicle, but missing component root, return.
        if (!carController.OtherAddonsManager)
            return;

        //  Of object is vehicle, but missing AI controller, return.
        if (!carController.OtherAddonsManager.AI)
            return;

        //  AI exits from the brake zone, call "ExitedBrakeZone" method in the AI script.
        carController.OtherAddonsManager.AI.ExitedBrakeZone();

    }

}
