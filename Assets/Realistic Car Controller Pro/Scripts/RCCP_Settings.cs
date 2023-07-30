//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// <summary>
/// Stored all general shared RCCP settings here.
/// </summary>
[System.Serializable]
public class RCCP_Settings : ScriptableObject {

    #region singleton
    private static RCCP_Settings instance;
    public static RCCP_Settings Instance { get { if (instance == null) instance = Resources.Load("RCCP_Settings") as RCCP_Settings; return instance; } }
    #endregion

    public BehaviorType SelectedBehaviorType {

        get {

            if (overrideBehavior)
                return behaviorTypes[behaviorSelectedIndex];
            else
                return null;

        }

    }

    public int behaviorSelectedIndex = 0;       //  Current selected behavior index.

    public bool overrideFPS = true;     //  Override FPS?
    public bool overrideFixedTimeStep = true;       //  Override fixed timestep?
    [Range(.005f, .06f)] public float fixedTimeStep = .02f;     //  Overrided fixed timestep value.
    [Range(.5f, 20f)] public float maxAngularVelocity = 6;      //  Maximum angular velocity.
    public int maxFPS = 60;     //  Maximum FPS.

    public bool overrideBehavior = true;        //  Override the behavior?

    /// <summary>
    /// Behavior Types
    /// </summary>
    [System.Serializable]
    public class BehaviorType {

        public string behaviorName = "New Behavior";        //  Behavior name.

        //  Driving helpers.
        [Header("Stability")]
        public bool ABS = true;
        public bool ESP = true;
        public bool TCS = true;
        public bool steeringHelper = true;
        public bool tractionHelper = true;
        public bool turnHelper = true;
        public bool angularDragHelper = false;
        public bool driftMode = false;

        //  Steering.
        [Header("Steering")]
        public AnimationCurve steeringCurve = new AnimationCurve(new Keyframe(0f, 40f), new Keyframe(50f, 20f), new Keyframe(100f, 11f), new Keyframe(150f, 6f), new Keyframe(200f, 5f));
        public float steeringSensitivity = 1f;
        public bool counterSteering = true;
        public bool limitSteering = true;

        [Header("Differential")]
        public RCCP_Differential.DifferentialType differentialType = RCCP_Differential.DifferentialType.Open;

        //  Counter steering limitations.
        [Space()]
        public float counterSteeringMinimum = .5f;
        public float counterSteeringMaximum = 1f;

        //  Steering sensitivity limitations.
        [Space()]
        public float steeringSpeedMinimum = .5f;
        public float steeringSpeedMaximum = 1f;

        //  Steering helper linear velocity limitations.
        [Range(0f, 1f)] public float steeringHelperStrengthMinimum = .1f;
        [Range(0f, 1f)] public float steeringHelperStrengthMaximum = 1f;

        //  Traction helper strength limitations.
        [Range(0f, 1f)] public float tractionHelperStrengthMinimum = .1f;
        [Range(0f, 1f)] public float tractionHelperStrengthMaximum = 1f;

        //  Traction helper strength limitations.
        [Range(0f, 1f)] public float turnHelperStrengthMinimum = .1f;
        [Range(0f, 1f)] public float turnHelperStrengthMaximum = 1f;

        //  Angular drag limitations.
        [Range(0f, 10f)] public float angularDrag = .1f;
        [Range(0f, 1f)] public float angularDragHelperMinimum = .1f;
        [Range(0f, 1f)] public float angularDragHelperMaximum = 1f;

        //  Anti roll limitations.
        [Space()]
        public float antiRollMinimum = 1000f;

        //  Gear shifting delay limitation.
        [Space()]
        [Range(.1f, .9f)] public float gearShiftingThreshold = .8f;
        [Range(0f, 1f)] public float gearShiftingDelayMinimum = .15f;
        [Range(0f, 1f)] public float gearShiftingDelayMaximum = .5f;

        //  Wheel frictions.
        [Header("Wheel Frictions Forward Front Side")]
        public float forwardExtremumSlip_F = .4f;
        public float forwardExtremumValue_F = 1f;
        public float forwardAsymptoteSlip_F = .8f;
        public float forwardAsymptoteValue_F = .5f;

        [Header("Wheel Frictions Forward Rear Side")]
        public float forwardExtremumSlip_R = .4f;
        public float forwardExtremumValue_R = .95f;
        public float forwardAsymptoteSlip_R = .75f;
        public float forwardAsymptoteValue_R = .5f;

