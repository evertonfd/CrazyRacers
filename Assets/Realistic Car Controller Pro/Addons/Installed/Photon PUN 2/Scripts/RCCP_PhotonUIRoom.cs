//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RCCP_PhotonUIRoom : MonoBehaviour {

    internal string roomNameString;
    public Text roomName;
    public Text maxPlayers;

    public void Check(string _roomName, string _maxPlayers) {

        roomNameString = _roomName;
        roomName.text = _roomName;
        maxPlayers.text = _maxPlayers;

    }

    public void JoinRoom() {

        RCCP_PhotonManager.Instance.JoinSelectedRoom(this);

    }

}
#endif