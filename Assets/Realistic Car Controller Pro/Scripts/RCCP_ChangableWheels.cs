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
/// Changes wheels at runtime. It holds changable wheels as prefab in an array.
/// </summary>
[System.Serializable]
public class RCCP_ChangableWheels : ScriptableObject {

    #region singleton
    private static RCCP_ChangableWheels instance;
    public static RCCP_ChangableWheels Instance { get { if (instance == null) instance = Resources.Load("RCCP_ChangableWheels") as RCCP_ChangableWheels; return instance; } }
    #endregion

    [System.Serializable]
    public class ChangableWheels {

        public GameObject wheel;

    }

    public ChangableWheels[] wheels;

}


