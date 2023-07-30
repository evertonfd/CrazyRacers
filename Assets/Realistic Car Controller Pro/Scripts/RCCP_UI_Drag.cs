//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Mobile UI Drag used for orbiting RCCP Camera.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/Mobile/RCCP UI Drag")]
public class RCCP_UI_Drag : MonoBehaviour, IDragHandler, IEndDragHandler {

    private bool isPressing = false;        //  Currently pressing?

    private void Awake() {

        //  If mobile controller is not enabled disable the gameobject and return.
        if (!RCCP_Settings.Instance.mobileControllerEnabled) {

            gameObject.SetActive(false);
            return;

        }

    }

    /// <summary>
    /// While dragging.
    /// </summary>
    /// <param name="data"></param>
    public void OnDrag(PointerEventData data) {

        //  If mobile controller is not enabled, return.
        if (!RCCP_Settings.Instance.mobileControllerEnabled)
            return;

        isPressing = true;

        if (RCCP_SceneManager.Instance.activePlayerCamera)
            RCCP_SceneManager.Instance.activePlayerCamera.OnDrag(data);

    }

    public void OnEndDrag(PointerEventData data) {

        //  If mobile controller is not enabled, return.
        if (!RCCP_Settings.Instance.mobileControllerEnabled)
            return;

        isPressing = false;

    }

    private void OnDisable() {

        isPressing = false;

    }

}
