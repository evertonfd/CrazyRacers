//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Ground materials for variable ground physics.
/// </summary>
[System.Serializable]
public class RCCP_GroundMaterials : ScriptableObject {

    #region singleton
    private static RCCP_GroundMaterials instance;
    public static RCCP_GroundMaterials Instance { get { if (instance == null) instance = Resources.Load("RCCP_GroundMaterials") as RCCP_GroundMaterials; return instance; } }
    #endregion

    [System.Serializable]
    public class GroundMaterialFrictions {

        public PhysicMaterial groundMaterial;       //  Physic material.

        public float forwardStiffness = 1f;     //  Forward stiffness.
        public float sidewaysStiffness = 1f;        //  Sideways stiffness.
        public float slip = .25f;       //  Target slip limit.
        public float damp = 1f;     //  Damp force.

        [Range(0f, 1f)] public float volume = 1f;       //  Volume of the ground sound.

        public GameObject groundParticles;      //  Ground particles.
        public AudioClip groundSound;       //  Ground audio clip.
        public RCCP_Skidmarks skidmark;      //  Skidmarks.

    }

    public GroundMaterialFrictions[] frictions;     //  Ground materials.

    /// <summary>
    /// Terrain ground materials.
    /// </summary>
    [System.Serializable]
    public class TerrainFrictions {

        public PhysicMaterial groundMaterial;

        [System.Serializable]
        public class SplatmapIndexes {

            public int index = 0;

        }

        public SplatmapIndexes[] splatmapIndexes;

    }

    public TerrainFrictions[] terrainFrictions;     //  Terrain ground materials.

}


