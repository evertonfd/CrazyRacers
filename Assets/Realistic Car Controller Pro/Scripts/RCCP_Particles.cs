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
/// Particle manager of the particles. All particles must be connected to this manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Particles")]
public class RCCP_Particles : MonoBehaviour {

    //  Main car controller.
    private RCCP_CarController _carController;
    private RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    //  Prefabs will be used.
    public GameObject contactSparklePrefab;
    public GameObject scratchSparklePrefab;
    public GameObject wheelSparklePrefab;

    public LayerMask collisionFilter = -1;     // LayerMask filter. Damage will be taken from the objects with these layers.

    private List<ParticleSystem> contactSparkeList = new List<ParticleSystem>();    // Array for Contact Particles.
    private List<ParticleSystem> scratchSparkeList = new List<ParticleSystem>();    // Array for Contact Particles.
    private List<ParticleSystem> wheelSparkleList = new List<ParticleSystem>();    // Array for Contact Particles.


    [System.Serializable]
    public class WheelParticles {

        public RCCP_WheelCollider wheelCollider;        //  Wheelcollider.
        public List<ParticleSystem> allWheelParticles = new List<ParticleSystem>();     //  And all particles belongs to the wheelcollider.

        /// <summary>
        /// Enabling target index only.
        /// </summary>
        /// <param name="index"></param>
        public void EnableParticleByIndex(int index) {

            for (int i = 0; i < allWheelParticles.Count; i++) {

                if (i != index) {

                    ParticleSystem.EmissionModule disabledEm = allWheelParticles[i].emission;
                    disabledEm.enabled = false;

                }

            }

            ParticleSystem.EmissionModule enabledEm = allWheelParticles[index].emission;
            enabledEm.enabled = true;

        }

        /// <summary>
        /// Disabling all particles.
        /// </summary>
        public void DisableParticles() {

            for (int i = 0; i < allWheelParticles.Count; i++) {

                ParticleSystem.EmissionModule em = allWheelParticles[i].emission;
                em.enabled = false;

            }

        }

    }

    public WheelParticles[] wheelParticles;     //  All wheel particles per wheel.

    private readonly int maximumContactSparkle = 5;     //	Contact Particles will be ready to use for collisions in pool. 

    private void Start() {

        //  If no car controller found, return.
        if (!CarController)
            return;

        // Particle System used for collision effects. Creating it at start. We will use this when we collide something.
        if (contactSparklePrefab && contactSparkeList.Count < 1) {

            for (int i = 0; i < maximumContactSparkle; i++) {

                GameObject sparks = Instantiate(contactSparklePrefab, transform.position, Quaternion.identity);
                sparks.transform.SetParent(transform, true);
                contactSparkeList.Add(sparks.GetComponent<ParticleSystem>());
                ParticleSystem.EmissionModule em = sparks.GetComponent<ParticleSystem>().emission;
                em.enabled = false;

            }

        }

        // Particle System used for collision effects. Creating it at start. We will use this when we collide something.
        if (scratchSparklePrefab && scratchSparkeList.Count < 1) {

            for (int i = 0; i < maximumContactSparkle; i++) {

                GameObject sparks = Instantiate(scratchSparklePrefab, transform.position, Quaternion.identity);
                sparks.transform.SetParent(transform, true);
                scratchSparkeList.Add(sparks.GetComponent<ParticleSystem>());
                ParticleSystem.EmissionModule em = sparks.GetComponent<ParticleSystem>().emission;
                em.enabled = false;

            }

        }

        // Particle System used for collision effects. Creating it at start. We will use this when we collide something.
        if (wheelSparklePrefab && wheelSparkleList.Count < 1) {

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

                GameObject sparks = Instantiate(wheelSparklePrefab, CarController.AllWheelColliders[i].transform.position, Quaternion.identity);
                sparks.transform.SetParent(CarController.AllWheelColliders[i].transform, true);
                wheelSparkleList.Add(sparks.GetComponent<ParticleSystem>());
                ParticleSystem.EmissionModule em = sparks.GetComponent<ParticleSystem>().emission;
                em.enabled = false;

            }

        }

        //  Creating a new array per wheel. Instantiating particles per ground material and assigning them.
        wheelParticles = new WheelParticles[CarController.AllWheelColliders.Length];

