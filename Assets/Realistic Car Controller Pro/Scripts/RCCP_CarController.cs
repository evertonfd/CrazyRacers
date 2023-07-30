//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Main car controller of the vehicle. Manages everything and takes control.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Main/RCCP Car Controller")]
public class RCCP_CarController : MonoBehaviour {

    public bool canControl = true;      //  Is this vehicle controllable now? RCCP_Inputs attached to the vehicle will receive inputs when enabled.
    public bool ineffectiveBehavior = false;        //  Selected behavior in RCCP_Settings won't affect this vehicle if this option is enabled.

    #region COMPONENTS

    //  Rigidbody.
    public Rigidbody Rigid {

        get {

            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();

            return _rigid;

        }

    }

    //  Engine.
    public RCCP_Engine Engine {

        get {

            if (!componentsTaken && _engine == null)
                _engine = GetComponentInChildren<RCCP_Engine>(true);

            return _engine;

        }
        set {

            _engine = value;

        }

    }

    //  Clutch
    public RCCP_Clutch Clutch {

        get {

            if (!componentsTaken && _clutch == null)
                _clutch = GetComponentInChildren<RCCP_Clutch>(true);

            return _clutch;

        }
        set {

            _clutch = value;

        }

    }

    //  Gearbox.
    public RCCP_Gearbox Gearbox {

        get {

            if (!componentsTaken && _gearbox == null)
                _gearbox = GetComponentInChildren<RCCP_Gearbox>(true);

            return _gearbox;

        }
        set {

            _gearbox = value;

        }

    }

    //  Differential.
    public RCCP_Differential Differential {

        get {

            if (!componentsTaken && _differential == null)
                _differential = GetComponentInChildren<RCCP_Differential>(true);

            return _differential;

        }
        set {

            _differential = value;

        }

    }

    //  Axles.
    public RCCP_Axles AxleManager {

        get {

            if (!componentsTaken && _axles == null)
                _axles = GetComponentInChildren<RCCP_Axles>(true);

            return _axles;

        }
        set {

            _axles = value;

        }

    }

    //  Front axle.
    public RCCP_Axle FrontAxle {

        get {

            if (AxleManager == null)
                return null;

            if (AxleManager.Axles.Count < 2)
                return null;

            float[] indexes = new float[AxleManager.Axles.Count];

            for (int i = 0; i < AxleManager.Axles.Count; i++)
                indexes[i] = AxleManager.Axles[i].leftWheelCollider.transform.localPosition.z;

            int biggestIndex = 0;
            int lowestIndex = 0;

            for (int i = 0; i < indexes.Length; i++) {

                if (indexes[i] >= biggestIndex)
                    biggestIndex = i;

                if (indexes[i] <= lowestIndex)
                    lowestIndex = i;

            }

            _axleFront = AxleManager.Axles[biggestIndex];

            return _axleFront;

        }

    }

    //  Rear axle.
    public RCCP_Axle RearAxle {

        get {

            if (AxleManager.Axles == null)
                return null;

            if (AxleManager.Axles.Count < 2)
                return null;

            float[] indexes = new float[AxleManager.Axles.Count];

            for (int i = 0; i < AxleManager.Axles.Count; i++)
                indexes[i] = AxleManager.Axles[i].leftWheelCollider.transform.localPosition.z;

            int biggestIndex = 0;
            int lowestIndex = 0;

            for (int i = 0; i < indexes.Length; i++) {

                if (indexes[i] >= biggestIndex)
                    biggestIndex = i;

                if (indexes[i] <= lowestIndex)
                    lowestIndex = i;

            }

            _axleRear = AxleManager.Axles[lowestIndex];

            return _axleRear;

        }

    }

    //  All wheelcolliders.
    public RCCP_WheelCollider[] AllWheelColliders {

        get {

            if (_allWheelColliders == null || (_allWheelColliders != null && _allWheelColliders.Length < 1))
                _allWheelColliders = GetComponentsInChildren<RCCP_WheelCollider>(true);

            return _allWheelColliders;

        }
        set {

            _allWheelColliders = value;

        }

    }

