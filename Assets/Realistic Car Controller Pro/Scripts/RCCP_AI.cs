//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI Controller of RCCP. It's not professional, but it does the job. Follows all waypoints, or follows/chases the target gameobject.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP AI")]
public class RCCP_AI : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public RCCP_AIWaypointsContainer waypointsContainer;                 // Waypoints Container.
    public int currentWaypointIndex = 0;                                            // Current index in Waypoint Container.
    public string targetTag = "Player";                                 // Search and chase gameobjects with tags.

    // AI Type
    public NavigationMode navigationMode;
    public enum NavigationMode { FollowWaypoints, ChaseTarget, FollowTarget, Off }

    // Raycast distances used for detecting obstacles at front of the AI vehicle.
    [Range(5f, 30f)] public float raycastLength = 10f;
    [Range(10f, 90f)] public float raycastAngle = 30f;
    public LayerMask obstacleLayers = -1;
    public GameObject obstacle;

    public bool useRaycasts = true;     //	Using forward and sideways raycasts to avoid obstacles.
    private float rayInput = 0f;                // Total ray input affected by raycast distances.
    private bool raycasting = false;        // Raycasts hits an obstacle now?
    private float resetTime = 0f;           // This timer was used for deciding go back or not, after crashing.
    public bool reversingNow = false;

    // Steer, Motor, And Brake inputs. Will feed RCC_CarController with these inputs.
    public float steerInput = 0f;
    public float throttleInput = 0f;
    public float brakeInput = 0f;
    public float handbrakeInput = 0f;

    // Limit speed.
    public bool limitSpeed = false;
    public float maximumSpeed = 100f;

    // Smoothed steering.
    public bool smoothedSteer = true;

    // Counts laps and how many waypoints were passed.
    public int lap = 0;
    public int stopLap = 10;
    public bool stopAfterLap = false;
    public int totalWaypointsPassed = 0;
    public int nextWaypointPassDistance = 20;
    public bool ignoreWaypointNow = false;

    // Detector radius.
    public int startChaseDistance = 300;
    public int startFollowDistance = 300;
    public int stopFollowDistance = 30;

    public RCCP_AIBrakeZone brakeZone;

    // Unity's Navigator.
    private NavMeshAgent navigator;

    //// Detector with Sphere Collider. Used for finding target Gameobjects in chasing mode.
    public Transform targetChase;       // Target Gameobject for chasing.

    private void OnEnable() {

        if (CarController)
            CarController.OtherAddonsManager.AI = this;
        else
            enabled = false;

        RCCP_Events.Event_OnRCCPAISpawned(CarController);

        //  Overriding inputs of the vehicle. AI will the vehicle with own inputs. Player inputs will be ignored.
        if (CarController.Inputs)
            CarController.Inputs.overrideInternalInputs = true;

    }

    private void Start() {

        // If Waypoints Container is not selected in Inspector Panel, find it on scene.
        if (!waypointsContainer)
            waypointsContainer = FindObjectOfType<RCCP_AIWaypointsContainer>(true);

        // Creating our Navigator and setting properties.
        GameObject navigatorObject = new GameObject("Navigator");
        navigatorObject.transform.SetParent(transform, false);
        navigator = navigatorObject.AddComponent<NavMeshAgent>();
        navigator.radius = 1;
        navigator.speed = 1;
        navigator.angularSpeed = 250f;
        navigator.acceleration = 20f;
        navigator.height = 1;
        navigator.avoidancePriority = 0;

    }

    private void Update() {

        // If not controllable, no need to go further.
        if (!CarController.canControl)
            return;

        // Assigning navigator's position to front wheels of the vehicle
        navigator.transform.localPosition = Vector3.zero;
        navigator.transform.localPosition += Vector3.forward * CarController.FrontAxle.leftWheelCollider.transform.localPosition.z;

        FeedRCC();      //  Feeding vehicle with own inputs.
        CheckReset();       //  Vehicle will try to go backwards if stucked.

    }

    private void FixedUpdate() {

        Navigation();       //  Navigation.

        //  If enabled, raycasts will be used to avoid obstacles at runtime.
        if (useRaycasts)
            FixedRaycasts();        // Recalculates steerInput if one of raycasts detects an object front of AI vehicle.

    }

    /// <summary>
    /// Navigation.
    /// </summary>
    private void Navigation() {

        // Navigator input for next target. It is multiplied by 1.5f for fast reactions.
        float navigatorInput = Mathf.Clamp(transform.InverseTransformDirection(navigator.desiredVelocity).x * 1.25f, -1f, 1f);

        //  Navigation has three modes.
        switch (navigationMode) {

            case NavigationMode.FollowWaypoints:

                // If our scene doesn't have a Waypoint Container, stop and return with error.
                if (!waypointsContainer) {

                    Debug.LogError("Waypoints Container Couldn't Found!");
                    Stop();
                    return;

                }

                // If our scene has Waypoints Container and it doesn't have any waypoints, stop and return with error.
                if (waypointsContainer && waypointsContainer.waypoints.Count < 1) {

                    Debug.LogError("Waypoints Container Doesn't Have Any Waypoints!");
                    Stop();
                    return;

                }

                //	If stop after lap is enabled, stop at target lap.
                if (stopAfterLap && lap >= stopLap) {

                    Stop();
                    return;

                }

                // Next waypoint and its position.
                RCCP_Waypoint currentWaypoint = waypointsContainer.waypoints[currentWaypointIndex];

                // Checks for the distance to next waypoint. If it is less than written value, then pass to next waypoint.
                float distanceToNextWaypoint = Vector3.Distance(transform.position, currentWaypoint.transform.position);

                // Setting destination of the Navigator.
                navigator.SetDestination(waypointsContainer.waypoints[currentWaypointIndex].transform.position);

                //  If distance to the next waypoint is not 0, and close enough to the vehicle, increase index of the current waypoint and total waypoint.
                if (distanceToNextWaypoint != 0 && distanceToNextWaypoint < nextWaypointPassDistance) {

                    currentWaypointIndex++;
                    totalWaypointsPassed++;

                    // If all waypoints were passed, sets the current waypoint to first waypoint and increase lap.
                    if (currentWaypointIndex >= waypointsContainer.waypoints.Count) {

                        currentWaypointIndex = 0;
                        lap++;

                    }

                }

                //  If vehicle goes forward, calculate throttle and brake inputs.
                if (!reversingNow) {

                    throttleInput = (distanceToNextWaypoint < (nextWaypointPassDistance * (Mathf.Abs(CarController.speed) / 30f))) ? (Mathf.Clamp01(currentWaypoint.targetSpeed - Mathf.Abs(CarController.speed))) : 1f;

                    if (limitSpeed)
                        throttleInput *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (CarController.speed) / maximumSpeed));

                    brakeInput = (distanceToNextWaypoint < (nextWaypointPassDistance * (Mathf.Abs(CarController.speed) / 30f))) ? (Mathf.Clamp01(Mathf.Abs(CarController.speed) - currentWaypoint.targetSpeed)) : 0f;
                    handbrakeInput = 0f;

                    //  If vehicle speed is high enough, calculate them related to navigator input. This will reduce throttle input, and increase brake input on sharp turns.
                    if (CarController.speed > 10f) {

                        throttleInput -= Mathf.Abs(navigatorInput) / 3f;
                        brakeInput += Mathf.Abs(navigatorInput) / 3f;

                    }

                }

                break;

            case NavigationMode.ChaseTarget:

                // If our scene doesn't have a target to chase, stop and return.
                if (!targetChase) {

                    Stop();
                    return;

                }

                // Setting destination of the Navigator. 
                if (navigator.isOnNavMesh)
                    navigator.SetDestination(targetChase.position);

                //  If vehicle goes forward, calculate throttle and brake inputs.
                if (!reversingNow) {

                    throttleInput = 1f;

                    if (limitSpeed)
                        throttleInput *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (Mathf.Abs(CarController.speed)) / maximumSpeed));

                    brakeInput = 0f;
                    handbrakeInput = 0f;

                    //  If vehicle speed is high enough, calculate them related to navigator input. This will reduce throttle input, and increase brake input on sharp turns.
                    if (CarController.speed > 30f) {

                        throttleInput -= Mathf.Abs(navigatorInput) / 3f;
                        brakeInput += Mathf.Abs(navigatorInput) / 3f;

                    }

                }

                break;

            case NavigationMode.FollowTarget:

                // If our scene doesn't have a Waypoints Container, return with error.
                if (!targetChase) {

                    Stop();
                    return;

                }

                // Setting destination of the Navigator. 
                if (navigator.isOnNavMesh)
                    navigator.SetDestination(targetChase.position);

                // Checks for the distance to target. 
                float distanceToTarget = GetPathLength(navigator.path);

                //  If vehicle goes forward, calculate throttle and brake inputs.
                if (!reversingNow) {

                    throttleInput = distanceToTarget < (stopFollowDistance * Mathf.Lerp(1f, 5f, Mathf.Abs(CarController.speed / 50f))) ? Mathf.Lerp(-5f, 1f, distanceToTarget / (stopFollowDistance / 1f)) : 1f;

                    if (limitSpeed)
                        throttleInput *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (Mathf.Abs(CarController.speed)) / maximumSpeed));

                    brakeInput = distanceToTarget < (stopFollowDistance * Mathf.Lerp(1f, 5f, Mathf.Abs(CarController.speed) / 50f)) ? Mathf.Lerp(5f, 0f, distanceToTarget / (stopFollowDistance / 1f)) : 0f;
                    handbrakeInput = 0f;

                    //  If vehicle speed is high enough, calculate them related to navigator input. This will reduce throttle input, and increase brake input on sharp turns.
                    if (Mathf.Abs(CarController.speed) > 30f) {

                        throttleInput -= Mathf.Abs(navigatorInput) / 3f;
                        brakeInput += Mathf.Abs(navigatorInput) / 3f;

                    }

                    if (throttleInput < .05f)
                        throttleInput = 0f;
                    if (brakeInput < .05f)
                        brakeInput = 0f;

                }

                break;

        }

        // Steer input.
        if (!smoothedSteer)
            steerInput = (ignoreWaypointNow ? rayInput : navigatorInput + rayInput);
        else
            steerInput = Mathf.MoveTowards(steerInput, (ignoreWaypointNow ? rayInput : navigatorInput + rayInput), Time.deltaTime * 3f);

        steerInput = Mathf.Clamp(steerInput, -1f, 1f) * CarController.direction;
        throttleInput -= brakeInput * 3f;

        //  Clamping inputs.
        throttleInput = Mathf.Clamp01(throttleInput);
        brakeInput = Mathf.Clamp01(brakeInput);
        handbrakeInput = Mathf.Clamp01(handbrakeInput);

        //  If vehicle goes backwards, set brake input to 1 for reversing.
        if (reversingNow) {

            throttleInput = 0f;
            brakeInput = 1f;
            handbrakeInput = 0f;

        } else {

            if (CarController.speed < 5f && brakeInput >= .5f) {

                brakeInput = 0f;
                handbrakeInput = 1f;

            }

        }

        if (brakeZone) {

            if (CarController.speed > brakeZone.targetSpeed) {

                throttleInput = 0f;
                brakeInput = 1f;

            }

        }

        if (brakeInput < .15f)
            brakeInput = 0f;

    }

    /// <summary>
    /// Feed the vehicle with own inputs.
    /// </summary>
    private void FeedRCC() {

        if (CarController.Inputs) {

            RCCP_Inputs overridedInputs = new RCCP_Inputs();

            overridedInputs.throttleInput = throttleInput;
            overridedInputs.steerInput = steerInput;
            overridedInputs.brakeInput = brakeInput;
            overridedInputs.handbrakeInput = handbrakeInput;

            CarController.Inputs.OverrideInputs(overridedInputs);

        }

    }

    /// <summary>
    /// Stops the vehicle immediately.
    /// </summary>
    private void Stop() {

        throttleInput = 0f;
        brakeInput = 0f;
        steerInput = 0f;
        handbrakeInput = 1f;

    }

    /// <summary>
    /// Vehicle will try to go backwards if crashed or stucked.
    /// </summary>
    private void CheckReset() {

        //  If navigation mode is set to follow, this means vehicle may stop. If vehicle is stopped near the target, no need to go backwards.
        if (navigationMode == NavigationMode.FollowTarget && GetPathLength(navigator.path) < stopFollowDistance) {

            reversingNow = false;
            resetTime = 0;
            return;

        }

        // If unable to move forward, puts the gear to R.
        if (Mathf.Abs(CarController.speed) <= 5)
            resetTime += Time.deltaTime;

        //  If car is stucked for 2 seconds, reverse now.
        if (resetTime >= 2)
            reversingNow = true;

        //  If car is stucked for 4 seconds, or speed exceeds 25, go forward.
        if (resetTime >= 4 || Mathf.Abs(CarController.speed) >= 25) {

            reversingNow = false;
            resetTime = 0;

        }

    }

    /// <summary>
    /// Using raycasts to avoid obstacles.
    /// </summary>
    private void FixedRaycasts() {

        //  Creating five raycasts with angles.
        int[] anglesOfRaycasts = new int[5];
        anglesOfRaycasts[0] = 0;
        anglesOfRaycasts[1] = Mathf.FloorToInt(raycastAngle / 3f);
        anglesOfRaycasts[2] = Mathf.FloorToInt(raycastAngle / 1f);
        anglesOfRaycasts[3] = -Mathf.FloorToInt(raycastAngle / 1f);
        anglesOfRaycasts[4] = -Mathf.FloorToInt(raycastAngle / 3f);

        // Ray pivot position.
        Vector3 pivotPos = transform.position;
        pivotPos += transform.forward * CarController.FrontAxle.leftWheelCollider.transform.localPosition.z;

        //  Ray hit.
        RaycastHit hit;
        rayInput = 0f;
        bool casted = false;

        //  Casting rays.
        for (int i = 0; i < anglesOfRaycasts.Length; i++) {

            //  Drawing normal gizmos.
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLength, Color.white);

            //  Casting the ray. If ray hits another obstacle...
            if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward, out hit, raycastLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform) {

                switch (navigationMode) {

                    case NavigationMode.FollowWaypoints:

                        //  Drawing hit gizmos.
                        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLength, Color.red);
                        casted = true;

                        //  Setting ray input related to distance to the obstacle.
                        if (i != 0)
                            rayInput -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLength));

                        break;

                    case NavigationMode.ChaseTarget:

                        if (targetChase && hit.transform != targetChase && !hit.transform.IsChildOf(targetChase)) {

                            //  Drawing hit gizmos.
                            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLength, Color.red);
                            casted = true;

                            //  Setting ray input related to distance to the obstacle.
                            if (i != 0)
                                rayInput -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLength));

                        }

                        break;

                    case NavigationMode.FollowTarget:

                        //  Drawing hit gizmos.
                        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLength, Color.red);
                        casted = true;

                        //  Setting ray input related to distance to the obstacle.
                        if (i != 0)
                            rayInput -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLength));

                        break;

                }

                //  If ray hits an obstacle, set obstacle. Otherwise set it to null.
                if (casted)
                    obstacle = hit.transform.gameObject;
                else
                    obstacle = null;

            }

        }

        //  Ray hits an obstacle or not?
        raycasting = casted;

        //  If so, clamp the ray input.
        rayInput = Mathf.Clamp(rayInput, -1f, 1f);

        //  If ray input is high enough, ignore the navigator input and directly use the ray input for steering.
        if (raycasting && Mathf.Abs(rayInput) > .5f)
            ignoreWaypointNow = true;
        else
            ignoreWaypointNow = false;

    }

    /// <summary>
    /// Calculates the length of the path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static float GetPathLength(NavMeshPath path) {

        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1)) {

            for (int i = 1; i < path.corners.Length; ++i)
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);

        }

        return lng;

    }

    /// <summary>
    /// When entered a brake zone...
    /// </summary>
    /// <param name="curBrakeZone"></param>
    public void EnteredBrakeZone(RCCP_AIBrakeZone curBrakeZone) {

        brakeZone = curBrakeZone;

    }

    /// <summary>
    /// When exited from a brake zone...
    /// </summary>
    public void ExitedBrakeZone() {

        brakeZone = null;

    }

    /// <summary>
    /// Gets the closest enemy.
    /// </summary>
    /// <param name="enemies"></param>
    /// <returns></returns>
    private Transform GetClosestEnemy(Transform[] enemies) {

        Transform bestTarget = null;

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform potentialTarget in enemies) {

            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr) {

                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;

            }

        }

        return bestTarget;

    }

    private void OnDisable() {

        RCCP_Events.Event_OnRCCPAIDestroyed(CarController);

        if (CarController.Inputs)
            CarController.Inputs.overrideInternalInputs = false;

    }

}