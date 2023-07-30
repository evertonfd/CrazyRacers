//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Color Picker with UI Sliders.
/// </summary>
public class RCCP_ColorPickerBySliders : MonoBehaviour {

    public Color color;     // Main color.

    // Sliders per color channel.
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    private void Update() {

        // Assigning new color to main color.
        color = new Color(redSlider.value, greenSlider.value, blueSlider.value);

    }

}
