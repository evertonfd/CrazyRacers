using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_FinishRacePoint : MonoBehaviour{

    public bool playerCanWin = true;
    public GameObject raceCourse;
    public GameObject marker;

    public void OnTriggerEnter(Collider other) {

        CR_AIRacer aIRacer = other.GetComponentInParent<CR_AIRacer>();

        if (aIRacer)
            playerCanWin = false;

        CR_PlayerManager player = other.GetComponentInParent<CR_PlayerManager>();

        if (player) {

            if (playerCanWin)
                RewardPlayer();
            else
                LosePlayer();

            raceCourse.SetActive(false);
            CR_GameplayManager.Instance.ToggleTraffic(true);
            marker.SetActive(true);

        }

    }

    private void RewardPlayer() {

        CR_UIManager.Instance.WinRace();
        CR_API.ChangeMoney(1000);
        CR_GameplayManager.Instance.player.money_Missions += 1000f;

    }

    private void LosePlayer() {

        CR_UIManager.Instance.LoseRace();

    }

}
