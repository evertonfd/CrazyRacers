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
#if BCG_NEWINPUTSYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Main input manager of the RCCP. Receives inputs from the corresponding device and let the other components use them.
/// </summary>
public class RCCP_InputManager : RCCP_Singleton<RCCP_InputManager> {

    public RCCP_Inputs inputs = new RCCP_Inputs();
    private bool overrideInputs = false;

#if BCG_NEWINPUTSYSTEM
    private static RCCP_InputActions newInputSystemActions;
#endif

    public delegate void onGearShiftedUp();
    public static event onGearShiftedUp OnGearShiftedUp;

    public delegate void onGearShiftedDown();
    public static event onGearShiftedDown OnGearShiftedDown;

    public delegate void onGearShiftedTo(int gearIndex);
    public static event onGearShiftedTo OnGearShiftedTo;

    public delegate void onChangedCamera();
    public static event onChangedCamera OnChangedCamera;

    public delegate void onLookBackCamera(bool state);
    public static event onLookBackCamera OnLookBackCamera;

    public delegate void onPressedLowBeamLights();
    public static event onPressedLowBeamLights OnPressedLowBeamLights;

    public delegate void onPressedHighBeamLights();
    public static event onPressedHighBeamLights OnPressedHighBeamLights;

    public delegate void onPressedLeftIndicatorLights();
    public static event onPressedLeftIndicatorLights OnPressedLeftIndicatorLights;

    public delegate void onPressedRightIndicatorLights();
    public static event onPressedRightIndicatorLights OnPressedRightIndicatorLights;

    public delegate void onPressedIndicatorLights();
    public static event onPressedIndicatorLights OnPressedIndicatorLights;

    public delegate void onStartEngine();
    public static event onStartEngine OnStartEngine;

    public delegate void onStopEngine();
    public static event onStopEngine OnStopEngine;

    public delegate void onSteeringHelper();
    public static event onSteeringHelper OnSteeringHelper;

    public delegate void onTractionHelper();
    public static event onTractionHelper OnTractionHelper;

    public delegate void onAngularDragHelper();
    public static event onAngularDragHelper OnAngularDragHelper;

    public delegate void onTurnHelper();
    public static event onTurnHelper OnTurnHelper;

    public delegate void onABS();
    public static event onABS OnABS;

    public delegate void onESP();
    public static event onESP OnESP;

    public delegate void onTCS();
    public static event onTCS OnTCS;

    public delegate void onRecord();
    public static event onRecord OnRecord;

    public delegate void onReplay();
    public static event onReplay OnReplay;

    public delegate void onTrailerDetach();
    public static event onTrailerDetach OnTrailerDetach;

    private void Awake() {

        gameObject.hideFlags = HideFlags.HideInHierarchy;

        //  Creating inputs.
        inputs = new RCCP_Inputs();

    }

    private void Update() {

        if (overrideInputs)
            return;

        //  Creating inputs.
        if (inputs == null)
            inputs = new RCCP_Inputs();

        //  Receive inputs from the controller.
        if (!RCCP_Settings.Instance.mobileControllerEnabled)
            inputs = KeyboardInputs();
        else
            inputs = MobileInputs();

    }

    /// <summary>
    /// Overrides inputs.
    /// </summary>
    /// <returns></returns>
    public void OverrideInputs(RCCP_Inputs overridedInputs) {

        overrideInputs = true;
        inputs = overridedInputs;

    }

    /// <summary>
    /// Returns player inputs.
    /// </summary>
    /// <returns></returns>
    public RCCP_Inputs GetInputs() {

        return inputs;

    }

