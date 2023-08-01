using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using CrazyGames;
using Photon;
using Photon.Pun;

public class CR_UIManager : MonoBehaviour {

    private static CR_UIManager instance;
    public static CR_UIManager Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_UIManager>();

            return instance;

        }

    }

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI speedingScoreText;
    public TextMeshProUGUI driftingScoreText;
    public TextMeshProUGUI jumpScoreText;
    public TextMeshProUGUI destructionScoreText;
    public TextMeshProUGUI nearMissText;
    public TextMeshProUGUI bustedPanelText;

    public Animator playerScoresAnimator;
    public string oldScoreText = "";

    public GameObject pausedPanel;
    public GameObject bustedPanel;
    public GameObject policeLightsPanel;
    public GameObject raceStartingPanel;
    public GameObject winRacePanel;
    public GameObject loseRacePanel;
    public GameObject returningGaragePanel;
    public GameObject rewardedAdsButton;

    // Update is called once per frame
    void Update() {

        if (!CR_GameplayManager.Instance.player)
            return;

        moneyText.text = "$ " + CR_GameplayManager.Instance.player.TotalMoney.ToString("F0");
        scoreText.text = CR_GameplayManager.Instance.player.TotalScore.ToString("F0");
        speedingScoreText.text = CR_GameplayManager.Instance.player.score_Speeding.ToString("F0");
        driftingScoreText.text = CR_GameplayManager.Instance.player.score_Drift.ToString("F0");
        jumpScoreText.text = CR_GameplayManager.Instance.player.score_AirTime.ToString("F0");
        destructionScoreText.text = CR_GameplayManager.Instance.player.score_Destruction.ToString("F0");
        nearMissText.text = CR_GameplayManager.Instance.player.score_NearMiss.ToString("F0");

        if (oldScoreText != scoreText.text) {

            playerScoresAnimator.SetBool("On", true);

        } else {

            playerScoresAnimator.SetBool("On", false);

        }

        oldScoreText = scoreText.text;

        if (!bustedPanel.activeSelf && CR_GameplayManager.Instance.player.busted) {

            bustedPanel.SetActive(true);
            bustedPanelText.text = "You must pay fine to be free! Click to pay " + "$ " + (CR_GameplayManager.Instance.player.TotalMoney / 10f).ToString("F0");

            if (CR_API.GetMoney() != 0)
                rewardedAdsButton.SetActive(true);
            else
                rewardedAdsButton.SetActive(false);

        }

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (pausedPanel.activeSelf)
                Resume();
            else
                Pause();

        }

        policeLightsPanel.SetActive(CR_PolicesManager.Instance.inPursueOnPlayer);

    }

    public void RaceCountdownEnable() {

        raceStartingPanel.SetActive(true);

    }

    public void RaceCountdownDisable() {

        raceStartingPanel.SetActive(false);

    }

    public void WinRace() {

        winRacePanel.SetActive(true);
        StartCoroutine(DisableObject(winRacePanel));

    }

    public void LoseRace() {

        loseRacePanel.SetActive(true);
        StartCoroutine(DisableObject(loseRacePanel));

    }

    private IEnumerator DisableObject(GameObject gameObject) {

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }

    public void SetBustedFree() {

        CR_GameplayManager.Instance.player.SetFree();
        bustedPanel.SetActive(false);
        CR_GameplayManager.Instance.player.money_Penalty += CR_GameplayManager.Instance.player.TotalMoney / 10f;

    }

    public void Pause() {

        pausedPanel.SetActive(true);

        if (PlayerPrefs.GetInt("Mode", 0) == 0) {

            Time.timeScale = 0;
            AudioListener.pause = true;

        }

        CrazyEvents.Instance.GameplayStop();

    }

    public void Resume() {

        pausedPanel.SetActive(false);

        if (PlayerPrefs.GetInt("Mode", 0) == 0) {

            Time.timeScale = 1.2f;
            AudioListener.pause = false;

        }

        CrazyEvents.Instance.GameplayStart();

    }

    public void Restart() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void MainMenu() {

        if (!PhotonNetwork.IsConnected)
            SceneManager.LoadScene(0);
        else
            PhotonNetwork.LoadLevel(0);

    }

    public void WatchRewardedAds() {

        StartCoroutine(WatchRewardedAdsWithDelay());

    }

    private IEnumerator WatchRewardedAdsWithDelay() {

        AudioListener.pause = true;

        yield return new WaitForFixedUpdate();

        CrazyAds.Instance.beginAdBreakRewarded(WatchedRewardedAdsWithSuccess, WatchedRewardedAdsWithFail);

    }

    private void WatchedRewardedAdsWithSuccess() {

        AudioListener.pause = false;

        CR_GameplayManager.Instance.player.SetFree();
        bustedPanel.SetActive(false);

    }

    private void WatchedRewardedAdsWithFail() {

        AudioListener.pause = false;

    }

}
