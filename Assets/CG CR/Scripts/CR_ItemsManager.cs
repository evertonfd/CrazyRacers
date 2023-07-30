using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_ItemsManager : MonoBehaviour{

    public GameObject[] spawnObjects;
    public Transform[] spawnLocations;

    public List<int> list = new List<int>();

    void Start(){

        list = new List<int>(new int[spawnLocations.Length]);
        int Rand;

        for (int j = 1; j < list.Count; j++) {

            Rand = Random.Range(1, list.Count);

            while (list.Contains(Rand))
                Rand = Random.Range(1, list.Count);

            list[j] = Rand;

        }

        for (int i = 0; i < spawnObjects.Length; i++) {

            GameObject spawned = Instantiate(spawnObjects[i], spawnLocations[list[i]].position, spawnLocations[list[i]].rotation, transform);

        }

    }

}
