using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CR_Photon : Photon.Pun.MonoBehaviourPunCallbacks {

    private static CR_Photon instance;
    public static CR_Photon Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_Photon>();

            return instance;

        }

    }

    public GameObject connectingToServer;
    public GameObject nickNameEnter;
    public GameObject joining;
    public string nickName = "";

    public override void OnEnable() {

        PhotonNetwork.AddCallbackTarget(this);
        OpenMenu(nickNameEnter);

    }

    public void ConnectToPhoton() {

        Debug.Log("Connecting to server");
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
        OpenMenu(connectingToServer);

    }

    public override void OnConnectedToMaster() {

        Debug.Log("Connected to server");
        Debug.Log("Entering to lobby");
        PhotonNetwork.JoinLobby();

    }

    //public void JoinLobby() {

    //    PhotonNetwork.JoinLobby();

    //}

    public void SetNickName(TMP_InputField inputField) {

        if (CheckBadWord.HasBadWord(inputField.text)) {

            CR_UIInformer.Instance.Display("Invalid Entry!", "Please use a proper name");
            return;

        }

        nickName = inputField.text;
        ConnectToPhoton();

    }

    public override void OnJoinedLobby() {

        Debug.Log("Entered to lobby");
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message) {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;

        Debug.Log("Entered to room");
        PhotonNetwork.JoinOrCreateRoom("New Room " + Random.Range(0, 999), roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom() {

        CR_MainMenuManager.Instance.OpenScene();

    }

    public override void OnCreatedRoom() {

        //CR_MainMenuManager.Instance.OpenScene();

    }

    private void OpenMenu(GameObject targetMenu) {

        connectingToServer.SetActive(false);
        nickNameEnter.SetActive(false);
        joining.SetActive(false);

        targetMenu.SetActive(true);

    }

    public override void OnDisable() {

        PhotonNetwork.RemoveCallbackTarget(this);
        

    }

}
