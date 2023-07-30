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

/// <summary>
/// Exhaust.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Exhaust")]
public class RCCP_Exhaust : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public bool flameOnCutOff = false;      //  Flames on engine cut off.

    private ParticleSystem particle;        //  Smoke particles.
    private ParticleSystem.EmissionModule emission;     //  Smoke emission.
    public ParticleSystem flame;        //  Flame particles.
    private ParticleSystem.EmissionModule subEmission;      //  Flame emission.

    private Light flameLight;       //  Flame light.
    private LensFlare lensFlare;        // Lensflare of the flame light.

    public float flareBrightness = 1f;      //  Frlare brightness.
    private float finalFlareBrightness;     //  Calculated flare brigtness.

    public float flameTime = 0f;        //  Flame time.

    public Color flameColor = Color.red;        //  Flame color.
    public Color boostFlameColor = Color.blue;      //  Boost / Nos flame color.

    public float minEmission = 5f;      //  Emission limits
    public float maxEmission = 20f;

    public float minSize = 1f;      //  Size limits.
    public float maxSize = 4f;

    public float minSpeed = .1f;        //  Speed limits.
    public float maxSpeed = 1f;

    private void Start() {

        // Getting components.
        particle = GetComponent<ParticleSystem>();
        emission = particle.emission;

        //  If flame exists...
        if (flame) {

            //  Getting emission of the flame, light, and creating audio source.
            subEmission = flame.emission;
            flameLight = flame.GetComponentInChildren<Light>();

            //  If flame light exists, set render mode of the light depending of the option in RCCP Settings.
            if (flameLight)
                flameLight.renderMode = LightRenderMode.ForceVertex;

        }

        //  Getting lensflare.
        lensFlare = GetComponentInChildren<LensFlare>();

        if (flameLight) {

            if (flameLight.flare != null)
                flameLight.flare = null;

        }

    }

    private void Update() {

        //  If no car controller found, or particle, return.
        if (!CarController || !particle)
            return;

        if (!CarController.Engine)
            return;

        Smoke();
        Flame();

        if (lensFlare)
            LensFlare();

    }

    /// <summary>
    /// Smoke particles.
    /// </summary>
    private void Smoke() {

        //  If engine is running, set speed, size, and emission rates of the smoke particles.
        if (CarController.Engine.engineRunning) {

            var main = particle.main;

            if (Mathf.Abs(CarController.speed) > 20) {

                if (emission.enabled)
                    emission.enabled = false;

                return;

            }

            if (!emission.enabled)
                emission.enabled = true;

            emission.rateOverTime = Mathf.Clamp(maxEmission * CarController.throttleInput_V, minEmission, maxEmission);
            main.startSpeed = Mathf.Clamp(maxSpeed * CarController.throttleInput_V, minSpeed, maxSpeed);
            main.startSize = Mathf.Clamp(maxSize * CarController.throttleInput_V, minSize, maxSize);

        } else {

            if (emission.enabled)
                emission.enabled = false;

        }

    }

    /// <summary>
    /// Flame particles with light effects.
    /// </summary>
    private void Flame() {

        //  If engine is running, set color of the flame, create audio source.
        if (CarController.Engine.engineRunning) {

            var main = flame.main;

            if (CarController.throttleInput_V >= .25f)
                flameTime = 0f;

            if ((flameOnCutOff && (CarController.engineRPM >= 5000 && CarController.engineRPM <= 5500 && CarController.throttleInput_V <= .25f && flameTime <= .5f)) || CarController.nosInput_V >= .75f) {

                flameTime += Time.deltaTime;
                subEmission.enabled = true;

                if (flameLight)
                    flameLight.intensity = 3f * Random.Range(.25f, 1f);

                if (CarController.nosInput_V >= .75f && flame) {

                    main.startColor = boostFlameColor;

                    if (flameLight)
                        flameLight.color = main.startColor.color;

                } else {

                    main.startColor = flameColor;

                    if (flameLight)
                        flameLight.color = main.startColor.color;

                }

            } else {

                subEmission.enabled = false;

                if (flameLight)
                    flameLight.intensity = 0f;

            }

        } else {

            if (emission.enabled)
                emission.enabled = false;

            subEmission.enabled = false;

            if (flameLight)
                flameLight.intensity = 0f;

        }

    }

    /// <summary>
    /// Lensflare calculation.
    /// </summary>
    private void LensFlare() {

        //  If there is no camera, return.
        if (!Camera.main)
            return;

        float distanceTocam = Vector3.Distance(transform.position, Camera.main.transform.position);
        float angle = Vector3.Angle(transform.forward, Camera.main.transform.position - transform.position);

        if (angle != 0)
            finalFlareBrightness = flareBrightness * (4 / distanceTocam) * ((100f - (1.11f * angle)) / 100f) / 2f;

        if (flameLight) {

            lensFlare.brightness = finalFlareBrightness * flameLight.intensity;
            lensFlare.color = flameLight.color;

        }

    }

}
