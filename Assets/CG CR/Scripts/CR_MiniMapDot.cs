using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_MiniMapDot : MonoBehaviour{

    public GameObject root;

    void Update(){

        if (!root)
            return;

        transform.position = root.transform.position;
        transform.position += Vector3.up * 50f;
        transform.rotation = Quaternion.identity;
        
    }

}
