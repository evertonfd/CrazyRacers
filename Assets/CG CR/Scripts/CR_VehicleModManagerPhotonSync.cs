using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CR_VehicleModManagerPhotonSync : Photon.Pun.MonoBehaviourPunCallbacks, IPunObservable {

    private CR_VehicleModManager vehicleModManager;

    public int selectedHoodIndex = -1;
    public int selectedBumper_FIndex = -1;
    public int selectedBumper_RIndex = -1;
    public int selectedSpoilerIndex = -1;
    public int selectedSideskirtIndex = -1;
    public int selectedFenderIndex = -1;

    private void Awake(){

        vehicleModManager = GetComponent<CR_VehicleModManager>();
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (!vehicleModManager)
            return;

        // Sending all inputs, position, rotation, and velocity to the server.
        if (stream.IsWriting) {

            //We own this player: send the others our data.
            stream.SendNext(vehicleModManager.selectedHoodIndex);
            stream.SendNext(vehicleModManager.selectedBumper_FIndex);
            stream.SendNext(vehicleModManager.selectedBumper_RIndex);
            stream.SendNext(vehicleModManager.selectedSpoilerIndex);
            stream.SendNext(vehicleModManager.selectedSideskirtIndex);
            stream.SendNext(vehicleModManager.selectedFenderIndex);

        } else if (stream.IsReading) {

            // Network player, receiving all inputs, position, rotation, and velocity from server. 
            selectedHoodIndex = (int)stream.ReceiveNext();
            selectedBumper_FIndex = (int)stream.ReceiveNext();
            selectedBumper_RIndex = (int)stream.ReceiveNext();
            selectedSpoilerIndex = (int)stream.ReceiveNext();
            selectedSideskirtIndex = (int)stream.ReceiveNext();
            selectedFenderIndex = (int)stream.ReceiveNext();

        }

    }

    private void Update() {

        if (!vehicleModManager)
            return;

        if (selectedHoodIndex != -1)
            vehicleModManager.hoods[selectedHoodIndex].part.SetActive(true);

        if (selectedBumper_FIndex != -1)
            vehicleModManager.bumpers_F[selectedBumper_FIndex].part.SetActive(true);

        if (selectedBumper_RIndex != -1)
            vehicleModManager.bumpers_R[selectedBumper_RIndex].part.SetActive(true);

        if (selectedSpoilerIndex != -1)
            vehicleModManager.spoilers[selectedSpoilerIndex].part.SetActive(true);

        if (selectedSideskirtIndex != -1)
            vehicleModManager.sideskirts[selectedSideskirtIndex].part.SetActive(true);

        if (selectedFenderIndex != -1)
            vehicleModManager.fenders[selectedFenderIndex].part.SetActive(true);

    }

}
