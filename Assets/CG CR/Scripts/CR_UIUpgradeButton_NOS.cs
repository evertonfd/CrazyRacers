using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CR_UIUpgradeButton_NOS : MonoBehaviour {

    public int price = 10000;

    public void CheckAndUpgrade() {

        if (!CR_ModManager.Instance)
            return;

        if (!CR_ModManager.Instance.player)
            return;

        if (CR_ModManager.Instance.player.ModManager.nosLevel < 1) {

            if (CR_API.GetMoney() >= price) {

                CR_API.ChangeMoney(-price);
                CR_ModManager.Instance.UpgradeNOS();

            } else {

                CR_UIInformer.Instance.Display("Not Enough Money", "You don't have enough money to upgrade nos of your vehicle!");

            }

        }

    }

}
