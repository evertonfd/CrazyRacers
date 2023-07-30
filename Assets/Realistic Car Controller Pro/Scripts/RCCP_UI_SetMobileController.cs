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
using TMPro;

/// <summary>
/// Sets the target mobile controller.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP UI Set Mobile Controller")]
public class RCCP_UI_SetMobileController : MonoBehaviour {

    private void OnEnable() {

        TMP_Dropdown dropdown = gameObject.GetComponent<TMP_Dropdown>();

        if (dropdown) {

            switch (RCCP_Settings.Instance.mobileController) {

                case RCCP_Settings.MobileController.TouchScreen:
                    dropdown.SetValueWithoutNotify(0);
                    break;

                case RCCP_Settings.MobileController.Gyro:
                    dropdown.SetValueWithoutNotify(1);
                    break;

                case RCCP_Settings.MobileController.SteeringWheel:
                    dropdown.SetValueWithoutNotify(2);
                    break;

                case RCCP_Settings.MobileController.Joystick:
                    dropdown.SetValueWithoutNotify(3);
                    break;

            }

        }

    }

    /// <summary>
    /// Sets the target mobile controller with index. 0 is touch, 1 is gyro, 2 is steering wheel, 3 is joystick.
    /// </summary>
    /// <param name="index"></param>
    public void SetMobileController(int index) {

        switch (index) {

            case 0:
                RCCP.SetMobileController(RCCP_Settings.MobileController.TouchScreen);
                break;

            case 1:
                RCCP.SetMobileController(RCCP_Settings.MobileController.Gyro);
                break;

            case 2:
                RCCP.SetMobileController(RCCP_Settings.MobileController.SteeringWheel);
                break;

            case 3:
                RCCP.SetMobileController(RCCP_Settings.MobileController.Joystick);
                break;

        }

    }

}
