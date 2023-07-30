using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CR_UIOptionsManager : MonoBehaviour {

    public Animator lowButton;
    public Animator medButton;
    public Animator highButton;

    public Slider audioSlider;
    public Slider musicSlider;



    // Start is called before the first frame update
    private void OnEnable() {

        if (QualitySettings.GetQualityLevel() == 0) {

            lowButton.Play("click");
            medButton.Play("normal");
            highButton.Play("normal");

        }

        if (QualitySettings.GetQualityLevel() == 1) {

            lowButton.Play("normal");
            medButton.Play("click");
            highButton.Play("normal");

        }

        if (QualitySettings.GetQualityLevel() == 2) {

            lowButton.Play("normal");
            medButton.Play("normal");
            highButton.Play("click");

        }

        audioSlider.value = PlayerPrefs.GetFloat("AudioVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

    }

    // Update is called once per frame
    public void SetQuality(int level) {

        QualitySettings.SetQualityLevel(level);

        if (QualitySettings.GetQualityLevel() == 0) {

            lowButton.Play("click");
            medButton.Play("normal");
            highButton.Play("normal");

        }

        if (QualitySettings.GetQualityLevel() == 1) {

            lowButton.Play("normal");
            medButton.Play("click");
            highButton.Play("normal");

        }

        if (QualitySettings.GetQualityLevel() == 2) {

            lowButton.Play("normal");
            medButton.Play("normal");
            highButton.Play("click");

        }

    }

    public void SetAudio(Slider slider) {

        AudioListener.volume = slider.value;
        PlayerPrefs.SetFloat("AudioVolume", slider.value);

    }

    public void SetMusic(Slider slider) {

        if(FindObjectOfType<CR_Soundtrack>())
            FindObjectOfType<CR_Soundtrack>().volume = slider.value;

        PlayerPrefs.SetFloat("MusicVolume", slider.value);

    }

}
