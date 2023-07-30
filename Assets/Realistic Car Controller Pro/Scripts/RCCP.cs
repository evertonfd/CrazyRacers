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

///<summary>
/// API for instantiating, registering new RCCP vehicles, and changes at runtime.
///</summary>
public class RCCP {

    ///<summary>
    /// Spawn a RCCP vehicle prefab with given position, rotation, sets its controllable, and engine state.
    ///</summary>
    public static RCCP_CarController SpawnRCC(RCCP_CarController vehiclePrefab, Vector3 position, Quaternion rotation, bool registerAsPlayerVehicle, bool isControllable, bool isEngineRunning) {

        RCCP_CarController spawnedRCC = GameObject.Instantiate(vehiclePrefab, position, rotation);
        spawnedRCC.gameObject.SetActive(true);
        spawnedRCC.SetCanControl(isControllable);

        if (registerAsPlayerVehicle)
            RCCP_SceneManager.Instance.RegisterPlayer(spawnedRCC);

        if (isEngineRunning)
            spawnedRCC.StartEngine();
        else
            spawnedRCC.KillEngine();

        return spawnedRCC;

    }

    ///<summary>
    /// Registers the vehicle as player vehicle. 
    ///</summary>
    public static void RegisterPlayerVehicle(RCCP_CarController vehicle) {

        RCCP_SceneManager.Instance.RegisterPlayer(vehicle);

    }

    ///<summary>
    /// Registers the vehicle as player vehicle with controllable state. 
    ///</summary>
    public static void RegisterPlayerVehicle(RCCP_CarController vehicle, bool isControllable) {

        RCCP_SceneManager.Instance.RegisterPlayer(vehicle, isControllable);

    }

    ///<summary>
    /// Registers the vehicle as player vehicle with controllable and engine state. 
    ///</summary>
    public static void RegisterPlayerVehicle(RCCP_CarController vehicle, bool isControllable, bool engineState) {

        RCCP_SceneManager.Instance.RegisterPlayer(vehicle, isControllable, engineState);

    }

    ///<summary>
    /// De-Registers the player vehicle. 
    ///</summary>
    public static void DeRegisterPlayerVehicle() {

        RCCP_SceneManager.Instance.DeRegisterPlayer();

    }

    ///<summary>
    /// Sets controllable state of the vehicle.
    ///</summary>
    public static void SetControl(RCCP_CarController vehicle, bool isControllable) {

        vehicle.SetCanControl(isControllable);

    }

    ///<summary>
    /// Sets engine state of the vehicle.
    ///</summary>
    public static void SetEngine(RCCP_CarController vehicle, bool engineState) {

        if (engineState)
            vehicle.StartEngine();
        else
            vehicle.KillEngine();

    }

    ///<summary>
    /// Sets the mobile controller type.
    ///</summary>
    public static void SetMobileController(RCCP_Settings.MobileController mobileController) {

        RCCP_Settings.Instance.mobileController = mobileController;
        Debug.Log("Mobile Controller has been changed to " + mobileController.ToString());

    }

    ///<summary>
    /// Sets the Automatic Gear.
    ///</summary>
    public static void SetAutomaticGear(RCCP_CarController vehicle, bool state) {

        if (!vehicle.Gearbox)
            return;

        vehicle.Gearbox.automaticTransmission = state;

    }

    ///<summary>
    /// Starts / stops to record the player vehicle.
    ///</summary>
    public static void StartStopRecord(RCCP_CarController vehicle) {

        if (!vehicle.OtherAddonsManager)
            return;

        if (!vehicle.OtherAddonsManager.Recorder)
            return;

        vehicle.OtherAddonsManager.Recorder.Record();

    }

    ///<summary>
    /// Start / stops replay of the last record.
    ///</summary>
    public static void StartStopReplay(RCCP_CarController vehicle) {

        if (!vehicle.OtherAddonsManager)
            return;

        if (!vehicle.OtherAddonsManager.Recorder)
            return;

        vehicle.OtherAddonsManager.Recorder.Play();

    }

    ///<summary>
    /// Stops record / replay of the specific record.
    ///</summary>
    public static void StartStopReplay(RCCP_CarController vehicle, RCCP_Recorder.RecordedClip recordedClip) {

        if (!vehicle.OtherAddonsManager)
            return;

        if (!vehicle.OtherAddonsManager.Recorder)
            return;

        vehicle.OtherAddonsManager.Recorder.Play(recordedClip);

    }

    ///<summary>
    /// Stops record / replay.
    ///</summary>
    public static void StopRecordReplay(RCCP_CarController vehicle) {

        if (!vehicle.OtherAddonsManager)
            return;

        if (!vehicle.OtherAddonsManager.Recorder)
            return;

        vehicle.OtherAddonsManager.Recorder.Stop();

    }

    ///<summary>
    /// Sets new behavior.
    ///</summary>
    public static void SetBehavior(int behaviorIndex) {

        RCCP_SceneManager.SetBehavior(behaviorIndex);
        Debug.Log("Behavior has been changed to " + behaviorIndex.ToString());

    }

    /// <summary>
    /// Changes the camera mode.
    /// </summary>
    public static void ChangeCamera() {

        RCCP_SceneManager.Instance.ChangeCamera();

    }

    /// <summary>
    /// Transport player vehicle the specified position and rotation.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="rotation">Rotation.</param>
    public static void Transport(Vector3 position, Quaternion rotation) {

        RCCP_SceneManager.Instance.Transport(position, rotation);

    }

    /// <summary>
    /// Transport the target vehicle to specified position and rotation.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public static void Transport(RCCP_CarController vehicle, Vector3 position, Quaternion rotation) {

        RCCP_SceneManager.Instance.Transport(vehicle, position, rotation);

    }

    /// <summary>
    /// Cleans all skidmarks on the current scene.
    /// </summary>
    public static void CleanSkidmarks() {

        RCCP_SkidmarksManager.Instance.CleanSkidmarks();

    }

    /// <summary>
    /// Cleans target skidmarks on the current scene.
    /// </summary>
    public static void CleanSkidmarks(int index) {

        RCCP_SkidmarksManager.Instance.CleanSkidmarks(index);

    }

    /// <summary>
    /// Repairs the target vehlcie.
    /// </summary>
    /// <param name="carController"></param>
    public static void Repair(RCCP_CarController carController) {

        carController.Damage.repairNow = true;

    }

    /// <summary>
    /// Repairs the player vehicle.
    /// </summary>
    public static void Repair() {

        RCCP_CarController carController = RCCP_SceneManager.Instance.activePlayerVehicle;

        if (!carController)
            carController.Damage.repairNow = true;

    }

}
