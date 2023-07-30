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
using UnityEngine.Events;

/// <summary>
/// Main power generator of the vehicle. Produces and transmits the generated power to the clutch.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Engine")]
public class RCCP_Engine : MonoBehaviour {

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

    public bool overrideEngineRPM = false;      //  Overrides engine rpm with given rpm value. All calculations will be ignored.

    public bool engineRunning = true;       //  Is engine running now?
    public bool engineStarting = false;     //  Is engine starting now?

    public float engineRPM = 0f;        //  Current engine rpm.
    public float minEngineRPM = 750f;       //  Minimum engine rpm.
    public float maxEngineRPM = 7000f;      //  Maximum engine rpm.
    private float wantedEngineRPMRaw = 0f;      //  Wanted actual engine rpm. This value will be smoothed.
    private float engineVelocity;       //  Engine velocity.

    public AnimationCurve NMCurve = new AnimationCurve(new Keyframe(750f, .8f), new Keyframe(4500f, 1f), new Keyframe(7000f, .85f));      //  Engine torque NM curve based on rpm.
    public bool autoCreateNMCurve = true;       //  Auto creates the NM curve based on minimum engine rpm, max torque at rpm, and maximum engine rpm.
    public float maxTorqueAtRPM = 4500f;        //  Target peak rpm.

    public bool engineRevLimiter = true;        //  Engine rev limiter. Cuts the fuel when engine rpm exceeds the maximum rpm.
    private bool cutFuel = false;       //  Cutting fuel right now?

    public bool turboCharged = false;       //  Engine supercharger or turbo.
    public float turboChargePsi = 0f;       //  Current supercharger psi.
    private float turboChargePsi_Old = 0f;      //  Previous supercharger psi.
    public float maxTurboChargePsi = 12f;       //  Maximum supercharger psi.
    public float turboChargerCoEfficient = 1.75f;       //  Maximum supercharger co efficient (torque multiplier).
    [HideInInspector] public bool turboBlowOut = false;        //  Turbo blow out will be used on sfx.

    private float multiplier = 1f;      //  Engine torque multiplier. Used on nos.
    [Range(0f, 1f)] public float engineFriction = .2f;      //  Engine friction. Engine will drop rpm on higher values.
    [Range(.01f, .5f)] public float engineInertia = .15f;       //  Engine inertia. Engine will rev up / down faster on lower values.

    public float fuelInput = 0f;        //  Fuel input.
    public float idleInput = 0f;        //  Idle fuel input.

    public float maximumTorqueAsNM = 300;       //  Maximum producable torque.
    public float producedTorqueAsNM = 0f;       //  Produced torque.

    //  Output with custom class.
    public RCCP_Event_Output outputEvent = new RCCP_Event_Output();
    public RCCP_Output output = new RCCP_Output();

    private void OnEnable() {

        if (CarController)
            CarController.Engine = this;
        else
            enabled = false;

        //  Make sure engine is not starting and not cutting fuel when enabled.
        engineStarting = false;
        cutFuel = false;
        fuelInput = 0f;
        idleInput = 0f;
        turboChargePsi = 0f;
        turboChargePsi_Old = 0f;
        producedTorqueAsNM = 0f;

        //  If engine is running as default, make sure engine rpm is minimum engine rpm.
        if (engineRunning) {

            engineRPM = minEngineRPM;
            wantedEngineRPMRaw = engineRPM;

        } else {

            wantedEngineRPMRaw = 0f;
            engineRPM = 0f;

        }

    }

    private void Update() {

        Inputs();

    }

    private void FixedUpdate() {

        RPM();
        TurboCharger();
        GenerateKW();
        FeedbackKW();
        Output();

    }

    /// <summary>
    /// Starts the engine.
    /// </summary>
    public void StartEngine() {

        //  If engine is running or starting right now, return.
        if (engineRunning || engineStarting)
            return;

        StartCoroutine(StartEngineDelayed());

    }

    /// <summary>
    /// Kills the engine.
    /// </summary>
    public void StopEngine() {

        engineRunning = false;

    }

    /// <summary>
    /// Starting the engine with timer delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartEngineDelayed() {

