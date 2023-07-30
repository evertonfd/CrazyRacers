using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_ShowroomCamera : MonoBehaviour{

    public Transform target;        //  Camera target.
    public float distance = 5f;      //  Distance to the target.

    public bool rotate = true;
    public float speed = 25f;     //  X speed of the camera.

    private float x = 0f;       //  Current X input.
    private float y = 0f;       //  Current Y input.

    private void Start() {

        //  Getting initial X and Y angles.
        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;

    }

    private void LateUpdate() {

        //  If there is no target, return.
        if (!target)
            return;

        //  If self turn is enabled, increase X related to time with multiplier.
        if(rotate)
            x += speed / 2f * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position;

        //  Setting position and rotation of the camera.
        transform.rotation = rotation;
        transform.position = position;

    }

    public void SetHorizontal(float value) {

        rotate = false;
        x = value;

    }

    public void SetVertical(float value) {

        rotate = false;
        y = value;

    }

}
