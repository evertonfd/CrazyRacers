using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_Item : MonoBehaviour{

    public GameObject content;

    public CR_VehicleModManager.PartType partType;

    private void OnTriggerEnter(Collider other) {

        CR_VehicleModManager modManager = other.GetComponentInParent<CR_VehicleModManager>();

        if (!modManager)
            return;

        modManager.UnlockPart(partType);
        content.SetActive(false);

        Invoke(nameof(ReEnable), 100f);

    }

    private void ReEnable() {

        content.SetActive(true);

    }

}
