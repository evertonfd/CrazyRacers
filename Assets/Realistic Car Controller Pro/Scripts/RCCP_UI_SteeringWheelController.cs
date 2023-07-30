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
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// UI Steering Wheel controller.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/Mobile/RCCP UI Steering Wheel")]
public class RCCP_UI_SteeringWheelController : MonoBehaviour {

    private Image steeringWheelTexture;     //  Steering wheel texture.

    public float input = 0f;
    private float steeringWheelAngle = 0f;
    public float steeringWheelMaximumsteerAngle = 270f;
    public float steeringWheelResetPosSpeed = 20f;
    public float steeringWheelCenterDeadZoneRadius = 5f;

    private RectTransform steeringWheelRect;
    private CanvasGroup steeringWheelCanvasGroup;

    private float steeringWheelTempAngle, steeringWheelNewAngle = 0f;
    private bool steeringWheelPressed = false;

    private Vector2 steeringWheelCenter, steeringWheelTouchPos = new Vector2();

    private EventTrigger eventTrigger;

    private void Awake() {

        //	Initializing the ui wheel with proper event triggers.
        SteeringWheelInit();

    }

    private void OnEnable() {

        //  Make sure to reset then when enabling / disabling the steering wheel.
        steeringWheelPressed = false;
        input = 0f;

    }

    private void LateUpdate() {

        //	Visual steering wheel controlling.
        SteeringWheelControlling();

        //	Receiving input from the steering wheel.
        input = GetSteeringWheelInput();

    }

    /// <summary>
    /// Initialization of the steering wheel.
    /// </summary>
    private void SteeringWheelInit() {

        steeringWheelTexture = GetComponent<Image>();

        if (steeringWheelRect && !steeringWheelTexture)
            return;

        steeringWheelRect = steeringWheelTexture.rectTransform;
        steeringWheelCanvasGroup = steeringWheelTexture.GetComponent<CanvasGroup>();
        steeringWheelCenter = steeringWheelRect.position;

        SteeringWheelEventsInit();

    }

    /// <summary>
    /// Events Initialization For Steering Wheel.
    /// </summary>
    private void SteeringWheelEventsInit() {

        eventTrigger = steeringWheelTexture.GetComponent<EventTrigger>();

        var a = new EventTrigger.TriggerEvent();
        a.AddListener(data => {
            var evData = (PointerEventData)data;
            data.Use();

            steeringWheelPressed = true;
            steeringWheelTouchPos = evData.position;
            steeringWheelTempAngle = Vector2.Angle(Vector2.up, evData.position - steeringWheelCenter);
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = a, eventID = EventTriggerType.PointerDown });


        var b = new EventTrigger.TriggerEvent();
        b.AddListener(data => {
            var evData = (PointerEventData)data;
            data.Use();
            steeringWheelTouchPos = evData.position;
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = b, eventID = EventTriggerType.Drag });


        var c = new EventTrigger.TriggerEvent();
        c.AddListener(data => {
            steeringWheelPressed = false;
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = c, eventID = EventTriggerType.EndDrag });

    }

    public float GetSteeringWheelInput() {

        return Mathf.Round(steeringWheelAngle / steeringWheelMaximumsteerAngle * 100) / 100;

    }

    public bool IsPressed() {

        return steeringWheelPressed;

    }

    private void SteeringWheelControlling() {

        if (!steeringWheelCanvasGroup || !steeringWheelRect) {

            if (steeringWheelTexture.gameObject)
                steeringWheelTexture.gameObject.SetActive(false);

            return;

        }

        if (!steeringWheelTexture.gameObject.activeSelf)
            steeringWheelTexture.gameObject.SetActive(true);

        if (steeringWheelPressed) {

            steeringWheelNewAngle = Vector2.Angle(Vector2.up, steeringWheelTouchPos - steeringWheelCenter);

            if (Vector2.Distance(steeringWheelTouchPos, steeringWheelCenter) > steeringWheelCenterDeadZoneRadius) {

                if (steeringWheelTouchPos.x > steeringWheelCenter.x)
                    steeringWheelAngle += steeringWheelNewAngle - steeringWheelTempAngle;
                else
                    steeringWheelAngle -= steeringWheelNewAngle - steeringWheelTempAngle;

            }

            if (steeringWheelAngle > steeringWheelMaximumsteerAngle)
                steeringWheelAngle = steeringWheelMaximumsteerAngle;
            else if (steeringWheelAngle < -steeringWheelMaximumsteerAngle)
                steeringWheelAngle = -steeringWheelMaximumsteerAngle;

            steeringWheelTempAngle = steeringWheelNewAngle;

        } else {

            if (!Mathf.Approximately(0f, steeringWheelAngle)) {

                float deltaAngle = steeringWheelResetPosSpeed;

                if (Mathf.Abs(deltaAngle) > Mathf.Abs(steeringWheelAngle)) {
                    steeringWheelAngle = 0f;
                    return;
                }

                steeringWheelAngle = Mathf.MoveTowards(steeringWheelAngle, 0f, deltaAngle * (Time.deltaTime * 100f));

            }

        }

        steeringWheelRect.eulerAngles = new Vector3(0f, 0f, -steeringWheelAngle);

    }

    private void OnDisable() {

        //  Make sure to reset then when enabling / disabling the steering wheel.
        steeringWheelPressed = false;
        input = 0f;

    }

}
