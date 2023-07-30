//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// A simple manager script for photon demo scene. It has an array of networked spawnable player vehicles, public methods, restart, and quit application.
/// </summary>
public class RCCP_PhotonDemo : Photon.Pun.MonoBehaviourPunCallbacks {

    private bool connectedWithThis = false;

    private int selectedCarIndex = 0;
    private int selectedBehaviorIndex = 0;

    public Transform[] spawnPoints;
    public GameObject menu;

    private void Start() {

        if (!PhotonNetwork.IsConnectedAndReady)
            ConnectToPhoton();
        else if (PhotonNetwork.IsConnectedAndReady)
            menu.SetActive(true);

    }

    private void ConnectToPhoton() {

        Debug.Log("Connecting to server");
        connectedWithThis = true;
        RCCP_UI_Informer.Instance.Display("Connecting...");
        PhotonNetwork.NickName = "New Player " + Random.Range(0, 99999).ToString();
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster() {

        if (!connectedWithThis)
            return;

        Debug.Log("Connected to server");
        Debug.Log("Entering to lobby");
        RCCP_UI_Informer.Instance.Display("Entering to lobby...");
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby() {

        if (!connectedWithThis)
            return;

        Debug.Log("Entered to lobby");
        RCCP_UI_Informer.Instance.Display("Creating / Joining Random Room");
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message) {

        if (!connectedWithThis)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;

        Debug.Log("Entered to room");
        RCCP_UI_Informer.Instance.Display("Entered Room");
        PhotonNetwork.JoinOrCreateRoom("New Room " + Random.Range(0, 999), roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom() {

        if (!connectedWithThis)
            return;

        if (menu)
            menu.SetActive(true);

    }

    public override void OnCreatedRoom() {

        if (!connectedWithThis)
            return;

        if (menu)
            menu.SetActive(true);

    }

    public void Spawn() {

        int actorNo = PhotonNetwork.LocalPlayer.ActorNumber;

        if (actorNo > spawnPoints.Length) {

            while (actorNo > spawnPoints.Length)
                actorNo -= spawnPoints.Length;

        }

        Vector3 lastKnownPos = Vector3.zero;
        Quaternion lastKnownRot = Quaternion.identity;

        RCCP_CarController newVehicle;

        if (RCCP_SceneManager.Instance.activePlayerVehicle) {

            lastKnownPos = RCCP_SceneManager.Instance.activePlayerVehicle.transform.position;
            lastKnownRot = RCCP_SceneManager.Instance.activePlayerVehicle.transform.rotation;

        }

        if (lastKnownPos == Vector3.zero) {

            lastKnownPos = spawnPoints[actorNo - 1].position;
            lastKnownRot = spawnPoints[actorNo - 1].rotation;

        }

        lastKnownRot.x = 0f;
        lastKnownRot.z = 0f;

        if (RCCP_SceneManager.Instance.activePlayerVehicle)
            PhotonNetwork.Destroy(RCCP_SceneManager.Instance.activePlayerVehicle.gameObject);

        newVehicle = PhotonNetwork.Instantiate("Photon Vehicles/" + RCCP_DemoVehicles_Photon.Instance.vehicles[selectedCarIndex].name, lastKnownPos + (Vector3.up), lastKnownRot, 0).GetComponent<RCCP_CarController>();

        RCCP.RegisterPlayerVehicle(newVehicle);
        RCCP.SetControl(newVehicle, true);

        if (RCCP_SceneManager.Instance.activePlayerCamera)
            RCCP_SceneManager.Instance.activePlayerCamera.SetTarget(newVehicle);

    }

    /// <summary>
    /// Selects the vehicle.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SelectVehicle(int index) {

        selectedCarIndex = index;

    }

    /// <summary>
    /// An integer index value used for setting behavior mode.
    /// </summary>
    /// <param name="index"></param>
    public void SetBehavior(int index) {

        selectedBehaviorIndex = index;

    }

    /// <summary>
    /// Here we are setting new selected behavior to corresponding one.
    /// </summary>
    public void InitBehavior() {

        RCCP.SetBehavior(selectedBehaviorIndex);

    }

    /// <summary>
    /// Sets the mobile controller type.
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

    /// <summary>
    /// Sets the quality.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SetQuality(int index) {

        QualitySettings.SetQualityLevel(index);

    }

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
#endif