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
/// Multiplies the received power from the engine --> clutch by x ratio, and transmits it to the differential. Higher ratios = faster accelerations, lower top speeds, lower ratios = slower accelerations, higher top speeds.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Gearbox")]
public class RCCP_Gearbox : MonoBehaviour {

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

    public bool overrideGear = false;

    public float[] gearRatios = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };      //  Gear ratios. Faster accelerations on higher values, but lower top speeds.

    public float[] GearRPMs {

        get {

            gearRPMs = new float[gearRatios.Length];

            for (int i = 0; i < gearRPMs.Length; i++) {

                gearRPMs[i] = CarController.Engine.maxEngineRPM / gearRatios[i];

            }

            return gearRPMs;

        }

    }

    public float[] gearRPMs;

    public int currentGear = 0;     //  Current gear.
    public float gearInput = 0f;        //  0 means N, 1 means any gear is in use now.
    public bool reverseGearEngaged = false;     //  Reverse gear engaged now?
    public bool neutralGearEngaged = false;     //  Neutral gear engaged now?
    public float[] targetSpeeds;
    public float shiftingTime = .2f;        //  Shifting time.
    public bool shiftingNow = false;        //  Shifting now?

    public bool dontShiftTimer = true;      //  Don't shift timer if too close to previous one.
    public float lastTimeShifted = 0f;      //  Timer for don't shift.

    public bool automaticTransmission = true;       //  Automatic transmission.
    [Range(.1f, .9f)] public float shiftThreshold = .8f;     //  Automatic transmission will shift up late on higher values.
    public float shiftUpRPM = 5500f;        //  Target engine rpm to shift up.
    public float shiftDownRPM = 2750f;      //  Target engine rpm to shift down.

    public float receivedTorqueAsNM = 0f;       //  Received torque from the component. It should be the clutch in this case.
    public float producedTorqueAsNM = 0f;       //  Produced and delivered torque to the component. It should be the differential in this case.

    public RCCP_Event_Output outputEvent = new RCCP_Event_Output();     //  Output with custom class.
    private RCCP_Output output = new RCCP_Output();

    private void OnEnable() {

        if (CarController)
            CarController.Gearbox = this;
        else
            enabled = false;

        //  Make sure shifting now, and neutral gear engaged is set to false when enabling the vehicle.
        shiftingNow = false;
        neutralGearEngaged = false;
        lastTimeShifted = 0f;
        currentGear = 0;
        gearInput = 0;
        producedTorqueAsNM = 0f;

    }

    private void Update() {

        //  Assign neutral gear while shifting.
        //if (shiftingNow)
        //    neutralGearEngaged = true;
        //else
        //    neutralGearEngaged = false;

        //  Setting timer for last shifting.
        if (lastTimeShifted > 0)
            lastTimeShifted -= Time.deltaTime;

        //  Clamping timer.
        lastTimeShifted = Mathf.Clamp(lastTimeShifted, 0f, 10f);

        //  If gear is not neutral, set gear input to 1. Otherwise to 0.
        if (!neutralGearEngaged)
            gearInput = 1f;
        else
            gearInput = 0f;

    }

    private void FixedUpdate() {

        //  Early out if no car controller found.
        if (!CarController)
            return;

        if (automaticTransmission)
            AutomaticTransmission();

        Output();

    }

    /// <summary>
    /// Calculates estimated speeds and rpms to shift up / down.
    /// </summary>
    private void AutomaticTransmission() {

        if (overrideGear)
            return;

        //  Getting engine rpm.
        float engineRPM = CarController.engineRPM;

        //  Creating float array for target speeds.
        float[] targetSpeeds = FindTargetSpeed();

        //  Creating low and high limits multiplied with threshold value.
        float lowLimit = 0;
        float highLimit = 0f;

        //  If current gear is not first gear, there is a low limit.
        if (currentGear > 0)
            lowLimit = targetSpeeds[currentGear - 1];

        //  High limit.
        highLimit = targetSpeeds[currentGear];

        bool canShiftUpNow = false;

        //  If reverse gear is not engaged, engine rpm is above shiftup rpm, and wheel & vehicle speed is above the high limit, shift up.
        if (currentGear < gearRatios.Length && !reverseGearEngaged && engineRPM >= shiftUpRPM && CarController.wheelRPM2Speed >= highLimit && CarController.speed >= highLimit)
            canShiftUpNow = true;

        bool canShiftDownNow = false;

        //  If reverse gear is not engaged, engine rpm is below shiftdown rpm, and wheel & vehicle speed is below the low limit, shift down.
        if (currentGear > 0 && !reverseGearEngaged && engineRPM <= shiftDownRPM) {

            if (FindEligibleGear() != currentGear)
                canShiftDownNow = true;
            else
                canShiftDownNow = false;

        }

        if (!dontShiftTimer)
            lastTimeShifted = 0f;

        if (!shiftingNow && lastTimeShifted <= .02f) {

            if (canShiftDownNow)
                ShiftToGear(FindEligibleGear());

            if (canShiftUpNow)
                ShiftUp();

        }

    }

    /// <summary>
    /// Received torque from the component.
    /// </summary>
    /// <param name="output"></param>
    public void ReceiveOutput(RCCP_Output output) {

        receivedTorqueAsNM = output.NM;

    }

    /// <summary>
    /// Finds eligible gear depends on the speed.
    /// </summary>
    /// <returns></returns>
    private float[] FindTargetSpeed() {

        //  Creating float array for target speeds.
        targetSpeeds = new float[gearRatios.Length];

        float partition = CarController.maximumSpeed / gearRatios.Length;

        //  Assigning target speeds.
        for (int i = targetSpeeds.Length - 1; i >= 0; i--)
            targetSpeeds[i] = partition * (i + 1) * shiftThreshold;

        return targetSpeeds;

    }

    /// <summary>
    /// Finds eligible gear depends on the speed.
    /// </summary>
    /// <returns></returns>
    private int FindEligibleGear() {

        float[] targetSpeeds = FindTargetSpeed();
        int eligibleGear = 0;

        for (int i = 0; i < targetSpeeds.Length; i++) {

            if (CarController.speed < targetSpeeds[i]) {

                eligibleGear = i;
                break;

            }

        }

        return eligibleGear;

    }

    /// <summary>
    /// Shift up.
    /// </summary>
    public void ShiftUp() {

        reverseGearEngaged = false;
        neutralGearEngaged = false;

        if (currentGear < gearRatios.Length - 1)
            StartCoroutine(ShiftTo(currentGear + 1));

    }

    /// <summary>
    /// Shift down.
    /// </summary>
    public void ShiftDown() {

        reverseGearEngaged = false;
        neutralGearEngaged = false;

        if (currentGear > 0)
            StartCoroutine(ShiftTo(currentGear - 1));

    }

    /// <summary>
    /// Shift reverse.
    /// </summary>
    public void ShiftReverse() {

        reverseGearEngaged = true;
        neutralGearEngaged = false;
        currentGear = 0;
        StartCoroutine(ShiftTo(-1));

    }

    /// <summary>
    /// Shift to specific gear.
    /// </summary>
    /// <param name="gear"></param>
    public void ShiftToGear(int gear) {

        reverseGearEngaged = false;
        neutralGearEngaged = false;
        StartCoroutine(ShiftTo(gear));

    }

    /// <summary>
    /// Shift to specific gear.
    /// </summary>
    /// <param name="gear"></param>
    public void ShiftToN() {

        currentGear = 0;
        reverseGearEngaged = false;
        neutralGearEngaged = true;

    }

    /// <summary>
    /// Shift to specific gear with delay.
    /// </summary>
    /// <param name="gear"></param>
    /// <returns></returns>
    private IEnumerator ShiftTo(int gear) {

        lastTimeShifted = .75f;
        shiftingNow = true;
        neutralGearEngaged = true;

        yield return new WaitForSeconds(shiftingTime);

        if (gear == -1)
            reverseGearEngaged = true;
        else
            reverseGearEngaged = false;

        if (gear == -2)
            neutralGearEngaged = true;
        else
            neutralGearEngaged = false;

        gear = Mathf.Clamp(gear, 0, gearRatios.Length - 1);
        currentGear = gear;
        shiftingNow = false;

    }

    /// <summary>
    /// Output.
    /// </summary>
    private void Output() {

        if (output == null)
            output = new RCCP_Output();

        producedTorqueAsNM = receivedTorqueAsNM * gearRatios[currentGear] * gearInput;

        if (reverseGearEngaged)
            producedTorqueAsNM *= -1;

        if (neutralGearEngaged)
            producedTorqueAsNM = 0f;

        output.NM = producedTorqueAsNM / outputEvent.GetPersistentEventCount();
        outputEvent.Invoke(output);

    }

    /// <summary>
    /// Inits the gears.
    /// </summary>
    public void InitGears(int totalGears) {

        //  Creating float array.
        gearRatios = new float[totalGears];

        //  Creating other arrays for specific.
        float[] gearRatio = new float[gearRatios.Length];
        int[] maxSpeedForGear = new int[gearRatios.Length];
        int[] targetSpeedForGear = new int[gearRatios.Length];

        //  Assigning array with preset values.
        if (gearRatios.Length == 1)
            gearRatio = new float[] { 1.0f };

        if (gearRatios.Length == 2)
            gearRatio = new float[] { 2.0f, 1.0f };

        if (gearRatios.Length == 3)
            gearRatio = new float[] { 2.0f, 1.5f, 1.0f };

        if (gearRatios.Length == 4)
            gearRatio = new float[] { 2.86f, 1.62f, 1.0f, .72f };

        if (gearRatios.Length == 5)
            gearRatio = new float[] { 4.23f, 2.52f, 1.66f, 1.22f, 1.0f, };

        if (gearRatios.Length == 6)
            gearRatio = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };

        if (gearRatios.Length == 7)
            gearRatio = new float[] { 4.5f, 2.5f, 1.66f, 1.23f, 1.0f, .9f, .8f };

        if (gearRatios.Length == 8)
            gearRatio = new float[] { 4.6f, 2.5f, 1.86f, 1.43f, 1.23f, 1.05f, .9f, .72f };

        gearRatios = gearRatio;
        
    }

    public void OverrideGear(int targetGear, bool targetReverseGear) {

        currentGear = targetGear;
        reverseGearEngaged = targetReverseGear;

    }

}
