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
/// Scene manager that contains current player vehicle, current player camera, current player UI, current player character, recording/playing mechanim, and other vehicles as well.
/// 
/// </summary>
public class RCCP_SceneManager : RCCP_Singleton<RCCP_SceneManager> {

    public RCCP_CarController activePlayerVehicle;     //  Current active player vehicle.
    public RCCP_Camera activePlayerCamera;       //  Current active player camera as RCCP Camera.
    public RCCP_UIManager activePlayerCanvas;       //  Current active UI canvas.
    public Camera activeMainCamera;     //  Current active main camera.
    private RCCP_CarController lastActivePlayerVehicle;        //  Last selected player vehicle.

    public bool registerLastVehicleAsPlayer = true;        //  Registers the lastly spawned vehicle as player vehicle.
    public bool disableUIWhenNoPlayerVehicle = false;       //  Disables the UI when there is no any player vehicle.

    public List<RCCP_CarController> allVehicles = new List<RCCP_CarController>();     //  All vehicles on the scene.

    public Terrain[] allTerrains;       //  All terrains on the scene.

    public class Terrains {

        //	Terrain data.
        public Terrain terrain;
        public TerrainData mTerrainData;
        public PhysicMaterial terrainCollider;
        public int alphamapWidth;
        public int alphamapHeight;

        public float[,,] mSplatmapData;
        public float mNumTextures;

    }

    public Terrains[] terrains;     //  All collected terrains with custom class.
    public bool terrainsInitialized = false;        //  All terrains are initialized yet?

    // Firing an event when main behavior changed.
    public delegate void onBehaviorChanged();
    public static event onBehaviorChanged OnBehaviorChanged;

    // Firing an event when player vehicle changed.
    public delegate void onVehicleChanged();
    public static event onVehicleChanged OnVehicleChanged;

    private void Awake() {

        //  Listening events.
        RCCP_Events.OnRCCPCameraSpawned += RCCP_Events_OnRCCPCameraSpawned;
        RCCP_Events.OnRCCPSpawned += RCC_CarControllerV3_OnRCCSpawned;
        RCCP_Events.OnRCCPDestroyed += RCC_CarControllerV3_OnRCCPlayerDestroyed;
        RCCP_Events.OnRCCPUISpawned += RCCP_Events_OnRCCPUISpawned;

        //#if BCG_ENTEREXIT
        //        BCG_EnterExitPlayer.OnBCGPlayerSpawned += BCG_EnterExitPlayer_OnBCGPlayerSpawned;
        //        BCG_EnterExitPlayer.OnBCGPlayerDestroyed += BCG_EnterExitPlayer_OnBCGPlayerDestroyed;
        //#endif

        //  Instantiate telemetry UI if it's enabled in RCCP Settings.
        if (RCCP_Settings.Instance.useTelemetry)
            Instantiate(RCCP_Settings.Instance.RCCPTelemetry, Vector3.zero, Quaternion.identity);

        // Overriding Fixed TimeStep.
        if (RCCP_Settings.Instance.overrideFixedTimeStep)
            Time.fixedDeltaTime = RCCP_Settings.Instance.fixedTimeStep;

        // Overriding FPS.
        if (RCCP_Settings.Instance.overrideFPS)
            Application.targetFrameRate = RCCP_Settings.Instance.maxFPS;

    }

    #region ONSPAWNED

    /// <summary>
    /// When RCCP vehicle is spawned.
    /// </summary>
    /// <param name="RCCP"></param>
    private void RCC_CarControllerV3_OnRCCSpawned(RCCP_CarController RCCP) {

        //  If all vehicles list doesn't contain spawned vehicle, add it to the list.
        if (!allVehicles.Contains(RCCP))
            allVehicles.Add(RCCP);

        //  Registers the last spawned vehicle as player vehicle.
        if (registerLastVehicleAsPlayer)
            RegisterPlayer(RCCP);

    }

    /// <summary>
    /// When RCCP Camera spawned.
    /// </summary>
    /// <param name="BCGCamera"></param>
    private void RCCP_Events_OnRCCPCameraSpawned(RCCP_Camera cam) {

        activePlayerCamera = cam;

    }

