using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_Soundtrack : MonoBehaviour{

    public AudioSource normalClip;
    public AudioSource pursueClip;
    public float volume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (CR_PolicesManager.Instance.inPursueOnPlayer) {

            normalClip.mute = true;
            pursueClip.mute = false;

        } else {

            normalClip.mute = false;
            pursueClip.mute = true;

        }

        normalClip.volume = volume;
        pursueClip.volume = volume;
        
    }
}
