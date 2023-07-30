using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CR_PolicesManager : MonoBehaviourPun, IPunObservable{

    private static CR_PolicesManager instance;
    public static CR_PolicesManager Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_PolicesManager>();

            return instance;

        }

    }

    public CR_PoliceManager[] polices;

    public bool inPursueOnPlayer = false;

    private void Update() {

        bool cur = false;

        for (int i = 0; i < polices.Length; i++) {

            if (polices[i].gameObject.activeSelf && polices[i].inPursue && polices[i].aiScript.targetChase == CR_GameplayManager.Instance.player.transform)
                cur = true;

        }

        inPursueOnPlayer = cur;

        if (!inPursueOnPlayer) {

            CR_GameplayManager.Instance.player.felony -= Time.deltaTime;

            if (CR_GameplayManager.Instance.player.felony < 0)
                CR_GameplayManager.Instance.player.felony = 0f;

        }

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < polices.Length; i++) {

            bool canRevive = true;

            for (int k = 0; k < CR_GameplayManager.Instance.allPlayers.Count; k++) {

                if (CR_GameplayManager.Instance.allPlayers[k] != null) {

                    if (polices[i].gameObject.activeSelf && Vector3.Distance(polices[i].transform.position, CR_GameplayManager.Instance.allPlayers[k].transform.position) < 150f)
                        canRevive = false;

                }

            }

            if (polices[i].gameObject.activeSelf && canRevive && !polices[i].alive)
                polices[i].Revive();

        }

        for (int i = 0; i < polices.Length; i++) {

            bool canDisable = true;

            for (int k = 0; k < CR_GameplayManager.Instance.allPlayers.Count; k++) {

                if (CR_GameplayManager.Instance.allPlayers[k] != null) {

                    if (Vector3.Distance(polices[i].transform.position, CR_GameplayManager.Instance.allPlayers[k].transform.position) < 150f)
                        canDisable = false;

                }

            }

            for (int k = 0; k < CR_GameplayManager.Instance.allPlayers.Count; k++) {

                if (CR_GameplayManager.Instance.allPlayers[k] != null) {

                    if (canDisable && Vector3.Distance(polices[i].transform.position, CR_GameplayManager.Instance.allPlayers[k].transform.position) > 150f)
                        polices[i].gameObject.SetActive(false);
                    else
                        polices[i].gameObject.SetActive(true);

                }

            }

        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {

            for (int i = 0; i < polices.Length; i++)
                stream.SendNext(polices[i].gameObject.activeSelf);

        }else if (stream.IsReading) {

            for (int i = 0; i < polices.Length; i++)
                polices[i].gameObject.SetActive((bool)stream.ReceiveNext());

        }

    }

}