    //  Aerodynamics.
    public RCCP_AeroDynamics AeroDynamics {

        get {

            if (!componentsTaken && _aero == null)
                _aero = GetComponentInChildren<RCCP_AeroDynamics>(true);

            return _aero;

        }
        set {

            _aero = value;

        }

    }

    //  Inputs.
    public RCCP_Input Inputs {

        get {

            if (!componentsTaken && _inputs == null)
                _inputs = GetComponentInChildren<RCCP_Input>(true);

            return _inputs;

        }
        set {

            _inputs = value;

        }

    }

    //  Audio.
    public RCCP_Audio Audio {

        get {

            if (!componentsTaken && _audio == null)
                _audio = GetComponentInChildren<RCCP_Audio>(true);

            return _audio;

        }
        set {

            _audio = value;

        }

    }

    //  Lights.
    public RCCP_Lights Lights {

        get {

            if (!componentsTaken && _lights == null)
                _lights = GetComponentInChildren<RCCP_Lights>(true);

            return _lights;

        }
        set {

            _lights = value;

        }

    }

    //  Stability.
    public RCCP_Stability Stability {

        get {

            if (!componentsTaken && _stability == null)
                _stability = GetComponentInChildren<RCCP_Stability>(true);

            return _stability;

        }
        set {

            _stability = value;

        }

    }

    //  Damage.
    public RCCP_Damage Damage {

        get {

            if (!componentsTaken && _damage == null)
                _damage = GetComponentInChildren<RCCP_Damage>(true);

            return _damage;

        }
        set {

            _damage = value;

        }

    }

    //  Particles.
    public RCCP_Particles Particles {

        get {

            if (!componentsTaken && _particles == null)
                _particles = GetComponentInChildren<RCCP_Particles>(true);

            return _particles;

        }
        set {

            _particles = value;

        }

    }

    //  Other addons.
    public RCCP_OtherAddons OtherAddonsManager {

        get {

            if (!componentsTaken && _otherAddons == null)
                _otherAddons = GetComponentInChildren<RCCP_OtherAddons>(true);

            return _otherAddons;

        }
        set {

            _otherAddons = value;

        }

    }

    //  Private fields for components.
    private Rigidbody _rigid = null;
    private RCCP_Input _inputs = null;
    private RCCP_Engine _engine = null;
    private RCCP_Clutch _clutch = null;
    private RCCP_Gearbox _gearbox = null;
    private RCCP_Differential _differential = null;
    private RCCP_Axles _axles = null;
    private RCCP_Axle _axleFront = null;
    private RCCP_Axle _axleRear = null;
    private RCCP_AeroDynamics _aero = null;
    private RCCP_Audio _audio = null;
    private RCCP_Lights _lights = null;
    private RCCP_Stability _stability = null;
    private RCCP_Damage _damage = null;
    private RCCP_Particles _particles = null;
    private RCCP_OtherAddons _otherAddons = null;
    private RCCP_WheelCollider[] _allWheelColliders = null;

    public bool componentsTaken = false;

    #endregion

#if UNITY_EDITOR
    [HideInInspector] public bool checkComponents = false;
#endif

    #region STATS

    public float engineRPM = 0f;        //  Current engine rpm.
    public float minEngineRPM = 800f;       //  Minimum engine rpm.
    public float maxEngineRPM = 8000f;      //  Maximum engine rpm.

    public int currentGear = 0;     //  Current gear.
    public float currentGearRatio = 1f;     //  Current gear ratio.
    public float lastGearRatio = 1f;        //  Last gear ratio.
    public float differentialRatio = 1f;        //  Differential ratio.

    public float speed = 0f;        //  Speed of the vehicle.

    [System.Obsolete("You can use 'speed' variable instead of 'physicalSpeed'.")]
    public float physicalSpeed {

        get {

            return speed;

        }

    }

    public float wheelRPM2Speed = 0f;       //  Wheel speed of the vehicle.
    public float maximumSpeed = 0f;     //  Maximum speed of the vehicle related to engine rpm, gear ratio, differential ratio, and wheel diameter.

