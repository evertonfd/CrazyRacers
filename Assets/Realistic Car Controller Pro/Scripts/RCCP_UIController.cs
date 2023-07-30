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
/// UI input (float) receiver from UI Button. 
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/Mobile/RCCP UI Controller")]
public class RCCP_UIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private Button button;      //  Actual button.

    public float input = 0f;        //  Input of the button. 1 when pressed, 0 when not pressed.
    public float sensitivity = 5f;      //  Sensitivity of the button. Input will reach the target value much sooner on higher values.
    public float gravity = 5f;      //  Gravity of the button. Input will reach to 0 much sooner on high values when button is not pressed.
    public bool isPressing = false;     //  Current pressing the button?

    private void Awake() {

        //  Getting button.
        button = GetComponent<Button>();

    }

    private void OnEnable() {

        //  Make sure to reset them.
        input = 0f;
        isPressing = false;

    }

    /// <summary>
    /// When down the button.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData) {

        isPressing = true;

    }

    /// <summary>
    /// When up the button.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData) {

        isPressing = false;

    }

    private void Update() {

        //  If button is not interactable, return wtih 0 input.
        if (button && !button.interactable) {

            isPressing = false;
            input = 0f;
            return;

        }

        //  If current pressing the button, increase it with sensitivity, and decrease it with gravity.
        if (isPressing)
            input += Time.deltaTime * sensitivity;
        else
            input -= Time.deltaTime * gravity;

        //  Clamping input between 0 - 1.
        if (input < 0f)
            input = 0f;

        if (input > 1f)
            input = 1f;

    }

    private void OnDisable() {

        input = 0f;
        isPressing = false;

    }

}