        for (int i = 0; i < wheelParticles.Length; i++) {

            wheelParticles[i] = new WheelParticles();

            for (int k = 0; k < RCCP_GroundMaterials.Instance.frictions.Length; k++) {

                GameObject ps = Instantiate(RCCP_GroundMaterials.Instance.frictions[k].groundParticles, transform.position, transform.rotation);
                ParticleSystem.EmissionModule em = ps.GetComponent<ParticleSystem>().emission;
                em.enabled = false;
                ps.transform.SetParent(CarController.AllWheelColliders[i].transform, false);
                ps.transform.localPosition = Vector3.zero;
                ps.transform.localRotation = Quaternion.identity;
                wheelParticles[i].allWheelParticles.Add(ps.GetComponent<ParticleSystem>());
                wheelParticles[i].wheelCollider = CarController.AllWheelColliders[i];

            }

        }

    }

    private void OnEnable() {

        if (CarController)
            CarController.Particles = this;
        else
            enabled = false;

    }

    private void Update() {

        //  If no car controller found, return.
        if (!CarController)
            return;

        //  Checking deflated wheels. If there are, enable the corresponding one. Otherwise disable.
        if (wheelSparkleList.Count >= 1) {

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

                if (CarController.AllWheelColliders[i].WheelCollider.enabled) {

                    if (CarController.AllWheelColliders[i].deflated && Mathf.Abs(CarController.AllWheelColliders[i].WheelCollider.rpm) >= 250f) {

                        ParticleSystem.EmissionModule em = wheelSparkleList[i].emission;

                        if (em.enabled == false)
                            em.enabled = true;

                    } else {

                        ParticleSystem.EmissionModule em = wheelSparkleList[i].emission;

                        if (em.enabled == true)
                            em.enabled = false;

                    }

                } else {

                    ParticleSystem.EmissionModule em = wheelSparkleList[i].emission;

                    if (em.enabled == true)
                        em.enabled = false;

                }

            }

        }

        // If wheel slip is bigger than ground physic material slip, enable particles. Otherwise, disable particles.
        for (int i = 0; i < wheelParticles.Length; i++) {

            if (wheelParticles[i].wheelCollider.WheelCollider.enabled && wheelParticles[i].wheelCollider.isSkidding)
                wheelParticles[i].EnableParticleByIndex(wheelParticles[i].wheelCollider.groundIndex);
            else
                wheelParticles[i].DisableParticles();

        }

    }

    /// <summary>
    /// On collisions.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision) {

        //  If component is not enabled, return.
        if (!enabled)
            return;

        //  If no car controller found, return.
        if (!CarController)
            return;

        //  If there are no any collision contact point, return.
        if (collision.contactCount < 1)
            return;

        //  If collision relative velocity is not high enough, return.
        if (collision.relativeVelocity.magnitude < 5)
            return;

        //  Enabling contact particles on collision and adjusting intensity with the impulse.
        for (int i = 0; i < contactSparkeList.Count; i++) {

            if (!contactSparkeList[i].isPlaying) {

                contactSparkeList[i].transform.position = collision.GetContact(0).point;
                ParticleSystem.EmissionModule em = contactSparkeList[i].emission;
                em.rateOverTimeMultiplier = collision.impulse.magnitude / 500f;
                em.enabled = true;
                contactSparkeList[i].Play();
                break;

            }

        }

    }

    /// <summary>
    /// On collision stay.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionStay(Collision collision) {

        //  If component is not enabled, return.
        if (!enabled)
            return;

        //  If no car controlelr found, return.
        if (!CarController)
            return;

        //  If there are no any collision, or relative velocity is not high enough, disable particles for scratch and return.
        if (collision.contactCount < 1 || collision.relativeVelocity.magnitude < 2f) {

            if (scratchSparkeList != null) {

                for (int i = 0; i < scratchSparkeList.Count; i++) {

                    ParticleSystem.EmissionModule em = scratchSparkeList[i].emission;
                    em.enabled = false;

                }

            }

            return;

        }

        //  Enabling scratches as long as collision stay continues. Otherwise, disable them.
        if (((1 << collision.gameObject.layer) & collisionFilter) != 0) {

            ContactPoint[] contacts = new ContactPoint[collision.contactCount];
            collision.GetContacts(contacts);

            int ind = -1;

            foreach (ContactPoint cp in contacts) {

                ind++;

                if (ind < scratchSparkeList.Count && !scratchSparkeList[ind].isPlaying) {

                    scratchSparkeList[ind].transform.position = cp.point;
                    ParticleSystem.EmissionModule em = scratchSparkeList[ind].emission;
                    em.enabled = true;
                    em.rateOverTimeMultiplier = collision.relativeVelocity.magnitude / 1f;
                    scratchSparkeList[ind].Play();

                }

            }

        }

    }

    /// <summary>
    /// On collision exits.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionExit(Collision collision) {

        //  If component is not enabled, return.
        if (!enabled)
            return;

        //  If no car controller found, return.
        if (!CarController)
            return;

        //  Make sure scratches are disabled on collision exits.
        for (int i = 0; i < scratchSparkeList.Count; i++) {

            ParticleSystem.EmissionModule em = scratchSparkeList[i].emission;
            em.enabled = true;
            scratchSparkeList[i].Stop();

        }

    }

    private void Reset() {

        contactSparklePrefab = RCCP_Settings.Instance.contactParticles;
        scratchSparklePrefab = RCCP_Settings.Instance.scratchParticles;
        wheelSparklePrefab = RCCP_Settings.Instance.wheelSparkleParticles;

    }

}
