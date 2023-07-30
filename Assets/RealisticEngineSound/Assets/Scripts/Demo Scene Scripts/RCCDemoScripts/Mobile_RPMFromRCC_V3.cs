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

public class Mobile_RPMFromRCC_V3 : MonoBehaviour {

    private RCCP_CarController rccV3;
    private RealisticEngineSound_mobile res_mob;
    private AudioClip noneClip;
    // rcc stock engine sounds
    private GameObject car;
    private GameObject rccEngineHigh;
    private GameObject rccEngineHighOff;
    private GameObject rccEngineIdle;
    private GameObject rccEngineReverse;
    //
    private void Start()
    {
        car = gameObject.GetFirstParentWithComponent<RCCP_CarController>();
        rccV3 = car.GetComponent<RCCP_CarController>();
        res_mob = gameObject.GetComponent<RealisticEngineSound_mobile>(); // GameObject with Realistic Engine Sound script
        res_mob.maxRPMLimit = rccV3.maxEngineRPM; // set Realistic Engine Sound's maximum RPM to Realistic Car Controller's maximum RPM
        // engine on
        rccEngineHigh = car.transform.Find("All Audio Sources/Engine Sound High AudioSource").gameObject;
        rccEngineHigh.GetComponent<AudioSource>().clip = noneClip;
        // engine off
        rccEngineHighOff = car.transform.Find("All Audio Sources/Engine Sound High Off AudioSource").gameObject;
        rccEngineHighOff.GetComponent<AudioSource>().clip = noneClip;
        // engine idle
        rccEngineIdle = car.transform.Find("All Audio Sources/Engine Sound Idle AudioSource").gameObject;
        rccEngineIdle.GetComponent<AudioSource>().clip = noneClip;
        // engine reverse
        rccEngineReverse = car.transform.Find("All Audio Sources/Reverse Sound AudioSource").gameObject;
        rccEngineReverse.GetComponent<AudioSource>().clip = noneClip;
        res_mob.carMaxSpeed = rccV3.maximumSpeed; // needed for straight cut gearbox script
    }
    void Update()
    {
        if (rccV3 != null)
        {
            res_mob.engineCurrentRPM = rccV3.engineRPM; // set Realistic Engine Sound script's current RPM to Realistic Car Controller's RPM
            res_mob.carCurrentSpeed = rccV3.physicalSpeed; // needed for straight cut gearbox script
            res_mob.isShifting = rccV3.shiftingNow; // needed for shifting sounds script
            // reverse gear sound controler
            if (res_mob.enableReverseGear)
            {
                if (rccV3.direction == -1) // RCC car is in reverse gear, play reversing sound
                {
                    res_mob.isReversing = true;
                }
                else // car is not in reverse gear
                {
                    res_mob.isReversing = false;
                }
            }
            // gas pedal
            if (rccV3.throttleInput_V >= 0.1f) // gas pedal is pressing
            {
                if (rccV3.shiftingNow)
                {
                    res_mob.gasPedalPressing = false;
                }
                else
                {
                    res_mob.gasPedalPressing = true;
                }
            }
            if (rccV3.throttleInput_V < 0.1f && rccV3.throttleInput_V > -0.1f) // gas pedal is not pressing
            {
                res_mob.gasPedalPressing = false;
            }
        }
        else
        {
            rccV3 = car.GetComponent<RCCP_CarController>(); // rccV3 is null
        }
    }
}
