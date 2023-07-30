using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_CarShadow : MonoBehaviour {

    public Transform vehicleTransform;

    void Update() {

        transform.forward = vehicleTransform.forward;
        transform.Rotate(Vector3.right, 90f, Space.Self);

    }

}
