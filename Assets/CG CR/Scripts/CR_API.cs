using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_API{

    public static int GetMoney() {

        return PlayerPrefs.GetInt("Money", 10000);

    }

    public static void ChangeMoney(int amount) {

        PlayerPrefs.SetInt("Money", GetMoney() + amount);

    }

}
