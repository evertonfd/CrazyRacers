using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class CR_PoliceManager : Photon.Pun.MonoBehaviourPunCallbacks, IPunObservable {

    public RCCP_AI aiScript;
    public GameObject policeLights;
    public bool inPursue = false;
    public AudioSource sirenSource;

    public bool alive = true;
    public float damage = 0f;
    public float damageMultiplier = 1f;
    public GameObject engineSmoke;

    public Slider healthSlider;

    public bool moveAwayFromPlayer = false;

    public GameObject explosionParticles;

    private void Update() {

        if (damage >= 100f)
            alive = false;
        else
            alive = true;

        policeLights.SetActive(inPursue);
        engineSmoke.SetActive(!alive);

        if (inPursue) {

            if (!sirenSource.isPlaying)
                sirenSource.Play();

        } else {

            if (sirenSource.isPlaying)
                sirenSource.Stop();

        }

        healthSlider.value = 100f - damage;

        if (Camera.main != null)
            healthSlider.transform.parent.rotation = Camera.main.transform.rotation;

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !photonView.IsMine)
            return;

        if (!CR_GameplayManager.Instance)
            return;

        if (!CR_GameplayManager.Instance.player)
            return;

        if (moveAwayFromPlayer) {

            Vector3 qwe = transform.position - CR_GameplayManager.Instance.player.transform.position;
            qwe.Normalize();
            GetComponent<Rigidbody>().AddForce(qwe * 50f, ForceMode.Acceleration);
            transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        }

        if (!alive) {

            inPursue = false;
            aiScript.targetChase = null;
            return;

        }

        CR_PlayerManager mostSuspicious = null;
        float mostFelony = 25f;

        for (int i = 0; i < CR_GameplayManager.Instance.allPlayers.Count; i++) {

            if (CR_GameplayManager.Instance.allPlayers[i] != null) {

                if (CR_GameplayManager.Instance.allPlayers[i].felony > mostFelony && Vector3.Distance(transform.position, CR_GameplayManager.Instance.allPlayers[i].transform.position) <= 150f) {

                    mostFelony = CR_GameplayManager.Instance.allPlayers[i].felony;
                    mostSuspicious = CR_GameplayManager.Instance.allPlayers[i];

                }

            }

        }

        if (mostSuspicious)
            aiScript.targetChase = mostSuspicious.transform;
        else
            aiScript.targetChase = null;

        inPursue = aiScript.targetChase;

    }

    private void OnCollisionEnter(Collision collision) {

        if (!photonView.IsMine)
            return;

        if (!alive)
            return;

        if (collision.relativeVelocity.magnitude < 5)
            return;

        damage += collision.impulse.magnitude / 1000 * damageMultiplier;

        if (damage > 100) {

            damage = 100f;
            Die();

        }

    }

    public void Die() {

        GetComponent<RCCP_CarController>().canControl = false;

        if (explosionParticles) {

            explosionParticles.SetActive(true);

        }

    }

    public void Revive() {

        GetComponent<RCCP_CarController>().canControl = true;
        alive = true;
        damage = 0f;


        if (explosionParticles) {

            explosionParticles.SetActive(false);

        }

    }

    public void MoveAwayFromPlayer() {

        moveAwayFromPlayer = true;
        Invoke("MoveAway", .15f);

    }

    private void MoveAway() {

        moveAwayFromPlayer = false;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        int a = -1;

        // Sending all inputs, position, rotation, and velocity to the server.
        if (stream.IsWriting) {

            //We own this player: send the others our data.
            stream.SendNext(damage);
            stream.SendNext(inPursue);

            if (aiScript.targetChase && aiScript.targetChase.GetComponent<PhotonView>())
                stream.SendNext(aiScript.targetChase.GetComponent<PhotonView>().ViewID);
            else
                stream.SendNext(-1);

        } else if (stream.IsReading) {

            // Network player, receiving all inputs, position, rotation, and velocity from server. 
            damage = (float)stream.ReceiveNext();
            inPursue = (bool)stream.ReceiveNext();
            a = (int)stream.ReceiveNext();

            if (a != -1)
                aiScript.targetChase = PhotonNetwork.GetPhotonView(a).transform;
            else
                aiScript.targetChase = null;

        }

    }

}
