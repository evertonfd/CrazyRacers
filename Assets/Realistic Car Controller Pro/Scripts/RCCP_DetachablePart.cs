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
/// Detachable part of the vehicle.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Detachable Part")]
public class RCCP_DetachablePart : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //	Configurable Joint.
    private ConfigurableJoint joint;
    private ConfigurableJoint Joint {

        get {

            if (joint == null)
                joint = GetComponent<ConfigurableJoint>();

            return joint;

        }

        set {

            joint = value;

        }

    }

    private RCCP_Joint jointProperties = new RCCP_Joint();      //	Joint properties class.
    private Rigidbody rigid;        //	Rigidbody.
    public Transform COM;       //	Center of mass.
    [HideInInspector] public Collider partCollider;       //  Collider.

    private Vector3 originalLocalPosition = Vector3.zero;
    private Quaternion originalLocalRotation = Quaternion.identity;

    public enum DetachablePartType { Hood, Trunk, Door, Bumper_F, Bumper_R, Other }
    public DetachablePartType partType = DetachablePartType.Hood;

    public bool lockAtStart = true;     //	Lock all motions of Configurable Joint at start.
    public float strength = 100f;       //	    Strength of the part. 
    internal float orgStrength = 100f;       //	    Original strength of the part. We will be using this original value while restoring the part.

    public bool isDetachable = true;     //	Can it break at certain damage?

    private bool broken = false;            //	Is this part broken currently?

    public int loosePoint = 50;     //  Part will be broken at this point.
    public int detachPoint = 0;     //  Part will be detached at this point.
    public float deactiveAfterSeconds = 5f; //	Part will be deactivated after the detachment.

    public Vector3 addTorqueAfterLoose = Vector3.zero;      //  	Adds angular velocity related to speed after the brake point reached.

    private void Awake() {

        rigid = GetComponent<Rigidbody>();     //	Getting Rigidbody of the part.
        orgStrength = strength;     //	Getting original strength of the part. We will be using this original value while restoring the part.

        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        //  Getting collider.
        if (!partCollider)
            partCollider = GetComponentInChildren<Collider>();

        //rigid.interpolation = RigidbodyInterpolation.Interpolate;

        //	Setting center of mass if selected.
        if (COM)
            rigid.centerOfMass = transform.InverseTransformPoint(COM.transform.position);

        //	Disable the script if configurable joint not found.
        if (!Joint) {

            Debug.LogWarning("Configurable Joint not found for " + gameObject.name + "!");
            enabled = false;
            return;

        }

        //	Getting original properties of the joint. We will be using the original data for restoring the part while repairing.
        GetJointProperties();

        //	Locks all motions of Configurable Joint at start.
        if (lockAtStart)
            RCCP_Joint.LockPart(Joint);

    }

    /// <summary>
    /// Getting original properties of the joint. We will be using the original data for restoring the part while repairing.
    /// </summary>
    private void GetJointProperties() {

        jointProperties = new RCCP_Joint();
        jointProperties.GetProperties(Joint);

    }

    private void Update() {

        // If part is broken, return.
        if (broken)
            return;

        //	If part is weak and loosen, apply angular velocity related to vehicle speed.
        if (addTorqueAfterLoose != Vector3.zero && strength <= loosePoint) {

            float speed = transform.InverseTransformDirection(rigid.velocity).z;        //	Local speed.
            rigid.AddRelativeTorque(new Vector3(addTorqueAfterLoose.x * speed, addTorqueAfterLoose.y * speed, addTorqueAfterLoose.z * speed));      //	Applying local torque.

        }

    }

    public void OnCollision(float impulse) {

        // If part is broken, return.
        if (broken)
            return;

        //	Decreasing strength of the part related to collision impulse.
        strength -= impulse * 5f;
        strength = Mathf.Clamp(strength, 0f, Mathf.Infinity);

        //	Check joint of the part based on strength.
        CheckJoint();

    }

    /// <summary>
    /// Checks joint of the part based on strength.
    /// </summary>
    private void CheckJoint() {

        // If part is broken, return.
        if (broken)
            return;

        // If strength is 0, unlock the parts and set their joint limits to none. Detach them from the vehicle. If strength is below detach point, only set joint limits to none.
        if (isDetachable && strength <= detachPoint) {

            if (Joint) {

                broken = true;
                Destroy(Joint);
                transform.SetParent(null);
                StartCoroutine(DisablePart(deactiveAfterSeconds));

            }

        } else if (strength <= loosePoint) {

            if (Joint) {

                Joint.angularXMotion = jointProperties.jointMotionAngularX;
                Joint.angularYMotion = jointProperties.jointMotionAngularY;
                Joint.angularZMotion = jointProperties.jointMotionAngularZ;

                Joint.xMotion = jointProperties.jointMotionX;
                Joint.yMotion = jointProperties.jointMotionY;
                Joint.zMotion = jointProperties.jointMotionZ;

            }

        }

    }

    /// <summary>
    /// Repairs, and restores the part.
    /// </summary>
    public void OnRepair() {

        // Enabling gameobject first if it's disabled.
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Setting strength to original strength value. And make sure part is not broken anymore.
        strength = orgStrength;
        broken = false;

        //	If joint is removed and part is detached, adding new configurable joint component. Configurable Joints cannot be toggled on or off. Therefore, we need to destroy and create configurable joints.
        if (Joint == null) {

            // Setting properties of the configurable joint to original properties.
            Joint = gameObject.AddComponent<ConfigurableJoint>();
            jointProperties.SetProperties(Joint);

        } else {

            //	Locking the part.
            Joint.angularXMotion = ConfigurableJointMotion.Locked;
            Joint.angularYMotion = ConfigurableJointMotion.Locked;
            Joint.angularZMotion = ConfigurableJointMotion.Locked;

            Joint.xMotion = ConfigurableJointMotion.Locked;
            Joint.yMotion = ConfigurableJointMotion.Locked;
            Joint.zMotion = ConfigurableJointMotion.Locked;

        }

    }

    /// <summary>
    /// Disables the part with delay.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator DisablePart(float delay) {

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);

    }

    private void Reset() {

        if (!COM) {

            COM = new GameObject("COM").transform;
            COM.SetParent(transform, false);
            COM.localPosition = Vector3.zero;
            COM.localRotation = Quaternion.identity;

        }

        ConfigurableJoint cJoint = GetComponent<ConfigurableJoint>();

        if (!cJoint)
            cJoint = gameObject.AddComponent<ConfigurableJoint>();

        cJoint.connectedBody = GetComponentInParent<RCCP_CarController>(true).gameObject.GetComponent<Rigidbody>();

        if (!partCollider)
            partCollider = GetComponentInChildren<Collider>();

    }

}
