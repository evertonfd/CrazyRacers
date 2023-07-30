//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2017 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRPMFromCarRCC_V3 : MonoBehaviour
{

    private RCCP_CarController rccV3;
    private float gasPedalSensity = 0.01f; // sets the sensity of detecting gas pedal pressing
    private RealisticEngineSound res;
    private AudioClip noneClip;
    // rcc stock engine sounds
    private GameObject car;
    private AudioSource rccEngineHigh;
    private AudioSource rccEngineHighOff;
    private AudioSource rccEngineIdle;
    private AudioSource rccEngineReverse;

    private void Start()
    {
        car = gameObject.GetFirstParentWithComponent<RCCP_CarController>();
        rccV3 = car.GetComponent<RCCP_CarController>();
        res = gameObject.GetComponent<RealisticEngineSound>(); // GameObject with Realistic Engine Sound script
        res.maxRPMLimit = rccV3.maxEngineRPM; // set Realistic Engine Sound's maximum RPM to Realistic Car Controller's maximum RPM
        res.carMaxSpeed = rccV3.maximumSpeed; // needed for straight cut gearbox script
    }
    void Update()
    {
        if (rccV3 != null)
        {
            res.engineCurrentRPM = rccV3.engineRPM; // set Realistic Engine Sound script's current RPM to Realistic Car Controller's RPM
            res.carCurrentSpeed = rccV3.physicalSpeed; // needed for straight cut gearbox script
            res.isShifting = rccV3.shiftingNow; // needed for shifting sounds script
            if (rccV3.throttleInput_V >= gasPedalSensity) // gas pedal is pressing
            {
                if (rccV3.shiftingNow)
                {
                    res.gasPedalPressing = false;
                }
                else
                {
                    res.gasPedalPressing = true;
                }
            }
            if (rccV3.throttleInput_V < gasPedalSensity && rccV3.throttleInput_V > -gasPedalSensity) // gas pedal is not pressing
            {
                res.gasPedalPressing = false;
            }
            if (rccV3.direction == -1) // RCC car is in reverse gear, play reversing sound
            {
                if (rccV3.throttleInput_V <= -gasPedalSensity) // gas pedal is pressing
                {
                    res.gasPedalPressing = true;
                }
                if (rccV3.shiftingNow)
                {
                    res.gasPedalPressing = false;
                }
                if (res.enableReverseGear)
                    res.isReversing = true;
            }
            else
            {
                res.isReversing = false;
            }
        }
        else
        {
            rccV3 = car.GetComponent<RCCP_CarController>(); // rccV3 is null
        }
    }
}
static class ExtensionHelpers
{
    public static GameObject GetFirstParentWithComponent<T>(this GameObject gameObject)
    {
        GameObject result = null;
        GameObject tempGameObject = gameObject.transform.parent.gameObject;
        while (result == null && tempGameObject != null)
        {
            if (tempGameObject.GetComponent<T>() != null)
            {
                result = tempGameObject;
            }
            else
            {
                tempGameObject = tempGameObject.transform.parent.gameObject;
            }
        }
        return result;
    }
}
