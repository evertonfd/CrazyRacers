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
/// Main light manager of the RCCP_Light. All lights must be connected to this manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Lights")]
public class RCCP_Lights : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public List<RCCP_Light> lights = new List<RCCP_Light>();        //  All ligths attached to the vehicle.

    //  Bools.
    public bool lowBeamHeadlights = false;
    public bool highBeamHeadlights = false;
    public bool brakeLights = false;
    public bool reverseLights = false;
    public bool indicatorsLeft = false;
    public bool indicatorsRight = false;
    public bool indicatorsAll = false;
    public float indicatorTimer = 0f;                           // Used timer for indicator on / off sequence.

    public bool tailLightFound = false;
    public bool highBeamLightFound = false;

    private void OnEnable() {

        if (CarController)
            CarController.Lights = this;
        else
            enabled = false;

    }

    private void Update() {

        CheckLights();
        Inputs();
        IndicatorTimer();

    }

    private void CheckLights() {

        if (lights != null) {

            for (int i = 0; i < lights.Count; i++) {

                if (lights[i] == null)
                    lights.RemoveAt(i);

            }

        }

    }

    /// <summary>
    /// Registers the target light.
    /// </summary>
    /// <param name="newLight"></param>
    public void RegisterLight(RCCP_Light newLight) {

        //  If list doesn't contain the target light, add it.
        if (!lights.Contains(newLight))
            lights.Add(newLight);

        //  If it's a taillight.
        if (newLight.lightType == RCCP_Light.LightType.Taillight)
            tailLightFound = true;

        //  If it's a high beam light.
        if (newLight.lightType == RCCP_Light.LightType.Headlight_HighBeam)
            highBeamLightFound = true;

    }

    /// <summary>
    /// Vehicle inputs.
    /// </summary>
    private void Inputs() {

        //  If vehicle is braking now.
        if (CarController.brakeInput_V >= .1f)
            brakeLights = true;
        else
            brakeLights = false;

        //  If vehicle is reversing now.
        if (CarController.reversingNow)
            reverseLights = true;
        else
            reverseLights = false;

    }

    /// <summary>
    /// Indicator timer.
    /// </summary>
    private void IndicatorTimer() {

        //  If indicators in use now, increase the timer. Otherwise set it to 0.
        if (indicatorsLeft || indicatorsRight || indicatorsAll)
            indicatorTimer += Time.deltaTime;
        else
            indicatorTimer = 0f;

        //  If indicator timer is 1, set it to 0.
        if (indicatorTimer >= 1f)
            indicatorTimer = 0f;

    }

}