    /// <summary>
    /// Keyboard inputs with old and new input system.
    /// </summary>
    /// <returns></returns>
    private RCCP_Inputs KeyboardInputs() {

        //  If new input system in use, listen events in the input actions script.
        if (RCCP_Settings.Instance.useNewInputSystem) {

#if BCG_NEWINPUTSYSTEM

            if (newInputSystemActions == null) {

                newInputSystemActions = new RCCP_InputActions();
                newInputSystemActions.Enable();

                newInputSystemActions.Vehicle.StartStopEngine.performed += StartEngine;
                newInputSystemActions.Vehicle.LowBeamLights.performed += LowBeamHeadlights;
                newInputSystemActions.Vehicle.HighBeamLights.performed += HighBeamHeadlights;
                newInputSystemActions.Camera.ChangeCamera.performed += ChangeCamera;
                newInputSystemActions.Vehicle.IndicatorLeft.performed += IndicatorLeftlights;
                newInputSystemActions.Vehicle.IndicatorRight.performed += IndicatorRightlights;
                newInputSystemActions.Vehicle.IndicatorHazard.performed += Indicatorlights;
                newInputSystemActions.Vehicle.GearShiftUp.performed += GearShiftUp;
                newInputSystemActions.Vehicle.GearShiftDown.performed += GearShiftDown;
                //inputActions.Vehicle.NGear.performed += NGear_performed;
                //inputActions.Vehicle.NGear.canceled += NGear_canceled;
                //inputActions.Optional.SlowMotion.performed += SlowMotion_performed;
                //inputActions.Optional.SlowMotion.canceled += SlowMotion_canceled;
                newInputSystemActions.Optional.Record.performed += Record;
                newInputSystemActions.Optional.Replay.performed += Replay;
                newInputSystemActions.Camera.LookBack.performed += LookBackCameraPerformed;
                newInputSystemActions.Camera.LookBack.canceled += LookBackCameraCanceled;
                newInputSystemActions.Vehicle.TrailerDetach.performed += this.TrailDetach;

#if RCCP_LOGITECH
                //	LOGITECH STEERING WHEEL INPUTS
                newInputSystemActions.Vehicle._1stGear.performed += _1stGear_performed;
                newInputSystemActions.Vehicle._2ndGear.performed += _2ndGear_performed;
                newInputSystemActions.Vehicle._3rdGear.performed += _3rdGear_performed;
                newInputSystemActions.Vehicle._4thGear.performed += _4thGear_performed;
                newInputSystemActions.Vehicle._5thGear.performed += _5thGear_performed;
                newInputSystemActions.Vehicle._6thGear.performed += _6thGear_performed;
                newInputSystemActions.Vehicle.RGear.performed += _RGear_performed;

                newInputSystemActions.Vehicle._1stGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle._2ndGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle._3rdGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle._4thGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle._5thGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle._6thGear.canceled += _Gear_canceled;
                newInputSystemActions.Vehicle.RGear.canceled += _Gear_canceled;
#endif

            }

            if (!RCCP_Settings.Instance.mobileControllerEnabled) {

                inputs.throttleInput = newInputSystemActions.Vehicle.Throttle.ReadValue<float>();
                inputs.brakeInput = newInputSystemActions.Vehicle.Brake.ReadValue<float>();
                inputs.steerInput = newInputSystemActions.Vehicle.Steering.ReadValue<float>();
                inputs.handbrakeInput = newInputSystemActions.Vehicle.Handbrake.ReadValue<float>();
                inputs.nosInput = newInputSystemActions.Vehicle.NOS.ReadValue<float>();
                inputs.clutchInput = newInputSystemActions.Vehicle.Clutch.ReadValue<float>();
                inputs.mouseInput = newInputSystemActions.Camera.Orbit.ReadValue<Vector2>();
                //inputs.gearInput = logitechGear;
                //inputs.orbitX = inputActions.Camera.Orbit.ReadValue<Vector2>().x;
                //inputs.orbitY = inputActions.Camera.Orbit.ReadValue<Vector2>().y;
                //inputs.scroll = inputActions.Camera.Zoom.ReadValue<Vector2>();

            } else {

                //inputs.throttleInput = RCC_MobileButtons.mobileInputs.throttleInput;
                //inputs.brakeInput = RCC_MobileButtons.mobileInputs.brakeInput;
                //inputs.steerInput = RCC_MobileButtons.mobileInputs.steerInput;
                //inputs.handbrakeInput = RCC_MobileButtons.mobileInputs.handbrakeInput;
                //inputs.boostInput = RCC_MobileButtons.mobileInputs.boostInput;

            }

#endif

        } else {

            //  Receiving player inputs with the old legacy input system.
            inputs.throttleInput = Mathf.Clamp01(Input.GetAxis("RCCP_Throttle"));
            inputs.brakeInput = Mathf.Clamp01(-Input.GetAxis("RCCP_Brake"));
            inputs.steerInput = Mathf.Clamp(Input.GetAxis("RCCP_Steering"), -1f, 1f);
            inputs.clutchInput = Mathf.Clamp(Input.GetAxis("RCCP_Clutch"), 0f, 1f);
            inputs.handbrakeInput = Mathf.Clamp(Input.GetAxis("RCCP_Handbrake"), 0f, 1f);
            inputs.nosInput = Mathf.Clamp(Input.GetAxis("RCCP_Nos"), 0f, 1f);
            inputs.mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            if (Input.GetKeyDown(KeyCode.I))
                StartEngine();

            if (Input.GetKeyDown(KeyCode.O))
                StopEngine();

            if (Input.GetKeyDown(KeyCode.LeftShift))
                GearShiftUp();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                GearShiftDown();

            if (Input.GetKeyDown(KeyCode.C))
                ChangeCamera();

            if (Input.GetKeyDown(KeyCode.L))
                LowBeamHeadlights();

            if (Input.GetKeyDown(KeyCode.K))
                HighBeamHeadlights();

            if (Input.GetKeyDown(KeyCode.Q))
                IndicatorLeftlights();

            if (Input.GetKeyDown(KeyCode.E))
                IndicatorRightlights();

            if (Input.GetKeyDown(KeyCode.Z))
                Indicatorlights();

            if (Input.GetKey(KeyCode.B))
                LookBackCamera(true);
            else
                LookBackCamera(false);

            if (Input.GetKeyDown(KeyCode.R))
                Record();

            if (Input.GetKeyDown(KeyCode.P))
                Replay();

            if (Input.GetKeyDown(KeyCode.T))
                TrailDetach();

        }

        return inputs;

    }

