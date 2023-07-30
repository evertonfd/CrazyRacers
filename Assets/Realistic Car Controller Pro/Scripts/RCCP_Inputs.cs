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
/// Main inputs of the player.
/// </summary>
[System.Serializable]
public class RCCP_Inputs {

    public float throttleInput = 0f;
    public float brakeInput = 0f;
    public float steerInput = 0f;
    public float handbrakeInput = 0f;
    public float clutchInput = 0f;
    public float nosInput = 0f;

    public Vector2 mouseInput = new Vector2(0f, 0f);

}