        engineRunning = false;
        engineStarting = true;
        yield return new WaitForSeconds(1);
        engineStarting = false;
        engineRunning = true;

    }

    /// <summary>
    /// Calculating the idle input and receiving throttle input from the player.
    /// </summary>
    private void Inputs() {

        if (overrideEngineRPM)
            return;

        //  If engine rpm is below the minimum, raise the idle input.
        if (engineRPM <= (minEngineRPM + (minEngineRPM / 5f)))
            idleInput = Mathf.Clamp01(Mathf.Lerp(1f, 0f, engineRPM / (minEngineRPM + (minEngineRPM / 5f))));
        else
            idleInput = 0f;

        //  Setting fuel input.
        fuelInput = CarController.throttleInput_P + idleInput;

        //  Clamping fuel input.
        fuelInput = Mathf.Clamp01(CarController.throttleInput_P + idleInput);

        //  If engine rpm exceeds the maximum rpm, cut the fuel.
        if (engineRPM >= maxEngineRPM)
            fuelInput = 0f;

        //  If cut fuel, set fuel to 0.
        if (cutFuel)
            fuelInput = 0f;

        //  If engine is not running, set fuel and idle input to 0.
        if (!engineRunning) {

            fuelInput = 0f;
            idleInput = 0f;

        }

    }

    /// <summary>
    /// Calculating the rpm.
    /// </summary>
    private void RPM() {

        if (overrideEngineRPM)
            return;

        wantedEngineRPMRaw += Mathf.Clamp01(CarController.clutchInput_V + (1f - CarController.gearInput_V)) * (fuelInput * maxEngineRPM) * Time.fixedDeltaTime;
        wantedEngineRPMRaw += (1f - CarController.clutchInput_V) * CarController.gearInput_V * (CarController.tractionWheelRPM2EngineRPM - wantedEngineRPMRaw) * Time.fixedDeltaTime * 5f;
        wantedEngineRPMRaw -= engineFriction * maxEngineRPM * Time.fixedDeltaTime * 1f;
        wantedEngineRPMRaw = Mathf.Clamp(wantedEngineRPMRaw, 0f, maxEngineRPM);

        //  Smoothing the engine rpm.
        engineRPM = Mathf.SmoothDamp(engineRPM, wantedEngineRPMRaw, ref engineVelocity, engineInertia);

        if (engineRevLimiter) {

            if (engineRPM >= maxEngineRPM * .975f)
                cutFuel = true;
            else if (engineRPM < maxEngineRPM * .975f)
                cutFuel = false;

        }

    }

    /// <summary>
    /// Turbocharges the engine based on engine rpm, fuel input and exhaust input.
    /// </summary>
    private void TurboCharger() {

        //  If engine is not running or supercharge is disabled, return with 0 psi.
        if (!engineRunning || !turboCharged) {

            turboChargePsi = 0f;
            return;

        }

        //  Supercharger will produce more psi on higher rpms above 2500 - 3000.
        float factor = Mathf.Clamp01(Mathf.Lerp(-1f, 1f, engineRPM / maxEngineRPM));

        //  Calculating the supercharger psi.
        turboChargePsi = Mathf.Lerp(0f, maxTurboChargePsi, fuelInput * factor);

        if (fuelInput == 0 && turboChargePsi < turboChargePsi_Old)
            turboBlowOut = true;
        else
            turboBlowOut = false;

        turboChargePsi_Old = turboChargePsi;

    }

    /// <summary>
    /// Overrides the engine rpm with given value.
    /// </summary>
    /// <param name="targetRPM"></param>
    public void OverrideRPM(float targetRPM) {

        engineRPM = targetRPM;

    }

    /// <summary>
    /// Generates and produces the torque.
    /// </summary>
    private void GenerateKW() {

        //  Producing the torque with curve.
        producedTorqueAsNM = NMCurve.Evaluate(engineRPM) * maximumTorqueAsNM * fuelInput;
        producedTorqueAsNM *= multiplier;

        if (turboCharged)
            producedTorqueAsNM *= Mathf.Lerp(1f, turboChargerCoEfficient, turboChargePsi / maxTurboChargePsi);

        multiplier = 1f;

    }

    /// <summary>
    /// Feedback torque received from the wheels.
    /// </summary>
    public void FeedbackKW() {

        producedTorqueAsNM *= Mathf.Lerp(1f, 0f, (CarController.tractionWheelRPM2EngineRPM - engineRPM) / (maxEngineRPM / 2f));

    }

    /// <summary>
    /// Multiplies the produced engine torque.
    /// </summary>
    /// <param name="multiplier"></param>
    public void Multiply(float multiplier) {

        this.multiplier = multiplier;

    }

    /// <summary>
    /// Output.
    /// </summary>
    private void Output() {

        if (output == null)
            output = new RCCP_Output();

        output.NM = producedTorqueAsNM;
        outputEvent.Invoke(output);

    }

    private void Reset() {

        

    }

}
