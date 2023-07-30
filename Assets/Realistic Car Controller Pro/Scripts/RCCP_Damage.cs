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
/// Deforms the meshes, wheels, lights, and other parts of the vehicle.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Damage")]
public class RCCP_Damage : MonoBehaviour {

    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public MeshFilter[] meshFilters;    //  Collected mesh filters.
    public RCCP_Light[] lights;     //  Collected lights.
    public RCCP_DetachablePart[] parts;     //  Collected parts.
    public RCCP_WheelCollider[] wheels;     //  Collected wheels.


    public bool automaticInstallation = true;       //  If set to enabled, all parts of the vehicle will be processed. If disabled, each part can be selected individually.
    public LayerMask damageFilter = -1;     // LayerMask filter. Damage will be taken from the objects with these layers.
    public float maximumDamage = .5f;       // Maximum Vert Distance For Limiting Damage. 0 Value Will Disable The Limit.

    // Mesh deformation
    [Space()]
    public bool meshDeformation = true;
    public DeformationMode deformationMode = DeformationMode.Fast;

    public enum DeformationMode { Accurate, Fast }

    public float deformationRadius = .75f;        // Verticies in this radius will be effected on collisions.
    public float deformationMultiplier = 1f;     // Damage multiplier.

    private readonly float minimumCollisionImpulse = .5f;       // Minimum collision force.
    private readonly float minimumVertDistanceForDamagedMesh = .002f;        // Comparing Original Vertex Positions Between Last Vertex Positions To Decide Mesh Is Repaired Or Not.

    public struct OriginalMeshVerts { public Vector3[] meshVerts; }     // Struct for Original Mesh Verticies positions.
    public struct OriginalWheelPos { public Vector3 wheelPosition; public Quaternion wheelRotation; }

    public OriginalMeshVerts[] originalMeshData;        // Array for struct above.
    public OriginalMeshVerts[] damagedMeshData;     // Array for struct above.
    public OriginalWheelPos[] originalWheelData;       // Array for struct above.
    public OriginalWheelPos[] damagedWheelData;        // Array for struct above.

    [Space()]
    public bool repairNow = false;      // Repairing now.
    public bool repaired = true;        // Returns true if vehicle is completely repaired.
    public bool deformingNow = false;      //  Deforming the mesh now.
    public bool deformed = true;        //  Returns true if vehicle is completely deformed.
    public float deformationTime = 0f;     //  Timer for deforming the vehicle. 

    [Space()]
    public bool recalculateNormals = false;      //  Recalculate normals while deforming / restoring the mesh.
    public bool recalculateBounds = false;       //  Recalculate bounds while deforming / restoring the mesh.

    // Wheel deformation
    [Space()]
    public bool wheelDamage = true;     //	Use wheel damage.
    public float wheelDamageRadius = .75f;        //   Wheel damage radius.
    public float wheelDamageMultiplier = 1f;        //  Wheel damage multiplier.
    public bool wheelDetachment = true;     //	Use wheel detachment.

    // Light deformation
    [Space()]
    public bool lightDamage = true;     //  Use light damage.
    public float lightDamageRadius = .75f;        //  Light damage radius.
    public float lightDamageMultiplier = 1f;        //  Light damage multiplier.

    // Part deformation
    [Space()]
    public bool partDamage = true;     //   Use part damage.
    public float partDamageRadius = .75f;        //   Part damage radius.
    public float partDamageMultiplier = 1f;        //   Part damage multiplier.

    [Space()]

    private ContactPoint contactPoint = new ContactPoint();

