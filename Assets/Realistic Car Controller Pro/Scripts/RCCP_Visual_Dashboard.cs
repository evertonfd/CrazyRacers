//----------------------------------------------
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
/// Receiving inputs from active vehicle on your scene, and feeds visual dashboard needles (Not UI).
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Visual Dashboard")]
public class RCCP_Visual_Dashboard : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    [Space()]
    public Transform steeringWheel;     // Driver steering wheel model. In case of if your vehicle has individual steering wheel model in interior.
    private Quaternion orgSteeringWheelRot;     // Original rotation of steering wheel.

    public enum SteeringWheelRotateAround { XAxis, YAxis, ZAxis }       //	Rotation axis of steering wheel.
    public SteeringWheelRotateAround steeringWheelRotateAround = SteeringWheelRotateAround.ZAxis;     // Current rotation of steering wheel.

    public float steeringAngleMultiplier = 3f;      //  Steering angle multiplier.

    [System.Serializable]
    public class RPMDial {

        public GameObject dial;     //  Actual dial model.
        public float multiplier = .05f;     //  Rotation multiplier.
        public RotateAround rotateAround = RotateAround.Z;      //  Rotation axis.
        private Quaternion dialOrgRotation = Quaternion.identity;       //  Rotation of the dial.
        public Text text;       //  Optional text.

        /// <summary>
        /// Initializing the dial.
        /// </summary>
        public void Init() {

            if (dial)
                dialOrgRotation = dial.transform.localRotation;

        }

        /// <summary>
        /// Updates the rotation of the dial.
        /// </summary>
        /// <param name="value"></param>
        public void Update(float value) {

            Vector3 targetAxis = Vector3.forward;

            switch (rotateAround) {

                case RotateAround.X:

                    targetAxis = Vector3.right;
                    break;

                case RotateAround.Y:

                    targetAxis = Vector3.up;
                    break;

                case RotateAround.Z:

                    targetAxis = Vector3.forward;
                    break;

            }

            dial.transform.localRotation = dialOrgRotation * Quaternion.AngleAxis(-multiplier * value, targetAxis);

            if (text)
                text.text = value.ToString("F0");

        }

    }

    [System.Serializable]
    public class InteriorLight {

        public Light light;
        public float intensity = 1f;
        public LightRenderMode renderMode = LightRenderMode.Auto;

        public void Init() {

            light.renderMode = renderMode;

        }

        public void Update(bool state) {

            if (!light.enabled)
                light.enabled = true;

            light.intensity = state ? intensity : 0f;

        }

    }

    [Space()]
    public RPMDial rPMDial = new RPMDial();
    [Space()]
    public RPMDial speedDial = new RPMDial();
    [Space()]
    public InteriorLight[] interiorLights = new InteriorLight[0];

    public enum RotateAround { X, Y, Z }

    private void Awake() {

        //  Initializing dials.
        rPMDial.Init();
        speedDial.Init();

        //  Initializing lights.
        for (int i = 0; i < interiorLights.Length; i++)
            interiorLights[i].Init();

    }

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.Dashboard = this;
        else
            enabled = false;

    }

    private void Update() {

        //  If no car controller found, return.
        if (!CarController)
            return;

        SteeringWheel();
        Dials();
        Lights();

    }

    /// <summary>
    /// Operating the steering wheel.
    /// </summary>
    private void SteeringWheel() {

        //Driver SteeringWheel Transform.
        if (steeringWheel) {

            if (orgSteeringWheelRot.eulerAngles == Vector3.zero)
                orgSteeringWheelRot = steeringWheel.transform.localRotation;

            switch (steeringWheelRotateAround) {

                case SteeringWheelRotateAround.XAxis:
                    steeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(CarController.steerAngle * steeringAngleMultiplier, -Vector3.right);
                    break;

                case SteeringWheelRotateAround.YAxis:
                    steeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(CarController.steerAngle * steeringAngleMultiplier, -Vector3.up);
                    break;

                case SteeringWheelRotateAround.ZAxis:
                    steeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(CarController.steerAngle * steeringAngleMultiplier, -Vector3.forward);
                    break;

            }

        }

    }

    /// <summary>
    /// Updates dials rotation.
    /// </summary>
    private void Dials() {

        if (rPMDial.dial != null)
            rPMDial.Update(CarController.engineRPM);

        if (speedDial.dial != null)
            speedDial.Update(Mathf.Abs(CarController.speed));

    }

    /// <summary>
    /// Updates lights of the dash.
    /// </summary>
    private void Lights() {

        if (!CarController.Lights)
            return;

        for (int i = 0; i < interiorLights.Length; i++)
            interiorLights[i].Update(CarController.Lights.lowBeamHeadlights);

    }

}
