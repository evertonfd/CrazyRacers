using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_SpeedParticles : MonoBehaviour {

    public RCCP_Camera cam;
    public ParticleSystem particles;

    private void Update() {

        if (cam.cameraTarget == null)
            return;

        ParticleSystem.EmissionModule em = particles.emission;

        if (cam.cameraTarget.playerVehicle.speed > 80f)
            em.enabled = true;
        else
            em.enabled = false;

    }

}
