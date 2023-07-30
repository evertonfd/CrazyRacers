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
/// Transmits the received power from the engine --> clutch --> gearbox to the axle. 
/// Open differential = RPM difference between both wheels will decide to which wheel needs more traction or not. 
/// Limited = almost same with open with slip limitation. Higher percents = more close to the locked system. 
/// Locked = both wheels will have the same traction.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Differential")]
public class RCCP_Differential : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

#if UNITY_EDITOR
    [HideInInspector] public bool completeSetup = false;
    [HideInInspector] public bool checkedSetup = false;
#endif

    public bool overrideDifferential = false;

    public enum DifferentialType {

        Open,
        Limited,
        FullLocked,
        Direct

    }

    public DifferentialType differentialType = DifferentialType.Limited;       // Differential type.

    [Range(50f, 100f)] public float limitedSlipRatio = 80f;     //  LSD.

    public float finalDriveRatio = 3.73f;       //  Final drive ratio multiplier. Faster accelerations and lower top speeds on higher values.
    public float receivedTorqueAsNM = 0f;       //  Received torque from the component. It should be the gearbox in this case.
    public float producedTorqueAsNM = 0f;       //  Deliveted torque to the component. It should be an axle in this case.

    public float leftWheelRPM = 0f;     //  Left wheel rpm.
    public float rightWheelRPM = 0f;        //  Right wheel rpm.

    public float wheelSlipRatio = 0f;       //  Wheel slip ratio.
    public float leftWheelSlipRatio = 0f;       //  Left slip ratio.
    public float rightWheelSlipRatio = 0f;      //  Right slip ratio.

    public float outputLeft = 0f;       //  Output of the left wheel.
    public float outputRight = 0f;      //  Output of the right wheel.

    public RCCP_Axle connectedAxle;     //  Connected axle to this differential. Each differential must have an axle.

    private void OnEnable() {

        if (CarController)
            CarController.Differential = this;
        else
            enabled = false;

        leftWheelRPM = 0f;
        rightWheelRPM = 0f;
        wheelSlipRatio = 0f;
        leftWheelSlipRatio = 0f;
        rightWheelSlipRatio = 0f;
        outputLeft = 0f;
        outputRight = 0f;
        producedTorqueAsNM = 0f;

    }

    private void FixedUpdate() {

        if (overrideDifferential)
            return;

        if (!connectedAxle)
            return;

        Gears();
        Output();

    }

    /// <summary>
    /// Calculating output torque of the left and right side.
    /// </summary>
    private void Gears() {

        //  Get rpm.
        if (connectedAxle.leftWheelCollider)
            leftWheelRPM = Mathf.Abs(connectedAxle.leftWheelCollider.WheelCollider.rpm);

        if (connectedAxle.rightWheelCollider)
            rightWheelRPM = Mathf.Abs(connectedAxle.rightWheelCollider.WheelCollider.rpm);

        //  Sum rpm and difference.
        float sumRPM = leftWheelRPM + rightWheelRPM;
        float diffRPM = leftWheelRPM - rightWheelRPM;

        //  Calculating the wheel slip ratio between left and right wheel.
        wheelSlipRatio = Mathf.InverseLerp(0f, sumRPM, Mathf.Abs(diffRPM));
        
        switch (differentialType) {

            //  If differential type is open...
            case DifferentialType.Open:

                if (Mathf.Sign(diffRPM) == 1) {

                    leftWheelSlipRatio = wheelSlipRatio;
                    rightWheelSlipRatio = -wheelSlipRatio;

                } else {

                    leftWheelSlipRatio = -wheelSlipRatio;
                    rightWheelSlipRatio = wheelSlipRatio;

                }

                break;

            //  If differential type is LSD...
            case DifferentialType.Limited:

                wheelSlipRatio *= Mathf.Lerp(1f, 0f, (limitedSlipRatio / 100f));

                if (Mathf.Sign(diffRPM) == -1) {

                    leftWheelSlipRatio = -wheelSlipRatio;
                    rightWheelSlipRatio = wheelSlipRatio;

                } else {

                    leftWheelSlipRatio = wheelSlipRatio;
                    rightWheelSlipRatio = -wheelSlipRatio;

                }

                break;

            //  If differential type is full locked...
            case DifferentialType.FullLocked:

                if (Mathf.Sign(diffRPM) == -1) {

                    leftWheelSlipRatio = -.5f;
                    rightWheelSlipRatio = .5f;

                } else {

                    leftWheelSlipRatio = .5f;
                    rightWheelSlipRatio = -.5f;

                }

                break;

            case DifferentialType.Direct:

                leftWheelSlipRatio = 0f;
                rightWheelSlipRatio = 0f;

                break;

        }

    }

    /// <summary>
    /// Overrides the differential output with given values.
    /// </summary>
    /// <param name="targetOutputLeft"></param>
    /// <param name="targetOutputRight"></param>
    public void OverrideDifferential(float targetOutputLeft, float targetOutputRight) {

        outputLeft = targetOutputLeft;
        outputRight = targetOutputRight;
        producedTorqueAsNM = outputLeft + outputRight;

        connectedAxle.isPower = true;
        connectedAxle.ReceiveOutput(targetOutputLeft, targetOutputRight);

    }

    /// <summary>
    /// Receive torque from the component.
    /// </summary>
    /// <param name="output"></param>
    public void ReceiveOutput(RCCP_Output output) {

        if (overrideDifferential)
            return;

        receivedTorqueAsNM = output.NM;

    }

    /// <summary>
    /// Output to the left and right wheels.
    /// </summary>
    private void Output() {

        producedTorqueAsNM = receivedTorqueAsNM * finalDriveRatio;

        outputLeft = producedTorqueAsNM / 2f;
        outputRight = producedTorqueAsNM / 2f;

        outputLeft -= producedTorqueAsNM * leftWheelSlipRatio;
        outputRight -= producedTorqueAsNM * rightWheelSlipRatio;

        connectedAxle.isPower = true;
        connectedAxle.ReceiveOutput(outputLeft, outputRight);

    }

}
