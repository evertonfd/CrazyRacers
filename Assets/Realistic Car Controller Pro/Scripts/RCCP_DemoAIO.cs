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
using UnityEngine.SceneManagement;

public class RCCP_DemoAIO : MonoBehaviour {

    public static RCCP_DemoAIO Instance;

    public GameObject content;
    public GameObject loading;
    public GameObject back;

    public GameObject[] photonButtons;
    public GameObject[] sharedAssetsButtons;
    public GameObject photonInfo;
    public GameObject sharedAssetsInfo;

    private void Awake() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else {

            Destroy(this.gameObject);

        }

    }

    void Start() {

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

#if RCCP_PHOTON

        for (int i = 0; i < photonButtons.Length; i++)
            photonButtons[i].SetActive(true);

        photonInfo.SetActive(false);

#else

        for (int i = 0; i < photonButtons.Length; i++)
            photonButtons[i].SetActive(false);

        photonInfo.SetActive(true);

#endif

#if BCG_ENTEREXIT

        for (int i = 0; i < sharedAssetsButtons.Length; i++)
            sharedAssetsButtons[i].SetActive(true);

        sharedAssetsInfo.SetActive(false);

#else

        for (int i = 0; i < sharedAssetsButtons.Length; i++)
            sharedAssetsButtons[i].SetActive(false);

        sharedAssetsInfo.SetActive(true);

#endif

    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1) {

        loading.SetActive(false);

    }

    public void LoadScene(int levelIndex) {

        loading.SetActive(true);
        SceneManager.LoadSceneAsync(levelIndex);

        if (levelIndex == 0) {

            content.SetActive(true);
            back.SetActive(false);

        } else {

            content.SetActive(false);
            back.SetActive(true);

        }

        if (SceneManager.GetSceneByBuildIndex(levelIndex).name == "RCCP_Scene_PhotonLobby" || SceneManager.GetSceneByBuildIndex(levelIndex).name == "RCCP_Scene_Blank_Photon")
            back.SetActive(false);

    }

}
