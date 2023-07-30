using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_PlayerVehicles : ScriptableObject{

    public static CR_PlayerVehicles Instance {

        get {

            return (CR_PlayerVehicles)Resources.Load("CR_PlayerVehicles", typeof(CR_PlayerVehicles));

        }

    }

    [System.Serializable]
    public class Car {

        public CR_PlayerManager vehicle;
        public int price = 0;

    }

    public Car[] cars;

}
