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
using Photon;
using Photon.Pun;

/// <summary>
/// Streaming player input, or receiving data from server. And then feeds the RCCP.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class RCCP_PhotonSync : Photon.Pun.MonoBehaviourPunCallbacks, IPunObservable {

    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public bool AI = false;

    public bool isMine = false;
    public bool createNameText = true;

    // Vehicle position and rotation. Will send these to server.
    private Vector3 correctPlayerPos = new Vector3(0f, 0f, 0f);
    private Quaternion correctPlayerRot = Quaternion.identity;

    // Used for projected (interpolated) position.
    private Vector3 currentVelocity = new Vector3(0f, 0f, 0f);
    private float updateTime = 0;

    // All inputs for RCCP. We will send these values if we own this vehicle. If this vehicle is owned by other player, receives all these inputs from server.
    private float gasInput = 0f;
    private float brakeInput = 0f;
    private float steerInput = 0f;
    private float handbrakeInput = 0f;
    private float boostInput = 0f;
    private float clutchInput = 0f;

    private float engineRPM = 0f;
    private int gear = 0;
    private int direction = 1;
    private float differentialOutputLeft = 0f;
    private float differentialOutputRight = 0f;
    private bool changingGear = false;
    private bool engineStarting = false;
    private bool engineRunning = false;

    // Lights.
    private bool lowBeamHeadLightsOn = false;
    private bool highBeamHeadLightsOn = false;
    private bool indicatorsLeft = false;
    private bool indicatorsRight = false;
    private bool indicatorsAll = false;

    // For Nickname Text
    private TextMesh nicknameText;

    private void Start() {

        //if (!photonView.ObservedComponents.Contains(this))
        //    photonView.ObservedComponents.Add(this);

        //photonView.Synchronization = ViewSynchronization.Unreliable;
        
        if (createNameText)
            CreateNickText();

    }

    public override void OnEnable() {

        GetInitialValues();

    }

    private void GetInitialValues() {

        correctPlayerPos = transform.position;
        correctPlayerRot = transform.rotation;

        gasInput = CarController.throttleInput_V;
        brakeInput = CarController.brakeInput_V;
        steerInput = CarController.steerInput_V;
        handbrakeInput = CarController.handbrakeInput_V;
        boostInput = CarController.nosInput_V;
        clutchInput = CarController.clutchInput_V;
        gear = CarController.currentGear;
        changingGear = CarController.shiftingNow;
        engineStarting = CarController.engineStarting;
        engineRunning = CarController.engineRunning;
        direction = CarController.direction;
        differentialOutputLeft = CarController.Differential.outputLeft;
        differentialOutputRight = CarController.Differential.outputRight;
        lowBeamHeadLightsOn = CarController.lowBeamLights;
        highBeamHeadLightsOn = CarController.highBeamLights;
        indicatorsLeft = CarController.indicatorsLeftLights;
        indicatorsRight = CarController.indicatorsRightLights;
        indicatorsAll = CarController.indicatorsAllLights;

    }

    private void CreateNickText() {

        GameObject nicknameTextGO = new GameObject("NickName TextMesh");
        nicknameTextGO.transform.SetParent(transform, false);
        nicknameTextGO.transform.localPosition = new Vector3(0f, 2f, 0f);
        nicknameTextGO.transform.localScale = new Vector3(.25f, .25f, .25f);
        nicknameText = nicknameTextGO.AddComponent<TextMesh>();
        nicknameText.anchor = TextAnchor.MiddleCenter;
        nicknameText.fontSize = 25;

    }

    private void FixedUpdate() {

        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        if (!CarController)
            return;

        isMine = photonView.IsMine;

        if (photonView.OwnershipTransfer == OwnershipOption.Fixed) {

            // If we are the owner of this vehicle, disable external controller and enable controller of the vehicle. Do opposite if we don't own this.
            if (!AI && CarController.Inputs) {

                CarController.Inputs.overrideInternalInputs = !isMine;
                CarController.Inputs.overrideExternalInputs = !isMine;

            }

            if (CarController.Engine)
                CarController.Engine.overrideEngineRPM = !isMine;

            if (CarController.Clutch)
                CarController.Clutch.overrideClutch = !isMine;

            if (CarController.Gearbox)
                CarController.Gearbox.overrideGear = !isMine;

            if (CarController.Differential)
                CarController.Differential.overrideDifferential = !isMine;

        }

        // If we are not owner of this vehicle, receive all inputs from server.
        if (!isMine) {

            Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);

            if (Vector3.Distance(transform.position, correctPlayerPos) < 15f)
                transform.SetPositionAndRotation(Vector3.Lerp(transform.position, projectedPosition, Time.deltaTime * 5f), Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5f));
            else
                transform.SetPositionAndRotation(correctPlayerPos, correctPlayerRot);

            RCCP_Inputs inputs = new RCCP_Inputs();

            inputs.throttleInput = gasInput;
            inputs.brakeInput = brakeInput;
            inputs.steerInput = steerInput;
            inputs.handbrakeInput = handbrakeInput;
            inputs.nosInput = boostInput;
            inputs.clutchInput = clutchInput;

            if (CarController.Inputs)
                CarController.Inputs.OverrideInputs(inputs);

            if (CarController.Engine) {

                CarController.Engine.engineStarting = engineStarting;
                CarController.Engine.engineRunning = engineRunning;
                CarController.Engine.OverrideRPM(engineRPM);

            }

            if (CarController.Clutch)
                CarController.Clutch.OverrideInput(clutchInput);

            if (CarController.Gearbox)
                CarController.Gearbox.OverrideGear(gear, direction != 1);

            if (CarController.Differential)
                CarController.Differential.OverrideDifferential(differentialOutputLeft, differentialOutputRight);

            CarController.Gearbox.shiftingNow = changingGear;
            CarController.direction = direction;

            CarController.Lights.lowBeamHeadlights = lowBeamHeadLightsOn;
            CarController.Lights.highBeamHeadlights = highBeamHeadLightsOn;
            CarController.Lights.indicatorsLeft = indicatorsLeft;
            CarController.Lights.indicatorsRight = indicatorsRight;
            CarController.Lights.indicatorsAll = indicatorsAll;

            if (nicknameText) {

                if (photonView.Owner != null)
                    nicknameText.text = photonView.Owner.NickName;
                else
                    nicknameText.text = "";

                if (RCCP_SceneManager.Instance.activeMainCamera)
                    nicknameText.transform.rotation = RCCP_SceneManager.Instance.activeMainCamera.transform.rotation;

            }

        } else {

            if (nicknameText)
                nicknameText.text = "";

        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (!CarController)
            return;

        // Sending all inputs, position, rotation, and velocity to the server.
        if (stream.IsWriting) {

            //We own this player: send the others our data
            stream.SendNext(CarController.throttleInput_V);
            stream.SendNext(CarController.brakeInput_V);
            stream.SendNext(CarController.steerInput_V);
            stream.SendNext(CarController.handbrakeInput_V);
            stream.SendNext(CarController.nosInput_V);
            stream.SendNext(CarController.clutchInput_V);

            stream.SendNext(CarController.engineRPM);
            stream.SendNext(CarController.currentGear);
            stream.SendNext(CarController.shiftingNow);
            stream.SendNext(CarController.engineStarting);
            stream.SendNext(CarController.engineRunning);
            stream.SendNext(CarController.direction);

            stream.SendNext(CarController.Differential.outputLeft);
            stream.SendNext(CarController.Differential.outputRight);

            stream.SendNext(CarController.lowBeamLights);
            stream.SendNext(CarController.highBeamLights);
            stream.SendNext(CarController.indicatorsLeftLights);
            stream.SendNext(CarController.indicatorsRightLights);
            stream.SendNext(CarController.indicatorsAllLights);

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CarController.Rigid.velocity);

        } else if (stream.IsReading) {

            // Network player, receiving all inputs, position, rotation, and velocity from server. 
            gasInput = (float)stream.ReceiveNext();
            brakeInput = (float)stream.ReceiveNext();
            steerInput = (float)stream.ReceiveNext();
            handbrakeInput = (float)stream.ReceiveNext();
            boostInput = (float)stream.ReceiveNext();
            clutchInput = (float)stream.ReceiveNext();

            engineRPM = (float)stream.ReceiveNext();
            gear = (int)stream.ReceiveNext();
            changingGear = (bool)stream.ReceiveNext();
            engineStarting = (bool)stream.ReceiveNext();
            engineRunning = (bool)stream.ReceiveNext();
            direction = (int)stream.ReceiveNext();

            differentialOutputLeft = (float)stream.ReceiveNext();
            differentialOutputRight = (float)stream.ReceiveNext();

            lowBeamHeadLightsOn = (bool)stream.ReceiveNext();
            highBeamHeadLightsOn = (bool)stream.ReceiveNext();
            indicatorsLeft = (bool)stream.ReceiveNext();
            indicatorsRight = (bool)stream.ReceiveNext();
            indicatorsAll = (bool)stream.ReceiveNext();

            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            currentVelocity = (Vector3)stream.ReceiveNext();

            updateTime = Time.time;

        }

    }

}
#endif