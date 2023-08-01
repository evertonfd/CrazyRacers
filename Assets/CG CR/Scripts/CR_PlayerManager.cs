using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CR_PlayerManager : MonoBehaviourPun, IPunObservable {

    private RCCP_CarController carController;
    public RCCP_CarController CarController {

        get {

            if (carController == null)
                carController = GetComponent<RCCP_CarController>();

            return carController;

        }

    }

    private CR_VehicleModManager modManager;
    public CR_VehicleModManager ModManager {

        get {

            if (modManager == null)
                modManager = GetComponent<CR_VehicleModManager>();

            return modManager;

        }

    }

    public float score_Destruction = 0;
    public float score_Speeding = 0;
    public float score_AirTime = 0;
    public float score_Drift = 0;
    public float score_NearMiss = 0;

    public float money_Destruction = 0;
    public float money_Speeding = 0;
    public float money_AirTime = 0;
    public float money_Drift = 0;
    public float money_NearMiss = 0;
    public float money_Penalty = 0;
    public float money_Missions = 0;

    public int TotalScore {

        get {

            return (int)(score_Destruction + score_Speeding + score_AirTime + score_Drift + score_NearMiss);

        }

    }

    public int TotalMoney {

        get {

            return (int)((money_Destruction + money_Speeding + money_AirTime + money_Drift + money_NearMiss + money_Missions) - money_Penalty);

        }
        set {

            TotalMoney = value;

        }

    }

    public float felony = 0f;

    public CR_PoliceManager closestPolice;
    public float bustedTime = 3f;
    public bool bustingNow = false;
    public bool busted = false;

    public string leftTrafficCarName = "";
    public string rightTrafficCarName = "";

    public float stuckedTimer = 0f;
    public float resetTimer = 0f;

    public bool driftMode = false;
    public float driftingCountdown = 0f;

    private void Update() {

        if (photonView && !photonView.IsMine)
            return;
        
        if (CarController.IsGrounded) {

            CarController.Rigid.angularDrag = .35f;

        } else {

            CarController.Rigid.angularDrag = 1f;

        }

        if (driftingCountdown > 0)
            driftingCountdown -= Time.deltaTime;

        driftingCountdown = Mathf.Clamp(driftingCountdown, 0, Mathf.Infinity);

        if (driftingCountdown > 0)
            driftMode = true;
        else
            driftMode = false;

        if (driftMode) {
            
            CarController.Rigid.AddRelativeTorque(Vector3.up * 5000f * CarController.steerInput_P, ForceMode.Force);

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++)
                CarController.AllWheelColliders[i].driftMode = true;

        } else {

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++)
                CarController.AllWheelColliders[i].driftMode = false;

        }

        if (CarController.speed > 120f && CarController.IsGrounded) {

            score_Speeding += Time.deltaTime * 100f;
            money_Speeding += Time.deltaTime * 33f;

        }

        if (CarController.speed > 20f && Mathf.Abs(CarController.RearAxle.leftWheelCollider.wheelSlipAmountSideways) >= .25f) {

            score_Drift += Time.deltaTime * 100f;
            money_Drift += Time.deltaTime * 33f;

        }

        if (!CarController.IsGrounded && Mathf.Abs(transform.InverseTransformDirection(CarController.Rigid.velocity).z) >= 5f) {

            score_AirTime += Time.deltaTime * 100f;
            money_AirTime += Time.deltaTime * 33f;

        }

        closestPolice = null;
        float closestPoliceDistance = Mathf.Infinity;

        if (CR_PolicesManager.Instance) {

            for (int i = 0; i < CR_PolicesManager.Instance.polices.Length; i++) {

                if (Vector3.Distance(transform.position, CR_PolicesManager.Instance.polices[i].transform.position) < 100f) {

                    if (Vector3.Distance(transform.position, CR_PolicesManager.Instance.polices[i].transform.position) < closestPoliceDistance) {

                        if (CR_PolicesManager.Instance.polices[i].alive) {

                            closestPoliceDistance = Vector3.Distance(transform.position, CR_PolicesManager.Instance.polices[i].transform.position);
                            closestPolice = CR_PolicesManager.Instance.polices[i];

                        }

                    }

                }

            }

        }

        if (closestPolice) {

            if (CarController.physicalSpeed >= 100f)
                felony += Time.deltaTime * 5f;

        }

        felony = Mathf.Clamp(felony, 0f, 100f);

        if (closestPolice && felony >= 25f && Vector3.Distance(transform.position, closestPolice.transform.position) < 15f) {

            if (Mathf.Abs(CarController.physicalSpeed) < 5f)
                bustingNow = true;
            else
                bustingNow = false;

        }

        if (!closestPolice)
            bustingNow = false;

        if (bustingNow)
            bustedTime -= Time.deltaTime;
        else
            bustedTime = 2f;

        if (!busted && bustedTime <= 0)
            Busted();

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, 3f)) {

            if (hit.transform.GetComponentInParent<TSSimpleCar>())
                rightTrafficCarName = hit.transform.GetComponentInParent<TSSimpleCar>().transform.name;

        } else {

            if (rightTrafficCarName != "") {

                rightTrafficCarName = "";

                if (CarController.physicalSpeed > 80f) {

                    score_NearMiss += 100;
                    money_NearMiss += 35;

                }

            }

        }

        if (Physics.Raycast(transform.position, -transform.right, out hit, 3f)) {

            if (hit.transform.GetComponentInParent<TSSimpleCar>())
                leftTrafficCarName = hit.transform.GetComponentInParent<TSSimpleCar>().transform.name;

        } else {

            if (leftTrafficCarName != "") {

                leftTrafficCarName = "";

                if (CarController.physicalSpeed > 80f) {

                    score_NearMiss += 100;
                    money_NearMiss += 35;

                }

            }

        }

        bool grounded = true;

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            if (!CarController.AllWheelColliders[i].isGrounded)
                grounded = false;

        }

        if (!grounded && CarController.Rigid.velocity.magnitude < 5f)
            stuckedTimer += Time.deltaTime;
        else
            stuckedTimer = 0f;

        if (stuckedTimer >= 2) {

            stuckedTimer = 0f;
            ResetVehicle();

        }

        resetTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && resetTimer >= 2f) {

            resetTimer = 0f;
            ResetVehicle();

        }

    }

    private void FixedUpdate() {

        if (photonView && !photonView.IsMine)
            return;

        CarController.Rigid.AddRelativeTorque(Vector3.forward * CarController.steerInput_P * -1f, ForceMode.Acceleration);

    }

    public void Busted() {

        busted = true;
        RCCP.SetControl(CarController, false);

    }

    public void SetFree() {

        busted = false;
        bustingNow = false;
        bustedTime = 3f;
        felony = 0f;
        RCCP.SetControl(CarController, true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);

        for (int i = 0; i < colliders.Length; i++) {

            CR_PoliceManager policeManager = colliders[i].GetComponentInParent<CR_PoliceManager>();

            if (policeManager)
                policeManager.MoveAwayFromPlayer();

        }

    }

    private void OnCollisionEnter(Collision collision) {

        if (photonView && !photonView.IsMine)
            return;

        CR_Prop prop = collision.gameObject.GetComponentInParent<CR_Prop>();

        if (prop && !prop.destroyed) {

            score_Destruction += prop.propScore;
            money_Destruction += prop.propScore * .33f;

        }

        if (collision.relativeVelocity.magnitude < 5)
            return;

        CR_PoliceManager police = collision.gameObject.GetComponentInParent<CR_PoliceManager>();

        if (!police)
            return;

        felony += 26f;

        if (felony > 100f)
            felony = 100f;

    }

    public void ResetVehicle() {

        if (photonView && !photonView.IsMine)
            return;

        RCCP.Transport(transform.position + Vector3.up * 2f, Quaternion.Euler(0f, transform.eulerAngles.y, 0f));

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {

            stream.SendNext(felony);

        } else if (stream.IsReading) {

            felony = (float)stream.ReceiveNext();

        }

    }

}