    /// <summary>
    /// Collecting all meshes and detachable parts of the vehicle.
    /// </summary>
    private void Start() {

        if (automaticInstallation) {

            MeshFilter[] allMeshFilters = CarController.gameObject.GetComponentsInChildren<MeshFilter>(true);
            List<MeshFilter> properMeshFilters = new List<MeshFilter>();

            // Model import must be readable. If it's not readable, inform the developer. We don't wanna deform wheel meshes. Exclude any meshes belongs to the wheels.
            foreach (MeshFilter mf in allMeshFilters) {

                if (mf.mesh != null) {

                    if (!mf.mesh.isReadable)
                        Debug.LogError("Not deformable mesh detected. Mesh of the " + mf.transform.name + " isReadable is false; Read/Write must be enabled in import settings for this model!");
                    else
                        properMeshFilters.Add(mf);

                }

            }

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

                if (CarController.AllWheelColliders[i].wheelModel != null) {

                    for (int k = 0; k < properMeshFilters.Count; k++) {

                        if (properMeshFilters[k] != null) {

                            if (properMeshFilters[k].transform.IsChildOf(CarController.AllWheelColliders[i].wheelModel))
                                properMeshFilters.RemoveAt(k);

                        }

                    }

                }

            }

            meshFilters = properMeshFilters.ToArray();

            parts = CarController.GetComponentsInChildren<RCCP_DetachablePart>();
            lights = CarController.GetComponentsInChildren<RCCP_Light>();
            wheels = CarController.GetComponentsInChildren<RCCP_WheelCollider>();

        }

