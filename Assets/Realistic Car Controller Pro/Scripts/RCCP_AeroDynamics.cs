//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Manages the dynamics of the vehicle.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Dynamics")]
public class RCCP_AeroDynamics : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public Transform COM;       //  Centre of mass.
    [Range(-100f, 100f)] public float downForce = 10f;      //  Downforce will be applied to the vehicle with speed related. Vehicle will be more controllable on higher speeds.

    public bool autoReset = true;       //  Resets the vehicle if upside down.
    public float autoResetTime = 3f;        //  Timer for reset.
    private float autoResetTimer = 0f;

    private void Awake() {

        //  Assigning center of mass position.
        CarController.Rigid.centerOfMass = transform.InverseTransformPoint(COM.position);

    }

    private void OnEnable() {

        if (CarController)
            CarController.AeroDynamics = this;
        else
            enabled = false;

        autoResetTimer = 0f;

    }

    private void FixedUpdate() {

        //  Applying downforce to the vehicle.
        CarController.Rigid.AddRelativeForce(Vector3.down * downForce * Mathf.Abs(CarController.speed), ForceMode.Force);

        //  If auto-reset is enabled, check upside down.
        if (autoReset)
            CheckUpsideDown();

    }

    /// <summary>
    /// Resets the car if upside down.
    /// </summary>
    private void CheckUpsideDown() {

        //  If vehicle speed is below 5 and upside down, it will count to the target seconds and resets the vehicle.
        if (Mathf.Abs(CarController.speed) < 5 && !CarController.Rigid.isKinematic) {

            if (CarController.transform.eulerAngles.z < 300 && CarController.transform.eulerAngles.z > 60) {

                autoResetTimer += Time.deltaTime;

                if (autoResetTimer > autoResetTime) {

                    CarController.transform.SetPositionAndRotation(

                        CarController.transform.position = new Vector3(CarController.transform.position.x, CarController.transform.position.y + 3, CarController.transform.position.z),
                        CarController.transform.rotation = Quaternion.Euler(0f, CarController.transform.eulerAngles.y, 0f)

                        );

                    autoResetTimer = 0f;

                }

            }

        }

    }

    private void Reset() {

        if (transform.Find("COM"))
            DestroyImmediate(transform.Find("COM").gameObject);

        GameObject newCom = new GameObject("COM");
        newCom.transform.SetParent(transform, false);
        COM = newCom.transform;
        COM.transform.localPosition = new Vector3(0f, -.25f, 0f);

    }

}
