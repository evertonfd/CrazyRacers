using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Drift : MonoBehaviour{

    public Transform raceStartPoint;

    private void OnTriggerEnter(Collider other) {

        CR_PlayerManager playerManager = other.GetComponentInParent<CR_PlayerManager>();

        if (!playerManager)
            return;

        playerManager.driftingCountdown = 100f;

        RCCP.Transport(playerManager.CarController, raceStartPoint.position, raceStartPoint.rotation);
        StartCoroutine(StartRace());

    }

    private IEnumerator StartRace() {

        CR_UIManager.Instance.RaceCountdownEnable();
        CR_GameplayManager.Instance.player.CarController.canControl = false;
        yield return new WaitForSeconds(3);
        CR_GameplayManager.Instance.player.CarController.canControl = true;
        yield return new WaitForSeconds(.9f);
        CR_UIManager.Instance.RaceCountdownDisable();
        gameObject.SetActive(false);

    }

}
