//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Showroom camera while selecting the vehicles.
/// </summary>
public class RCCP_CameraCarSelection : MonoBehaviour {

    public Transform target;        //  Camera target.
    public float distance = 5f;      //  Distance to the target.

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
        x += speed / 2f * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position;

        //  Setting position and rotation of the camera.
        transform.SetPositionAndRotation(position, rotation);

    }

}