        [Header("Wheel Frictions Sideways Front Side")]
        public float sidewaysExtremumSlip_F = .4f;
        public float sidewaysExtremumValue_F = 1f;
        public float sidewaysAsymptoteSlip_F = .5f;
        public float sidewaysAsymptoteValue_F = .75f;

        [Header("Wheel Frictions Sideways Rear Side")]
        public float sidewaysExtremumSlip_R = .4f;
        public float sidewaysExtremumValue_R = 1.05f;
        public float sidewaysAsymptoteSlip_R = .5f;
        public float sidewaysAsymptoteValue_R = .8f;

    }

    // Behavior Types
    public BehaviorType[] behaviorTypes;

    public bool useFixedWheelColliders = true;      //  Fixed wheelcolliders with higher mass will be used.

    // Main Controller Settings
    public bool autoReset = true;       //  All vehicles can be resetted if upside down.

    // Information telemetry about current vehicle
    public bool useTelemetry = false;
    public bool useInputDebugger = false;

    //  Input types
    public bool useNewInputSystem = true;

    // For mobile inputs
    public enum MobileController { TouchScreen, Gyro, SteeringWheel, Joystick }
    public MobileController mobileController;
    public bool mobileControllerEnabled = false;

    // Accelerometer sensitivity
    public float gyroSensitivity = 2.5f;

    public bool setLayers = true;       //  Setting layers.
    public string RCCPLayer = "RCCP_Vehicle";     //  Layer of the vehicle.
    public string RCCPWheelColliderLayer = "RCCP_WheelCollider";     //  Wheelcollider layer.
    public string RCCPDetachablePartLayer = "RCCP_DetachablePart";       //  Detachable part's layer.

    // Used for using the lights more efficent and realistic
    public bool useHeadLightsAsVertexLights = false;
    public bool useBrakeLightsAsVertexLights = true;
    public bool useReverseLightsAsVertexLights = true;
    public bool useIndicatorLightsAsVertexLights = true;
    public bool useOtherLightsAsVertexLights = true;

    #region Setup Prefabs

    // Light prefabs.
    public GameObject headLights_Low;
    public GameObject headLights_High;
    public GameObject brakeLights;
    public GameObject reverseLights;
    public GameObject indicatorLights_L;
    public GameObject indicatorLights_R;
    public GameObject tailLights;
    public GameObject lightBox;

    //  Camera prefabs.
    public RCCP_Camera RCCPMainCamera;
    public GameObject RCCPHoodCamera;
    public GameObject RCCPWheelCamera;
    public GameObject RCCPCinematicCamera;
    public GameObject RCCPFixedCamera;

    //  UI prefabs.
    public GameObject RCCPCanvas;
    public GameObject RCCPTelemetry;

    // Sound FX.
    public AudioMixerGroup audioMixer;
    public AudioClip engineLowClipOn;
    public AudioClip engineLowClipOff;
    public AudioClip engineMedClipOn;
    public AudioClip engineMedClipOff;
    public AudioClip engineHighClipOn;
    public AudioClip engineHighClipOff;
    public AudioClip engineStartClip;
    public AudioClip reversingClip;
    public AudioClip windClip;
    public AudioClip brakeClip;
    public AudioClip wheelDeflateClip;
    public AudioClip wheelInflateClip;
    public AudioClip wheelFlatClip;
    public AudioClip indicatorClip;
    public AudioClip bumpClip;
    public AudioClip NOSClip;
    public AudioClip turboClip;
    public AudioClip[] gearClips;
    public AudioClip[] crashClips;
    public AudioClip[] blowoutClip;
    public AudioClip[] exhaustFlameClips;

    //  Particles
    public GameObject contactParticles;
    public GameObject scratchParticles;
    public GameObject wheelSparkleParticles;

    //  Other prefabs.
    public GameObject exhaustGas;
    public RCCP_SkidmarksManager skidmarksManager;

    #endregion

    // Used for folding sections of RCCP Settings.
    public bool foldGeneralSettings = false;
    public bool foldBehaviorSettings = false;
    public bool foldControllerSettings = false;
    public bool foldUISettings = false;
    public bool foldWheelPhysics = false;
    public bool foldOptimization = false;
    public bool foldTagsAndLayers = false;
    public bool resourcesSettings = false;

}
