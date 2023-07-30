using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CR_PhotonTrafficVehicle : MonoBehaviourPun{

    public bool isMine = false;
    public MonoBehaviour[] components;

    // Start is called before the first frame update
    void Awake(){

        if (PhotonNetwork.IsConnected)
            isMine = PhotonNetwork.IsMasterClient;
        else
            isMine = true;

        for (int i = 0; i < components.Length; i++) {

            if(!isMine)
                Destroy(components[i]);

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
