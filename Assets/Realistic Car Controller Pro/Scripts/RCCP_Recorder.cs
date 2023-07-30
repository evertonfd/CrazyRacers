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

/// <summary>
/// Record / Replay system. Saves player's input, vehicle rigid velocity, position, and rotation on record, and replays it when on playback.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Recorder")]
public class RCCP_Recorder : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    /// <summary>
    /// Recorded clip.
    /// </summary>
    [System.Serializable]
    public class RecordedClip {

        public string recordName = "New Record";        //  Record name.

        [HideInInspector] public VehicleInput[] inputs;      //  All vehicle inputs recorded frame by frame.
        [HideInInspector] public VehicleTransform[] transforms;      //  All position and rotation recorded frame by frame.
        [HideInInspector] public VehicleVelocity[] rigids;      //  All velocities recorded frame by frame.

        /// <summary>
        /// Saved clip.
        /// </summary>
        /// <param name="_inputs"></param>
        /// <param name="_transforms"></param>
        /// <param name="_rigids"></param>
        /// <param name="_recordName"></param>
        public RecordedClip(VehicleInput[] _inputs, VehicleTransform[] _transforms, VehicleVelocity[] _rigids, string _recordName) {

            inputs = _inputs;
            transforms = _transforms;
            rigids = _rigids;
            recordName = _recordName;

        }

    }

    public RecordedClip recorded;       //  Last recorded clip.

    private List<VehicleInput> Inputs;        //  Vehicle inputs, not player inputs.
    private List<VehicleTransform> Transforms;        //  Positions and rotations.
    private List<VehicleVelocity> Rigidbodies;       //  Velocities.

    /// <summary>
    /// Inputs of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleInput {

        public float throttleInput = 0f;
        public float brakeInput = 0f;
        public float steerInput = 0f;
        public float handbrakeInput = 0f;
        public float clutchInput = 0f;
        public float nosInput = 0f;
        public int direction = 1;
        public int currentGear = 0;
        public bool changingGear = false;

        public bool lowBeamHeadLightsOn = false;
        public bool highBeamHeadLightsOn = false;
        public bool indicatorsLeft;
        public bool indicatorsRight;
        public bool indicatorsAll;

        public VehicleInput(float _gasInput, float _brakeInput, float _steerInput, float _handbrakeInput, float _clutchInput, float _boostInput, int _direction, int _currentGear, bool _changingGear, bool _lowBeamHeadLightsOn, bool _highBeamHeadLightsOn, bool _indicatorsLeft, bool _indicatorsRight, bool _indicatorsAll) {

            throttleInput = _gasInput;
            brakeInput = _brakeInput;
            steerInput = _steerInput;
            handbrakeInput = _handbrakeInput;
            clutchInput = _clutchInput;
            nosInput = _boostInput;
            direction = _direction;
            currentGear = _currentGear;
            changingGear = _changingGear;

            lowBeamHeadLightsOn = _lowBeamHeadLightsOn;
            highBeamHeadLightsOn = _highBeamHeadLightsOn;
            indicatorsLeft = _indicatorsLeft;
            indicatorsRight = _indicatorsRight;
            indicatorsAll = _indicatorsAll;

        }

    }

    /// <summary>
    /// Position and rotation of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleTransform {

        public Vector3 position;
        public Quaternion rotation;

        public VehicleTransform(Vector3 _pos, Quaternion _rot) {

            position = _pos;
            rotation = _rot;

        }

    }

    /// <summary>
    /// Linear and angular velocity of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleVelocity {

        public Vector3 velocity;
        public Vector3 angularVelocity;

        public VehicleVelocity(Vector3 _vel, Vector3 _angVel) {

            velocity = _vel;
            angularVelocity = _angVel;

        }

    }

    //  Current state.
    public enum Mode { Neutral, Play, Record }
    public Mode mode = Mode.Neutral;

    private void Awake() {

        //  Creating new lists for inputs, transforms, and rigids.
        Inputs = new List<VehicleInput>();
        Transforms = new List<VehicleTransform>();
        Rigidbodies = new List<VehicleVelocity>();

    }

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.Recorder = this;
        else
            enabled = false;

        // Listening input events.
        RCCP_InputManager.OnRecord += RCC_InputManager_OnRecord;
        RCCP_InputManager.OnReplay += RCC_InputManager_OnReplay;

    }

    /// <summary>
    /// Replay.
    /// </summary>
    private void RCC_InputManager_OnReplay() {

        Play();

    }

    /// <summary>
    /// Record
    /// </summary>
    private void RCC_InputManager_OnRecord() {

        Record();

    }

    /// <summary>
    /// Record.
    /// </summary>
    public void Record() {

        //  If current state is not record, set it to record. Otherwise set it to neutral and save the clip.
        if (mode != Mode.Record) {

            mode = Mode.Record;

        } else {

            mode = Mode.Neutral;
            SaveRecord();

        }

        //  If mode is set to record before, clear all lists. That means we've saved the clip.
        if (mode == Mode.Record) {

            Inputs.Clear();
            Transforms.Clear();
            Rigidbodies.Clear();

        }

    }

    /// <summary>
    /// Save record clip.
    /// </summary>
    public void SaveRecord() {

        print("Record saved!");
        recorded = new RecordedClip(Inputs.ToArray(), Transforms.ToArray(), Rigidbodies.ToArray(), RCCP_Records.Instance.records.Count.ToString() + "_" + CarController.transform.name);
        RCCP_Records.Instance.records.Add(recorded);

    }

    /// <summary>
    /// Play.
    /// </summary>
    public void Play() {

        //  If clip not found, return.
        if (recorded == null)
            return;

        //  If current state is not play, set it to play. Otherwise set it to neutral.
        if (mode != Mode.Play)
            mode = Mode.Play;
        else
            mode = Mode.Neutral;

        //  If current state is play, enable external controller of the car controller.
        if (mode == Mode.Play)
            Override(true);
        else
            Override(false);

        if (mode == Mode.Play) {

            StartCoroutine(Replay());

            if (recorded != null && recorded.transforms.Length > 0)
                CarController.transform.SetPositionAndRotation(recorded.transforms[0].position, recorded.transforms[0].rotation);

            StartCoroutine(Revel());

        }

    }

    /// <summary>
    /// Play.
    /// </summary>
    /// <param name="_recorded"></param>
    public void Play(RecordedClip _recorded) {

        recorded = _recorded;

        print("Replaying record " + recorded.recordName);

        if (recorded == null)
            return;

        if (mode != Mode.Play)
            mode = Mode.Play;
        else
            mode = Mode.Neutral;

        if (mode == Mode.Play)
            Override(true);
        else
            Override(false);

        if (mode == Mode.Play) {

            StartCoroutine(Replay());

            if (recorded != null && recorded.transforms.Length > 0)
                CarController.transform.SetPositionAndRotation(recorded.transforms[0].position, recorded.transforms[0].rotation);

            StartCoroutine(Revel());

        }

    }

    /// <summary>
    /// Stop.
    /// </summary>
    public void Stop() {

        mode = Mode.Neutral;
        Override(false);

    }

    /// <summary>
    /// Replay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Replay() {

        for (int i = 0; i < recorded.inputs.Length && mode == Mode.Play; i++) {

            Override(true);

            RCCP_Inputs inputs = new RCCP_Inputs();

            inputs.throttleInput = recorded.inputs[i].throttleInput;
            inputs.brakeInput = recorded.inputs[i].brakeInput;
            inputs.steerInput = recorded.inputs[i].steerInput;
            inputs.handbrakeInput = recorded.inputs[i].handbrakeInput;
            inputs.clutchInput = recorded.inputs[i].clutchInput;
            inputs.nosInput = recorded.inputs[i].nosInput;

            if (CarController.Inputs)
                CarController.Inputs.OverrideInputs(inputs);

            if (CarController.Gearbox)
                CarController.Gearbox.OverrideGear(recorded.inputs[i].currentGear, recorded.inputs[i].direction == 1 ? false : true);

            CarController.direction = recorded.inputs[i].direction;
            CarController.currentGear = recorded.inputs[i].currentGear;
            CarController.shiftingNow = recorded.inputs[i].changingGear;

            if (CarController.Lights) {

                CarController.Lights.lowBeamHeadlights = recorded.inputs[i].lowBeamHeadLightsOn;
                CarController.Lights.highBeamHeadlights = recorded.inputs[i].highBeamHeadLightsOn;
                CarController.Lights.indicatorsLeft = recorded.inputs[i].indicatorsLeft;
                CarController.Lights.indicatorsRight = recorded.inputs[i].indicatorsRight;
                CarController.Lights.indicatorsAll = recorded.inputs[i].indicatorsAll;

            }

            yield return new WaitForFixedUpdate();

        }

        mode = Mode.Neutral;

        Override(false);

    }

    private IEnumerator Revel() {

        for (int i = 0; i < recorded.rigids.Length && mode == Mode.Play; i++) {

            CarController.Rigid.velocity = recorded.rigids[i].velocity;
            CarController.Rigid.angularVelocity = recorded.rigids[i].angularVelocity;

            yield return new WaitForFixedUpdate();

        }

        mode = Mode.Neutral;

        Override(false);

    }

    private void FixedUpdate() {

        if (!CarController)
            return;

        switch (mode) {

            case Mode.Neutral:

                break;

            case Mode.Play:

                Override(true);

                break;

            case Mode.Record:

                Inputs.Add(new VehicleInput(CarController.throttleInput_V, CarController.brakeInput_V, CarController.steerInput_V, CarController.handbrakeInput_V, CarController.clutchInput_V, CarController.nosInput_V, CarController.direction, CarController.currentGear, CarController.shiftingNow, CarController.lowBeamLights, CarController.highBeamLights, CarController.indicatorsLeftLights, CarController.indicatorsRightLights, CarController.indicatorsAllLights));
                Transforms.Add(new VehicleTransform(CarController.transform.position, CarController.transform.rotation));
                Rigidbodies.Add(new VehicleVelocity(CarController.Rigid.velocity, CarController.Rigid.angularVelocity));

                break;

        }

    }

    private void Override(bool overrideState) {

        if (CarController.Inputs)
            CarController.Inputs.overrideInternalInputs = CarController.Inputs.overrideExternalInputs = overrideState;

        if (CarController.Gearbox)
            CarController.Gearbox.overrideGear = overrideState;

    }

    private void OnDisable() {

        // Listening input events.
        RCCP_InputManager.OnRecord -= RCC_InputManager_OnRecord;
        RCCP_InputManager.OnReplay -= RCC_InputManager_OnReplay;

    }

}