    public float tractionWheelRPM2EngineRPM = 0f;       //  RPM of the traction wheels.
    public float targetWheelSpeedForCurrentGear = 0f;       //  Target wheel speed for current gear.

    public float producedEngineTorque = 0f;     //  Produced engine torque.
    public float producedGearboxTorque = 0f;        //  Produced gearbox torque.
    public float producedDifferentialTorque = 0f;       //  Produced differential torque.

    //  Axles.
    public List<RCCP_Axle> poweredAxles = new List<RCCP_Axle>();
    public List<RCCP_Axle> brakedAxles = new List<RCCP_Axle>();
    public List<RCCP_Axle> steeredAxles = new List<RCCP_Axle>();
    public List<RCCP_Axle> handbrakedAxles = new List<RCCP_Axle>();

    public int direction = 1;       //  1 = Forward, -1 = Reverse.

    public bool engineStarting = false;     //  Is engine starting now?
    public bool engineRunning = false;      //  Is engine running now?
    public bool shiftingNow = false;        //  Is gearbox shifting now?
    public bool reversingNow = false;       //  Is reversing now?
    public float steerAngle = 0f;       //  Current steer angle.

    //  Inputs of the vehicle. These values taken from the components, not player inputs.
    public float fuelInput_V = 0f;
    public float throttleInput_V = 0f;
    public float brakeInput_V = 0f;
    public float steerInput_V = 0f;
    public float handbrakeInput_V = 0f;
    public float clutchInput_V = 0f;
    public float gearInput_V = 1f;
    public float nosInput_V = 0f;

    //  Inputs of the player. These values taken from the player inputs, not components.
    public float throttleInput_P = 0f;
    public float brakeInput_P = 0f;
    public float steerInput_P = 0f;
    public float handbrakeInput_P = 0f;
    public float clutchInput_P = 0f;
    public float nosInput_P = 0f;

    //  Light states.
    public bool lowBeamLights = false;
    public bool highBeamLights = false;
    public bool indicatorsLeftLights = false;
    public bool indicatorsRightLights = false;
    public bool indicatorsAllLights = false;

    #endregion

    /// <summary>
    /// Vehicle is grounded or not?
    /// </summary>
    public bool IsGrounded {

        get {

            bool grounded = false;

            if (AxleManager != null && AxleManager.Axles.Count >= 1) {

                for (int i = 0; i < AxleManager.Axles.Count; i++) {

                    if (AxleManager.Axles[i].isGrounded)
                        grounded = true;

                }

            }

            return grounded;

        }

    }

    private void Awake() {

        //  Make sure some variables are set to default values before doing anything.
        engineRPM = 0f;
        currentGear = 0;
        currentGearRatio = 1f;
        lastGearRatio = 1f;
        differentialRatio = 1f;
        speed = 0f;
        wheelRPM2Speed = 0f;
        tractionWheelRPM2EngineRPM = 0f;
        targetWheelSpeedForCurrentGear = 0f;
        maximumSpeed = 0f;
        producedEngineTorque = 0f;
        producedGearboxTorque = 0f;
        producedDifferentialTorque = 0f;
        direction = 1;
        engineStarting = false;
        engineRunning = false;
        shiftingNow = false;
        reversingNow = false;
        steerAngle = 0f;
        fuelInput_V = 0f;
        throttleInput_V = 0f;
        brakeInput_V = 0f;
        steerInput_V = 0f;
        handbrakeInput_V = 0f;
        clutchInput_V = 0f;
        gearInput_V = 0f;
        nosInput_V = 0f;
        throttleInput_P = 0f;
        brakeInput_P = 0f;
        steerInput_P = 0f;
        handbrakeInput_P = 0f;
        clutchInput_P = 0f;
        nosInput_P = 0f;
        lowBeamLights = false;
        highBeamLights = false;
        indicatorsLeftLights = false;
        indicatorsRightLights = false;
        indicatorsAllLights = false;

    }

    /// <summary>
    /// Getting all connected components.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ComponentsTaken() {

