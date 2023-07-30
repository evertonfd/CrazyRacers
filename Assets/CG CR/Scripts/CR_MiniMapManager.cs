using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_MiniMapManager : MonoBehaviour{

    public Camera cam;
    public float distance = 100f;

    // Start is called before the first frame update
    void Start(){


        
    }

    // Update is called once per frame
    void Update(){

        if (!CR_GameplayManager.Instance)
            return;

        if (!CR_GameplayManager.Instance.player)
            return;

        cam.transform.position = CR_GameplayManager.Instance.player.transform.position;
        cam.transform.rotation = Quaternion.Euler(90f, 0f, -CR_GameplayManager.Instance.player.transform.eulerAngles.y);
        cam.transform.position += Vector3.up * distance;
        
    }
}