    /// <summary>
    /// When RCCP Canvas spawned.
    /// </summary>
    /// <param name="UI"></param>
    private void RCCP_Events_OnRCCPUISpawned(RCCP_UIManager UI) {

        activePlayerCanvas = UI;

    }

    #endregion

    #region ONDESTROYED

    /// <summary>
    /// When a vehicle destroyed.
    /// </summary>
    /// <param name="RCCP"></param>
    private void RCC_CarControllerV3_OnRCCPlayerDestroyed(RCCP_CarController RCCP) {

        if (allVehicles.Contains(RCCP))
            allVehicles.Remove(RCCP);

    }

    #endregion

    private void Start() {

        //  Getting all terrains.
        StartCoroutine(GetAllTerrains());

    }

    /// <summary>
    /// Getting all terrains.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetAllTerrains() {

        yield return new WaitForFixedUpdate();
        allTerrains = Terrain.activeTerrains;
        yield return new WaitForFixedUpdate();

        //  If terrains found...
        if (allTerrains != null && allTerrains.Length >= 1) {

            terrains = new Terrains[allTerrains.Length];

            for (int i = 0; i < allTerrains.Length; i++) {

                if (allTerrains[i].terrainData == null) {

                    Debug.LogError("Terrain data of the " + allTerrains[i].transform.name + " is missing! Check the terrain data...");
                    yield return null;

                }

            }

            //  Initializing terrains.
            for (int i = 0; i < terrains.Length; i++) {

                terrains[i] = new Terrains();
                terrains[i].terrain = allTerrains[i];
                terrains[i].mTerrainData = allTerrains[i].terrainData;
                terrains[i].terrainCollider = allTerrains[i].GetComponent<TerrainCollider>().sharedMaterial;
                terrains[i].alphamapWidth = allTerrains[i].terrainData.alphamapWidth;
                terrains[i].alphamapHeight = allTerrains[i].terrainData.alphamapHeight;

                terrains[i].mSplatmapData = allTerrains[i].terrainData.GetAlphamaps(0, 0, terrains[i].alphamapWidth, terrains[i].alphamapHeight);
                terrains[i].mNumTextures = terrains[i].mSplatmapData.Length / (terrains[i].alphamapWidth * terrains[i].alphamapHeight);

            }

            terrainsInitialized = true;

        }

    }

    private void Update() {

        //  When player vehicle changed...
        if (activePlayerVehicle) {

            if (activePlayerVehicle != lastActivePlayerVehicle) {

                if (OnVehicleChanged != null)
                    OnVehicleChanged();

            }

            lastActivePlayerVehicle = activePlayerVehicle;

        }

        //  Checking UI canvas.
        if (disableUIWhenNoPlayerVehicle && activePlayerCanvas)
            CheckCanvas();

        //  Getting main camera.
        if (Camera.main != null)
            activeMainCamera = Camera.main;

    }

    /// <summary>
    /// Registers the target vehicle as player vehicle.
    /// </summary>
    /// <param name="playerVehicle"></param>
    public void RegisterPlayer(RCCP_CarController playerVehicle) {

        activePlayerVehicle = playerVehicle;

        if (activePlayerCamera)
            activePlayerCamera.SetTarget(activePlayerVehicle);

    }

    /// <summary>
    /// Registers the target vehicle as player vehicle. Also sets controllable state of the vehicle.
    /// </summary>
    /// <param name="playerVehicle"></param>
    /// <param name="isControllable"></param>
    public void RegisterPlayer(RCCP_CarController playerVehicle, bool isControllable) {

        activePlayerVehicle = playerVehicle;
        activePlayerVehicle.SetCanControl(isControllable);

        if (activePlayerCamera)
            activePlayerCamera.SetTarget(activePlayerVehicle);

    }

    /// <summary>
    /// Registers the target vehicle as player vehicle. Also sets controllable state and engine state of the vehicle.
    /// </summary>
    /// <param name="playerVehicle"></param>
    /// <param name="isControllable"></param>
    /// <param name="engineState"></param>
    public void RegisterPlayer(RCCP_CarController playerVehicle, bool isControllable, bool engineState) {

        activePlayerVehicle = playerVehicle;
        activePlayerVehicle.SetCanControl(isControllable);
        activePlayerVehicle.SetEngine(engineState);

        if (activePlayerCamera)
            activePlayerCamera.SetTarget(activePlayerVehicle);

    }

    /// <summary>
    /// Deregisters the player vehicle.
    /// </summary>
    public void DeRegisterPlayer() {

        if (activePlayerVehicle)
            activePlayerVehicle.SetCanControl(false);

        activePlayerVehicle = null;

        if (activePlayerCamera)
            activePlayerCamera.RemoveTarget();

    }

    /// <summary>
    /// Checks UI canvas.
    /// </summary>
    public void CheckCanvas() {

        //if (!activePlayerVehicle || !activePlayerVehicle.canControl || !activePlayerVehicle.gameObject.activeInHierarchy || !activePlayerVehicle.enabled) {

        //    activePlayerCanvas.SetDisplayType(RCC_UIDashboardDisplay.DisplayType.Off);

        //    return;

        //}

        //if (activePlayerCanvas.displayType != RCC_UIDashboardDisplay.DisplayType.Customization)
        //    activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;

    }

    ///<summary>
    /// Sets new behavior.
    ///</summary>
    public static void SetBehavior(int behaviorIndex) {

        RCCP_Settings.Instance.overrideBehavior = true;
        RCCP_Settings.Instance.behaviorSelectedIndex = behaviorIndex;

        if (OnBehaviorChanged != null)
            OnBehaviorChanged();

    }

    /// <summary>
    /// Changes current camera mode.
    /// </summary>
    public void ChangeCamera() {

        if (activePlayerCamera)
            activePlayerCamera.ChangeCamera();

    }

    /// <summary>
    /// Transport player vehicle the specified position and rotation.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="rotation">Rotation.</param>
    public void Transport(Vector3 position, Quaternion rotation) {

        if (activePlayerVehicle) {

            activePlayerVehicle.Rigid.velocity = Vector3.zero;
            activePlayerVehicle.Rigid.angularVelocity = Vector3.zero;

            activePlayerVehicle.transform.SetPositionAndRotation(position, rotation);

            //activePlayerVehicle.throttleInput = 0f;
            //activePlayerVehicle.brakeInput = 1f;
            //activePlayerVehicle.engineRPM = activePlayerVehicle.minEngineRPM;
            //activePlayerVehicle.currentGear = 0;

            for (int i = 0; i < activePlayerVehicle.AllWheelColliders.Length; i++)
                activePlayerVehicle.AllWheelColliders[i].WheelCollider.motorTorque = 0f;

            //StartCoroutine(Freeze(activePlayerVehicle));

        }

    }

    /// <summary>
    /// Transport target vehicle the specified position and rotation.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Transport(RCCP_CarController vehicle, Vector3 position, Quaternion rotation) {

        if (vehicle) {

            vehicle.Rigid.velocity = Vector3.zero;
            vehicle.Rigid.angularVelocity = Vector3.zero;

            vehicle.transform.SetPositionAndRotation(position, rotation);

            for (int i = 0; i < vehicle.AllWheelColliders.Length; i++)
                vehicle.AllWheelColliders[i].WheelCollider.motorTorque = 0f;

        }

    }

    private void OnDestroy() {

        RCCP_Events.OnRCCPSpawned -= RCC_CarControllerV3_OnRCCSpawned;
        RCCP_Events.OnRCCPDestroyed -= RCC_CarControllerV3_OnRCCPlayerDestroyed;
        RCCP_Events.OnRCCPCameraSpawned -= RCCP_Events_OnRCCPCameraSpawned;
        RCCP_Events.OnRCCPUISpawned -= RCCP_Events_OnRCCPUISpawned;

        //#if BCG_ENTEREXIT
        //        BCG_EnterExitPlayer.OnBCGPlayerSpawned -= BCG_EnterExitPlayer_OnBCGPlayerSpawned;
        //        BCG_EnterExitPlayer.OnBCGPlayerDestroyed -= BCG_EnterExitPlayer_OnBCGPlayerDestroyed;
        //#endif

    }

}
