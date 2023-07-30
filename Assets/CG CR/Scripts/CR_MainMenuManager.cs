using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using CrazyGames;

public class CR_MainMenuManager : MonoBehaviour {

    private static CR_MainMenuManager instance;
    public static CR_MainMenuManager Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_MainMenuManager>();

            return instance;

        }

    }

    public Transform spawnPoint;
    public List<RCCP_CarController> spawnedVehicles = new List<RCCP_CarController>();

    public int currentVehicleIndex = 0;

    public TextMeshProUGUI totalMoneyText;
    public TMP_InputField nickName;

    public GameObject selectVehicleButton;
    public GameObject purchaseVehicleButton;

    public GameObject panel_Customize;
    public GameObject panel_Upgrade;
    public GameObject panel_Play;
    public GameObject panel_SelectVehicle;
    public GameObject panel_Settings;

    public GameObject driveButton;
    public GameObject continueButton;
    public bool watchedAd = false;

    public AudioSource musicSource;

    void Start() {

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        nickName.text = "Crazy Gamer " + Random.Range(0, 9999).ToString();
        FindObjectOfType<CR_Photon>(true).nickName = nickName.text;

        Time.timeScale = 1.2f;
        AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        AudioListener.pause = false;
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        CrazyEvents.Instance.GameplayStop();

        currentVehicleIndex = PlayerPrefs.GetInt("VehicleIndex", 0);

        Invoke("SpawnAllVehicles", .1f);

        if (RCCP_PlayerPrefsX.GetVector3("PlayerPosition", Vector3.zero) != Vector3.zero) {

            driveButton.SetActive(false);
            continueButton.SetActive(true);

        } else {

            driveButton.SetActive(true);
            continueButton.SetActive(false);

        }

        CrazySDK.Instance.GetSystemInfo(systemInfo => {
            Debug.Log(systemInfo.countryCode); // US
                                               // For browser and os, format is the same as https://github.com/faisalman/ua-parser-js
            Debug.Log(systemInfo.browser.name); // Chrome
            Debug.Log(systemInfo.browser.version); // 99.0.2534.75
            Debug.Log(systemInfo.os.name); // Windows
            Debug.Log(systemInfo.os.version); // 10
            Debug.Log(systemInfo.device.type); // possible values: "desktop", "tablet", "mobile"

            if (systemInfo.device.type == "tablet" || systemInfo.device.type == "mobile")
                RCCP_Settings.Instance.mobileControllerEnabled = false;
            else
                RCCP_Settings.Instance.mobileControllerEnabled = false;

        });


    }

    private void SpawnAllVehicles() {

        for (int i = 0; i < CR_PlayerVehicles.Instance.cars.Length; i++) {

            RCCP_CarController spawned = RCCP.SpawnRCC(CR_PlayerVehicles.Instance.cars[i].vehicle.GetComponent<RCCP_CarController>(), spawnPoint.position, spawnPoint.rotation, false, false, false);
            spawned.gameObject.SetActive(false);
            spawnedVehicles.Add(spawned);

            if (CR_PlayerVehicles.Instance.cars[i].price == 0)
                PlayerPrefs.SetInt(CR_PlayerVehicles.Instance.cars[i].vehicle.name, 1);

            Destroy(spawned.GetComponent<RCCP_PhotonSync>());
            Destroy(spawned.GetComponent<PhotonView>());

        }

        spawnedVehicles[currentVehicleIndex].gameObject.SetActive(true);
        EnableVehicle();

    }

    public void Next() {

        currentVehicleIndex++;

        if (currentVehicleIndex >= CR_PlayerVehicles.Instance.cars.Length)
            currentVehicleIndex = 0;

        EnableVehicle();

    }

    public void Prev() {

        currentVehicleIndex--;

        if (currentVehicleIndex < 0)
            currentVehicleIndex = CR_PlayerVehicles.Instance.cars.Length - 1;

        EnableVehicle();

    }

    private void EnableVehicle() {

        for (int i = 0; i < spawnedVehicles.Count; i++)
            spawnedVehicles[i].gameObject.SetActive(false);

        spawnedVehicles[currentVehicleIndex].gameObject.SetActive(true);
        spawnedVehicles[currentVehicleIndex].GetComponent<CR_VehicleModManager>().Initialize();
        spawnedVehicles[currentVehicleIndex].GetComponent<CR_VehicleModManager>().CheckUpgrades();

        bool ownedVehicle = false;

        if (PlayerPrefs.HasKey(CR_PlayerVehicles.Instance.cars[currentVehicleIndex].vehicle.name))
            ownedVehicle = true;

        selectVehicleButton.SetActive(ownedVehicle);
        purchaseVehicleButton.SetActive(!ownedVehicle);

        if (purchaseVehicleButton.activeSelf) {

            purchaseVehicleButton.GetComponentInChildren<TextMeshProUGUI>().text = "$ " + CR_PlayerVehicles.Instance.cars[currentVehicleIndex].price.ToString();

        }

        StartCoroutine(qwe());

    }

    private IEnumerator qwe() {

        yield return new WaitForFixedUpdate();
        CR_ModManager.Instance.CheckVehicle(spawnedVehicles[currentVehicleIndex].GetComponent<CR_PlayerManager>());

    }

    public void PurchaseVehicle() {

        int current = CR_API.GetMoney();

        if (current < CR_PlayerVehicles.Instance.cars[currentVehicleIndex].price) {

            CR_UIInformer.Instance.Display("Not Enough Cash", "You need to earn " + (CR_PlayerVehicles.Instance.cars[currentVehicleIndex].price - current).ToString() + " more cash to purchase this vehicle");
            return;

        }

        CR_API.ChangeMoney(-CR_PlayerVehicles.Instance.cars[currentVehicleIndex].price);
        PlayerPrefs.SetInt(CR_PlayerVehicles.Instance.cars[currentVehicleIndex].vehicle.name, 1);
        EnableVehicle();
        CR_UIInformer.Instance.Display("Congratulations!", "You have purchased this vehicle!");

    }

    public void SelectVehicle() {

        PlayerPrefs.SetInt("VehicleIndex", currentVehicleIndex);

    }

    public void SelectMode(int mode) {

        PlayerPrefs.SetInt("Mode", mode);

        if (mode == 0)
            OpenScene();

    }

    public void OpenPanel(GameObject activeMenu) {

        ClosePanels();

        if (activeMenu)
            activeMenu.SetActive(true);

    }

    public void OpenScene() {

        AudioListener.volume = 0f;
        AudioListener.pause = true;

        if (PlayerPrefs.GetInt("Mode", 0) == 0) {

            if (!watchedAd)
                CrazyAds.Instance.beginAdBreak(OpenSceneAfterAds, OpenSceneAfterAds);
            else
                OpenSceneAfterAds();

        } else {

            OpenSceneAfterAds();

        }

    }

    public void OpenSceneAfterAds() {

        AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        AudioListener.pause = false;

        if (PlayerPrefs.GetInt("Mode", 0) == 0)
            SceneManager.LoadSceneAsync(1);
        else
            PhotonNetwork.LoadLevel(1);

    }

    public void Continue() {

        PlayerPrefs.SetInt("Mode", 0);
        OpenScene();

    }

    public void FreeCashWithAds() {

        AudioListener.volume = 0f;
        AudioListener.pause = true;

        CrazyAds.Instance.beginAdBreakRewarded(SucceededRewardedAds, FailedRewardedAds);

    }

    public void SucceededRewardedAds() {

        watchedAd = true;
        AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        AudioListener.pause = false;

        CR_UIInformer.Instance.Display("Congratulations!", "You have been rewarded by $2500 by watching an ad!");
        CR_API.ChangeMoney(2500);

    }

    public void FailedRewardedAds() {

        AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        AudioListener.pause = false;

        CR_UIInformer.Instance.Display("Couldn't Reward!", "Try again!");

    }

    public void ClosePanels() {

        if (panel_Customize)
            panel_Customize.SetActive(false);

        if (panel_Upgrade)
            panel_Upgrade.SetActive(false);

        if (panel_Play)
            panel_Play.SetActive(false);

        if (panel_SelectVehicle)
            panel_SelectVehicle.SetActive(false);

        if (panel_Settings)
            panel_Settings.SetActive(false);

    }

    private void Update() {

        totalMoneyText.text = "$ " + CR_API.GetMoney().ToString();

    }

    private void OpenPhoton() {

        CR_Photon.Instance.ConnectToPhoton();

    }

    public void AddCash() {

        CR_API.ChangeMoney(10000);

    }

    public void DeleteSave() {

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}
