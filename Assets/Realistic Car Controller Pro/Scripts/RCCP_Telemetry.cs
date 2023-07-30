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
using UnityEngine.UI;

/// <summary>
/// UI telemetry for info.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP Telemetry")]
public class RCCP_Telemetry : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController carController;

    [System.Serializable]
    public class WheelInfo {

        public Text wheelName;
        public Text RPM_Wheel;
        public Text Torque_Wheel;
        public Text Brake_Wheel;
        public Text Force_Wheel;
        public Text Angle_Wheel;
        public Text Sideways_Wheel;
        public Text Forward_Wheel;
        public Text GroundHit_Wheel;

    }
    [Space()]
    //  All wheel info with custom class.
    public WheelInfo[] wheelInfos;
    [Space()]
    public Text ABS;        //  ABS text.
    public Text ESP;        //  ESP text.
    public Text TCS;        //  TCS text.
    [Space()]
    public Text wheelSpeed;     //  Wheel speec text.
    public Text physicalSpeed;      //  Vehicle speed text.
    public Text engineRPM;      //  Engine rpm text.
    public Text gear;       //  Current gear text.
    public Text finalTorque;        //  Differential final drive torque text.
    public Text controllable;       //  Can Control state of the car controller.
    [Space()]
    public Text throttle_P;     //  Player throttle input.
    public Text steer_P;        //  Player steer input.
    public Text brake_P;        //  Player brake input.
    public Text handbrake_P;        //  Player handbrake input.
    public Text clutch_P;       //  Player clutch input.
    [Space()]
    public Text throttle_V;     //  Vehicle throttle input.
    public Text steer_V;        //  Vehicle steer input.
    public Text brake_V;        //  Vehicle brake input.
    public Text handbrake_V;        //  Vehicle handbrake input.
    public Text clutch_V;       //  Vehicle clutch input.

    private void Update() {

        //  Getting active player car controller on the scene.
        carController = RCCP_SceneManager.Instance.activePlayerVehicle;

        //  If no active player car found, return.
        if (!carController)
            return;

        //  If vehicle has wheelcolliders, assign wheel texts.
        if (carController.AllWheelColliders != null && carController.AllWheelColliders.Length >= 1) {

            //  Telemetry has four panels. Even if vehicle has over four wheels, only four panel will be visible.
            for (int i = 0; i < 4; i++) {

                RCCP_WheelCollider wheelCollider_RCCP = carController.AllWheelColliders[i];

                wheelInfos[i].wheelName.text = "Name: " + wheelCollider_RCCP.WheelCollider.name;
                wheelInfos[i].RPM_Wheel.text = "RPM: " + wheelCollider_RCCP.WheelCollider.rpm.ToString("F0");
                wheelInfos[i].Torque_Wheel.text = "Torque: " + wheelCollider_RCCP.WheelCollider.motorTorque.ToString("F0");
                wheelInfos[i].Brake_Wheel.text = "Brake: " + wheelCollider_RCCP.WheelCollider.brakeTorque.ToString("F0");
                wheelInfos[i].Force_Wheel.text = "Force: " + wheelCollider_RCCP.bumpForce.ToString("F0");
                wheelInfos[i].Angle_Wheel.text = "Angle: " + wheelCollider_RCCP.WheelCollider.steerAngle.ToString("F0");
                wheelInfos[i].Sideways_Wheel.text = "Slip_Sd: " + wheelCollider_RCCP.wheelSlipAmountSideways.ToString("F");
                wheelInfos[i].Forward_Wheel.text = "Slip_Fwd: " + wheelCollider_RCCP.wheelSlipAmountForward.ToString("F");

                if (wheelCollider_RCCP.wheelHit.collider != null)
                    wheelInfos[i].GroundHit_Wheel.text = "Hit: " + (wheelCollider_RCCP.WheelCollider.isGrounded ? wheelCollider_RCCP.wheelHit.collider.name : "");

            }

        }

        if (carController.Stability) {

            ABS.text = "ABS: " + (carController.Stability.ABSEngaged ? "Engaged" : "Not Engaged");
            ESP.text = "ESP: " + (carController.Stability.ESPEngaged ? "Engaged" : "Not Engaged");
            TCS.text = "TCS: " + (carController.Stability.TCSEngaged ? "Engaged" : "Not Engaged");

        } else {

            ABS.text = "ABS: Not Equipped";
            ESP.text = "ESP: Not Equipped";
            TCS.text = "TCS: Not Equipped";

        }

        wheelSpeed.text = "Wheel Speed Average: " + carController.wheelRPM2Speed.ToString("F0");
        physicalSpeed.text = "Physical Speed: " + carController.speed.ToString("F0");
        engineRPM.text = "Engine RPM: " + carController.engineRPM.ToString("F0");
        finalTorque.text = "Final Torque: " + carController.producedDifferentialTorque.ToString("F0");

        if (carController.direction == 1) {

            if (!carController.shiftingNow)
                gear.text = "Gear: " + (carController.currentGear + 1).ToString("F0");
            else
                gear.text = "Gear: " + "N";

        } else {

            gear.text = "Gear: " + "R";

        }

        controllable.text = "Controllable: " + (carController.canControl ? "True" : "False");

        throttle_P.text = "Player Throttle: " + carController.throttleInput_P.ToString("F2");
        steer_P.text = "Player Steer: " + carController.steerInput_P.ToString("F2");
        brake_P.text = "Player Brake: " + carController.brakeInput_P.ToString("F2");
        handbrake_P.text = "Player Handbrake: " + carController.handbrakeInput_P.ToString("F2");
        clutch_P.text = "Player Clutch: " + carController.clutchInput_P.ToString("F2");

        throttle_V.text = "Vehicle Throttle: " + carController.throttleInput_V.ToString("F2");
        steer_V.text = "Vehicle Steer: " + carController.steerInput_V.ToString("F2");
        brake_V.text = "Vehicle Brake: " + carController.brakeInput_V.ToString("F2");
        handbrake_V.text = "Vehicle Handbrake: " + carController.handbrakeInput_V.ToString("F2");
        clutch_V.text = "Vehicle Clutch: " + carController.clutchInput_V.ToString("F2");

    }

}
