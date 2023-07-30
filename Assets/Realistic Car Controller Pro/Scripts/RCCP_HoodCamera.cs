//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// RCCP Camera will be parented to this gameobject when current camera mode is Hood Camera.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Camera/RCCP Hood Camera")]
public class RCCP_HoodCamera : MonoBehaviour {

    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    private void Awake() {

        CheckJoint();

    }

    /// <summary>
    /// Fixing shake bug of the rigid.
    /// </summary>
    public void FixShake() {

        StartCoroutine(FixShakeDelayed());

    }

    IEnumerator FixShakeDelayed() {

        //  If no rigid found, return.
        if (!GetComponent<Rigidbody>())
            yield break;

        yield return new WaitForFixedUpdate();
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        yield return new WaitForFixedUpdate();
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

    }

    /// <summary>
    /// Checking configurable joint.
    /// </summary>
    private void CheckJoint() {

        //  Getting configurable joint.
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();

        //  If no joint found, return.
        if (!joint)
            return;

        //  If connected body of the joint is null, set it to car controller itself.
        if (joint.connectedBody == null) {

            if (CarController) {

                joint.connectedBody = CarController.Rigid;

            } else {

                Debug.LogError("Hood camera of the " + transform.root.name + " has configurable joint with no connected body! Disabling rigid and joint of the camera.");
                Destroy(joint);

                Rigidbody rigid = GetComponent<Rigidbody>();

                if (rigid)
                    Destroy(rigid);

            }

        }

    }

    private void Reset() {

        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();

        if (!joint)
            return;

        if (!CarController)
            return;

        joint.connectedBody = CarController.GetComponent<Rigidbody>();
        joint.connectedMassScale = 0f;

    }

}

