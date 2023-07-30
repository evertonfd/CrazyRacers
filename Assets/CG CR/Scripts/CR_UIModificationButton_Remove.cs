using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_UIModificationButton_Remove : MonoBehaviour{

    public CR_VehicleModManager.PartType partType;

    private void OnEnable() {

        transform.SetAsLastSibling();

    }

    public void DetachPart() {

        CR_ModManager.Instance.player.ModManager.DisablePart(partType);

    }

}
