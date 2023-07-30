using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_SpeedBooster : MonoBehaviour {

    public float force = 1000f;
    public AudioSource audioSource;

    private void OnTriggerStay(Collider other) {

        CR_PlayerManager playerManager = other.GetComponentInParent<CR_PlayerManager>();

        if (!playerManager)
            return;

        if (Mathf.Abs(CalculateAngle(transform.rotation, playerManager.transform.rotation)) > 30f)
            return;

        if (playerManager.CarController.throttleInput_V >= .5f) {

            playerManager.CarController.Rigid.AddForce(transform.forward * force, ForceMode.Force);

            if (!audioSource.isPlaying)
                audioSource.Play();

        }

    }

    public static float CalculateAngle(Quaternion transformA, Quaternion transformB) {

        var forwardA = transformA * Vector3.forward;
        var forwardB = transformB * Vector3.forward;

        float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
        float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

        var angleDiff = Mathf.DeltaAngle(angleA, angleB);

        return angleDiff;

    }

}
