using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CR_UIModificationButton : MonoBehaviour{

    public string vehicleName = "";
    public CR_VehicleModManager.PartType partType;
    public int partIndex = -1;

    public bool dontShowUnlocked = false;
    public bool unlockedPart = false;
    public bool seenByPlayer = false;

    public TextMeshProUGUI text;
    public TextMeshProUGUI newPartText;

    public void Initialize(CR_VehicleModManager vehicle, CR_VehicleModManager.PartType newPartType, int newPartIndex, string newPartName) {

        gameObject.name = newPartName;
        vehicleName = vehicle.name;
        partType = newPartType;
        partIndex = newPartIndex;
        text.text = newPartName;

        List<CR_VehicleModManager.Part> correctParts = new List<CR_VehicleModManager.Part>();

        for (int i = 0; i < vehicle.parts.Length; i++) {

            if (vehicle.parts[i].partType == partType)
                correctParts.Add(vehicle.parts[i]);

        }

        unlockedPart = PlayerPrefs.HasKey(vehicleName + correctParts[partIndex].part.name);

        if (!unlockedPart)
            GetComponent<Button>().image.color = new Color(.5f, 0f, 0f, .5f);

        if (PlayerPrefs.GetInt("Seen_" + vehicleName + gameObject.name, 0) == 1)
            seenByPlayer = true;
        else
            seenByPlayer = false;

        if (unlockedPart && !seenByPlayer)
            newPartText.gameObject.SetActive(true);
        else
            newPartText.gameObject.SetActive(false);

    }

    private void Update() {

        //if (unlockedPart && !seenByPlayer)
        //    newPartText.gameObject.SetActive(true);
        //else
        //    newPartText.gameObject.SetActive(false);

    }

    public void AttachPart() {

        seenByPlayer = true;
        PlayerPrefs.SetInt("Seen_" + vehicleName + gameObject.name, 1);
        newPartText.gameObject.SetActive(false);
        CR_ModManager.Instance.player.ModManager.EnablePart(partType, partIndex);

    }

    public void DetachPart() {

        CR_ModManager.Instance.player.ModManager.DisablePart(partType);

    }

}
