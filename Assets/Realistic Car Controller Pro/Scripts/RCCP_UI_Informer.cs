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
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI informer panel with the text.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP UI Informer")]
public class RCCP_UI_Informer : MonoBehaviour {

    //  Informer as instance.
    private static RCCP_UI_Informer instance;
    public static RCCP_UI_Informer Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<RCCP_UI_Informer>();

            return instance;

        }

    }

    public TMP_Text informerText;       //  Informer text.
    public CanvasGroup cGroup;      //  Canvas group.
    public float timer = 3f;        //  Timer to deactive the canvas.
    private float time = 0f;        //  Timer.

    private void OnEnable() {

        RCCP_Events.OnRCCPUIInformer += RCCP_Events_OnRCCPUIInformer;

    }

    private void RCCP_Events_OnRCCPUIInformer(string text) {

        Display(text);

    }

    private void Update() {

        //  Timer.
        time -= Time.deltaTime;

        //  Limiting the timer.
        if (time < 0)
            time = 0f;

        //  If timer is 0, disable the canvas group.
        if (time <= 0 && cGroup.gameObject.activeSelf)
            cGroup.gameObject.SetActive(false);

    }

    /// <summary>
    /// Displaying the target string.
    /// </summary>
    /// <param name="textToDisplay"></param>
    public void Display(string textToDisplay) {

        //  If no informer text found, or no canvas group found, return.
        if (!informerText || !cGroup)
            return;

        time = timer;
        cGroup.gameObject.SetActive(true);
        informerText.gameObject.GetComponent<Animator>().Play(0);
        informerText.text = textToDisplay;

    }

    private void OnDisable() {

        RCCP_Events.OnRCCPUIInformer -= RCCP_Events_OnRCCPUIInformer;

    }

}