    //  Receiving mobile player inputs from the RCCP_MobileInputs script attached to the RCCP_Canvas.
    private RCCP_Inputs MobileInputs() {

        //  Getting instance of the mobile controller.
        RCCP_MobileInputs mobileInputs = RCCP_MobileInputs.Instance;

        //  If no mobile inputs found, return.
        if (mobileInputs) {

            //  Receiving inputs.
            inputs.throttleInput = mobileInputs.throttleInput;
            inputs.brakeInput = mobileInputs.brakeInput;
            inputs.steerInput = mobileInputs.steerInput;
            inputs.handbrakeInput = mobileInputs.ebrakeInput;
            inputs.nosInput = mobileInputs.nosInput;

        }

        return inputs;

    }

    public void GearShiftUp() {

        if (OnGearShiftedUp != null)
            OnGearShiftedUp();

    }

    public void GearShiftDown() {

        if (OnGearShiftedDown != null)
            OnGearShiftedDown();

    }

    public void ChangeCamera() {

        if (OnChangedCamera != null)
            OnChangedCamera();

    }

    public void LowBeamHeadlights() {

        if (OnPressedLowBeamLights != null)
            OnPressedLowBeamLights();

    }

    public void HighBeamHeadlights() {

        if (OnPressedHighBeamLights != null)
            OnPressedHighBeamLights();

    }

    public void IndicatorLeftlights() {

        if (OnPressedLeftIndicatorLights != null)
            OnPressedLeftIndicatorLights();

    }

    public void IndicatorRightlights() {

        if (OnPressedRightIndicatorLights != null)
            OnPressedRightIndicatorLights();

    }

    public void Indicatorlights() {

        if (OnPressedIndicatorLights != null)
            OnPressedIndicatorLights();

    }

    public void LookBackCamera(bool state) {

        if (OnLookBackCamera != null)
            OnLookBackCamera(state);

    }

    public void StartEngine() {

        if (OnStartEngine != null)
            OnStartEngine();

    }

    public void StopEngine() {

        if (OnStopEngine != null)
            OnStopEngine();

    }

    public void SteeringHelper() {

        if (OnSteeringHelper != null)
            OnSteeringHelper();

    }

    public void TractionHelper() {

        if (OnTractionHelper != null)
            OnTractionHelper();

    }

    public void AngularDragHelper() {

        if (OnAngularDragHelper != null)
            OnAngularDragHelper();

    }

    public void TurnHelper() {

        if (OnTurnHelper != null)
            OnTurnHelper();

    }

    public void ABS() {

        if (OnABS != null)
            OnABS();

    }

