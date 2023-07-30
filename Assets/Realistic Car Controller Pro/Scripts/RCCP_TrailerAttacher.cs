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
/// Vehicle and trailer must have trailer attachers. Must be added to the box collider with trigger enabled.
/// When two trailer attachers triggers each other, attachment will be processed.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Trailer Attacher")]
public class RCCP_TrailerAttacher : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  Trailer.
    private RCCP_TruckTrailer _trailer;
    public RCCP_TruckTrailer Trailer {

        get {

            if (_trailer == null)
                _trailer = GetComponentInParent<RCCP_TruckTrailer>(true);

            return _trailer;

        }

    }

    public RCCP_TruckTrailer attachedTrailer;       //  Attached trailer.
    private BoxCollider trigger;        //  This triggger collider.

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.TrailAttacher = this;
        else
            enabled = false;

    }

    private void OnTriggerEnter(Collider col) {

        //  Getting other attacher.
        RCCP_TrailerAttacher otherAttacher = col.gameObject.GetComponent<RCCP_TrailerAttacher>();

        //  If no attacher found, return.
        if (!otherAttacher)
            return;

        //  If no trailer found on the other side, return.
        if (!otherAttacher.Trailer)
            return;

        //  Assigning the attached trailer with the other side.
        attachedTrailer = otherAttacher.Trailer;
        RCCP_CarController carController = GetComponentInParent<RCCP_CarController>();

        //  If this attacher belongs to the vehicle, not trailer, let the other trailer attach to this vehicle.
        if (carController)
            otherAttacher.Trailer.AttachTrailer(carController);

    }

    private void Reset() {

        //  Resetting.
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        transform.localPosition = new Vector3(0f, 0f, -2.75f);
        trigger = GetComponent<BoxCollider>();

        if (trigger)
            return;

        trigger = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(.1f, 1f, .1f);

    }

}
