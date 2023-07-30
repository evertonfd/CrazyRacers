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
/// Audio system for engine, brake, crashes, transmission, and other stuff.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Audio")]
public class RCCP_Audio : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    [System.Serializable]
    public class EngineSound {

        [HideInInspector] public AudioSource audioSourceOn;
        public AudioClip audioClipOn;

        [HideInInspector] public AudioSource audioSourceOff;
        public AudioClip audioClipOff;
        public Vector3 localPosition = new Vector3(0f, 0f, 1.5f);

        public float minPitch = .1f;
        public float maxPitch = 1f;

        public float minRPM = 600f;
        public float maxRPM = 8000f;

        public float minDistance = 10f;
        public float maxDistance = 200f;

        public float minVolume = 0f;
        public float maxVolume = 1f;

    }

    public EngineSound[] engineSounds = new EngineSound[3];

    [System.Serializable]
    public class EngineStart {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 1.5f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;

    }

    public EngineStart engineStart = new EngineStart();

    [System.Serializable]
    public class GearboxSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip[] audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 1f;
        public float maxDistance = 10f;

        public float maxVolume = 1f;

    }

    public GearboxSound gearboxSound = new GearboxSound();
    private int lastGear = 0;

    [System.Serializable]
    public class CrashSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip[] audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;

    }

    public CrashSound crashSound = new CrashSound();

    [System.Serializable]
    public class ReverseSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float minPitch = .8f;
        public float maxPitch = 1f;

        public float maxVolume = 1f;

    }

    public ReverseSound reverseSound = new ReverseSound();

    [System.Serializable]
    public class WindSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = .1f;

    }

    public WindSound windSound = new WindSound();

    [System.Serializable]
    public class BrakeSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = .1f;

    }

    public BrakeSound brakeSound = new BrakeSound();

    [System.Serializable]
    public class NosSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;

    }

    public NosSound nosSound = new NosSound();

    [System.Serializable]
    public class TurboSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 1.5f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;

    }

    public TurboSound turboSound = new TurboSound();

    [System.Serializable]
    public class BlowSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip[] audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, -1.5f);

        public float minDistance = 1f;
        public float maxDistance = 20f;

        public float maxVolume = .2f;

    }

    public BlowSound blowSound = new BlowSound();

    [System.Serializable]
    public class DeflateSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 1f);

        public float minDistance = 10f;
        public float maxDistance = 20f;

        public float maxVolume = 1f;

    }

    public DeflateSound wheelDeflateSound = new DeflateSound();

    [System.Serializable]
    public class InflateSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;
        public bool lastInflate = true;

    }

    public InflateSound wheelInflateSound = new InflateSound();

    [System.Serializable]
    public class FlatSound {

        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClips;
        public Vector3 localPosition = new Vector3(0f, 0f, 0f);

        public float minDistance = 10f;
        public float maxDistance = 100f;

        public float maxVolume = 1f;

    }

    public FlatSound wheelFlatSound;

    private void OnEnable() {

        if (CarController)
            CarController.Audio = this;
        else
            enabled = false;

    }

    /// <summary>
    /// On collision.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollision(Collision collision) {

        if (crashSound != null && crashSound.audioClips.Length >= 1) {

            int randomClip = Random.Range(0, crashSound.audioClips.Length);
            float volume = Mathf.InverseLerp(0f, 20000f, collision.impulse.magnitude);
            volume *= crashSound.maxVolume;

            crashSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, crashSound.audioClips[randomClip].name, crashSound.minDistance, crashSound.maxDistance, volume, crashSound.audioClips[randomClip], false, true, true);
            crashSound.audioSource.transform.localPosition = crashSound.localPosition;

        }

    }


    /// <summary>
    /// Engine sounds.
    /// </summary>
    private void Engine() {

        if (!CarController.Engine)
            return;

        if (engineStart != null && engineStart.audioClips) {

            if (!engineStart.audioSource) {

                engineStart.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, engineStart.audioClips.name, engineStart.minDistance, engineStart.maxDistance, engineStart.maxVolume, engineStart.audioClips, false, false, false);
                engineStart.audioSource.transform.localPosition = engineStart.localPosition;

            }

            if (engineStart.audioSource && !engineStart.audioSource.isPlaying && CarController.Engine.engineStarting)
                engineStart.audioSource.Play();

        }

        if (engineSounds != null && engineSounds.Length >= 1) {

            for (int i = 0; i < engineSounds.Length; i++) {

                if (engineSounds[i] != null) {

                    if (!engineSounds[i].audioSourceOn && engineSounds[i].audioClipOn) {

                        engineSounds[i].audioSourceOn = RCCP_AudioSource.NewAudioSource(gameObject, engineSounds[i].audioClipOn.name, engineSounds[i].minDistance, engineSounds[i].maxDistance, 0f, engineSounds[i].audioClipOn, true, false, false);
                        engineSounds[i].audioSourceOn.transform.localPosition = engineSounds[i].localPosition;

                    }

                    if (engineSounds[i].audioSourceOn && !engineSounds[i].audioSourceOn.isPlaying)
                        engineSounds[i].audioSourceOn.Play();

                    if (!engineSounds[i].audioSourceOff && engineSounds[i].audioClipOff) {

                        engineSounds[i].audioSourceOff = RCCP_AudioSource.NewAudioSource(gameObject, engineSounds[i].audioClipOff.name, engineSounds[i].minDistance, engineSounds[i].maxDistance, 0f, engineSounds[i].audioClipOff, true, false, false);
                        engineSounds[i].audioSourceOff.transform.localPosition = engineSounds[i].localPosition;

                    }

                    if (engineSounds[i].audioSourceOff && !engineSounds[i].audioSourceOff.isPlaying)
                        engineSounds[i].audioSourceOff.Play();

                    if (engineSounds[i].audioSourceOn && engineSounds[i].audioSourceOff) {

                        float minPitchPoint = Mathf.Lerp(engineSounds[i].minPitch, engineSounds[i].maxPitch, (CarController.engineRPM) / (CarController.maxEngineRPM));
                        float maxPitchPoint = Mathf.Lerp(minPitchPoint, engineSounds[i].minPitch, (CarController.engineRPM) / (CarController.maxEngineRPM));

                        engineSounds[i].audioSourceOn.pitch = minPitchPoint;

                        float minVolPoint = Mathf.Lerp(engineSounds[i].minVolume, engineSounds[i].maxVolume, (CarController.engineRPM - engineSounds[i].minRPM) / (engineSounds[i].maxRPM - engineSounds[i].minRPM));
                        float maxVolPoint = Mathf.Lerp(minVolPoint, 0f, (CarController.engineRPM - engineSounds[i].maxRPM) / (engineSounds[i].maxRPM - engineSounds[i].minRPM));

                        if (CarController.engineRPM < engineSounds[i].maxRPM)
                            engineSounds[i].audioSourceOn.volume = minVolPoint;
                        else
                            engineSounds[i].audioSourceOn.volume = maxVolPoint;

                        engineSounds[i].audioSourceOff.pitch = engineSounds[i].audioSourceOn.pitch;
                        engineSounds[i].audioSourceOff.volume = engineSounds[i].audioSourceOn.volume;

                        engineSounds[i].audioSourceOn.volume *= CarController.fuelInput_V;
                        engineSounds[i].audioSourceOff.volume *= (1f - CarController.fuelInput_V);

                    }

                }

            }

        }

        if (turboSound != null) {

            if (!turboSound.audioSource) {

                turboSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, turboSound.audioClips.name, turboSound.minDistance, turboSound.maxDistance, 0f, turboSound.audioClips, true, true, false);
                turboSound.audioSource.transform.localPosition = turboSound.localPosition;

            }

        }

        if (turboSound.audioSource) {

            turboSound.audioSource.volume = Mathf.Lerp(0f, turboSound.maxVolume, CarController.Engine.turboChargePsi / CarController.Engine.maxTurboChargePsi);

        }

        if (blowSound != null && blowSound.audioClips.Length >= 1f) {

            if (!blowSound.audioSource) {

                blowSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, blowSound.audioClips[0].name, blowSound.minDistance, blowSound.maxDistance, blowSound.maxVolume, blowSound.audioClips[0], false, false, false);
                blowSound.audioSource.transform.localPosition = blowSound.localPosition;

            }

        }

        if (blowSound.audioSource) {

            if (CarController.Engine.turboBlowOut && !blowSound.audioSource.isPlaying) {

                blowSound.audioSource.clip = blowSound.audioClips[Random.Range(0, blowSound.audioClips.Length)];
                blowSound.audioSource.Play();

            } else if (!blowSound.audioSource.isPlaying) {

                blowSound.audioSource.Stop();

            }

        }

    }

    /// <summary>
    /// Gearbox sounds.
    /// </summary>
    private void Gearbox() {

        if (!CarController.Gearbox)
            return;

        if (gearboxSound != null && gearboxSound.audioClips.Length >= 1) {

            if (lastGear != CarController.currentGear) {

                int randomClip = Random.Range(0, gearboxSound.audioClips.Length);

                gearboxSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, gearboxSound.audioClips[randomClip].name, gearboxSound.minDistance, gearboxSound.maxDistance, gearboxSound.maxVolume, gearboxSound.audioClips[randomClip], false, true, true);
                gearboxSound.audioSource.transform.localPosition = gearboxSound.localPosition;

            }

            lastGear = CarController.currentGear;

        }

        if (reverseSound != null && reverseSound.audioClips != null) {

            if (!reverseSound.audioSource) {

                reverseSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, reverseSound.audioClips.name, reverseSound.minDistance, reverseSound.maxDistance, reverseSound.maxVolume, reverseSound.audioClips, true, true, false);
                reverseSound.audioSource.transform.localPosition = reverseSound.localPosition;

            }

            reverseSound.audioSource.volume = Mathf.InverseLerp(0f, -40f, CarController.speed) * reverseSound.maxVolume;
            reverseSound.audioSource.pitch = Mathf.Lerp(reverseSound.minPitch, reverseSound.maxPitch, Mathf.InverseLerp(0f, -40f, CarController.speed));

        }

    }

    /// <summary>
    /// Wheel sounds.
    /// </summary>
    private void Wheel() {

        if (CarController.AllWheelColliders == null || CarController.AllWheelColliders.Length < 1)
            return;

        if (brakeSound != null && brakeSound.audioClips != null) {

            if (!brakeSound.audioSource) {

                brakeSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, brakeSound.audioClips.name, brakeSound.minDistance, brakeSound.maxDistance, 0f, brakeSound.audioClips, true, true, false);
                brakeSound.audioSource.transform.localPosition = brakeSound.localPosition;

            }

            if (CarController.FrontAxle != null && CarController.FrontAxle.leftWheelCollider && CarController.FrontAxle.rightWheelCollider)
                brakeSound.audioSource.volume = Mathf.Lerp(0f, brakeSound.maxVolume, Mathf.Clamp01((CarController.FrontAxle.leftWheelCollider.WheelCollider.brakeTorque + CarController.FrontAxle.rightWheelCollider.WheelCollider.brakeTorque) / (CarController.FrontAxle.maxBrakeTorque * 2f)) * Mathf.Lerp(0f, 1f, ((CarController.FrontAxle.leftWheelCollider.WheelCollider.rpm + CarController.FrontAxle.rightWheelCollider.WheelCollider.rpm) / 2f) / 50f));

        }

        if (wheelFlatSound != null && wheelFlatSound.audioClips != null) {

            bool deflated = false;

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

                if (CarController.AllWheelColliders[i].deflated)
                    deflated = true;

            }

            if (deflated) {

                if (wheelFlatSound.audioSource == null) {

                    wheelFlatSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, wheelFlatSound.audioClips.name, 1f, 15f, .5f, wheelFlatSound.audioClips, true, false, false);
                    wheelFlatSound.audioSource.transform.localPosition = wheelFlatSound.localPosition;

                }

                wheelFlatSound.audioSource.volume = Mathf.Clamp01(Mathf.Abs(CarController.tractionWheelRPM2EngineRPM * .001f));
                wheelFlatSound.audioSource.volume *= CarController.IsGrounded ? 1f : 0f;

                if (!wheelFlatSound.audioSource.isPlaying)
                    wheelFlatSound.audioSource.Play();

            } else {

                if (wheelFlatSound.audioSource && wheelFlatSound.audioSource.isPlaying)
                    wheelFlatSound.audioSource.Stop();

            }

        }

    }

    /// <summary>
    /// Exhaust Sounds.
    /// </summary>
    private void Exhaust() {

        if (!CarController.OtherAddonsManager)
            return;

        if (CarController.OtherAddonsManager && CarController.OtherAddonsManager.Nos) {

            if (nosSound != null && nosSound.audioClips != null) {

                if (!nosSound.audioSource) {

                    nosSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, nosSound.audioClips.name, nosSound.minDistance, nosSound.maxDistance, 0f, nosSound.audioClips, true, true, false);
                    nosSound.audioSource.transform.localPosition = nosSound.localPosition;

                }

                nosSound.audioSource.volume = (CarController.OtherAddonsManager.Nos.nosInUse ? 1f : 0f) * nosSound.maxVolume;

            }

        }

    }

    /// <summary>
    /// Other sounds.
    /// </summary>
    private void Others() {

        if (windSound != null && windSound.audioClips != null) {

            if (!windSound.audioSource) {

                windSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, windSound.audioClips.name, windSound.minDistance, windSound.maxDistance, windSound.maxVolume, windSound.audioClips, true, true, false);
                windSound.audioSource.transform.localPosition = windSound.localPosition;

            }

            windSound.audioSource.volume = Mathf.InverseLerp(0f, 200f, Mathf.Abs(Mathf.Abs(CarController.speed))) * windSound.maxVolume;

        }

    }

    private void Update() {

        Engine();
        Gearbox();
        Wheel();
        Exhaust();
        Others();

    }

    /// <summary>
    /// Deflate wheel sound.
    /// </summary>
    public void DeflateWheel() {

        if (wheelDeflateSound != null && wheelDeflateSound.audioClips != null) {

            wheelDeflateSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, wheelDeflateSound.audioClips.name, 5f, 50f, 1f, wheelDeflateSound.audioClips, false, true, true);
            wheelDeflateSound.audioSource.transform.localPosition = wheelDeflateSound.localPosition;

        }

    }

    /// <summary>
    /// Inflate wheel sound.
    /// </summary>
    public void InflateWheel() {

        if (wheelInflateSound != null && wheelInflateSound.audioClips != null) {

            wheelInflateSound.audioSource = RCCP_AudioSource.NewAudioSource(gameObject, wheelInflateSound.audioClips.name, 5f, 50f, 1f, wheelInflateSound.audioClips, false, true, true);
            wheelInflateSound.audioSource.transform.localPosition = wheelInflateSound.localPosition;

        }

    }

    public void DisableEngineSounds() {

        if (engineSounds == null)
            return;

        for (int i = 0; i < engineSounds.Length; i++) {

            if (engineSounds[i] != null) {

                if (engineSounds[i].audioSourceOn)
                    Destroy(engineSounds[i].audioSourceOn.gameObject);

                if (engineSounds[i].audioSourceOff)
                    Destroy(engineSounds[i].audioSourceOff.gameObject);

            }

        }

        engineSounds = null;


    }

    private void Reset() {

        engineSounds = new EngineSound[3];

        for (int i = 0; i < engineSounds.Length; i++) {

            engineSounds[i] = new EngineSound();

            engineSounds[i].minDistance = 10f;
            engineSounds[i].maxDistance = 200f;

            engineSounds[i].minPitch = .85f;
            engineSounds[i].maxPitch = 1.65f;

        }

        engineSounds[0].minRPM = 0f;
        engineSounds[0].maxRPM = 3000f;
        engineSounds[0].maxVolume = .75f;

        engineSounds[1].minRPM = 2000f;
        engineSounds[1].maxRPM = 6000f;
        engineSounds[1].maxVolume = .85f;

        engineSounds[2].minRPM = 6000f;
        engineSounds[2].maxRPM = 8000f;
        engineSounds[2].maxVolume = .9f;

        engineSounds[0].audioClipOn = RCCP_Settings.Instance.engineLowClipOn;
        engineSounds[0].audioClipOff = RCCP_Settings.Instance.engineLowClipOff;

        engineSounds[1].audioClipOn = RCCP_Settings.Instance.engineMedClipOn;
        engineSounds[1].audioClipOff = RCCP_Settings.Instance.engineMedClipOff;

        engineSounds[2].audioClipOn = RCCP_Settings.Instance.engineHighClipOn;
        engineSounds[2].audioClipOff = RCCP_Settings.Instance.engineHighClipOff;

        gearboxSound = new GearboxSound();
        gearboxSound.minDistance = 1f;
        gearboxSound.maxDistance = 10f;
        gearboxSound.maxVolume = 1f;

        crashSound = new CrashSound();
        crashSound.minDistance = 10f;
        crashSound.maxDistance = 100f;
        crashSound.maxVolume = 1f;

        engineStart = new EngineStart();

        if (RCCP_Settings.Instance.engineStartClip)
            engineStart.audioClips = RCCP_Settings.Instance.engineStartClip;

        gearboxSound = new GearboxSound();

        if (RCCP_Settings.Instance.gearClips != null)
            gearboxSound.audioClips = RCCP_Settings.Instance.gearClips;

        crashSound = new CrashSound();

        if (RCCP_Settings.Instance.crashClips != null)
            crashSound.audioClips = RCCP_Settings.Instance.crashClips;

        reverseSound = new ReverseSound();

        if (RCCP_Settings.Instance.reversingClip != null)
            reverseSound.audioClips = RCCP_Settings.Instance.reversingClip;

        windSound = new WindSound();

        if (RCCP_Settings.Instance.windClip != null)
            windSound.audioClips = RCCP_Settings.Instance.windClip;

        brakeSound = new BrakeSound();

        if (RCCP_Settings.Instance.brakeClip != null)
            brakeSound.audioClips = RCCP_Settings.Instance.brakeClip;

        nosSound = new NosSound();

        if (RCCP_Settings.Instance.NOSClip != null)
            nosSound.audioClips = RCCP_Settings.Instance.NOSClip;

        turboSound = new TurboSound();

        if (RCCP_Settings.Instance.turboClip != null)
            turboSound.audioClips = RCCP_Settings.Instance.turboClip;

        blowSound = new BlowSound();

        if (RCCP_Settings.Instance.blowoutClip != null)
            blowSound.audioClips = RCCP_Settings.Instance.blowoutClip;

        wheelDeflateSound = new DeflateSound();

        if (RCCP_Settings.Instance.wheelDeflateClip != null)
            wheelDeflateSound.audioClips = RCCP_Settings.Instance.wheelDeflateClip;

        wheelInflateSound = new InflateSound();

        if (RCCP_Settings.Instance.wheelInflateClip != null)
            wheelInflateSound.audioClips = RCCP_Settings.Instance.wheelInflateClip;

        wheelFlatSound = new FlatSound();

        if (RCCP_Settings.Instance.wheelFlatClip != null)
            wheelFlatSound.audioClips = RCCP_Settings.Instance.wheelFlatClip;

    }

}
