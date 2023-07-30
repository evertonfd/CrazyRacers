using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class CR_PhotonManager : Photon.Pun.MonoBehaviourPunCallbacks {

    public override void OnEnable() {

        PhotonNetwork.AddCallbackTarget(this);

    }

    public override void OnDisconnected(DisconnectCause cause) {

        PhotonNetwork.LoadLevel(0);

    }

    public override void OnDisable() {

        PhotonNetwork.RemoveCallbackTarget(this);

    }

}
