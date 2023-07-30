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
/// Recorded clips.
/// </summary>
public class RCCP_Records : ScriptableObject {

    #region singleton
    private static RCCP_Records instance;
    public static RCCP_Records Instance { get { if (instance == null) instance = Resources.Load("RCCP_Records") as RCCP_Records; return instance; } }
    #endregion

    public List<RCCP_Recorder.RecordedClip> records = new List<RCCP_Recorder.RecordedClip>();

}
