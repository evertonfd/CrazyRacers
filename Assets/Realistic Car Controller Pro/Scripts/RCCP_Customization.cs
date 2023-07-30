//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

///<summary>
/// Main Customization Class For RCCP.
///</summary>
public class RCCP_Customization : MonoBehaviour {

    /// <summary>
    /// Set Customization Mode. This will enable / disable controlling the vehicle, and enable / disable orbit camera mode.
    /// </summary>
    public static void SetCustomizationMode(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!vehicle) {

            Debug.LogError("Player vehicle is not selected for customization! Use RCCP_Customization.SetCustomizationMode(playerVehicle, true/false); for enabling / disabling customization mode for player vehicle.");
            return;

        }

        //  Finding camera and dashboard.
        RCCP_Camera cam = RCCP_SceneManager.Instance.activePlayerCamera;
        RCCP_UIManager UI = RCCP_SceneManager.Instance.activePlayerCanvas;

        //  If enabled customization mode, set camera mode to TPS and set UI type to Customization. Set controllable state of the vehicle to false, we don't want to control the vehicle while customizing.
        if (state) {

            vehicle.SetCanControl(false);

            if (cam)
                cam.ChangeCamera(RCCP_Camera.CameraMode.TPS);

            if (UI) {

                if (UI.customizer)
                    UI.customizer.SetActive(true);

            }

        } else {

            vehicle.SetCanControl(true);

            if (cam)
                cam.ChangeCamera(RCCP_Camera.CameraMode.TPS);

            if (UI) {

                if (UI.customizer)
                    UI.customizer.SetActive(false);

            }

        }

    }

    /// <summary>
    /// Set Smoke Color.
    /// </summary>
    public static void SetSmokeColor(RCCP_CarController vehicle, int indexOfGroundMaterial, Color color) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Particles)
            return;

        if (vehicle.Particles.wheelParticles == null)
            return;

        for (int i = 0; i < vehicle.Particles.wheelParticles.Length; i++) {

            //  And setting color of the particles.
            foreach (ParticleSystem wheelParticle in vehicle.Particles.wheelParticles[i].allWheelParticles) {

                ParticleSystem.MainModule psmain = wheelParticle.main;
                color.a = psmain.startColor.color.a;
                psmain.startColor = color;

            }

        }

    }

    /// <summary>
    /// Set Headlights Color.
    /// </summary>
    public static void SetHeadlightsColor(RCCP_CarController vehicle, Color color) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Lights)
            return;

        if (vehicle.Lights.lights == null)
            return;

        //  Enabling headlights.
        vehicle.Lights.lowBeamHeadlights = true;

        //  If light is headlight, set color.
        foreach (RCCP_Light l in vehicle.Lights.lights) {

            if (l.lightType == RCCP_Light.LightType.Headlight_LowBeam || l.lightType == RCCP_Light.LightType.Headlight_HighBeam)
                l.lightSource.color = color;

        }

    }

    /// <summary>
    /// Set Front Wheel Cambers.
    /// </summary>
    public static void SetFrontCambers(RCCP_CarController vehicle, float camberAngle) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.FrontAxle)
            return;

        if (vehicle.FrontAxle.leftWheelCollider)
            vehicle.FrontAxle.leftWheelCollider.camber = camberAngle;

        if (vehicle.FrontAxle.rightWheelCollider)
            vehicle.FrontAxle.rightWheelCollider.camber = camberAngle;

    }

    /// <summary>
    /// Set Rear Wheel Cambers.
    /// </summary>
    public static void SetRearCambers(RCCP_CarController vehicle, float camberAngle) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.RearAxle)
            return;

        if (vehicle.RearAxle.leftWheelCollider)
            vehicle.RearAxle.leftWheelCollider.camber = camberAngle;

        if (vehicle.RearAxle.rightWheelCollider)
            vehicle.RearAxle.rightWheelCollider.camber = camberAngle;

    }

    /// <summary>
    /// Change Wheel Models. You can find your wheel models array in Tools --> BCG --> RCCP --> Configure Changable Wheels.
    /// </summary>
    public static void ChangeWheels(RCCP_CarController vehicle, GameObject wheel, bool applyRadius) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (vehicle.AllWheelColliders == null)
            return;

        //  Getting all wheelcolliders.
        for (int i = 0; i < vehicle.AllWheelColliders.Length; i++) {

            //  Disabling renderer of the wheelmodel.
            if (vehicle.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>())
                vehicle.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>().enabled = false;

            //  Disabling all child models of the wheel.
            foreach (Transform t in vehicle.AllWheelColliders[i].wheelModel.GetComponentInChildren<Transform>())
                t.gameObject.SetActive(false);

            //  Instantiating new wheel.
            GameObject newWheel = Instantiate(wheel, vehicle.AllWheelColliders[i].wheelModel.position, vehicle.AllWheelColliders[i].wheelModel.rotation, vehicle.AllWheelColliders[i].wheelModel);

            //  If wheel is at right side, multiply scale X by -1 for symetry.
            if (vehicle.AllWheelColliders[i].wheelModel.localPosition.x > 0f)
                newWheel.transform.localScale = new Vector3(newWheel.transform.localScale.x * -1f, newWheel.transform.localScale.y, newWheel.transform.localScale.z);

            //  If apply radius is set to true, calculate the radius.
            if (applyRadius)
                vehicle.AllWheelColliders[i].WheelCollider.radius = RCCP_GetBounds.MaxBoundsExtent(wheel.transform);

        }

    }

    /// <summary>
    /// Set Front Suspension targetPositions. It changes targetPosition of the front WheelColliders.
    /// </summary>
    public static void SetFrontSuspensionsTargetPos(RCCP_CarController vehicle, float targetPosition) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.FrontAxle)
            return;

        //  Sets target position.
        targetPosition = Mathf.Clamp01(targetPosition);

        if (vehicle.FrontAxle.leftWheelCollider) {

            JointSpring spring1 = vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring;
            spring1.targetPosition = 1f - targetPosition;

            vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring1;

        }

        if (vehicle.FrontAxle.rightWheelCollider) {

            JointSpring spring1 = vehicle.FrontAxle.rightWheelCollider.WheelCollider.suspensionSpring;
            spring1.targetPosition = 1f - targetPosition;

            vehicle.FrontAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring1;

        }

    }

    /// <summary>
    /// Set Rear Suspension targetPositions. It changes targetPosition of the rear WheelColliders.
    /// </summary>
    public static void SetRearSuspensionsTargetPos(RCCP_CarController vehicle, float targetPosition) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.RearAxle)
            return;

        //  Sets target position.
        targetPosition = Mathf.Clamp01(targetPosition);

        if (vehicle.RearAxle.leftWheelCollider) {

            JointSpring spring1 = vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring;
            spring1.targetPosition = 1f - targetPosition;

            vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring1;

        }

        if (vehicle.RearAxle.rightWheelCollider) {

            JointSpring spring1 = vehicle.RearAxle.rightWheelCollider.WheelCollider.suspensionSpring;
            spring1.targetPosition = 1f - targetPosition;

            vehicle.RearAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring1;

        }

    }

    /// <summary>
    /// Set All Suspension targetPositions. It changes targetPosition of the all WheelColliders.
    /// </summary>
    public static void SetAllSuspensionsTargetPos(RCCP_CarController vehicle, float targetPosition) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        SetFrontSuspensionsTargetPos(vehicle, targetPosition);
        SetRearSuspensionsTargetPos(vehicle, targetPosition);

    }

    /// <summary>
    /// Set Front Suspension Distances.
    /// </summary>
    public static void SetFrontSuspensionsDistances(RCCP_CarController vehicle, float distance) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        //  Make sure new distance is not close to 0.
        if (distance <= .01)
            distance = .05f;

        if (!vehicle.FrontAxle)
            return;

        //  Setting suspension distance of front wheelcolliders.
        if (vehicle.FrontAxle.leftWheelCollider)
            vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionDistance = distance;

        if (vehicle.FrontAxle.rightWheelCollider)
            vehicle.FrontAxle.rightWheelCollider.WheelCollider.suspensionDistance = distance;

    }

    /// <summary>
    /// Set Rear Suspension Distances.
    /// </summary>
    public static void SetRearSuspensionsDistances(RCCP_CarController vehicle, float distance) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        //  Make sure new distance is not close to 0.
        if (distance <= .01)
            distance = .05f;

        if (!vehicle.RearAxle)
            return;

        //  Setting suspension distance of front wheelcolliders.
        if (vehicle.RearAxle.leftWheelCollider)
            vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionDistance = distance;

        if (vehicle.RearAxle.rightWheelCollider)
            vehicle.RearAxle.rightWheelCollider.WheelCollider.suspensionDistance = distance;

    }

    /// <summary>
    /// Set Gear Shifting Threshold. Automatic gear will shift up at earlier rpm on lower values. Automatic gear will shift up at later rpm on higher values. 
    /// </summary>
    public static void SetGearShiftingThreshold(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Gearbox)
            return;

        vehicle.Gearbox.shiftThreshold = targetValue;

    }

    /// <summary>
    /// Set Clutch Threshold. Automatic gear will shift up at earlier rpm on lower values. Automatic gear will shift up at later rpm on higher values. 
    /// </summary>
    public static void SetClutchThreshold(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Clutch)
            return;

        vehicle.Clutch.clutchInertia = targetValue;

    }

    /// <summary>
    /// Enable / Disable Counter Steering while vehicle is drifting. Useful for avoid spinning.
    /// </summary>
    public static void SetCounterSteering(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Inputs)
            return;

        vehicle.Inputs.counterSteering = state;

    }

    /// <summary>
    /// Enable / Disable Steering Limiter while vehicle is drifting. Useful for avoid spinning.
    /// </summary>
    public static void SetSteeringLimit(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Inputs)
            return;

        vehicle.Inputs.steeringLimiter = state;

    }

    /// <summary>
    /// Enable / Disable NOS.
    /// </summary>
    public static void SetNOS(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.OtherAddonsManager)
            return;

        if (!vehicle.OtherAddonsManager.Nos)
            return;

        vehicle.OtherAddonsManager.Nos.enabled = state;

    }

    ///// <summary>
    ///// Enable / Disable Turbo.
    ///// </summary>
    //public static void SetTurbo(RCCP_CarController vehicle, bool state) {

    //    //  If no vehicle found, return.
    //    if (!CheckVehicle(vehicle))
    //        return;

    //    vehicle.useTurbo = state;

    //    OverrideRCC(vehicle);

    //}

    /// <summary>
    /// Enable / Disable Rev Limiter.
    /// </summary>
    public static void SetRevLimiter(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Engine)
            return;

        vehicle.Engine.engineRevLimiter = state;

    }

    /// <summary>
    /// Set Front Suspension Spring Force.
    /// </summary>
    public static void SetFrontSuspensionsSpringForce(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.FrontAxle)
            return;

        if (!vehicle.FrontAxle.leftWheelCollider || !vehicle.FrontAxle.rightWheelCollider)
            return;

        JointSpring spring = vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring;
        spring.spring = targetValue;
        vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring;
        vehicle.FrontAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring;

    }

    /// <summary>
    /// Set Rear Suspension Spring Force.
    /// </summary>
    public static void SetRearSuspensionsSpringForce(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.RearAxle)
            return;

        if (!vehicle.RearAxle.leftWheelCollider || !vehicle.RearAxle.rightWheelCollider)
            return;

        JointSpring spring = vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring;
        spring.spring = targetValue;
        vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring;
        vehicle.RearAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring;

    }

    /// <summary>
    /// Set Front Suspension Spring Damper.
    /// </summary>
    public static void SetFrontSuspensionsSpringDamper(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.FrontAxle)
            return;

        if (!vehicle.FrontAxle.leftWheelCollider || !vehicle.RearAxle.rightWheelCollider)
            return;

        JointSpring spring = vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring;
        spring.damper = targetValue;
        vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring;
        vehicle.FrontAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring;

    }

    /// <summary>
    /// Set Rear Suspension Spring Damper.
    /// </summary>
    public static void SetRearSuspensionsSpringDamper(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.RearAxle)
            return;

        if (!vehicle.RearAxle.leftWheelCollider || !vehicle.RearAxle.rightWheelCollider)
            return;

        JointSpring spring = vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring;
        spring.damper = targetValue;
        vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring = spring;
        vehicle.RearAxle.rightWheelCollider.WheelCollider.suspensionSpring = spring;

    }

    ///// <summary>
    ///// Set Maximum Speed of the vehicle.
    ///// </summary>
    //public static void SetMaximumSpeed(RCCP_CarController vehicle, float targetValue) {

    //    //  If no vehicle found, return.
    //    if (!CheckVehicle(vehicle))
    //        return;

    //    vehicle.maxspeed = Mathf.Clamp(targetValue, 10f, 400f);

    //    OverrideRCC(vehicle);

    //}

    /// <summary>
    /// Set Maximum Engine Torque of the vehicle.
    /// </summary>
    public static void SetMaximumTorque(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Engine)
            return;

        vehicle.Engine.maximumTorqueAsNM = Mathf.Clamp(targetValue, 50f, 5000f);

    }

    /// <summary>
    /// Set Maximum Brake of the vehicle.
    /// </summary>
    public static void SetMaximumBrake(RCCP_CarController vehicle, float targetValue) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (vehicle.AxleManager == null)
            return;

        for (int i = 0; i < vehicle.AxleManager.Axles.Count; i++)
            vehicle.AxleManager.Axles[i].maxBrakeTorque = Mathf.Clamp(targetValue, 0f, 50000f);

    }

    /// <summary>
    /// Repair vehicle.
    /// </summary>
    public static void Repair(RCCP_CarController vehicle) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Damage)
            return;

        vehicle.Damage.repairNow = true;

    }

    /// <summary>
    /// Enable / Disable ESP.
    /// </summary>
    public static void SetESP(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Stability)
            return;

        vehicle.Stability.ESP = state;

    }

    /// <summary>
    /// Enable / Disable ABS.
    /// </summary>
    public static void SetABS(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Stability)
            return;

        vehicle.Stability.ABS = state;

    }

    /// <summary>
    /// Enable / Disable TCS.
    /// </summary>
    public static void SetTCS(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Stability)
            return;

        vehicle.Stability.TCS = state;

    }

    /// <summary>
    /// Enable / Disable Steering Helper.
    /// </summary>
    public static void SetSH(RCCP_CarController vehicle, bool state) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Stability)
            return;

        vehicle.Stability.steeringHelper = state;

    }

    /// <summary>
    /// Set Steering Helper strength.
    /// </summary>
    public static void SetSHStrength(RCCP_CarController vehicle, float value) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Stability)
            return;

        vehicle.Stability.steerHelperStrength = value;

    }

    /// <summary>
    /// Set Transmission of the vehicle.
    /// </summary>
    public static void SetTransmission(RCCP_CarController vehicle, bool automatic) {

        if (!CheckVehicle(vehicle))
            return;

        if (!vehicle.Gearbox)
            return;

        vehicle.Gearbox.automaticTransmission = automatic;

    }

    /// <summary>
    /// Save all stats with PlayerPrefs.
    /// </summary>
    public static void SaveStats(RCCP_CarController vehicle) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        PlayerPrefs.SetInt(vehicle.transform.name + "_Customization", 1);

        //  Saving all major settings of the vehicle with PlayerPrefs.
        if (vehicle.FrontAxle && vehicle.RearAxle) {

            PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontCamber", vehicle.FrontAxle.leftWheelCollider.camber);
            PlayerPrefs.SetFloat(vehicle.transform.name + "_RearCamber", vehicle.RearAxle.leftWheelCollider.camber);

            PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsDistance", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionDistance);
            PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsDistance", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionDistance);

            PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsSpring", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);
            PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsSpring", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);

            PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsDamper", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);
            PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsDamper", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);

            PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumBrake", vehicle.FrontAxle.maxBrakeTorque);

        }

        //PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumSpeed", vehicle.maxspeed);

        if (vehicle.Engine) {

            PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumTorque", vehicle.Engine.maximumTorqueAsNM);
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "RevLimiter", vehicle.Engine.engineRevLimiter);

        }

        if (vehicle.Gearbox)
            PlayerPrefs.SetFloat(vehicle.transform.name + "_GearShiftingThreshold", vehicle.Gearbox.shiftThreshold);

        if (vehicle.Clutch)
            PlayerPrefs.SetFloat(vehicle.transform.name + "_ClutchingThreshold", vehicle.Clutch.clutchInertia);

        if (vehicle.Inputs) {

            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_CounterSteering", vehicle.Inputs.counterSteering);
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_LimitSteering", vehicle.Inputs.steeringLimiter);

        }

        if (vehicle.Lights) {

            foreach (RCCP_Light _light in vehicle.Lights.lights) {

                if (_light.lightType == RCCP_Light.LightType.Headlight_LowBeam) {

                    RCCP_PlayerPrefsX.SetColor(vehicle.transform.name + "_HeadlightsColor", _light.lightSource.color);
                    break;

                }

            }

        }

        if (vehicle.Particles) {

            ParticleSystem ps = vehicle.Particles.wheelParticles[0].allWheelParticles[0];
            ParticleSystem.MainModule psmain = ps.main;
            RCCP_PlayerPrefsX.SetColor(vehicle.transform.name + "_WheelsSmokeColor", psmain.startColor.color);

        }

        if (vehicle.Stability) {

            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_ABS", vehicle.Stability.ABS);
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_ESP", vehicle.Stability.ESP);
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_TCS", vehicle.Stability.TCS);
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "_SH", vehicle.Stability.steeringHelper);

        }

        if (vehicle.OtherAddonsManager && vehicle.OtherAddonsManager.Nos)
            RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "NOS", vehicle.OtherAddonsManager.Nos.enabled);

        //RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "Turbo", vehicle.useTurbo);
        //RCCP_PlayerPrefsX.SetBool(vehicle.transform.name + "ExhaustFlame", vehicle.useExhaustFlame);

    }

    /// <summary>
    /// Load all stats with PlayerPrefs.
    /// </summary>
    public static void LoadStats(RCCP_CarController vehicle) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        if (!PlayerPrefs.HasKey(vehicle.transform.name + "_Customization"))
            return;

        //  Loading all major settings of the vehicle with PlayerPrefs.
        if (vehicle.FrontAxle && vehicle.RearAxle) {

            SetFrontCambers(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontCamber", vehicle.FrontAxle.leftWheelCollider.camber));
            SetRearCambers(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearCamber", vehicle.FrontAxle.rightWheelCollider.camber));

            SetFrontSuspensionsDistances(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsDistance", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionDistance));
            SetRearSuspensionsDistances(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsDistance", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionDistance));

            SetFrontSuspensionsSpringForce(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsSpring", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring));
            SetRearSuspensionsSpringForce(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsSpring", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring));

            SetFrontSuspensionsSpringDamper(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsDamper", vehicle.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper));
            SetRearSuspensionsSpringDamper(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsDamper", vehicle.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper));

            //SetMaximumSpeed(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumSpeed", vehicle.maxspeed));
            SetMaximumBrake(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumBrake", vehicle.FrontAxle.maxBrakeTorque));

        }

        if (vehicle.Engine) {

            SetMaximumTorque(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumTorque", vehicle.Engine.maximumTorqueAsNM));
            SetRevLimiter(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "RevLimiter", vehicle.Engine.engineRevLimiter));

        }

        if (vehicle.Gearbox)
            SetGearShiftingThreshold(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_GearShiftingThreshold", vehicle.Gearbox.shiftThreshold));

        if (vehicle.Clutch)
            SetClutchThreshold(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_ClutchingThreshold", vehicle.Clutch.clutchInertia));

        if (vehicle.Inputs) {

            SetCounterSteering(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_CounterSteering", vehicle.Inputs.counterSteering));
            SetSteeringLimit(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_LimitSteering", vehicle.Inputs.steeringLimiter));

        }

        if (vehicle.Stability) {

            SetABS(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_ABS", vehicle.Stability.ABS));
            SetESP(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_ESP", vehicle.Stability.ESP));
            SetTCS(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_TCS", vehicle.Stability.TCS));
            SetSH(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "_SH", vehicle.Stability.steeringHelper));

        }

        if (vehicle.OtherAddonsManager && vehicle.OtherAddonsManager.Nos)
            SetNOS(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "NOS", vehicle.OtherAddonsManager.Nos.enabled));

        //SetTurbo(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "Turbo", vehicle.useTurbo));
        //SetUseExhaustFlame(vehicle, RCCP_PlayerPrefsX.GetBool(vehicle.transform.name + "ExhaustFlame", vehicle.useExhaustFlame));

        if (PlayerPrefs.HasKey(vehicle.transform.name + "_WheelsSmokeColor"))
            SetSmokeColor(vehicle, 0, RCCP_PlayerPrefsX.GetColor(vehicle.transform.name + "_WheelsSmokeColor"));

        if (PlayerPrefs.HasKey(vehicle.transform.name + "_HeadlightsColor"))
            SetHeadlightsColor(vehicle, RCCP_PlayerPrefsX.GetColor(vehicle.transform.name + "_HeadlightsColor"));

    }

    /// <summary>
    /// Resets all stats and saves default values with PlayerPrefs.
    /// </summary>
    public static void ResetStats(RCCP_CarController vehicle, RCCP_CarController defaultCar) {

        //  If no vehicle found, return.
        if (!CheckVehicle(vehicle))
            return;

        PlayerPrefs.DeleteKey(vehicle.transform.name + "_Customization");

        //  If no default vehicle found, return.
        if (!CheckVehicle(defaultCar))
            return;

        if (vehicle.FrontAxle && vehicle.RearAxle) {

            SetFrontCambers(vehicle, defaultCar.FrontAxle.leftWheelCollider.camber);
            SetRearCambers(vehicle, defaultCar.RearAxle.leftWheelCollider.camber);

            SetFrontSuspensionsDistances(vehicle, defaultCar.FrontAxle.leftWheelCollider.WheelCollider.suspensionDistance);
            SetRearSuspensionsDistances(vehicle, defaultCar.RearAxle.leftWheelCollider.WheelCollider.suspensionDistance);

            SetFrontSuspensionsSpringForce(vehicle, defaultCar.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);
            SetRearSuspensionsSpringForce(vehicle, defaultCar.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.spring);

            SetFrontSuspensionsSpringDamper(vehicle, defaultCar.FrontAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);
            SetRearSuspensionsSpringDamper(vehicle, defaultCar.RearAxle.leftWheelCollider.WheelCollider.suspensionSpring.damper);

            //SetMaximumSpeed(vehicle, defaultCar.maxspeed);
            SetMaximumBrake(vehicle, defaultCar.FrontAxle.maxBrakeTorque);

        }

        if (vehicle.Engine) {

            SetMaximumTorque(vehicle, defaultCar.Engine.maximumTorqueAsNM);
            SetRevLimiter(vehicle, defaultCar.Engine.engineRevLimiter);

        }

        if (vehicle.Gearbox)
            SetGearShiftingThreshold(vehicle, defaultCar.Gearbox.shiftThreshold);

        if (vehicle.Clutch)
            SetClutchThreshold(vehicle, defaultCar.Clutch.clutchInertia);

        if (vehicle.Inputs) {

            SetCounterSteering(vehicle, defaultCar.Inputs.counterSteering);
            SetSteeringLimit(vehicle, defaultCar.Inputs.steeringLimiter);

        }

        if (vehicle.Stability) {

            SetABS(vehicle, defaultCar.Stability.ABS);
            SetESP(vehicle, defaultCar.Stability.ESP);
            SetTCS(vehicle, defaultCar.Stability.TCS);
            SetSH(vehicle, defaultCar.Stability.steeringHelper);

        }

        if (vehicle.OtherAddonsManager && vehicle.OtherAddonsManager.Nos)
            SetNOS(vehicle, defaultCar.OtherAddonsManager.Nos.enabled);

        //SetTurbo(vehicle, defaultCar.useTurbo);
        //SetUseExhaustFlame(vehicle, defaultCar.useExhaustFlame);

        SetSmokeColor(vehicle, 0, Color.white);
        SetHeadlightsColor(vehicle, Color.white);

        SaveStats(vehicle);

    }

    /// <summary>
    /// Checks the player vehicle.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns></returns>
    public static bool CheckVehicle(RCCP_CarController vehicle) {

        //  If no vehicle found, return with an error.
        if (!vehicle) {

            Debug.LogError("Vehicle is missing!");
            return false;

        }

        return true;

    }

}