    public void ESP() {

        if (OnESP != null)
            OnESP();

    }

    public void TCS() {

        if (OnTCS != null)
            OnTCS();

    }

    public void Record() {

        if (OnRecord != null)
            OnRecord();

    }

    public void Replay() {

        if (OnReplay != null)
            OnReplay();

    }

    public void TrailDetach() {

        if (OnTrailerDetach != null)
            OnTrailerDetach();

    }

#if BCG_NEWINPUTSYSTEM

    public void GearShiftUp(InputAction.CallbackContext obj) {

        if (OnGearShiftedUp != null)
            OnGearShiftedUp();

    }

    public void GearShiftDown(InputAction.CallbackContext obj) {

        if (OnGearShiftedDown != null)
            OnGearShiftedDown();

    }

    public void ChangeCamera(InputAction.CallbackContext obj) {

        if (OnChangedCamera != null)
            OnChangedCamera();

    }

    public void LowBeamHeadlights(InputAction.CallbackContext obj) {

        if (OnPressedLowBeamLights != null)
            OnPressedLowBeamLights();

    }

    public void HighBeamHeadlights(InputAction.CallbackContext obj) {

        if (OnPressedHighBeamLights != null)
            OnPressedHighBeamLights();

    }

    public void IndicatorLeftlights(InputAction.CallbackContext obj) {

        if (OnPressedLeftIndicatorLights != null)
            OnPressedLeftIndicatorLights();

    }

    public void IndicatorRightlights(InputAction.CallbackContext obj) {

        if (OnPressedRightIndicatorLights != null)
            OnPressedRightIndicatorLights();

    }

    public void Indicatorlights(InputAction.CallbackContext obj) {

        if (OnPressedIndicatorLights != null)
            OnPressedIndicatorLights();

    }

    public void LookBackCameraPerformed(InputAction.CallbackContext obj) {

        if (OnLookBackCamera != null)
            OnLookBackCamera(true);

    }

    public void LookBackCameraCanceled(InputAction.CallbackContext obj) {

        if (OnLookBackCamera != null)
            OnLookBackCamera(false);

    }

    public void StartEngine(InputAction.CallbackContext obj) {

        if (OnStartEngine != null)
            OnStartEngine();

    }

    public void StopEngine(InputAction.CallbackContext obj) {

        if (OnStopEngine != null)
            OnStopEngine();

    }

    public void SteeringHelper(InputAction.CallbackContext obj) {

        if (OnSteeringHelper != null)
            OnSteeringHelper();

    }

    public void TractionHelper(InputAction.CallbackContext obj) {

        if (OnTractionHelper != null)
            OnTractionHelper();

    }

    public void AngularDragHelper(InputAction.CallbackContext obj) {

        if (OnAngularDragHelper != null)
            OnAngularDragHelper();

    }

    public void TurnHelper(InputAction.CallbackContext obj) {

        if (OnTurnHelper != null)
            OnTurnHelper();

    }

    public void ABS(InputAction.CallbackContext obj) {

        if (OnABS != null)
            OnABS();

    }

    public void ESP(InputAction.CallbackContext obj) {

        if (OnESP != null)
            OnESP();

    }

    public void TCS(InputAction.CallbackContext obj) {

        if (OnTCS != null)
            OnTCS();

    }

    public void Record(InputAction.CallbackContext obj) {

        if (OnRecord != null)
            OnRecord();

    }

    public void Replay(InputAction.CallbackContext obj) {

        if (OnReplay != null)
            OnReplay();

    }

    public void TrailDetach(InputAction.CallbackContext obj) {

        if (OnTrailerDetach != null)
            OnTrailerDetach();

    }

#if RCCP_LOGITECH
    //	LOGITECH H SHIFTER INPUTS
    private static void _1stGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(0);

    }

    private static void _Gear_canceled(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(-2);

    }

    private static void _2ndGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(1);

    }

    private static void _3rdGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(2);

    }

    private static void _4thGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(3);

    }

    private static void _5thGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(4);

    }

    private static void _6thGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(5);

    }

    private static void _RGear_performed(InputAction.CallbackContext obj) {

        if (OnGearShiftedTo != null)
            OnGearShiftedTo(-1);

    }

#endif

#endif

}