        CheckMeshData();
        CheckWheelData();

    }

    private void OnEnable() {

        if (CarController)
            CarController.Damage = this;
        else
            enabled = false;

        repairNow = false;
        repaired = true;
        deformingNow = false;
        deformed = true;
        deformationTime = 0f;

    }

    public void GetMeshesEditor() {

        MeshFilter[] allMeshFilters = CarController.gameObject.GetComponentsInChildren<MeshFilter>(true);
        List<MeshFilter> properMeshFilters = new List<MeshFilter>();

        // Model import must be readable. If it's not readable, inform the developer. We don't wanna deform wheel meshes. Exclude any meshes belongs to the wheels.
        foreach (MeshFilter mf in allMeshFilters) {

            if (mf.sharedMesh != null) {

                if (!mf.sharedMesh.isReadable)
                    Debug.LogError("Not deformable mesh detected. Mesh of the " + mf.transform.name + " isReadable is false; Read/Write must be enabled in import settings for this model!");
                else
                    properMeshFilters.Add(mf);

            }

        }

        meshFilters = properMeshFilters.ToArray();

    }

    private void Update() {

        CheckRepair();
        CheckDamage();

    }

    /// <summary>
    /// We will be using two structs for deformed sections. Original part struction, and deformed part struction. 
    /// All damaged meshes and wheel transforms will be using these structs. At this section, we're creating them with original struction.
    /// </summary>
    private void CheckMeshData() {

        originalMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {

            meshFilters[i].mesh.MarkDynamic();
            originalMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

        }

        damagedMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
            damagedMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

    }

    /// <summary>
    /// We will be using two structs for deformed sections. Original part struction, and deformed part struction. 
    /// All damaged meshes and wheel transforms will be using these structs. At this section, we're creating them with original struction.
    /// </summary>
    private void CheckWheelData() {

        originalWheelData = new OriginalWheelPos[CarController.AllWheelColliders.Length];

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            originalWheelData[i].wheelPosition = CarController.AllWheelColliders[i].transform.localPosition;
            originalWheelData[i].wheelRotation = CarController.AllWheelColliders[i].transform.localRotation;

        }

        damagedWheelData = new OriginalWheelPos[CarController.AllWheelColliders.Length];

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            damagedWheelData[i].wheelPosition = CarController.AllWheelColliders[i].transform.localPosition;
            damagedWheelData[i].wheelRotation = CarController.AllWheelColliders[i].transform.localRotation;

        }

    }

    /// <summary>
    /// Moving deformed vertices to their original positions while repairing.
    /// </summary>
    public void CheckRepair() {

        if (!CarController)
            return;

        //  If vehicle is not repaired completely, and repairNow is enabled, restore all deformed meshes to their original structions.
        if (!repaired && repairNow) {

            int k;
            repaired = true;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the original positions. If verticies are close enough to the original
            //  position, repaired = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    for (int i = 0; i < vertices.Length; i++) {

                        //  And then move all single verticies to the original positions
                        if (deformationMode == DeformationMode.Accurate)
                            vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 5f);
                        else
                            vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]);

                        //  If verticies are close enough to their original positions, repaired = true;
                        if ((originalMeshData[k].meshVerts[i] - vertices[i]).magnitude >= minimumVertDistanceForDamagedMesh)
                            repaired = false;

                    }

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedMeshData[k].meshVerts = vertices;

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    //  You may want to disable last three lines.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    //  Get all verticies of the mesh first.
                    Vector3 wheelPos = CarController.AllWheelColliders[k].transform.localPosition;

                    //  And then move all single verticies to the original positions
                    if (deformationMode == DeformationMode.Accurate)
                        wheelPos += (originalWheelData[k].wheelPosition - wheelPos) * (Time.deltaTime * 5f);
                    else
                        wheelPos += (originalWheelData[k].wheelPosition - wheelPos);

                    //  If verticies are close enough to their original positions, repaired = true;
                    if ((originalWheelData[k].wheelPosition - wheelPos).magnitude >= minimumVertDistanceForDamagedMesh)
                        repaired = false;

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedWheelData[k].wheelPosition = wheelPos;

                    CarController.AllWheelColliders[k].transform.localPosition = wheelPos;
                    CarController.AllWheelColliders[k].transform.localRotation = Quaternion.identity;

                    if (!CarController.AllWheelColliders[k].WheelCollider.enabled)
                        CarController.AllWheelColliders[k].WheelCollider.enabled = true;

                    //carController.ESPBroken = false;

                    CarController.AllWheelColliders[k].Inflate();

                }

            }

            //  Repairing and restoring all detachable parts of the vehicle.
            for (int i = 0; i < parts.Length; i++) {

                if (parts[i] != null)
                    parts[i].OnRepair();

            }

            //  Repairing and restoring all lights of the vehicle.
            if (CarController.Lights) {

                for (int i = 0; i < CarController.Lights.lights.Count; i++) {

                    if (CarController.Lights.lights[i] != null)
                        CarController.Lights.lights[i].OnRepair();

                }

            }

            //  If all meshes are completely restored, make sure repairing now is false.
            if (repaired)
                repairNow = false;

        }

    }

    /// <summary>
    /// Moving vertices of the collided meshes to the damaged positions while deforming.
    /// </summary>
    public void CheckDamage() {

        if (!CarController)
            return;

        //  If vehicle is not deformed completely, and deforming is enabled, deform all meshes to their damaged structions.
        if (!deformed && deformingNow) {

            int k;
            deformed = true;
            deformationTime += Time.deltaTime;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the damaged positions. If verticies are close enough to the original
            //  position, deformed = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    //  And then move all single verticies to the damaged positions.
                    for (int i = 0; i < vertices.Length; i++) {

                        if (deformationMode == DeformationMode.Accurate)
                            vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 5f);
                        else
                            vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]);

                    }

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    Vector3 vertices = CarController.AllWheelColliders[k].transform.localPosition;

                    if (deformationMode == DeformationMode.Accurate)
                        vertices += (damagedWheelData[k].wheelPosition - vertices) * (Time.deltaTime * 5f);
                    else
                        vertices += (damagedWheelData[k].wheelPosition - vertices);

                    CarController.AllWheelColliders[k].transform.localPosition = vertices;

                }

            }

            //  Make sure deforming proccess takes only 1 second.
            if (deformationMode == DeformationMode.Accurate && deformationTime <= 1f)
                deformed = false;

            //  If all meshes are completely deformed, make sure deforming is false and timer is set to 0.
            if (deformed) {

                deformingNow = false;
                deformationTime = 0f;

            }

        }

    }

    /// <summary>
    /// Deforming meshes.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageMesh(float impulse) {

        if (!CarController)
            return;

        //  We will be checking all mesh filters with these contact points. If contact point is close enough to the mesh, deformation will be applied.
        for (int i = 0; i < meshFilters.Length; i++) {

            //  If mesh filter is not null, enabled, and has a valid mesh data...
            if (meshFilters[i] != null && meshFilters[i].mesh != null && meshFilters[i].gameObject.activeSelf) {

                //  Getting closest point to the mesh. Distance value will be set to closest point of the mesh - contact point.
                float distance = Vector3.Distance(NearestVertex(meshFilters[i].transform, meshFilters[i], contactPoint.point), contactPoint.point);

                //  If distance between contact point and closest point of the mesh is in range...
                if (distance <= deformationRadius) {

                    //  Collision direction.
                    Vector3 collisionDirection = contactPoint.point - CarController.transform.position;
                    collisionDirection = -collisionDirection.normalized;

                    //  All vertices of the mesh.
                    Vector3[] vertices = damagedMeshData[i].meshVerts;

                    for (int k = 0; k < vertices.Length; k++) {

                        //  Contact point is a world space unit. We need to transform to the local space unit with mesh origin. Verticies are local space units.
                        Vector3 point = meshFilters[i].transform.InverseTransformPoint(contactPoint.point);
                        //  Distance between vertex and contact point.
                        float distanceToVert = (point - vertices[k]).magnitude;

                        //  If distance between vertex and contact point is in range...
                        if (distanceToVert <= deformationRadius) {

                            //  Default impulse of the collision.
                            float damage = impulse;

                            // The damage should decrease with distance from the contact point.
                            damage -= damage * Mathf.Clamp01(distanceToVert / deformationRadius);

                            Quaternion rot = Quaternion.identity;

                            Vector3 vW = CarController.transform.TransformPoint(vertices[k]);

                            vW += rot * (collisionDirection * damage * (deformationMultiplier / 10f));

                            vertices[k] = CarController.transform.InverseTransformPoint(vW);

                            //  If distance between original vertex position and deformed vertex position exceeds limits, make sure they are in the limits.
                            if (maximumDamage > 0 && ((vertices[k] - originalMeshData[i].meshVerts[k]).magnitude) > maximumDamage)
                                vertices[k] = originalMeshData[i].meshVerts[k] + (vertices[k] - originalMeshData[i].meshVerts[k]).normalized * (maximumDamage);

                        }

                    }

                }

            }

        }



    }

    /// <summary>
    /// Deforming wheels. Actually changing their local positions and rotations based on the impact.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageWheel(float impulse) {

        if (!CarController)
            return;

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            if (CarController.AllWheelColliders[i] != null && CarController.AllWheelColliders[i].WheelCollider.enabled) {

                Vector3 wheelPos = damagedWheelData[i].wheelPosition;

                Vector3 collisionDirection = contactPoint.point - CarController.transform.position;
                collisionDirection = -collisionDirection.normalized;

                Vector3 closestPoint = CarController.AllWheelColliders[i].WheelCollider.ClosestPointOnBounds(contactPoint.point);
                float distance = Vector3.Distance(closestPoint, contactPoint.point);

                if (distance < wheelDamageRadius) {

                    float damage = impulse;

                    // The damage should decrease with distance from the contact point.
                    damage -= damage * Mathf.Clamp01(distance / wheelDamageRadius);

                    Vector3 vW = CarController.transform.TransformPoint(wheelPos);

                    vW += (collisionDirection * damage * (wheelDamageMultiplier / 50f));

                    wheelPos = CarController.transform.InverseTransformPoint(vW);

                    if (maximumDamage > 0 && ((wheelPos - originalWheelData[i].wheelPosition).magnitude) > (maximumDamage / 2f)) {

                        //wheelPos = originalWheelData[i].wheelPosition + (wheelPos - originalWheelData[i].wheelPosition).normalized * (maximumDamage);

                        if (wheelDetachment && CarController.AllWheelColliders[i].WheelCollider.enabled)
                            DetachWheel(CarController.AllWheelColliders[i]);

                    }

                    damagedWheelData[i].wheelPosition = wheelPos;

                }

            }

        }

    }

    /// <summary>
    /// Deforming the detachable parts.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamagePart(float impulse) {

        if (!CarController)
            return;

        if (parts != null && parts.Length >= 1) {

            for (int i = 0; i < parts.Length; i++) {

                if (parts[i] != null && parts[i].gameObject.activeSelf) {

                    if (parts[i].partCollider != null) {

                        Vector3 closestPoint = parts[i].partCollider.ClosestPointOnBounds(contactPoint.point);
                        float distance = Vector3.Distance(closestPoint, contactPoint.point);
                        float damage = impulse;

                        // The damage should decrease with distance from the contact point.
                        damage -= damage * Mathf.Clamp01(distance / deformationRadius);

                        if (distance <= deformationRadius)
                            parts[i].OnCollision(damage * partDamageMultiplier);

                    } else {

                        if ((contactPoint.point - parts[i].transform.position).magnitude < 1f)
                            parts[i].OnCollision(impulse * partDamageMultiplier);

                    }

                }

            }

        }

    }

    /// <summary>
    /// Deforming the lights.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageLight(float impulse) {

        if (!CarController)
            return;

        impulse *= lightDamageMultiplier;

        if (CarController.Lights) {

            for (int i = 0; i < CarController.Lights.lights.Count; i++) {

                if (CarController.Lights.lights[i] != null && CarController.Lights.lights[i].gameObject.activeSelf) {

                    if ((contactPoint.point - CarController.Lights.lights[i].transform.position).magnitude < lightDamageRadius)
                        CarController.Lights.lights[i].OnCollision(impulse);

                }

            }

        }

    }

    /// <summary>
    /// Detach wheel.
    /// </summary>
    /// <param name="wheelCollider"></param>
    public void DetachWheel(RCCP_WheelCollider wheelCollider) {

        if (!CarController)
            return;

        if (!wheelCollider)
            return;

        if (!wheelCollider.enabled)
            return;

        wheelCollider.WheelCollider.enabled = false;
        Transform wheelModel = wheelCollider.wheelModel;

        GameObject clonedWheel = Instantiate(wheelModel.gameObject, wheelModel.transform.position, wheelModel.transform.rotation, null);
        clonedWheel.SetActive(true);
        clonedWheel.AddComponent<Rigidbody>();

        GameObject clonedMeshCollider = new GameObject("Mesh Collider");
        clonedMeshCollider.transform.SetParent(clonedWheel.transform, false);
        clonedMeshCollider.transform.position = RCCP_GetBounds.GetBoundsCenter(clonedWheel.transform);
        MeshCollider mc = clonedMeshCollider.AddComponent<MeshCollider>();
        MeshFilter biggestMesh = RCCP_GetBounds.GetBiggestMesh(clonedWheel.transform);
        mc.sharedMesh = biggestMesh.mesh;
        mc.convex = true;

        //carController.ESPBroken = true;

    }

    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">Collision.</param>
    public void OnCollisionEnter(Collision collision) {

        if (!enabled)
            return;

        if (!CarController)
            return;

        if (((1 << collision.gameObject.layer) & damageFilter) != 0) {

            float impulse = collision.impulse.magnitude / 7500f;

            if (impulse < minimumCollisionImpulse)
                impulse = 0f;

            if (impulse > 10f)
                impulse = 10f;

            if (impulse > 0f) {

                deformingNow = true;
                deformed = false;

                repairNow = false;
                repaired = false;

                //  First, we are getting all contact points.
                contactPoint = collision.GetContact(0);

                if (meshFilters != null && meshFilters.Length >= 1 && meshDeformation)
                    DamageMesh(impulse);

                if (CarController.AllWheelColliders != null && CarController.AllWheelColliders.Length >= 1 && wheelDamage)
                    DamageWheel(impulse);

                if (parts != null && parts.Length >= 1 && partDamage)
                    DamagePart(impulse);

                if (CarController.Lights && lightDamage)
                    DamageLight(impulse);

            }

        }

    }

    /// <summary>
    /// Finds closest vertex to the target point.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="mf"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 NearestVertex(Transform trans, MeshFilter mf, Vector3 point) {

        // Convert point to local space.
        point = trans.InverseTransformPoint(point);

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;

        // Check all vertices to find nearest.
        foreach (Vector3 vertex in mf.mesh.vertices) {

            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr) {

                minDistanceSqr = distSqr;
                nearestVertex = vertex;

            }

        }

        // Convert nearest vertex back to the world space.
        return trans.TransformPoint(nearestVertex);

    }

}
