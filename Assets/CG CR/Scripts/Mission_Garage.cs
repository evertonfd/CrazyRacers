using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission_Garage : MonoBehaviour {

    public bool cancelInvoke = false;
    public bool counting = false;

    private void OnTriggerEnter(Collider other) {

        CR_PlayerManager playerManager = other.GetComponentInParent<CR_PlayerManager>();

        if (!playerManager)
            return;

        if (!counting) {

            counting = true;

            if (PlayerPrefs.GetInt("Mode", 0) == 0) {

                playerManager.transform.position += -playerManager.transform.forward * 5f;

                RCCP_PlayerPrefsX.SetVector3("PlayerPosition", playerManager.transform.position);
                RCCP_PlayerPrefsX.SetVector3("PlayerRotation", playerManager.transform.eulerAngles);

                playerManager.transform.position += playerManager.transform.forward * 5f;

            }

            CR_UIManager.Instance.returningGaragePanel.SetActive(true);
            CR_UIManager.Instance.returningGaragePanel.GetComponent<Animator>().SetTrigger("Count");

            Invoke("LoadGarage", 3f);

        }

    }

    private void OnTriggerExit(Collider other) {

        CR_PlayerManager playerManager = other.GetComponentInParent<CR_PlayerManager>();

        if (!playerManager)
            return;

        CR_UIManager.Instance.returningGaragePanel.SetActive(false);

        counting = false;

    }

    private void LoadGarage() {

        if(counting)
            SceneManager.LoadScene(0);

    }

}
