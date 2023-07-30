using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CR_UIGauges : MonoBehaviour {

    public RectTransform speed;
    public RectTransform minimap;

    public Image speedFill;
    public Image nosFill;
    public Image felonyFill;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI speedingText;
    public TextMeshProUGUI inPursueText;
    public TextMeshProUGUI driftingCountdownText;

    public float speedingTextTimer = 0f;
    public float previousFelony = 0f;

    // Start is called before the first frame update
    void Start() {

        if (!RCCP_Settings.Instance.mobileControllerEnabled)
            return;

        speed.anchorMin = new Vector2(1, 1);
        speed.anchorMax = new Vector2(1, 1);
        speed.anchoredPosition = new Vector2(speed.anchoredPosition.x, -speed.anchoredPosition.y - 70);

        minimap.anchorMin = new Vector2(0, 1);
        minimap.anchorMax = new Vector2(0, 1);
        minimap.anchoredPosition = new Vector2(minimap.anchoredPosition.x, -minimap.anchoredPosition.y - 70);

    }

    // Update is called once per frame
    void Update() {

        if (!CR_GameplayManager.Instance.player)
            return;

        speedFill.fillAmount = Mathf.Lerp(0f, 1f, CR_GameplayManager.Instance.player.CarController.speed / 300f);

        if (CR_GameplayManager.Instance.player.CarController.OtherAddonsManager.Nos.enabled)
            nosFill.fillAmount = Mathf.Lerp(0f, 1f, CR_GameplayManager.Instance.player.CarController.OtherAddonsManager.Nos.amount);
        else
            nosFill.fillAmount = 0f;

        felonyFill.fillAmount = CR_GameplayManager.Instance.player.felony / 100f;

        if (!inPursueText.gameObject.activeSelf) {

            if (previousFelony != CR_GameplayManager.Instance.player.felony)
                speedingTextTimer = 2f;

            if (speedingTextTimer > 0)
                speedingTextTimer -= Time.deltaTime;

            if (speedingTextTimer > 0 && CR_GameplayManager.Instance.player.felony > previousFelony)
                speedingText.gameObject.SetActive(true);
            else
                speedingText.gameObject.SetActive(false);

        } else {

            speedingText.gameObject.SetActive(false);
            
        }

        inPursueText.gameObject.SetActive(CR_PolicesManager.Instance.inPursueOnPlayer);

        previousFelony = CR_GameplayManager.Instance.player.felony;

        speedText.text = RCCP_SceneManager.Instance.activePlayerVehicle.speed.ToString("F0");

        if (RCCP_SceneManager.Instance.activePlayerVehicle.direction == 1)
            gearText.text = (RCCP_SceneManager.Instance.activePlayerVehicle.currentGear + 1).ToString("F0");
        else
            gearText.text = "R";

        if (CR_GameplayManager.Instance.player.driftingCountdown != 0) {

            driftingCountdownText.gameObject.SetActive(true);

            float minutes = Mathf.FloorToInt(CR_GameplayManager.Instance.player.driftingCountdown / 60);
            float seconds = Mathf.FloorToInt(CR_GameplayManager.Instance.player.driftingCountdown % 60);

            driftingCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        } else {

            driftingCountdownText.text = "";
            driftingCountdownText.gameObject.SetActive(false);

        }

    }

}
