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
using UnityEngine.SceneManagement;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

/// <summary>
/// A simple manager script for all demo scenes. It has an array of spawnable player vehicles, public methods, setting new behavior modes, restart, and quit application.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP Demo Manager")]
public class RCCP_Demo : MonoBehaviour {

    internal int selectedVehicleIndex = 1;      // An integer index value used for spawning a new vehicle.
    internal int selectedBehaviorIndex = 0;     // An integer index value used for setting a new behavior mode.

    /// <summary>
    /// An integer index value used for spawning a new vehicle.
    /// </summary>
    /// <param name="index"></param>
    public void SelectVehicle(int index) {

        selectedVehicleIndex = index;

    }

    /// <summary>
    /// Spawns the player vehicle.
    /// </summary>
    public void Spawn() {

        if (RCCP_SceneManager.Instance.activePlayerCamera)
            RCCP_SceneManager.Instance.activePlayerCamera.cameraMode = RCCP_Camera.CameraMode.TPS;

        // Last known position and rotation of last active vehicle.
        Vector3 lastKnownPos = new Vector3();
        Quaternion lastKnownRot = new Quaternion();

        // Checking if there is a player vehicle on the scene.
        if (RCCP_SceneManager.Instance.activePlayerVehicle) {

            lastKnownPos = RCCP_SceneManager.Instance.activePlayerVehicle.transform.position;
            lastKnownRot = RCCP_SceneManager.Instance.activePlayerVehicle.transform.rotation;

        }

        // If last known position and rotation is not assigned, camera's position and rotation will be used.
        if (lastKnownPos == Vector3.zero) {

            if (RCCP_SceneManager.Instance.activePlayerCamera) {

                lastKnownPos = RCCP_SceneManager.Instance.activePlayerCamera.transform.position;
                lastKnownRot = RCCP_SceneManager.Instance.activePlayerCamera.transform.rotation;

            }

        }

        // We don't need X and Z rotation angle. Just Y.
        lastKnownRot.x = 0f;
        lastKnownRot.z = 0f;

        // Is there any last vehicle?
        RCCP_CarController lastVehicle = RCCP_SceneManager.Instance.activePlayerVehicle;

#if BCG_ENTEREXIT

        BCG_EnterExitVehicle lastEnterExitVehicle;
        bool enterExitVehicleFound = false;

        if (lastVehicle) {

            lastEnterExitVehicle = lastVehicle.GetComponentInChildren<BCG_EnterExitVehicle>();

            if (lastEnterExitVehicle && lastEnterExitVehicle.driver) {

                enterExitVehicleFound = true;
                BCG_EnterExitManager.Instance.waitTime = 10f;
                lastEnterExitVehicle.driver.GetOut();

            }

        }

#endif

        // If we have controllable vehicle by player on scene, destroy it.
        if (lastVehicle)
            Destroy(lastVehicle.gameObject);

        // Here we are creating our new vehicle.
        RCCP.SpawnRCC(RCCP_DemoVehicles.Instance.vehicles[selectedVehicleIndex], lastKnownPos, lastKnownRot, true, true, true);

#if BCG_ENTEREXIT

        if (enterExitVehicleFound) {

            lastEnterExitVehicle = null;

            lastEnterExitVehicle = RCCP_SceneManager.Instance.activePlayerVehicle.GetComponentInChildren<BCG_EnterExitVehicle>();

            if (!lastEnterExitVehicle)
                lastEnterExitVehicle = RCCP_SceneManager.Instance.activePlayerVehicle.gameObject.AddComponent<BCG_EnterExitVehicle>();

            if (BCG_EnterExitManager.Instance.activePlayer && lastEnterExitVehicle && lastEnterExitVehicle.driver == null) {

                BCG_EnterExitManager.Instance.waitTime = 10f;
                BCG_EnterExitManager.Instance.activePlayer.GetIn(lastEnterExitVehicle);

            }

        }

#endif

    }

#if PHOTON_UNITY_NETWORKING && RCCP_PHOTON
    /// <summary>
    /// Spawns the player vehicle.
    /// </summary>
    public void SpawnPhoton() {

        if (RCCP_SceneManager.Instance.activePlayerCamera)
            RCCP_SceneManager.Instance.activePlayerCamera.cameraMode = RCCP_Camera.CameraMode.TPS;

        // Last known position and rotation of last active vehicle.
        Vector3 lastKnownPos = new Vector3();
        Quaternion lastKnownRot = new Quaternion();

        // Checking if there is a player vehicle on the scene.
        if (RCCP_SceneManager.Instance.activePlayerVehicle) {

            lastKnownPos = RCCP_SceneManager.Instance.activePlayerVehicle.transform.position;
            lastKnownRot = RCCP_SceneManager.Instance.activePlayerVehicle.transform.rotation;

        }

        // If last known position and rotation is not assigned, camera's position and rotation will be used.
        if (lastKnownPos == Vector3.zero) {

            if (RCCP_SceneManager.Instance.activePlayerCamera) {

                lastKnownPos = RCCP_SceneManager.Instance.activePlayerCamera.transform.position;
                lastKnownRot = RCCP_SceneManager.Instance.activePlayerCamera.transform.rotation;

            }

        }

        // We don't need X and Z rotation angle. Just Y.
        lastKnownRot.x = 0f;
        lastKnownRot.z = 0f;

        // Is there any last vehicle?
        RCCP_CarController lastVehicle = RCCP_SceneManager.Instance.activePlayerVehicle;

#if BCG_ENTEREXIT

        BCG_EnterExitVehicle lastEnterExitVehicle;
        bool enterExitVehicleFound = false;

        if (lastVehicle) {

            lastEnterExitVehicle = lastVehicle.GetComponentInChildren<BCG_EnterExitVehicle>();

            if (lastEnterExitVehicle && lastEnterExitVehicle.driver) {

                enterExitVehicleFound = true;
                BCG_EnterExitManager.Instance.waitTime = 10f;
                lastEnterExitVehicle.driver.GetOut();

            }

        }

#endif

        // If we have controllable vehicle by player on scene, destroy it.
        if (lastVehicle)
            PhotonNetwork.Destroy(lastVehicle.gameObject);

        // Here we are creating our new vehicle.
        RCCP_CarController spawnedVehicle = PhotonNetwork.Instantiate(RCCP_DemoVehicles_Photon.Instance.vehicles[selectedVehicleIndex].transform.name, lastKnownPos, lastKnownRot).gameObject.GetComponent<RCCP_CarController>();
        RCCP.RegisterPlayerVehicle(spawnedVehicle, true, true);

#if BCG_ENTEREXIT

        if (enterExitVehicleFound) {

            lastEnterExitVehicle = null;

            lastEnterExitVehicle = RCCP_SceneManager.Instance.activePlayerVehicle.GetComponentInChildren<BCG_EnterExitVehicle>();

            if (!lastEnterExitVehicle)
                lastEnterExitVehicle = RCCP_SceneManager.Instance.activePlayerVehicle.gameObject.AddComponent<BCG_EnterExitVehicle>();

            if (BCG_EnterExitManager.Instance.activePlayer && lastEnterExitVehicle && lastEnterExitVehicle.driver == null) {

                BCG_EnterExitManager.Instance.waitTime = 10f;
                BCG_EnterExitManager.Instance.activePlayer.GetIn(lastEnterExitVehicle);

            }

        }

#endif

    }
#endif

    /// <summary>
    /// Simply restarting the current scene.
    /// </summary>
    public void RestartScene() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    /// <summary>
    /// Simply quit application. Not working on Editor.
    /// </summary>
    public void Quit() {

        Application.Quit();

    }

}
