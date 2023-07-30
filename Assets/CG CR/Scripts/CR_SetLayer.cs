using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_SetLayer : MonoBehaviour{

    public string layerName = "";

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
