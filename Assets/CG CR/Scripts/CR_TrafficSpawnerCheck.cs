using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_TrafficSpawnerCheck : MonoBehaviour {

    public TSTrafficSpawner spawner;

    // Start is called before the first frame update
    void Start() {

        if (PlayerPrefs.GetInt("Mode", 0) == 0)
            spawner.enabled = true;
        else
            spawner.enabled = false;

    }

}
