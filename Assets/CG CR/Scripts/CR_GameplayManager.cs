using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using CrazyGames;

public class CR_GameplayManager : MonoBehaviour {

    private static CR_GameplayManager instance;
    public static CR_GameplayManager Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_GameplayManager>();

            return instance;

        }

    }

    public CR_PlayerManager player;
    public List<CR_PlayerManager> allPlayers = new List<CR_PlayerManager>();

    public Transform[] spawnPoint;

    public int lastSavedMoney = 0;
    public GameObject trafficContainer;

    public GameObject firstTimeIntro;
    public RCCP_Camera rcccam;
    public GameObject dashboard;

    // Start is called before the first frame update
    void Start() {

        Time.timeScale = 1.2f;
        AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        AudioListener.pause = false;

        FindObjectOfType<CR_Soundtrack>().volume = PlayerPrefs.GetFloat("MusicVolume", 1f) / 2f;

        SpawnPlayer();

        CrazyEvents.Instance.GameplayStart();

        InvokeRepeating("SaveMoneyPeriodic", 1f, 1f);
        InvokeRepeating("GetOtherPlayers", 1f, 1f);

        if (!PlayerPrefs.HasKey("FirstTime"))
            StartCoroutine(FirstTimeIntro());
        else
            firstTimeIntro.SetActive(false);

    }

    private void GetOtherPlayers() {

        CR_PlayerManager[] otherPlayersArray = FindObjectsOfType<CR_PlayerManager>();
        allPlayers.Clear();

        for (int i = 0; i < otherPlayersArray.Length; i++) {

            allPlayers.Add(otherPlayersArray[i]);

        }

    }

    private IEnumerator FirstTimeIntro() {

        yield return new WaitForFixedUpdate();

        firstTimeIntro.SetActive(true);
        rcccam.gameObject.SetActive(false);
        dashboard.SetActive(false);

        PlayerPrefs.SetInt("FirstTime", 1);

        yield return new WaitForSeconds(25);

        firstTimeIntro.SetActive(false);
        rcccam.gameObject.SetActive(true);
        dashboard.SetActive(true);

    }

    // Update is called once per frame
    private void SpawnPlayer() {

        if (PlayerPrefs.GetInt("Mode", 0) == 0) {

            int randomPoint = Random.Range(0, spawnPoint.Length);

            player = RCCP.SpawnRCC(CR_PlayerVehicles.Instance.cars[PlayerPrefs.GetInt("VehicleIndex", 0)].vehicle.GetComponent<RCCP_CarController>(), spawnPoint[randomPoint].position, spawnPoint[randomPoint].rotation, true, true, true).gameObject.GetComponent<CR_PlayerManager>();

            Destroy(player.GetComponent<RCCP_PhotonSync>());
            Destroy(player.GetComponent<PhotonView>());

        } else {

            player = PhotonNetwork.Instantiate("Player Vehicles/" + CR_PlayerVehicles.Instance.cars[PlayerPrefs.GetInt("VehicleIndex", 0)].vehicle.GetComponent<RCCP_CarController>().gameObject.name, spawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, spawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].rotation, 0).GetComponent<CR_PlayerManager>();
            RCCP.RegisterPlayerVehicle(player.CarController, true, true);

        }

        if (RCCP_PlayerPrefsX.GetVector3("PlayerPosition", Vector3.zero) != Vector3.zero) {

            player.transform.position = RCCP_PlayerPrefsX.GetVector3("PlayerPosition", Vector3.zero);
            player.transform.eulerAngles = RCCP_PlayerPrefsX.GetVector3("PlayerRotation", Vector3.zero);

            PlayerPrefs.DeleteKey("PlayerPosition");
            PlayerPrefs.DeleteKey("PlayerRotation");

        }

    }

    private void SaveMoneyPeriodic() {

        CR_API.ChangeMoney(player.TotalMoney - lastSavedMoney);
        lastSavedMoney = player.TotalMoney;

    }

    public void ToggleTraffic(bool state) {

        if (!trafficContainer)
            trafficContainer = GameObject.Find("TrafficCarsContainer");

        if (trafficContainer)
            trafficContainer.SetActive(state);

    }

    public void SkipIntro() {

        StopCoroutine(FirstTimeIntro());
        PlayerPrefs.SetInt("FirstTime", 1);
        firstTimeIntro.SetActive(false);
        rcccam.gameObject.SetActive(true);
        dashboard.SetActive(true);

    }

}
