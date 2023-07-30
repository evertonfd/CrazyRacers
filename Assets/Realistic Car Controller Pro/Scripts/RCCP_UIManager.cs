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
using TMPro;

/// <summary>
/// RCCP UI Canvas that manages the event systems, panels, gauges, images and texts related to the vehicle and player.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP UI Manager")]
public class RCCP_UIManager : MonoBehaviour {

    private RCCP_CarController carController;       //  Main car controller.

    [Header("Event Systems")]
    public GameObject oldEventReceiver;     //  Old event system.
    public GameObject newEventReceiver;     //  New input system.

    [Header("Panels")]
    public GameObject dashboard;        //  Dashboard panel.
    public GameObject mobileControllers;        //  Mobile controllers panel.
    public GameObject options;      //  Options panel.
    public GameObject customizer;       //  Customizer panel.
    public GameObject informer;     //  Informer panel.

    [System.Serializable]
    public class SpeedOMeter {

        public GameObject needle;       //  Needle.

        public enum TurnAxis { X, Y, Z }        //  Turn axis.
        public TurnAxis turnAxis = TurnAxis.Z;

        public float multiplierRotation = -0.0245f;      //  Turn multiplier.
        private float defRotation = -1f;        //  Default rotation of the needle.
        private float currentRotation = 0f;     //  Current rotation of the needle.
        private float input = 0f;       //  Input.

        /// <summary>
        /// Operates the needle with given input.
        /// </summary>
        /// <param name="_input"></param>
        public void Operate(float _input) {

            //  Taking default rotation of the needle.
            if (defRotation == -1f) {

                switch (turnAxis) {

                    case TurnAxis.X:
                        defRotation = needle.transform.localEulerAngles.x;
                        break;

                    case TurnAxis.Y:
                        defRotation = needle.transform.localEulerAngles.y;
                        break;

                    case TurnAxis.Z:
                        defRotation = needle.transform.localEulerAngles.z;
                        break;

                }

            }

            //  Input.
            input = _input;

            //  Current rotation of the needle.
            currentRotation = defRotation + (input * multiplierRotation);

            //  And turning the needle.
            switch (turnAxis) {

                case TurnAxis.X:
                    needle.transform.localEulerAngles = new Vector3(currentRotation, needle.transform.localEulerAngles.y, needle.transform.localEulerAngles.z);
                    break;

                case TurnAxis.Y:
                    needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, currentRotation, needle.transform.localEulerAngles.z);
                    break;

                case TurnAxis.Z:
                    needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, currentRotation);
                    break;

            }

        }

    }

    public SpeedOMeter speedometer;

    [Header("Images")]
    public Image left;
    public Image right;
    public Image headlights;
    public Image ESP;
    public Image ABS;
    public Image TCS;
    public Image NOS;

    [Header("Texts")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI RPMText;
    public TextMeshProUGUI gearText;
    public GameObject recording;
    public GameObject replaying;

    private void Awake() {

        //  Enabling / disabling correct event system for old and new input system.
        if ((newEventReceiver && oldEventReceiver)) {

            if (RCCP_Settings.Instance.useNewInputSystem) {

                newEventReceiver.SetActive(true);
                oldEventReceiver.SetActive(false);

            } else {

                newEventReceiver.SetActive(false);
                oldEventReceiver.SetActive(true);

            }

        }

    }

    private void OnEnable() {

        //  Firing an event when RCCP Canvas spawns.
        RCCP_Events.Event_OnRCCPUISpawned(this);

    }

    private void LateUpdate() {

        //  Finding player vehicle on the scene.
        carController = RCCP_SceneManager.Instance.activePlayerVehicle;

        //  If car controller not found, at disable ui option is enabled, diasble panels.
        if (RCCP_SceneManager.Instance.disableUIWhenNoPlayerVehicle) {

            if (RCCP_Settings.Instance.mobileControllerEnabled && mobileControllers && mobileControllers.activeSelf != carController)
                mobileControllers.SetActive(carController);

            if (dashboard && dashboard.activeSelf != carController)
                dashboard.SetActive(carController);

        }

        //  If no car controller found, return.
        if (!carController)
            return;

        if (speedometer != null && speedometer.needle)
            speedometer.Operate(carController.engineRPM);

        //  If vehicle has stability component, control the ESP and ABS images.
        if (carController.Stability) {

            if (ESP)
                ESP.color = carController.Stability.ESPEngaged ? Color.white : new Color(0f, 0f, 0f, .2f);

            if (ABS)
                ABS.color = carController.Stability.ABSEngaged ? Color.white : new Color(0f, 0f, 0f, .2f);

            if (TCS)
                TCS.color = carController.Stability.TCSEngaged ? Color.white : new Color(0f, 0f, 0f, .2f);

        }

        //  If vehicle has lights component, control the light images.
        if (carController.Lights) {

            if (headlights)
                headlights.color = carController.Lights.lowBeamHeadlights ? Color.white : new Color(0f, 0f, 0f, .2f);

            if (left)
                left.color = carController.Lights.indicatorsLeft ? Color.white : new Color(0f, 0f, 0f, .2f);

            if (right)
                right.color = carController.Lights.indicatorsRight ? Color.white : new Color(0f, 0f, 0f, .2f);

            //if (hazard)
            //    hazard.color = carController.Lights.indicatorsAll ? Color.white : new Color(0f, 0f, 0f, .2f);

        }

        //  If vehicle has nos component, control the nos sliders.
        if (carController.OtherAddonsManager && carController.OtherAddonsManager.Nos) {

            if (NOS)
                NOS.fillAmount = carController.OtherAddonsManager.Nos.amount;

        } else {

            if (NOS)
                NOS.fillAmount = 0f;

        }

        //  Assigning text of the speed.
        if (speedText)
            speedText.text = carController.speed.ToString("F0");

        //  Assigning text of the rpm.
        if (RPMText)
            RPMText.text = carController.engineRPM.ToString("F0");

        //  Assigning text of the gear.
        if (gearText) {

            if (carController.direction == 1) {

                if (!carController.shiftingNow)
                    gearText.text = (carController.currentGear + 1).ToString("F0");
                else
                    gearText.text = "N";

            } else {

                gearText.text = "R";

            }

        }

        //  If vehicle has recorder component, control the recording and playing texts.
        if (carController.OtherAddonsManager && carController.OtherAddonsManager.Recorder) {

            switch (carController.OtherAddonsManager.Recorder.mode) {

                case RCCP_Recorder.Mode.Neutral:

                    if (recording)
                        recording.SetActive(false);

                    if (replaying)
                        replaying.SetActive(false);

                    break;

                case RCCP_Recorder.Mode.Record:

                    if (recording)
                        recording.SetActive(true);

                    if (replaying)
                        replaying.SetActive(false);

                    break;

                case RCCP_Recorder.Mode.Play:

                    if (recording)
                        recording.SetActive(false);

                    if (replaying)
                        replaying.SetActive(true);

                    break;

            }

        }

    }

    private void OnDisable() {

        RCCP_Events.Event_OnRCCPUIDestroyed(this);

    }

}