        //  Getting all connected components for 0.04 seconds. Usually, all components are registering themself onenable, but
        //  I wanted to double check. This process may be heavy due to get methods, that's why I'm checking them for 0.04 seconds.
        componentsTaken = false;
        yield return new WaitForSeconds(.04f);
        componentsTaken = true;

    }

    /// <summary>
    /// Updates all connected components.
    /// </summary>
    public void UpdateComponents() {

        //  Getting all connected components for 0.04 seconds. Usually, all components are registering themself onenable, but
        //  I wanted to double check. This process may be heavy due to get methods, that's why I'm checking them for 0.04 seconds.
        StartCoroutine(ComponentsTaken());

    }

    private void OnEnable() {

        //  Getting all connected components.
        StartCoroutine(ComponentsTaken());

        //  Firing an event when a vehicle spawned.
        RCCP_Events.Event_OnRCCPSpawned(this);

        //  Listening an event when behavior changed.
        RCCP_SceneManager.OnBehaviorChanged += CheckBehavior;

        //  Checking current behavior if selected.
        CheckBehavior();

    }

    private void Update() {

        //  Finding axles for steer, brake, torque, and handbrake.
        FindAxles();

        //  Receiving player inputs.
        PlayerInputs();

        //  Receiving vehicle inputs taken from the components.
        VehicleInputs();

        //  Calculating drivetrain components.
        Drivetrain();

    }

    /// <summary>
    /// Checking and finding the axles at runtime.
    /// </summary>
    private void FindAxles() {

        //  Creating lists for axles.
        if (poweredAxles == null)
            poweredAxles = new List<RCCP_Axle>();

        if (brakedAxles == null)
            brakedAxles = new List<RCCP_Axle>();

        if (steeredAxles == null)
            steeredAxles = new List<RCCP_Axle>();

        if (handbrakedAxles == null)
            handbrakedAxles = new List<RCCP_Axle>();

        //  Clearing axle lists.
        poweredAxles.Clear();
        brakedAxles.Clear();
        steeredAxles.Clear();
        handbrakedAxles.Clear();

        //  Finding Powered axles.
        if (AxleManager != null && AxleManager.Axles.Count >= 1) {

            for (int i = 0; i < AxleManager.Axles.Count; i++) {

                if (AxleManager.Axles[i].isPower)
                    poweredAxles.Add(AxleManager.Axles[i]);

            }

            //  Finding Braked axles.
            for (int i = 0; i < AxleManager.Axles.Count; i++) {

                if (AxleManager.Axles[i].isBrake)
                    brakedAxles.Add(AxleManager.Axles[i]);

            }

            //  Finding Steered axles.
            for (int i = 0; i < AxleManager.Axles.Count; i++) {

                if (AxleManager.Axles[i].isSteer)
                    steeredAxles.Add(AxleManager.Axles[i]);

            }

            //  Finding Handbraked axles.
            for (int i = 0; i < AxleManager.Axles.Count; i++) {

                if (AxleManager.Axles[i].isHandbrake)
                    handbrakedAxles.Add(AxleManager.Axles[i]);

            }

        }

    }

    /// <summary>
    /// Finding drivetrain components.
    /// </summary>
    private void Drivetrain() {

        //  Getting important variables from the engine.
        if (Engine) {

            engineStarting = Engine.engineStarting;
            engineRunning = Engine.engineRunning;
            engineRPM = Engine.engineRPM;
            minEngineRPM = Engine.minEngineRPM;
            maxEngineRPM = Engine.maxEngineRPM;

        }

        //  Getting important variables from the gearbox.
        if (Gearbox) {

            currentGear = Gearbox.currentGear;
            currentGearRatio = Gearbox.gearRatios[currentGear];
            lastGearRatio = Gearbox.gearRatios[Gearbox.gearRatios.Length - 1];

            if (Gearbox.reverseGearEngaged)
                direction = -1;
            else
                direction = 1;

            shiftingNow = Gearbox.shiftingNow;
            reversingNow = Gearbox.reverseGearEngaged;

        }

        //  Getting important variables from the differential.
        if (Differential)
            differentialRatio = Differential.finalDriveRatio;

        //  Calculating average traction wheel rpm.
        float averagePowerWheelRPM = 0f;

        if (poweredAxles != null) {

            for (int i = 0; i < poweredAxles.Count; i++) {

                if (poweredAxles[i].leftWheelCollider && poweredAxles[i].leftWheelCollider.WheelCollider.enabled)
                    averagePowerWheelRPM += Mathf.Abs(poweredAxles[i].leftWheelCollider.WheelCollider.rpm);

                if (poweredAxles[i].rightWheelCollider && poweredAxles[i].rightWheelCollider.WheelCollider.enabled)
                    averagePowerWheelRPM += Mathf.Abs(poweredAxles[i].rightWheelCollider.WheelCollider.rpm);

            }

            if (averagePowerWheelRPM > .1f)
                averagePowerWheelRPM /= (float)Mathf.Clamp(poweredAxles.Count * 2f, 1f, 40f);

        }

        //  Calculating average traction wheel radius.
        float averagePowerWheelRadius = 0f;

        if (poweredAxles != null) {

            for (int i = 0; i < poweredAxles.Count; i++) {

                if (poweredAxles[i].leftWheelCollider && poweredAxles[i].leftWheelCollider.WheelCollider.enabled)
                    averagePowerWheelRadius += poweredAxles[i].leftWheelCollider.WheelCollider.radius;

                if (poweredAxles[i].rightWheelCollider && poweredAxles[i].rightWheelCollider.WheelCollider.enabled)
                    averagePowerWheelRadius += poweredAxles[i].rightWheelCollider.WheelCollider.radius;

            }

            if (averagePowerWheelRadius >= .1f)
                averagePowerWheelRadius /= (float)Mathf.Clamp(poweredAxles.Count * 2f, 1f, 40f);

        }

        //  Calculating speed as km/h unit.
        speed = transform.InverseTransformDirection(Rigid.velocity).z * 3.6f;

        //  Converting traction wheel rpm to engine rpm.
        tractionWheelRPM2EngineRPM = (averagePowerWheelRPM * differentialRatio * currentGearRatio) * (1f - clutchInput_V) * gearInput_V;

        //  Converting wheel rpm to speed as km/h unit.
        wheelRPM2Speed = (averagePowerWheelRPM * averagePowerWheelRadius * Mathf.PI * 2f) * 60f / 1000f;

        //  Calculating target max speed for the current gear.
        targetWheelSpeedForCurrentGear = engineRPM / currentGearRatio / differentialRatio;
        targetWheelSpeedForCurrentGear *= (averagePowerWheelRadius * Mathf.PI * 2f) * 60f / 1000f;

        //  Calculating max speed at last gear as km/h unit.
        maximumSpeed = maxEngineRPM / lastGearRatio / differentialRatio;
        maximumSpeed = (maximumSpeed * 60f) / 1000f;
        maximumSpeed *= 2 * Mathf.PI * averagePowerWheelRadius;

        //  Produced torques.
        if (Engine)
            producedEngineTorque = Engine.producedTorqueAsNM;

        if (Gearbox)
            producedGearboxTorque = Gearbox.producedTorqueAsNM;

        if (Differential)
            producedDifferentialTorque = Differential.producedTorqueAsNM;

    }

    /// <summary>
    /// Inputs of the vehicle, not player. 
    /// </summary>
    private void VehicleInputs() {

        //  Resetting all inputs to 0 before assigning them.
        fuelInput_V = 0f;
        throttleInput_V = 0f;
        brakeInput_V = 0f;
        steerInput_V = 0f;
        handbrakeInput_V = 0f;
        clutchInput_V = 0f;
        gearInput_V = 0f;
        nosInput_V = 0f;

        //  Fuel input of the engine.
        if (Engine)
            fuelInput_V = Engine.fuelInput;

        //  Throttle input.
        if (poweredAxles != null) {

            for (int i = 0; i < poweredAxles.Count; i++)
                throttleInput_V += poweredAxles[i].throttleInput;

            throttleInput_V /= (float)Mathf.Clamp(poweredAxles.Count, 1, 20);

        }

        //  Brake input.
        if (brakedAxles != null) {

            for (int i = 0; i < brakedAxles.Count; i++)
                brakeInput_V += brakedAxles[i].brakeInput;

            brakeInput_V /= (float)Mathf.Clamp(brakedAxles.Count, 1, 20);

        }

        //  Steer input.
        if (steeredAxles != null) {

            for (int i = 0; i < steeredAxles.Count; i++)
                steerInput_V += steeredAxles[i].steerInput;

            steerInput_V /= (float)Mathf.Clamp(steeredAxles.Count, 1, 20);

        }


        //  Handbrake input.
        if (handbrakedAxles != null) {

            for (int i = 0; i < handbrakedAxles.Count; i++)
                handbrakeInput_V += handbrakedAxles[i].handbrakeInput;

            handbrakeInput_V /= (float)Mathf.Clamp(handbrakedAxles.Count, 1, 20);

        }

        //  Clutch input.
        if (Clutch)
            clutchInput_V = Clutch.clutchInput;

        //  Gearbox input.
        if (Gearbox)
            gearInput_V = Gearbox.gearInput;

        //  Nos input.
        if (OtherAddonsManager && OtherAddonsManager.Nos)
            nosInput_V = OtherAddonsManager.Nos.nosInUse ? 1f : 0f;

        //  Lights input.
        if (Lights) {

            lowBeamLights = Lights.lowBeamHeadlights;
            highBeamLights = Lights.highBeamHeadlights;
            indicatorsLeftLights = Lights.indicatorsLeft;
            indicatorsRightLights = Lights.indicatorsRight;
            indicatorsAllLights = Lights.indicatorsAll;

        }

        if (FrontAxle)
            steerAngle = FrontAxle.steerAngle;

    }

    /// <summary>
    /// Player inputs, not vehicle inputs.
    /// </summary>
    private void PlayerInputs() {

        //  Early out if vehicle has no input component.
        if (!Inputs)
            return;

        //  Getting player inputs.
        throttleInput_P = Inputs.throttleInput;
        brakeInput_P = Inputs.brakeInput;
        steerInput_P = Inputs.steerInput;
        handbrakeInput_P = Inputs.handbrakeInput;
        clutchInput_P = Inputs.clutchInput;
        nosInput_P = Inputs.nosInput;

    }

    /// <summary>
    /// Sets controllable state of the vehicle.
    /// </summary>
    /// <param name="state"></param>
    public void SetCanControl(bool state) {

        canControl = state;

    }

    /// <summary>
    /// Starts the engine.
    /// </summary>
    public void StartEngine() {

        if (Engine)
            Engine.engineRunning = true;

    }

    /// <summary>
    /// Kills the engine.
    /// </summary>
    public void KillEngine() {

        if (Engine)
            Engine.engineRunning = false;

    }

    /// <summary>
    /// Sets the engine.
    /// </summary>
    /// <param name="state"></param>
    public void SetEngine(bool state) {

        if (Engine)
            Engine.engineRunning = state;

    }

    /// <summary>
    /// Firing an event on collisions, and calling methods in the damage and particle components if they are attached.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision) {

        RCCP_Events.Event_OnRCCPCollision(this, collision);

        if (Damage)
            Damage.OnCollisionEnter(collision);

        if (Particles)
            Particles.OnCollisionEnter(collision);

        if (Audio)
            Audio.OnCollision(collision);

    }

    /// <summary>
    /// Firing an event on collisions, and calling methods in the particle component if attached.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionStay(Collision collision) {

        RCCP_Events.Event_OnRCCPCollision(this, collision);

        if (Particles)
            Particles.OnCollisionStay(collision);

    }

    /// <summary>
    /// Firing an event on collision exits, and calling methods in the particle component if attached.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionExit(Collision collision) {

        if (Particles)
            Particles.OnCollisionExit(collision);

    }

    /// <summary>
    /// When a wheel is deflated, call method in the audio component if attached.
    /// </summary>
    public void OnWheelDeflated() {

        if (Audio)
            Audio.DeflateWheel();

    }

    /// <summary>
    /// When a wheel is inflated, call method in the audio component if attached.
    /// </summary>
    public void OnWheelInflated() {

        if (Audio)
            Audio.InflateWheel();

    }

    /// <summary>
    /// Checks and overrides the behavior selected in the RCCP_Settings.
    /// </summary>
    private void CheckBehavior() {

        //	If override is enabled, return.
        if (ineffectiveBehavior)
            return;

        //  Early out if override behavior is disabled in the RCCP_Settings.
        if (!RCCP_Settings.Instance.overrideBehavior)
            return;

        //	If selected behavior is none, return as well.
        if (RCCP_Settings.Instance.SelectedBehaviorType == null)
            return;

        StartCoroutine(CheckBehaviorDelayed());

    }

    private IEnumerator CheckBehaviorDelayed() {

        yield return new WaitForFixedUpdate();

        // If any behavior is selected in RCCP_Settings, override changes.
        RCCP_Settings.BehaviorType currentBehaviorType = RCCP_Settings.Instance.SelectedBehaviorType;

        //  Setting angular drag of the rigidbody.
        Rigid.angularDrag = currentBehaviorType.angularDrag;

        //  Setting stability settings if attached to the vehicle.
        if (Stability) {

            Stability.ABS = currentBehaviorType.ABS;
            Stability.ESP = currentBehaviorType.ESP;
            Stability.TCS = currentBehaviorType.TCS;

            Stability.steeringHelper = currentBehaviorType.steeringHelper;
            Stability.tractionHelper = currentBehaviorType.tractionHelper;
            Stability.angularDragHelper = currentBehaviorType.angularDragHelper;
            Stability.turnHelper = currentBehaviorType.turnHelper;

            Stability.steerHelperStrength = Mathf.Clamp(Stability.steerHelperStrength, currentBehaviorType.steeringHelperStrengthMinimum, currentBehaviorType.steeringHelperStrengthMaximum);
            Stability.tractionHelperStrength = Mathf.Clamp(Stability.tractionHelperStrength, currentBehaviorType.tractionHelperStrengthMinimum, currentBehaviorType.tractionHelperStrengthMaximum);
            Stability.angularDragHelperStrength = Mathf.Clamp(Stability.angularDragHelperStrength, currentBehaviorType.angularDragHelperMinimum, currentBehaviorType.angularDragHelperMaximum);
            Stability.turnHelperStrength = Mathf.Clamp(Stability.turnHelperStrength, currentBehaviorType.turnHelperStrengthMinimum, currentBehaviorType.turnHelperStrengthMaximum);

        }

        //  Setting input settings if attached to the vehicle.
        if (Inputs) {

            Inputs.steeringCurve = currentBehaviorType.steeringCurve;
            Inputs.steeringLimiter = currentBehaviorType.limitSteering;
            Inputs.counterSteering = currentBehaviorType.counterSteering;
            Inputs.counterSteerFactor = Mathf.Clamp(Inputs.counterSteerFactor, currentBehaviorType.counterSteeringMinimum, currentBehaviorType.counterSteeringMaximum);

            Inputs.ResetInputs();

        }

        //  Setting axle settings if attached to the vehicle.
        if (AxleManager != null && AxleManager.Axles.Count > 1) {

            for (int i = 0; i < AxleManager.Axles.Count; i++) {

                AxleManager.Axles[i].antirollForce = Mathf.Clamp(AxleManager.Axles[i].antirollForce, currentBehaviorType.antiRollMinimum, Mathf.Infinity);
                AxleManager.Axles[i].steerSpeed = Mathf.Clamp(AxleManager.Axles[i].steerSpeed, currentBehaviorType.steeringSpeedMinimum, currentBehaviorType.steeringSpeedMaximum);

                if (AxleManager.Axles[i].leftWheelCollider) {

                    AxleManager.Axles[i].leftWheelCollider.driftMode = currentBehaviorType.driftMode;


                    if (AxleManager.Axles[i].leftWheelCollider.transform.localPosition.z > 0) {

                        AxleManager.Axles[i].leftWheelCollider.SetFrictionCurvesForward(currentBehaviorType.forwardExtremumSlip_F, currentBehaviorType.forwardExtremumValue_F, currentBehaviorType.forwardAsymptoteSlip_F, currentBehaviorType.forwardAsymptoteValue_F);
                        AxleManager.Axles[i].leftWheelCollider.SetFrictionCurvesSideways(currentBehaviorType.sidewaysExtremumSlip_F, currentBehaviorType.sidewaysExtremumValue_F, currentBehaviorType.sidewaysAsymptoteSlip_F, currentBehaviorType.sidewaysAsymptoteValue_F);

                    } else {

                        AxleManager.Axles[i].leftWheelCollider.SetFrictionCurvesForward(currentBehaviorType.forwardExtremumSlip_R, currentBehaviorType.forwardExtremumValue_R, currentBehaviorType.forwardAsymptoteSlip_R, currentBehaviorType.forwardAsymptoteValue_R);
                        AxleManager.Axles[i].leftWheelCollider.SetFrictionCurvesSideways(currentBehaviorType.sidewaysExtremumSlip_R, currentBehaviorType.sidewaysExtremumValue_R, currentBehaviorType.sidewaysAsymptoteSlip_R, currentBehaviorType.sidewaysAsymptoteValue_R);

                    }

                }

                if (AxleManager.Axles[i].rightWheelCollider) {

                    AxleManager.Axles[i].rightWheelCollider.driftMode = currentBehaviorType.driftMode;

                    if (AxleManager.Axles[i].rightWheelCollider.transform.localPosition.z > 0) {

                        AxleManager.Axles[i].rightWheelCollider.SetFrictionCurvesForward(currentBehaviorType.forwardExtremumSlip_F, currentBehaviorType.forwardExtremumValue_F, currentBehaviorType.forwardAsymptoteSlip_F, currentBehaviorType.forwardAsymptoteValue_F);
                        AxleManager.Axles[i].rightWheelCollider.SetFrictionCurvesSideways(currentBehaviorType.sidewaysExtremumSlip_F, currentBehaviorType.sidewaysExtremumValue_F, currentBehaviorType.sidewaysAsymptoteSlip_F, currentBehaviorType.sidewaysAsymptoteValue_F);

                    } else {

                        AxleManager.Axles[i].rightWheelCollider.SetFrictionCurvesForward(currentBehaviorType.forwardExtremumSlip_R, currentBehaviorType.forwardExtremumValue_R, currentBehaviorType.forwardAsymptoteSlip_R, currentBehaviorType.forwardAsymptoteValue_R);
                        AxleManager.Axles[i].rightWheelCollider.SetFrictionCurvesSideways(currentBehaviorType.sidewaysExtremumSlip_R, currentBehaviorType.sidewaysExtremumValue_R, currentBehaviorType.sidewaysAsymptoteSlip_R, currentBehaviorType.sidewaysAsymptoteValue_R);

                    }

                }

            }

        }

        //  Setting gearbox settings if attached to the vehicle.
        if (Gearbox) {

            Gearbox.shiftThreshold = currentBehaviorType.gearShiftingThreshold;
            Gearbox.shiftingTime = Mathf.Clamp(Gearbox.shiftingTime, currentBehaviorType.gearShiftingDelayMinimum, currentBehaviorType.gearShiftingDelayMaximum);

        }

        //  Setting differential settings if attached to the vehicle.
        if (Differential)
            Differential.differentialType = currentBehaviorType.differentialType;

    }

    private void OnDisable() {

        //  Firing an event when disabing / destroying the vehicle.
        RCCP_Events.Event_OnRCCPDestroyed(this);
        RCCP_SceneManager.OnBehaviorChanged -= CheckBehavior;

    }

    private void Reset() {

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 1350f;
        rigidbody.drag = .01f;
        rigidbody.angularDrag = .25f;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

    }

}
