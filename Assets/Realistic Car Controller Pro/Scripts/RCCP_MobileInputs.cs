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
/// Receives inputs from the ui controllers. RCCP_InputManager will process these inputs if controller type is mobile.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/Mobile/RCCP Mobile Inputs")]
public class RCCP_MobileInputs : MonoBehaviour {

    private static RCCP_MobileInputs instance;
    public static RCCP_MobileInputs Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<RCCP_MobileInputs>();

            return instance;

        }

    }

    public GameObject mobileCanvas;     //  Root canvas to enable / disable depends on the selected option in the RCCP_Settings ("Use Mobile Controller").

    //  UI controller buttons.
    public RCCP_UIController throttle;
    public RCCP_UIController brake;
    public RCCP_UIController left;
    public RCCP_UIController right;
    public RCCP_UIController ebrake;
    public RCCP_UIController nos;

    public RCCP_UI_SteeringWheelController steeringWheel;       //  Steering wheel.
    public RCCP_UI_Joystick joystick;       //  Joystick.

    //  Output inputs.
    public float throttleInput = 0f;
    public float steerInput = 0f;
    public float brakeInput = 0f;
    public float ebrakeInput = 0f;
    public float nosInput = 0f;

    private void Update() {

        //  If mobile controller is enabled, set canvas true. Otherwise false.
        if (!RCCP_Settings.Instance.mobileControllerEnabled) {

            if (mobileCanvas.activeSelf)
                mobileCanvas.SetActive(false);

            return;

        }

        if (!mobileCanvas.activeSelf)
            mobileCanvas.SetActive(true);

        //  Mobile controller types.
        switch (RCCP_Settings.Instance.mobileController) {

            //  If touch screen, enable and disable corresponding buttons.
            case RCCP_Settings.MobileController.TouchScreen:

                if (steeringWheel && steeringWheel.gameObject.activeSelf)
                    steeringWheel.gameObject.SetActive(false);

                if (joystick && joystick.gameObject.activeSelf)
                    joystick.gameObject.SetActive(false);

                if (left && !left.gameObject.activeSelf)
                    left.gameObject.SetActive(true);

                if (right && !right.gameObject.activeSelf)
                    right.gameObject.SetActive(true);

                break;

            //  If gyro, enable and disable corresponding buttons.
            case RCCP_Settings.MobileController.Gyro:

                if (steeringWheel && steeringWheel.gameObject.activeSelf)
                    steeringWheel.gameObject.SetActive(false);

                if (joystick && joystick.gameObject.activeSelf)
                    joystick.gameObject.SetActive(false);

                if (left && left.gameObject.activeSelf)
                    left.gameObject.SetActive(false);

                if (right && right.gameObject.activeSelf)
                    right.gameObject.SetActive(false);

#if BCG_NEWINPUTSYSTEM

                if (UnityEngine.InputSystem.Accelerometer.current != null && UnityEngine.InputSystem.Accelerometer.current.device.enabled == false)
                    UnityEngine.InputSystem.InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);

#endif

                break;

            //  If steering wheel, enable and disable corresponding buttons.
            case RCCP_Settings.MobileController.SteeringWheel:

                if (steeringWheel && !steeringWheel.gameObject.activeSelf)
                    steeringWheel.gameObject.SetActive(true);

                if (joystick && joystick.gameObject.activeSelf)
                    joystick.gameObject.SetActive(false);

                if (left && left.gameObject.activeSelf)
                    left.gameObject.SetActive(false);

                if (right && right.gameObject.activeSelf)
                    right.gameObject.SetActive(false);

                break;

            //  If joystick, enable and disable corresponding buttons.
            case RCCP_Settings.MobileController.Joystick:

                if (steeringWheel && steeringWheel.gameObject.activeSelf)
                    steeringWheel.gameObject.SetActive(false);

                if (joystick && !joystick.gameObject.activeSelf)
                    joystick.gameObject.SetActive(true);

                if (left && left.gameObject.activeSelf)
                    left.gameObject.SetActive(false);

                if (right && right.gameObject.activeSelf)
                    right.gameObject.SetActive(false);

                break;

        }

        //  Inputs.
        if (throttle)
            throttleInput = throttle.input;

        if (left && right)
            steerInput = -left.input + right.input;

        if (steeringWheel)
            steerInput += steeringWheel.input;

        if (joystick)
            steerInput += joystick.inputHorizontal;

        if (brake)
            brakeInput = brake.input;

        if (ebrake)
            ebrakeInput = ebrake.input;

        if (nos)
            nosInput = nos.input;

        throttleInput += nosInput;   //  Increasing throttle input with he nos input. But clamping it to 0 - 1 below.

        if (RCCP_Settings.Instance.mobileController == RCCP_Settings.MobileController.Gyro) {

#if BCG_NEWINPUTSYSTEM
            if (UnityEngine.InputSystem.Accelerometer.current != null)
                steerInput += UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue().x * RCCP_Settings.Instance.gyroSensitivity;
#else
            steerInput += Input.acceleration.x * RCCP_Settings.Instance.gyroSensitivity;
#endif

        }

        throttleInput = Mathf.Clamp01(throttleInput);
        steerInput = Mathf.Clamp(steerInput, -1f, 1f);
        brakeInput = Mathf.Clamp01(brakeInput);
        ebrakeInput = Mathf.Clamp01(ebrakeInput);
        nosInput = Mathf.Clamp01(nosInput);

    }

}
