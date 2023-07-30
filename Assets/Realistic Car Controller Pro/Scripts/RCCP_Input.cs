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
/// Input receiver from the RCCP_InputManager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Inputs")]
public class RCCP_Input : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public bool overrideInternalInputs = false;
    public bool overrideExternalInputs = false;

    //  Inputs.
    public RCCP_Inputs inputs = new RCCP_Inputs();

    public float throttleInput = 0f;
    public float steerInput = 0f;
    public float brakeInput = 0f;
    public float handbrakeInput = 0f;
    public float clutchInput = 0f;
    public float nosInput = 0f;

    public AnimationCurve steeringCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(100f, .2f), new Keyframe(200f, .15f));        //  Steering Curve. Reduces maximum steering angle on higher speeds.
    public bool steeringLimiter = true;        //  Steering limiter. Limits the maximum steering angle if vehicle is skidding.
    public bool counterSteering = true;     //  Applies counter steering based on steering wheels sideways friction.
    [Range(0f, 1f)] public float counterSteerFactor = .5f;      //  Counter steering multiplier.
    private float steerInputCounter = 0f;       //  Current steering input.

    public bool autoReverse = true;
    public bool inverseThrottleBrakeOnReverse = true;       //  Inverse throttle / brake inputs on reverse gear. 
    public bool cutThrottleWhenShifting = true;     //  Cuts throttle while shifting.

    private void OnEnable() {

        if (CarController)
            CarController.Inputs = this;
        else
            enabled = false;

        throttleInput = 0f;
        steerInput = 0f;
        brakeInput = 0f;
        handbrakeInput = 0f;
        clutchInput = 0f;
        nosInput = 0f;
        steerInputCounter = 0f;

        //  Listening events for inputs.
        RCCP_InputManager.OnStartEngine += RCCP_InputManager_OnStartEngine;
        RCCP_InputManager.OnStopEngine += RCCP_InputManager_OnStopEngine;
        RCCP_InputManager.OnSteeringHelper += RCCP_InputManager_OnSteeringHelper;
        RCCP_InputManager.OnTractionHelper += RCCP_InputManager_OnTractionHelper;
        RCCP_InputManager.OnAngularDragHelper += RCCP_InputManager_OnAngularDragHelper;
        RCCP_InputManager.OnTurnHelper += RCCP_InputManager_OnTurnHelper;
        RCCP_InputManager.OnABS += RCCP_InputManager_OnABS;
        RCCP_InputManager.OnESP += RCCP_InputManager_OnESP;
        RCCP_InputManager.OnTCS += RCCP_InputManager_OnTCS;
        RCCP_InputManager.OnGearShiftedUp += RCCP_InputManager_OnGearShiftedUp;
        RCCP_InputManager.OnGearShiftedDown += RCCP_InputManager_OnGearShiftedDown;
        RCCP_InputManager.OnPressedLowBeamLights += RCCP_InputManager_OnPressedLowBeamLights;
        RCCP_InputManager.OnPressedHighBeamLights += RCCP_InputManager_OnPressedHighBeamLights;
        RCCP_InputManager.OnPressedLeftIndicatorLights += RCCP_InputManager_OnPressedLeftIndicatorLights;
        RCCP_InputManager.OnPressedRightIndicatorLights += RCCP_InputManager_OnPressedRightIndicatorLights;
        RCCP_InputManager.OnPressedIndicatorLights += RCCP_InputManager_OnPressedIndicatorLights;
        RCCP_InputManager.OnTrailerDetach += RCCP_InputManager_OnTrailerDetach;

        //RCCP_InputManager.OnGearShiftedTo += RCCP_InputManager_OnGearShiftedTo;

    }
     
    private void RCCP_InputManager_OnGearShiftedTo(int gearIndex) {

        if (!CarController.Gearbox)
            return;

        CarController.Gearbox.ShiftToGear(gearIndex);

    }

    private void Update() {

        //  If can control is not enabled, return with 0 inputs except handbrake. Vehicle will stop.
        if (!CarController.canControl) {

            throttleInput = 0f;
            steerInput = 0f;
            brakeInput = 0f;
            handbrakeInput = 1f;
            clutchInput = 0f;
            nosInput = 0f;
            return;

        }

        if (!overrideInternalInputs)
            InternalInputs();

        if (inputs != null) {

            throttleInput = inputs.throttleInput;
            steerInput = inputs.steerInput;
            brakeInput = inputs.brakeInput;
            clutchInput = inputs.clutchInput;
            handbrakeInput = inputs.handbrakeInput;
            nosInput = inputs.nosInput;

            throttleInput = Mathf.Clamp01(throttleInput);
            brakeInput = Mathf.Clamp01(brakeInput);
            steerInput = Mathf.Clamp(steerInput, -1f, 1f);
            clutchInput = Mathf.Clamp01(clutchInput);
            handbrakeInput = Mathf.Clamp01(handbrakeInput);
            nosInput = Mathf.Clamp01(nosInput);

        }

        if (!overrideExternalInputs)
            ExternalInputs();

    }

    /// <summary>
    /// Overrides inputs with given inputs.
    /// </summary>
    /// <param name="overridedInputs"></param>
    public void OverrideInputs(RCCP_Inputs overridedInputs) {

        inputs = overridedInputs;

    }

    /// <summary>
    /// Internal inputs mainly focused on direct inputs.
    /// </summary>
    private void InternalInputs() {

        inputs = RCCP_InputManager.Instance.GetInputs();

    }

    /// <summary>
    /// External inputs mainly focused on processing additional inputs.
    /// </summary>
    private void ExternalInputs() {

        //  If vehicle has a gearbox...
        if (CarController.Gearbox) {

            if (autoReverse) {

                //  If speed of the vehicle is below 1, and brake input is still high, put it to reverse gear.
                if (CarController.speed <= 1f && inputs.brakeInput >= .75f) {

                    if (!CarController.reversingNow)
                        CarController.Gearbox.ShiftReverse();

                } else {

                    //  If speeed of the vehicle is above -1 and still at reverse gear, put it to first gear.
                    if (CarController.speed >= -1 && CarController.reversingNow)
                        CarController.Gearbox.ShiftToGear(0);

                }

            }

        }

        //  Cuts throttle input when shifting.
        if (cutThrottleWhenShifting && CarController.shiftingNow)
            throttleInput = 0;

        //  Inverse throttle and brake inputs on reverse gear.
        if (inverseThrottleBrakeOnReverse && CarController.reversingNow) {

            throttleInput = inputs.brakeInput;
            brakeInput = inputs.throttleInput;

        }

        //  If counter steering is enabled, get sideways slip of the steering wheels and apply it as steer input counter. 
        if (counterSteering) {

            float sidewaysSlip = 0f;

            if (CarController.FrontAxle)
                sidewaysSlip = (CarController.FrontAxle.leftWheelCollider.wheelSlipAmountSideways + CarController.FrontAxle.rightWheelCollider.wheelSlipAmountSideways) / 2f;

            steerInputCounter = (sidewaysSlip * counterSteerFactor);
            steerInputCounter = Mathf.Clamp(steerInputCounter, -1f, 1f);

            steerInput += steerInputCounter * (1f - Mathf.Abs(steerInput));

        }

        // Steering limiter. Limits the maximum steering angle if vehicle is skidding.
        if (steeringLimiter) {

            //  If speed of the vehicle is below 5, return.
            if (Mathf.Abs(CarController.speed) < 5f)
                return;

            float sidewaysSlip = 0f;        //	Total sideways slip of all wheels.

            //  Getting all sideways slips average.
            foreach (RCCP_WheelCollider w in CarController.AllWheelColliders)
                sidewaysSlip += w.wheelSlipAmountSideways;

            sidewaysSlip /= CarController.AllWheelColliders.Length;

            float maxSteerInput = Mathf.Clamp(1f - Mathf.Abs(sidewaysSlip), -1f, 1f);      //	Subtract total average sideways slip from max steer input (1f).;
            float sign = -Mathf.Sign(sidewaysSlip);      //	Is sideways slip is left or right?

            //	If slip is high enough, apply counter input.
            if (maxSteerInput > 0f)
                steerInput = Mathf.Clamp(steerInput, -maxSteerInput, maxSteerInput);
            else
                steerInput = Mathf.Clamp(steerInput, sign * maxSteerInput, sign * maxSteerInput);

        }

        //  Steering curve based on speed. Reduces the maximum steering angle on higher speeds.
        if (steeringCurve != null)
            steerInput *= steeringCurve.Evaluate(Mathf.Abs(CarController.speed));

    }

    /// <summary>
    /// When pressed indicator all lights button.
    /// </summary>
    private void RCCP_InputManager_OnPressedIndicatorLights() {

        //  If no lights component found, return.
        if (!CarController.Lights)
            return;

        //  Setting lights.
        CarController.Lights.indicatorsAll = !CarController.Lights.indicatorsAll;
        CarController.Lights.indicatorsLeft = false;
        CarController.Lights.indicatorsRight = false;

        //  Informing.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched All Indicators To " + CarController.Lights.indicatorsAll);

    }

    /// <summary>
    /// When pressed indicators right button.
    /// </summary>
    private void RCCP_InputManager_OnPressedRightIndicatorLights() {

        //  If no lights found, return.
        if (!CarController.Lights)
            return;

        //  Setting lights.
        CarController.Lights.indicatorsRight = !CarController.Lights.indicatorsRight;
        CarController.Lights.indicatorsLeft = false;
        CarController.Lights.indicatorsAll = false;

        //  Informing.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Right Indicators To " + CarController.Lights.indicatorsRight);

    }

    /// <summary>
    /// When pressed indicators left button.
    /// </summary>
    private void RCCP_InputManager_OnPressedLeftIndicatorLights() {

        //  If no lights found, return.
        if (!CarController.Lights)
            return;

        //  Setting lights.
        CarController.Lights.indicatorsLeft = !CarController.Lights.indicatorsLeft;
        CarController.Lights.indicatorsRight = false;
        CarController.Lights.indicatorsAll = false;

        //  Informing.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Left Indicators To " + CarController.Lights.indicatorsLeft);

    }

    /// <summary>
    /// When pressed high beam lights button.
    /// </summary>
    private void RCCP_InputManager_OnPressedHighBeamLights() {

        //  If no lights found, return.
        if (!CarController.Lights)
            return;

        //  Setting lights.
        CarController.Lights.highBeamHeadlights = !CarController.Lights.highBeamHeadlights;

        //  Informing.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched High Beam Lights To " + CarController.Lights.highBeamHeadlights);

    }

    /// <summary>
    /// When pressed low beam lights button.
    /// </summary>
    private void RCCP_InputManager_OnPressedLowBeamLights() {

        //  If no lights found, return.
        if (!CarController.Lights)
            return;

        //  Setting lights.
        CarController.Lights.lowBeamHeadlights = !CarController.Lights.lowBeamHeadlights;

        //  Informing.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Low Beam Lights To " + CarController.Lights.lowBeamHeadlights);

    }

    /// <summary>
    /// When pressed steering helper button.
    /// </summary>
    private void RCCP_InputManager_OnSteeringHelper() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.steeringHelper = !CarController.Stability.steeringHelper;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Steering Helper To " + CarController.Stability.steeringHelper);

    }

    /// <summary>
    /// When pressed to traction helper button.
    /// </summary>
    private void RCCP_InputManager_OnTractionHelper() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.tractionHelper = !CarController.Stability.tractionHelper;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Traction Helper To " + CarController.Stability.tractionHelper);

    }

    /// <summary>
    /// When pressed to angular drag helper button.
    /// </summary>
    private void RCCP_InputManager_OnAngularDragHelper() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.angularDragHelper = !CarController.Stability.angularDragHelper;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Angular Drag Helper To " + CarController.Stability.angularDragHelper);

    }

    /// <summary>
    /// When pressed turn helper button.
    /// </summary>
    private void RCCP_InputManager_OnTurnHelper() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.turnHelper = !CarController.Stability.turnHelper;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched Turn Helper To " + CarController.Stability.turnHelper);

    }

    /// <summary>
    /// When pressed abs button.
    /// </summary>
    private void RCCP_InputManager_OnABS() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.ABS = !CarController.Stability.ABS;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched ABS To " + CarController.Stability.ABS);

    }

    /// <summary>
    /// When pressed esp button.
    /// </summary>
    private void RCCP_InputManager_OnESP() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.ESP = !CarController.Stability.ESP;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched ESP To " + CarController.Stability.ESP);

    }

    /// <summary>
    /// When pressed tcs button.
    /// </summary>
    private void RCCP_InputManager_OnTCS() {

        //  If no stability found, return.
        if (!CarController.Stability)
            return;

        //  Setting.
        CarController.Stability.TCS = !CarController.Stability.TCS;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Switched TCS To " + CarController.Stability.TCS);

    }

    /// <summary>
    /// When pressed stop engine button.
    /// </summary>
    private void RCCP_InputManager_OnStopEngine() {

        //  If no car controller found, return.
        if (!CarController.Engine)
            return;

        //  Killing the engine.
        CarController.Engine.StopEngine();

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Stopped Engine");

    }

    /// <summary>
    /// When pressed start engine button.
    /// </summary>
    private void RCCP_InputManager_OnStartEngine() {

        //  If no car controller found, return.
        if (!CarController.Engine)
            return;

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer(!CarController.Engine.engineRunning ? "Starting Engine" : "Killing Engine");

        //  Starting the engine.
        if (!CarController.Engine.engineRunning)
            CarController.Engine.StartEngine();
        else
            CarController.Engine.StopEngine();

    }

    /// <summary>
    /// When pressed gear shift down button.
    /// </summary>
    private void RCCP_InputManager_OnGearShiftedDown() {

        //  If no gearbox found, return.
        if (!CarController.Gearbox)
            return;

        //  Shifting down.
        CarController.Gearbox.ShiftDown();

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Shifted Down");

    }

    /// <summary>
    /// When pressed gear shift down button.
    /// </summary>
    private void RCCP_InputManager_OnGearShiftedUp() {

        //  If no gearbox found, return.
        if (!CarController.Gearbox)
            return;

        //  Shifting up.
        CarController.Gearbox.ShiftUp();

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Shifted Up");

    }

    /// <summary>
    /// When pressed trailer detach button.
    /// </summary>
    private void RCCP_InputManager_OnTrailerDetach() {

        //  If no other addons found, return.
        if (!CarController.OtherAddonsManager)
            return;

        //  If no trailer attacher found, return.
        if (!CarController.OtherAddonsManager.TrailAttacher)
            return;

        //  Detaching the railer.
        CarController.OtherAddonsManager.TrailAttacher.attachedTrailer.DetachTrailer();

        //  Informer.
        if (RCCP_Settings.Instance.useInputDebugger)
            RCCP_Events.Event_OnRCCPUIInformer("Trailer Detached");

    }

    private void OnDisable() {

        RCCP_InputManager.OnStartEngine -= RCCP_InputManager_OnStartEngine;
        RCCP_InputManager.OnStopEngine -= RCCP_InputManager_OnStopEngine;
        RCCP_InputManager.OnSteeringHelper -= RCCP_InputManager_OnSteeringHelper;
        RCCP_InputManager.OnTractionHelper -= RCCP_InputManager_OnTractionHelper;
        RCCP_InputManager.OnAngularDragHelper -= RCCP_InputManager_OnAngularDragHelper;
        RCCP_InputManager.OnTurnHelper -= RCCP_InputManager_OnTurnHelper;
        RCCP_InputManager.OnABS -= RCCP_InputManager_OnABS;
        RCCP_InputManager.OnESP -= RCCP_InputManager_OnESP;
        RCCP_InputManager.OnTCS -= RCCP_InputManager_OnTCS;
        RCCP_InputManager.OnGearShiftedUp -= RCCP_InputManager_OnGearShiftedUp;
        RCCP_InputManager.OnGearShiftedDown -= RCCP_InputManager_OnGearShiftedDown;
        RCCP_InputManager.OnPressedLowBeamLights -= RCCP_InputManager_OnPressedLowBeamLights;
        RCCP_InputManager.OnPressedHighBeamLights -= RCCP_InputManager_OnPressedHighBeamLights;
        RCCP_InputManager.OnPressedLeftIndicatorLights -= RCCP_InputManager_OnPressedLeftIndicatorLights;
        RCCP_InputManager.OnPressedRightIndicatorLights -= RCCP_InputManager_OnPressedRightIndicatorLights;
        RCCP_InputManager.OnPressedIndicatorLights -= RCCP_InputManager_OnPressedIndicatorLights;
        RCCP_InputManager.OnTrailerDetach -= RCCP_InputManager_OnTrailerDetach;

    }

    /// <summary>
    /// Resets all inputs to 0.
    /// </summary>
    public void ResetInputs() {

        throttleInput = 0f;
        steerInput = 0f;
        brakeInput = 0f;
        handbrakeInput = 0f;
        clutchInput = 0f;
        nosInput = 0f;

    }

    private void Reset() {

        Keyframe[] ks = new Keyframe[3];

        ks[0] = new Keyframe(0f, 1f);
        ks[0].outTangent = -.0135f;    // -5 units on the y axis for 1 unit on the x axis.

        ks[1] = new Keyframe(100f, .2f);
        ks[1].inTangent = -.0015f;    // straight
        ks[1].outTangent = -.001f;    // straight

        ks[2] = new Keyframe(200f, .15f);
        //ks[2].inTangent = -5f;    // +5 units on the y axis for 1 unit on the x axis.

        steeringCurve = new AnimationCurve(ks);

    }

}
