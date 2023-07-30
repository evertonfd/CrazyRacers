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
/// Transmits the received power from the differential and share to the wheels (if differential is connected to this axle). Steering, braking, traction, and all wheel related processes are managed by this axle. Has two connected wheels.
/// </summary>
[System.Serializable]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Axle")]
public class RCCP_Axle : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  Main axle manager.
    private RCCP_Axles _axleManager;
    private RCCP_Axles AxleManager {

        get {

            //  Trying to find the axle manager attached to the vehicle.
            //  If there is no axle manager, create it and make sure all axles are children of it.
            if (_axleManager == null)
                _axleManager = GetComponentInParent<RCCP_Axles>(true);

            if (_axleManager == null)
                _axleManager = CarController.GetComponentInChildren<RCCP_Axles>(true);

            if (_axleManager == null) {

                GameObject newAxleManager = new GameObject("RCCP_Axles");
                newAxleManager.transform.SetParent(CarController.transform, false);
                _axleManager = newAxleManager.AddComponent<RCCP_Axles>();

            }

            if (_axleManager && transform.parent != _axleManager.transform)
                transform.SetParent(_axleManager.transform, false);

            return _axleManager;

        }

    }

#if UNITY_EDITOR
    [HideInInspector] public bool completeSetup = false;
    [HideInInspector] public bool checkedSetup = false;
    [HideInInspector] public bool autoAlignWheelColliders = true;
