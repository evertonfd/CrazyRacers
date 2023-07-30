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
/// RCCP Events. You can listen any events below in your game.
/// </summary>
public class RCCP_Events {

    /// <summary>
    /// When a vehicle spawns.
    /// </summary>
    /// <param name="rccp"></param>
    public delegate void onRCCPSpawned(RCCP_CarController rccp);
    public static event onRCCPSpawned OnRCCPSpawned;

    /// <summary>
    /// When a vehicle destoyed or disabled.
    /// </summary>
    /// <param name="rccp"></param>
    public delegate void onRCCPDestroyed(RCCP_CarController rccp);
    public static event onRCCPDestroyed OnRCCPDestroyed;

    /// <summary>
    /// When a vehicle spawned with AI component.
    /// </summary>
    /// <param name="rccp"></param>
    public delegate void onRCCPAISpawned(RCCP_CarController rccp);
    public static event onRCCPAISpawned OnRCCPAISpawned;

    /// <summary>
    /// When a vehicle destroyed or disabled with AI component.
    /// </summary>
    /// <param name="rccp"></param>
    public delegate void onRCCPAIDestroyed(RCCP_CarController rccp);
    public static event onRCCPAIDestroyed OnRCCPAIDestroyed;

    /// <summary>
    /// When a vehicle collides.
    /// </summary>
    /// <param name="rccp"></param>
    /// <param name="collision"></param>
    public delegate void onRCCPCollision(RCCP_CarController rccp, Collision collision);
    public static event onRCCPCollision OnRCCPCollision;

    /// <summary>
    /// When RCCP camera spawns.
    /// </summary>
    /// <param name="cam"></param>
    public delegate void onRCCPCameraSpawned(RCCP_Camera cam);
    public static event onRCCPCameraSpawned OnRCCPCameraSpawned;

    /// <summary>
    /// When RCCP UI spawns.
    /// </summary>
    /// <param name="UI"></param>
    public delegate void onRCCPUISpawned(RCCP_UIManager UI);
    public static event onRCCPUISpawned OnRCCPUISpawned;

    /// <summary>
    /// When RCCP UI destoyed or disabled.
    /// </summary>
    /// <param name="UI"></param>
    public delegate void onRCCPUIDestroyed(RCCP_UIManager UI);
    public static event onRCCPUIDestroyed OnRCCPUIDestroyed;

    public delegate void onRCCPUIInformer(string text);
    public static event onRCCPUIInformer OnRCCPUIInformer;

    public static void Event_OnRCCPSpawned(RCCP_CarController rccp) {

        if (OnRCCPSpawned != null)
            OnRCCPSpawned(rccp);

    }

    public static void Event_OnRCCPDestroyed(RCCP_CarController rccp) {

        if (OnRCCPDestroyed != null)
            OnRCCPDestroyed(rccp);

    }

    public static void Event_OnRCCPCameraSpawned(RCCP_Camera cam) {

        if (OnRCCPCameraSpawned != null)
            OnRCCPCameraSpawned(cam);

    }

    public static void Event_OnRCCPCollision(RCCP_CarController rccp, Collision collision) {

        if (OnRCCPCollision != null)
            OnRCCPCollision(rccp, collision);

    }

    public static void Event_OnRCCPUISpawned(RCCP_UIManager UI) {

        if (OnRCCPUISpawned != null)
            OnRCCPUISpawned(UI);

    }

    public static void Event_OnRCCPUIDestroyed(RCCP_UIManager UI) {

        if (OnRCCPUIDestroyed != null)
            OnRCCPUIDestroyed(UI);

    }

    public static void Event_OnRCCPAISpawned(RCCP_CarController AI) {

        if (OnRCCPAISpawned != null)
            OnRCCPAISpawned(AI);

    }

    public static void Event_OnRCCPAIDestroyed(RCCP_CarController AI) {

        if (OnRCCPAIDestroyed != null)
            OnRCCPAIDestroyed(AI);

    }

    public static void Event_OnRCCPUIInformer(string info) {

        if (OnRCCPUIInformer != null)
            OnRCCPUIInformer(info);

    }

}
