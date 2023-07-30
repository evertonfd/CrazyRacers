//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// A simple customizer example script used for receiving methods from UI elements and send them to RCC_Customization script. Also updates all UI elements for new spawned vehicles too.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP Customizer Example")]
public class RCCP_CustomizerExample : MonoBehaviour {

    private static RCCP_CustomizerExample instance;
    public static RCCP_CustomizerExample Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<RCCP_CustomizerExample>();

            return instance;

        }

    }

    public bool autoLoadWhenVehicleSpawns = false;      //  Auto loads the spawned vehicle if any saved data found.

    [Header("UI Menus")]
    public GameObject wheelsMenu;
    public GameObject configurationMenu;
    public GameObject steeringAssistancesMenu;
    public GameObject colorsMenu;

    [Header("UI Sliders")]
    public Slider frontCamber;
    public Slider rearCamber;
    public Slider frontSuspensionDistances;
    public Slider rearSuspensionDistances;
    public Slider frontSuspensionDampers;
    public Slider rearSuspensionDampers;
    public Slider frontSuspensionSprings;
    public Slider rearSuspensionSprings;
    public Slider gearShiftingThreshold;
    public Slider clutchThreshold;

    [Header("UI Toggles")]
    public Toggle TCS;
    public Toggle ABS;
    public Toggle ESP;
    public Toggle SH;
    public Toggle counterSteering;
    public Toggle limitSteering;
    public Toggle NOS;
    public Toggle turbo;
    public Toggle exhaustFlame;
    public Toggle revLimiter;
    public Toggle transmission;

    [Header("UI InputFields")]
    public InputField maxSpeed;
    public InputField maxBrake;
    public InputField maxTorque;

    private void Start() {

        //  Checking ui elements first.
        CheckUIs();

    }

    private void OnEnable() {

        //  Listening events when a vehicle spawns.
        RCCP_Events.OnRCCPSpawned += RCCP_Events_OnRCCPSpawned;

    }

    /// <summary>
    /// When a vehicle spawned.
    /// </summary>
    /// <param name="rccp"></param>
    private void RCCP_Events_OnRCCPSpawned(RCCP_CarController rccp) {

        //  Loading latest save data for the vehicle if found.
        if (autoLoadWhenVehicleSpawns)
            LoadStats();

    }

    /// <summary>
    /// Checking all ui elements.
    /// </summary>
    public void CheckUIs() {

        RCCP_CarController carController = RCCP_SceneManager.Instance.activePlayerVehicle;

        //  Early out if there are no player vehicle.
        if (!carController)
            return;

        //  Assigning variables of the ui elements based on vehicle settings.

        if (carController.FrontAxle && carController.RearAxle) {

            if (frontCamber)
                frontCamber.SetValueWithoutNotify(carController.FrontAxle.leftWheelCollider.camber);

            if (rearCamber)
                rearCamber.SetValueWithoutNotify(carController.RearAxle.leftWheelCollider.camber);

            if (frontSuspensionDistances)
                frontSuspensionDistances.SetValueWithoutNotify(carController.FrontAxle.leftWheelCollider.WheelCollider.suspensionDistance);

            if (rearSuspensionDistances)
                rearSuspensionDistances.SetValueWithoutNotify(carController.RearAxle.leftWheelCollider.WheelCollider.suspensionDistance);

            if (frontSuspensionDampers)
                frontSuspensionDampers.SetValueWithoutNotify(carController.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);

            if (rearSuspensionDampers)
                rearSuspensionDampers.SetValueWithoutNotify(carController.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);

            if (frontSuspensionSprings)
                frontSuspensionSprings.SetValueWithoutNotify(carController.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);

            if (rearSuspensionSprings)
                rearSuspensionSprings.SetValueWithoutNotify(carController.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);

        }

        if (clutchThreshold && carController.Clutch)
            clutchThreshold.SetValueWithoutNotify(carController.Clutch.clutchInertia);

        if (carController.Stability) {

            if (TCS)
                TCS.SetIsOnWithoutNotify(carController.Stability.TCS);

            if (ABS)
                ABS.SetIsOnWithoutNotify(carController.Stability.ABS);

            if (ESP)
                ESP.SetIsOnWithoutNotify(carController.Stability.ESP);

            if (SH)
                SH.SetIsOnWithoutNotify(carController.Stability.steeringHelper);

        }

        if (carController.Inputs) {

            if (counterSteering)
                counterSteering.SetIsOnWithoutNotify(carController.Inputs.counterSteering);

            if (limitSteering)
                limitSteering.SetIsOnWithoutNotify(carController.Inputs.steeringLimiter);

        }

        if (NOS && carController.OtherAddonsManager && carController.OtherAddonsManager.Nos)
            NOS.SetIsOnWithoutNotify(carController.OtherAddonsManager.Nos.enabled);

        //turbo.isOn = carController.useTurbo;
        //exhaustFlame.isOn = carController.useExhaustFlame;

        if (revLimiter && carController.Engine)
            revLimiter.SetIsOnWithoutNotify(carController.Engine.engineRevLimiter);

        if (carController.Gearbox) {

            if (transmission)
                transmission.SetIsOnWithoutNotify(carController.Gearbox.automaticTransmission);

            if (gearShiftingThreshold)
                gearShiftingThreshold.SetValueWithoutNotify(carController.Gearbox.shiftThreshold);

        }

        //maxSpeed.text = carController.maxspeed.ToString();

        if (maxBrake && carController.FrontAxle)
            maxBrake.text = carController.FrontAxle.maxBrakeTorque.ToString();

        if (maxTorque && carController.Engine)
            maxTorque.text = carController.Engine.maximumTorqueAsNM.ToString();

    }

    /// <summary>
    /// Opens the target menu.
    /// </summary>
    /// <param name="activeMenu"></param>
    public void OpenMenu(GameObject activeMenu) {

        if (activeMenu.activeInHierarchy) {

            activeMenu.SetActive(false);
            return;

        }

        if (wheelsMenu)
            wheelsMenu.SetActive(false);

        if (configurationMenu)
            configurationMenu.SetActive(false);

        if (steeringAssistancesMenu)
            steeringAssistancesMenu.SetActive(false);

        if (colorsMenu)
            colorsMenu.SetActive(false);

        activeMenu.SetActive(true);

    }

    /// <summary>
    /// Closes all menus.
    /// </summary>
    public void CloseAllMenus() {

        if (wheelsMenu)
            wheelsMenu.SetActive(false);

        if (configurationMenu)
            configurationMenu.SetActive(false);

        if (steeringAssistancesMenu)
            steeringAssistancesMenu.SetActive(false);

        if (colorsMenu)
            colorsMenu.SetActive(false);

    }

    /// <summary>
    /// Sets customization mode.
    /// </summary>
    /// <param name="state"></param>
    public void SetCustomizationMode(bool state) {

        //  Early out if there are no player vehicle.
        if (!RCCP_SceneManager.Instance.activePlayerVehicle)
            return;

        RCCP_Customization.SetCustomizationMode(RCCP_SceneManager.Instance.activePlayerVehicle, state);

        if (state)
            CheckUIs();

    }

    /// <summary>
    /// Front cambers.
    /// </summary>
    /// <param name="slider"></param>
    public void SetFrontCambersBySlider(Slider slider) {

        RCCP_Customization.SetFrontCambers(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Rear cambers.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRearCambersBySlider(Slider slider) {

        RCCP_Customization.SetRearCambers(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Smoke color.
    /// </summary>
    /// <param name="color"></param>
    public void SetSmokeColorByColorPicker(RCCP_ColorPickerBySliders color) {

        RCCP_Customization.SetSmokeColor(RCCP_SceneManager.Instance.activePlayerVehicle, 0, color.color);

    }

    /// <summary>
    /// Headlight color.
    /// </summary>
    /// <param name="color"></param>
    public void SetHeadlightColorByColorPicker(RCCP_ColorPickerBySliders color) {

        RCCP_Customization.SetHeadlightsColor(RCCP_SceneManager.Instance.activePlayerVehicle, color.color);

    }

    /// <summary>
    /// Wheel changes.
    /// </summary>
    /// <param name="slider"></param>
    public void ChangeWheelsBySlider(Slider slider) {

        RCCP_Customization.ChangeWheels(RCCP_SceneManager.Instance.activePlayerVehicle, RCCP_ChangableWheels.Instance.wheels[(int)slider.value].wheel, true);

    }

    /// <summary>
    /// Front suspension targets.
    /// </summary>
    /// <param name="slider"></param>
    public void SetFrontSuspensionTargetsBySlider(Slider slider) {

        RCCP_Customization.SetFrontSuspensionsTargetPos(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Rear suspension targets.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRearSuspensionTargetsBySlider(Slider slider) {

        RCCP_Customization.SetRearSuspensionsTargetPos(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// All suspension targets.
    /// </summary>
    /// <param name="strength"></param>
    public void SetAllSuspensionTargetsByButton(float strength) {

        RCCP_Customization.SetAllSuspensionsTargetPos(RCCP_SceneManager.Instance.activePlayerVehicle, strength);

    }

    /// <summary>
    /// Front suspension distances.
    /// </summary>
    /// <param name="slider"></param>
    public void SetFrontSuspensionDistancesBySlider(Slider slider) {

        RCCP_Customization.SetFrontSuspensionsDistances(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Rear suspension distances.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRearSuspensionDistancesBySlider(Slider slider) {

        RCCP_Customization.SetRearSuspensionsDistances(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Gear shifting threshold.
    /// </summary>
    /// <param name="slider"></param>
    public void SetGearShiftingThresholdBySlider(Slider slider) {

        RCCP_Customization.SetGearShiftingThreshold(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, .5f, .95f));

    }
    /// <summary>
    /// Clutch shifting threshold.
    /// </summary>
    /// <param name="slider"></param>
    public void SetClutchThresholdBySlider(Slider slider) {

        RCCP_Customization.SetClutchThreshold(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, .1f, .9f));

    }

    /// <summary>
    /// Counter steering.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetCounterSteeringByToggle(Toggle toggle) {

        RCCP_Customization.SetCounterSteering(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Steering limiter.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetSteeringLimitByToggle(Toggle toggle) {

        RCCP_Customization.SetSteeringLimit(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Nos.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetNOSByToggle(Toggle toggle) {

        RCCP_Customization.SetNOS(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Engine rev limiter.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetRevLimiterByToggle(Toggle toggle) {

        RCCP_Customization.SetRevLimiter(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Front suspension forces.
    /// </summary>
    /// <param name="slider"></param>
    public void SetFrontSuspensionsSpringForceBySlider(Slider slider) {

        RCCP_Customization.SetFrontSuspensionsSpringForce(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, 10000f, 100000f));

    }

    /// <summary>
    /// Rear suspension forces.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRearSuspensionsSpringForceBySlider(Slider slider) {

        RCCP_Customization.SetRearSuspensionsSpringForce(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, 10000f, 100000f));

    }

    /// <summary>
    /// Front suspension dampers.
    /// </summary>
    /// <param name="slider"></param>
    public void SetFrontSuspensionsSpringDamperBySlider(Slider slider) {

        RCCP_Customization.SetFrontSuspensionsSpringDamper(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, 1000f, 10000f));

    }

    /// <summary>
    /// Rear suspension dampers.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRearSuspensionsSpringDamperBySlider(Slider slider) {

        RCCP_Customization.SetRearSuspensionsSpringDamper(RCCP_SceneManager.Instance.activePlayerVehicle, Mathf.Clamp(slider.value, 1000f, 10000f));

    }

    //public void SetMaximumSpeedByInputField(InputField inputField) {

    //    RCC_Customization.SetMaximumSpeed(RCCP_SceneManager.Instance.activePlayerVehicle, StringToFloat(inputField.text, 200f));
    //    inputField.text = RCCP_SceneManager.Instance.activePlayerVehicle.maxspeed.ToString();

    //}

    /// <summary>
    /// Maximum engine torque.
    /// </summary>
    /// <param name="inputField"></param>
    public void SetMaximumTorqueByInputField(InputField inputField) {

        RCCP_Customization.SetMaximumTorque(RCCP_SceneManager.Instance.activePlayerVehicle, StringToFloat(inputField.text, 2000f));
        inputField.text = RCCP_SceneManager.Instance.activePlayerVehicle.Engine.maximumTorqueAsNM.ToString();

    }

    /// <summary>
    /// Maximum brake torque.
    /// </summary>
    /// <param name="inputField"></param>
    public void SetMaximumBrakeByInputField(InputField inputField) {

        RCCP_Customization.SetMaximumBrake(RCCP_SceneManager.Instance.activePlayerVehicle, StringToFloat(inputField.text, 2000f));
        inputField.text = RCCP_SceneManager.Instance.activePlayerVehicle.FrontAxle.maxBrakeTorque.ToString();

    }

    /// <summary>
    /// Repair.
    /// </summary>
    public void RepairCar() {

        RCCP_Customization.Repair(RCCP_SceneManager.Instance.activePlayerVehicle);

    }

    /// <summary>
    /// ESP.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetESP(Toggle toggle) {

        RCCP_Customization.SetESP(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// ABS.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetABS(Toggle toggle) {

        RCCP_Customization.SetABS(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// TCS.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetTCS(Toggle toggle) {

        RCCP_Customization.SetTCS(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Steering helper.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetSH(Toggle toggle) {

        RCCP_Customization.SetSH(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Steering helper strength.
    /// </summary>
    /// <param name="slider"></param>
    public void SetSHStrength(Slider slider) {

        RCCP_Customization.SetSHStrength(RCCP_SceneManager.Instance.activePlayerVehicle, slider.value);

    }

    /// <summary>
    /// Gearbox auto or not.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetTransmission(Toggle toggle) {

        RCCP_Customization.SetTransmission(RCCP_SceneManager.Instance.activePlayerVehicle, toggle.isOn);

    }

    /// <summary>
    /// Saves the configuration.
    /// </summary>
    public void SaveStats() {

        RCCP_Customization.SaveStats(RCCP_SceneManager.Instance.activePlayerVehicle);

    }

    /// <summary>
    /// Loads the configuration.
    /// </summary>
    public void LoadStats() {

        RCCP_Customization.LoadStats(RCCP_SceneManager.Instance.activePlayerVehicle);
        CheckUIs();

    }

    /// <summary>
    /// Resets the configuration.
    /// </summary>
    public void ResetStats() {

        int selectedVehicleIndex = -1;

        if (FindObjectOfType<RCCP_Demo>())
            selectedVehicleIndex = FindObjectOfType<RCCP_Demo>().selectedVehicleIndex;

        if (selectedVehicleIndex != -1)
            RCCP_Customization.ResetStats(RCCP_SceneManager.Instance.activePlayerVehicle, RCCP_DemoVehicles.Instance.vehicles[selectedVehicleIndex]);

        CheckUIs();

    }

    /// <summary>
    /// String to float.
    /// </summary>
    /// <param name="stringValue"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private float StringToFloat(string stringValue, float defaultValue) {

        float result = defaultValue;
        float.TryParse(stringValue, out result);
        return result;

    }

    private void OnDisable() {

        RCCP_Events.OnRCCPSpawned -= RCCP_Events_OnRCCPSpawned;

    }

}