#endif

    //  Wheel models. Just for visual.
    public Transform leftWheelModel;
    public Transform rightWheelModel;

    //  Wheelcolliders.
    public RCCP_WheelCollider leftWheelCollider;
    public RCCP_WheelCollider rightWheelCollider;

    public float antirollForce = 500f;      //  Anti roll force.

    public bool isPower = false;        //  Is this axle powered?
    public bool isSteer = false;        //  Is this axle steered?
    public bool isBrake = false;        //  Is this axle braked?
    public bool isHandbrake = false;        //  Is this axle handbraked?

    //  Multipliers.
    [Range(-1f, 1f)] public float powerMultiplier = 1f;
    [Range(-1f, 1f)] public float steerMultiplier = 1f;
    [Range(0f, 1f)] public float brakeMultiplier = 1f;
    [Range(0f, 1f)] public float handbrakeMultiplier = 1f;

    //  Received inputs from the vehicle.
    public float throttleInput = 0f;
    public float brakeInput = 0f;
    public float steerInput = 0f;
    public float handbrakeInput = 0f;

    public float steerAngle = 0f;       //  Current steer angle.
    [Range(.01f, 1f)] public float steerSpeed = 1f;     //  Steering speed.

    public bool isGrounded = false;     //  Is this axle grounded?

    public float maxBrakeTorque = 5000f;        //  Max brake torque.
    public float maxSteerAngle = 40f;       //  Max steer angle.
    public float tractionHelpedSidewaysStiffness = 1f;      //  Traction helper received from the RCCP_Stability. Used for avoiding spins.

    //  Produced torques for left and right wheels.
    public float producedMotorTorqueNM_Left = 0f;
    public float producedMotorTorqueNM_Right = 0f;
    public float producedBrakeTorqueNM = 0f;
    public float producedHandbrakeTorqueNM = 0f;

    private void OnEnable() {

        if (AxleManager) {

            if (!AxleManager.Axles.Contains(this))
                AxleManager.Axles.Add(this);

        } else {

            enabled = false;

        }

        throttleInput = 0f;
        brakeInput = 0f;
        steerInput = 0f;
        handbrakeInput = 0f;
        steerAngle = 0f;
        tractionHelpedSidewaysStiffness = 1f;
        producedBrakeTorqueNM = 0f;
        producedHandbrakeTorqueNM = 0f;
        producedMotorTorqueNM_Left = 0f;
        producedMotorTorqueNM_Right = 0f;

    }

    private void Update() {

        Inputs();
        CheckGrounded();

    }

    /// <summary>
    /// Receiving inputs.
    /// </summary>
    private void Inputs() {

        throttleInput = CarController.throttleInput_P;
        steerInput = CarController.steerInput_P;
        brakeInput = CarController.brakeInput_P;
        handbrakeInput = CarController.handbrakeInput_P;

    }

    /// <summary>
    /// Is this axle grounded or not?
    /// </summary>
    private void CheckGrounded() {

        //  If wheelcolliders are not selected, return.
        if (!leftWheelCollider || !rightWheelCollider)
            return;

        if ((leftWheelCollider.WheelCollider.enabled && leftWheelCollider.WheelCollider.isGrounded) || (rightWheelCollider.WheelCollider.enabled && rightWheelCollider.WheelCollider.isGrounded))
            isGrounded = true;
        else
            isGrounded = false;

    }

    private void FixedUpdate() {

        //  Connecting wheels to this axle.
        if (leftWheelCollider)
            leftWheelCollider.connectedAxle = this;

        if (rightWheelCollider)
            rightWheelCollider.connectedAxle = this;

        //  Assigning steer angle with steer speed.
        steerAngle = Mathf.MoveTowards(steerAngle, steerInput * maxSteerAngle, Time.fixedDeltaTime * steerSpeed * 150f);

        producedBrakeTorqueNM = brakeInput * maxBrakeTorque;        //  Calculating produced brake torque.
        producedHandbrakeTorqueNM = handbrakeInput * maxBrakeTorque;        //  Calculating produced handbrake torque.

        AntiRollBars();
        Output();

    }

    /// <summary>
    /// Antiroll bars.
    /// </summary>
    private void AntiRollBars() {

        //  If wheelcolliders are not selected, return.
        if (!leftWheelCollider || !rightWheelCollider)
            return;

        //  If wheelcolliders are not enabled, return.
        if (!leftWheelCollider.WheelCollider.enabled || !rightWheelCollider.WheelCollider.enabled)
            return;

        //  If wheelcolliders are not enabled, return.
        if (!leftWheelCollider.wheelHit.collider || !rightWheelCollider.wheelHit.collider)
            return;

        //  Left and right travels.
        float travel_L = 1f;
        float travel_R = 1f;

        //  Only calculate when left and right wheels are grounded.
        bool grounded_L = leftWheelCollider.isGrounded;

        //  Calculating travel L.
        if (grounded_L)
            travel_L = (-leftWheelCollider.transform.InverseTransformPoint(leftWheelCollider.wheelHit.point).y - leftWheelCollider.WheelCollider.radius) / leftWheelCollider.WheelCollider.suspensionDistance;

        //  Only calculate when left and right wheels are grounded.
        bool grounded_R = rightWheelCollider.isGrounded;

        //  Calculating travel R.
        if (grounded_R)
            travel_R = (-rightWheelCollider.transform.InverseTransformPoint(rightWheelCollider.wheelHit.point).y - rightWheelCollider.WheelCollider.radius) / rightWheelCollider.WheelCollider.suspensionDistance;

        //  Calculating final torque.
        float calculatedForce = (travel_L - travel_R) * antirollForce;

        //  If both wheels are still enabled, apply the force.
        if (leftWheelCollider.WheelCollider.enabled && rightWheelCollider.WheelCollider.enabled) {

            if (grounded_L)
                CarController.Rigid.AddForceAtPosition(leftWheelCollider.transform.up * -calculatedForce, leftWheelCollider.transform.position);
            if (grounded_R)
                CarController.Rigid.AddForceAtPosition(rightWheelCollider.transform.up * calculatedForce, rightWheelCollider.transform.position);

        }

    }

    /// <summary>
    /// Receive output.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void ReceiveOutput(float left, float right) {

        producedMotorTorqueNM_Left = left;
        producedMotorTorqueNM_Right = right;

    }

    /// <summary>
    /// Output.
    /// </summary>
    private void Output() {

        //  Applying multipliers.
        producedMotorTorqueNM_Left *= powerMultiplier;
        producedMotorTorqueNM_Right *= powerMultiplier;
        producedBrakeTorqueNM *= brakeMultiplier;
        producedHandbrakeTorqueNM *= handbrakeMultiplier;

        //  If this wheel is powered, distribute power to the wheels.
        if (isPower) {

            if (leftWheelCollider)
                leftWheelCollider.AddMotorTorque(producedMotorTorqueNM_Left);

            if (rightWheelCollider)
                rightWheelCollider.AddMotorTorque(producedMotorTorqueNM_Right);

        }

        //  If this wheel is steered, apply steering angle to the wheels.
        if (isSteer) {

            if (leftWheelCollider)
                leftWheelCollider.ApplySteering(steerAngle * steerMultiplier);

            if (rightWheelCollider)
                rightWheelCollider.ApplySteering(steerAngle * steerMultiplier);

        }

        //  If this wheel is braked, apply brake torque to the wheels.
        if (isBrake) {

            if (leftWheelCollider)
                leftWheelCollider.AddBrakeTorque(producedBrakeTorqueNM / 2f);

            if (rightWheelCollider)
                rightWheelCollider.AddBrakeTorque(producedBrakeTorqueNM / 2f);

        }

        //  If this wheel is handbraked, apply handbrake torque to the wheels.
        if (isHandbrake && handbrakeInput >= .2f) {

            if (leftWheelCollider)
                leftWheelCollider.AddHandbrakeTorque(producedHandbrakeTorqueNM / 2f);

            if (rightWheelCollider)
                rightWheelCollider.AddHandbrakeTorque(producedHandbrakeTorqueNM / 2f);

        }

    }

    private void OnDisable() {

        if (AxleManager) {

            if (AxleManager.Axles.Contains(this))
                AxleManager.Axles.Remove(this);

        }

    }

    private void Reset() {

        RCCP_WheelCollider[] oldWheelColliders = gameObject.GetComponentsInChildren<RCCP_WheelCollider>();

        for (int i = 0; i < oldWheelColliders.Length; i++)
            DestroyImmediate(oldWheelColliders[i].gameObject);

        GameObject newWheelCollider_L = new GameObject("WheelCollider_L");
        newWheelCollider_L.transform.SetParent(transform, false);
        RCCP_WheelCollider wheelCollider_L = newWheelCollider_L.AddComponent<RCCP_WheelCollider>();

        GameObject newWheelCollider_R = new GameObject("WheelCollider_R");
        newWheelCollider_R.transform.SetParent(transform, false);
        RCCP_WheelCollider wheelCollider_R = newWheelCollider_R.AddComponent<RCCP_WheelCollider>();

        leftWheelCollider = wheelCollider_L;
        rightWheelCollider = wheelCollider_R;

        leftWheelCollider.connectedAxle = this;
        rightWheelCollider.connectedAxle = this;

    }

}
