//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Don't rotate the image.
/// </summary>
public class RCCP_UI_Element_DontRotate : MonoBehaviour {

    private void Update() {

        transform.rotation = Quaternion.identity;

    }

}
