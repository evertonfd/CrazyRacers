//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// RCCP Camera will be parented to this gameobject when current camera mode is Wheel Camera.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Camera/RCCP Wheel Camera")]
public class RCCP_WheelCamera : MonoBehaviour {

    /// <summary>
    /// Fix shaking bug related to rigidbody.
    /// </summary>
    public void FixShake() {

        StartCoroutine(FixShakeDelayed());

    }

    private IEnumerator FixShakeDelayed() {

        if (!GetComponent<Rigidbody>())
            yield break;

        yield return new WaitForFixedUpdate();
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        yield return new WaitForFixedUpdate();
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

    }

}
