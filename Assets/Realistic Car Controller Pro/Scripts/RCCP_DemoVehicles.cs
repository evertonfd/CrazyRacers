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
/// All demo vehicles.
/// </summary>
public class RCCP_DemoVehicles : ScriptableObject {

    public RCCP_CarController[] vehicles;

    #region singleton
    private static RCCP_DemoVehicles instance;
    public static RCCP_DemoVehicles Instance { get { if (instance == null) instance = Resources.Load("RCCP_DemoVehicles") as RCCP_DemoVehicles; return instance; } }
    #endregion

}
