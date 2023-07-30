using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CR_ModManager : MonoBehaviour{

    private static CR_ModManager instance;
    public static CR_ModManager Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_ModManager>();

            return instance;

        }

    }

    public CR_PlayerManager player;

    public List<CR_UIModificationButton> modificationButtonList = new List<CR_UIModificationButton>();
    public GameObject modificationButtonPrefab;

    public GameObject modificationButtonContent_Hood;
    public GameObject modificationButtonContent_BumpersF;
    public GameObject modificationButtonContent_BumpersR;
    public GameObject modificationButtonContent_Spoilers;
    public GameObject modificationButtonContent_Sideskirts;
    public GameObject modificationButtonContent_Fenders;

    public Image engineUpgradeFill;
    public Image handlingUpgradeFill;
    public Image brakeUpgradeFill;
    public Image nosUpgradeFill;

    public bool newParts = false;
    public GameObject newPartsText;
    public GameObject newPartsText_Hood;
    public GameObject newPartsText_BumpersF;
    public GameObject newPartsText_BumpersR;
    public GameObject newPartsText_Spoilers;
    public GameObject newPartsText_Sideskirts;
    public GameObject newPartsText_Fenders;

    public void CheckVehicle(CR_PlayerManager newPlayer) {

        player = newPlayer;

        for (int i = 0; i < modificationButtonList.Count; i++)
            Destroy(modificationButtonList[i].gameObject);

        modificationButtonList.Clear();

        CR_VehicleModManager manager = player.ModManager;

        List<GameObject> foundBumpersF = new List<GameObject>();
        List<GameObject> foundBumpersR = new List<GameObject>();
        List<GameObject> foundFenders = new List<GameObject>();
        List<GameObject> foundHoods = new List<GameObject>();
        List<GameObject> foundSideskirts = new List<GameObject>();
        List<GameObject> foundSpoilers = new List<GameObject>();

        for (int i = 0; i < manager.parts.Length; i++) {

            switch (manager.parts[i].partType) {

                case CR_VehicleModManager.PartType.Bumper_F:
                    foundBumpersF.Add(manager.parts[i].part);
                    break;

                case CR_VehicleModManager.PartType.Bumper_R:
                    foundBumpersR.Add(manager.parts[i].part);
                    break;

                case CR_VehicleModManager.PartType.Fenders:
                    foundFenders.Add(manager.parts[i].part);
                    break;

                case CR_VehicleModManager.PartType.Hood:
                    foundHoods.Add(manager.parts[i].part);
                    break;

                case CR_VehicleModManager.PartType.Sideskirts:
                    foundSideskirts.Add(manager.parts[i].part);
                    break;

                case CR_VehicleModManager.PartType.Spoiler:
                    foundSpoilers.Add(manager.parts[i].part);
                    break;

            }

        }

        for (int i = 0; i < foundBumpersF.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_BumpersF.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Bumper_F, i, "Front Bumper " + (i+1).ToString());

            if (i == 0) {

                modificationButton.seenByPlayer = true;
                modificationButton.newPartText.gameObject.SetActive(false);

            }

        }

        for (int i = 0; i < foundBumpersR.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_BumpersR.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Bumper_R, i, "Rear Bumper " + (i + 1).ToString());

            if (i == 0) {

                modificationButton.seenByPlayer = true;
                modificationButton.newPartText.gameObject.SetActive(false);

            }

        }

        for (int i = 0; i < foundFenders.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_Fenders.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Fenders, i, "Fender " + (i + 1).ToString());

        }

        for (int i = 0; i < foundHoods.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_Hood.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Hood, i, "Hood " + (i + 1).ToString());

            if (i == 0) {

                modificationButton.seenByPlayer = true;
                modificationButton.newPartText.gameObject.SetActive(false);

            }

        }

        for (int i = 0; i < foundSideskirts.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_Sideskirts.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Sideskirts, i, "SideSkirt " + (i + 1).ToString());

        }

        for (int i = 0; i < foundSpoilers.Count; i++) {

            CR_UIModificationButton modificationButton = Instantiate(modificationButtonPrefab, modificationButtonContent_Spoilers.transform).GetComponent<CR_UIModificationButton>();
            modificationButtonList.Add(modificationButton);
            modificationButton.Initialize(player.ModManager, CR_VehicleModManager.PartType.Spoiler, i, "Spoiler " + (i + 1).ToString());

        }

    }

    public void ChangePart(CR_VehicleModManager.PartType partType, int index) {

        player.ModManager.EnablePart(partType, index);

    }

    public void UpgradeEngine() {

        if (!player)
            return;

        player.ModManager.EngineUpgrade();

    }

    public void UpgradeHandling() {

        if (!player)
            return;

        player.ModManager.HandlingUpgrade();

    }

    public void UpgradeBrake() {

        if (!player)
            return;

        player.ModManager.BrakeUpgrade();

    }

    public void UpgradeNOS() {

        if (!player)
            return;

        player.ModManager.NOSUpgrade();

    }

    private void Update() {

        if (!player)
            return;

        engineUpgradeFill.fillAmount = Mathf.Lerp(0f, 1f, player.ModManager.engineLevel / 5f);
        handlingUpgradeFill.fillAmount = Mathf.Lerp(0f, 1f, player.ModManager.handlingLevel / 5f);
        brakeUpgradeFill.fillAmount = Mathf.Lerp(0f, 1f, player.ModManager.brakeLevel / 5f);
        nosUpgradeFill.fillAmount = Mathf.Lerp(0f, 1f, player.ModManager.nosLevel / 1f);

        newParts = false;

        for (int i = 0; i < modificationButtonList.Count; i++) {

            if (modificationButtonList[i].newPartText.gameObject.activeSelf) {

                newParts = true;

            }

        }

        newPartsText.SetActive(newParts);

        bool newHood = false;
        bool newBumperF = false;
        bool newBumperR = false;
        bool newSpoilers = false;
        bool newSideskirts = false;
        bool newFenders = false;

        for (int i = 0; i < modificationButtonList.Count; i++) {

            switch (modificationButtonList[i].partType) {

                case CR_VehicleModManager.PartType.Hood:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newHood = true;

                    break;

                case CR_VehicleModManager.PartType.Bumper_F:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newBumperF = true;

                    break;

                case CR_VehicleModManager.PartType.Bumper_R:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newBumperR = true;

                    break;

                case CR_VehicleModManager.PartType.Spoiler:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newSpoilers = true;

                    break;

                case CR_VehicleModManager.PartType.Sideskirts:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newSideskirts = true;

                    break;

                case CR_VehicleModManager.PartType.Fenders:

                    if (modificationButtonList[i].unlockedPart && !modificationButtonList[i].seenByPlayer)
                        newFenders = true;

                    break;

            }

        }

        newPartsText_Hood.SetActive(newHood);
        newPartsText_BumpersF.SetActive(newBumperF);
        newPartsText_BumpersR.SetActive(newBumperR);
        newPartsText_Spoilers.SetActive(newSpoilers);
        newPartsText_Sideskirts.SetActive(newSideskirts);
        newPartsText_Fenders.SetActive(newFenders);

    }

}
