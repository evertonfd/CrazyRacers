using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CG_TrafficVehicle : MonoBehaviour {

    public float health = 100f;
    public GameObject engineSmoke;
    public GameObject explosion;

    private void OnEnable() {

        health = 100f;

    }

    private void OnCollisionEnter(Collision collision) {

        health -= collision.impulse.magnitude / 400f;

        if (health < 0)
            health = 0;

    }

    private void Update() {

        if (health < 50) {

            if (!engineSmoke.activeSelf)
                engineSmoke.SetActive(true);

        } else {

            if (engineSmoke.activeSelf)
                engineSmoke.SetActive(false);

        }

        if (health < 10) {

            if (!explosion.activeSelf)
                explosion.SetActive(true);

        } else {

            if (explosion.activeSelf)
                explosion.SetActive(false);

        }

    }

    private void OnDisable() {

        health = 100f;

    }

}
