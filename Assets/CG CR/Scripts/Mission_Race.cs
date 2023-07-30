using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Race : MonoBehaviour{

    public Transform raceStartPoint;
    public GameObject raceCource;

    public RCCP_CarController raceCar1;
    public RCCP_CarController raceCar2;

    public Transform raceCar1Position;
    public Transform raceCar2Position;

    private void OnTriggerEnter(Collider other) {

        CR_PlayerManager playerManager = other.GetComponentInParent<CR_PlayerManager>();

        if (!playerManager)
            return;

        RCCP.Transport(playerManager.CarController, raceStartPoint.position, raceStartPoint.rotation);
        StartCoroutine(StartRace());

    }

    private IEnumerator StartRace() {

        raceCar1.transform.position = raceCar1Position.position;
        raceCar2.transform.position = raceCar2Position.position;
        raceCar1.transform.rotation = raceCar1Position.rotation;
        raceCar2.transform.rotation = raceCar2Position.rotation;

        raceCar1.canControl = false;
        raceCar2.canControl = false;

        raceCar1.OtherAddonsManager.AI.currentWaypointIndex = 0;
        raceCar2.OtherAddonsManager.AI.currentWaypointIndex = 0;

        CR_GameplayManager.Instance.ToggleTraffic(false);

        raceCource.SetActive(true);
        CR_UIManager.Instance.RaceCountdownEnable();
        CR_GameplayManager.Instance.player.CarController.canControl = false;
        yield return new WaitForSeconds(3);
        CR_GameplayManager.Instance.player.CarController.canControl = true;
        raceCar1.canControl = true;
        raceCar2.canControl = true;
        yield return new WaitForSeconds(.9f);
        CR_UIManager.Instance.RaceCountdownDisable();
        gameObject.SetActive(false);

    }

}
