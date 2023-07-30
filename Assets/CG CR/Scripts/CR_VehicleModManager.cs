using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CR_VehicleModManager : MonoBehaviour {

    private RCCP_CarController carController;
    public RCCP_CarController CarController {

        get {

            if (carController == null)
                carController = GetComponent<RCCP_CarController>();

            return carController;

        }

    }

    private PhotonView photonView;

    public enum PartType { Hood, Bumper_F, Bumper_R, Fenders, Sideskirts, Spoiler }

    [System.Serializable]
    public class Part {

        public PartType partType;
        public GameObject part;
        public bool available;

    }

    public Part[] parts;

    public int selectedHoodIndex = -1;
    public int selectedBumper_FIndex = -1;
    public int selectedBumper_RIndex = -1;
    public int selectedSpoilerIndex = -1;
    public int selectedSideskirtIndex = -1;
    public int selectedFenderIndex = -1;

    public List<Part> hoods = new List<Part>();
    public List<Part> bumpers_F = new List<Part>();
    public List<Part> bumpers_R = new List<Part>();
    public List<Part> fenders = new List<Part>();
    public List<Part> sideskirts = new List<Part>();
    public List<Part> spoilers = new List<Part>();

    public bool removableSpoiler = true;
    public bool removableSideskirt = true;
    public bool removableFender = true;

    public int engineLevel = 0;
    public float maxEngineTorque = 500f;
    public float defEngineTorque = 500f;

    public int handlingLevel = 0;
    public int brakeLevel = 0;
    public int nosLevel = 0;

    [System.Serializable]
    public class PartData {

        public int[] partArray;

    }

    private void Awake() {

        for (int i = 0; i < parts.Length; i++) {

            switch (parts[i].partType) {

                case PartType.Hood:

                    hoods.Add(parts[i]);
                    break;

                case PartType.Bumper_F:

                    bumpers_F.Add(parts[i]);
                    break;

                case PartType.Bumper_R:

                    bumpers_R.Add(parts[i]);
                    break;

                case PartType.Fenders:

                    fenders.Add(parts[i]);
                    break;

                case PartType.Sideskirts:

                    sideskirts.Add(parts[i]);
                    break;

                case PartType.Spoiler:

                    spoilers.Add(parts[i]);
                    break;

            }

            parts[i].part.SetActive(false);

        }

    }

    private void Start() {

        if (photonView && !photonView.IsMine)
            return;

        Initialize();
        CheckUpgrades();

    }

    public void Initialize() {

        if (photonView && !photonView.IsMine)
            return;

        for (int i = 0; i < parts.Length; i++) {

            if (parts[i].available)
                PlayerPrefs.SetInt(transform.name + parts[i].part.name, 1);

        }

        defEngineTorque = CarController.Engine.maximumTorqueAsNM;

        if (PlayerPrefs.GetInt(transform.name + "Upgrade_Engine", -1) != -1)
            engineLevel = PlayerPrefs.GetInt(transform.name + "Upgrade_Engine");

        if (PlayerPrefs.GetInt(transform.name + "Upgrade_Handling", -1) != -1)
            handlingLevel = PlayerPrefs.GetInt(transform.name + "Upgrade_Handling");

        if (PlayerPrefs.GetInt(transform.name + "Upgrade_Brake", -1) != -1)
            brakeLevel = PlayerPrefs.GetInt(transform.name + "Upgrade_Brake");

        nosLevel = PlayerPrefs.GetInt(transform.name + "Upgrade_NOS", 0);

        Load();

        if (selectedHoodIndex != -1)
            hoods[selectedHoodIndex].part.SetActive(true);

        if (selectedBumper_FIndex != -1)
            bumpers_F[selectedBumper_FIndex].part.SetActive(true);

        if (selectedBumper_RIndex != -1)
            bumpers_R[selectedBumper_RIndex].part.SetActive(true);

        if (selectedSpoilerIndex != -1)
            spoilers[selectedSpoilerIndex].part.SetActive(true);

        if (selectedSideskirtIndex != -1)
            sideskirts[selectedSideskirtIndex].part.SetActive(true);

        if (selectedFenderIndex != -1)
            fenders[selectedFenderIndex].part.SetActive(true);

    }

    public void EnablePart(PartType partType, int partIndex) {

        List<Part> correctParts = new List<Part>();

        for (int i = 0; i < parts.Length; i++) {

            if (parts[i].partType == partType)
                correctParts.Add(parts[i]);

        }

        if (!PlayerPrefs.HasKey(transform.name + correctParts[partIndex].part.name)) {

            CR_UIInformer.Instance.Display("Locked Part", "This part is locked. Search city to pick them up!");
            return;

        }

        switch (partType) {

            case PartType.Hood:
                if (partIndex != selectedHoodIndex)
                    selectedHoodIndex = partIndex;
                else
                    return;
                break;

            case PartType.Bumper_F:
                if (partIndex != selectedBumper_FIndex)
                    selectedBumper_FIndex = partIndex;
                else
                    return;
                break;

            case PartType.Bumper_R:
                if (partIndex != selectedBumper_RIndex)
                    selectedBumper_RIndex = partIndex;
                else
                    return;
                break;

            case PartType.Spoiler:
                if (partIndex != selectedSpoilerIndex)
                    selectedSpoilerIndex = partIndex;
                else
                    return;
                break;

            case PartType.Sideskirts:
                if (partIndex != selectedSideskirtIndex)
                    selectedSideskirtIndex = partIndex;
                else
                    return;
                break;

            case PartType.Fenders:
                if (partIndex != selectedFenderIndex)
                    selectedFenderIndex = partIndex;
                else
                    return;
                break;

        }

        for (int i = 0; i < correctParts.Count; i++) {

            if (correctParts[i].part.activeSelf)
                StartCoroutine(DisablePartWithDelay(correctParts[i].part));

        }

        correctParts[partIndex].part.SetActive(true);
        correctParts[partIndex].part.GetComponent<CR_ModificationPartAnimation>().Animate_On();

        Save();

    }

    public void DisablePart(PartType partType) {

        List<Part> correctParts = new List<Part>();

        for (int i = 0; i < parts.Length; i++) {

            if (parts[i].partType == partType)
                correctParts.Add(parts[i]);

        }

        switch (partType) {

            case PartType.Spoiler:
                selectedSpoilerIndex = -1;
                break;

            case PartType.Fenders:
                selectedFenderIndex = -1;
                break;

            case PartType.Sideskirts:
                selectedSideskirtIndex = -1;
                break;

        }

        for (int i = 0; i < correctParts.Count; i++) {

            if (correctParts[i].part.activeSelf)
                StartCoroutine(DisablePartWithDelay(correctParts[i].part));

        }

        Save();

    }

    private IEnumerator DisablePartWithDelay(GameObject part) {

        part.GetComponent<CR_ModificationPartAnimation>().Animate_Off();

        yield return new WaitForSeconds(.5f);

        part.SetActive(false);

    }

    private void Save() {

        PartData partData = new PartData();
        partData.partArray = new int[6];
        partData.partArray[0] = selectedHoodIndex;
        partData.partArray[1] = selectedBumper_FIndex;
        partData.partArray[2] = selectedBumper_RIndex;
        partData.partArray[3] = selectedSpoilerIndex;
        partData.partArray[4] = selectedSideskirtIndex;
        partData.partArray[5] = selectedFenderIndex;

        PlayerPrefs.SetString("PartData_" + transform.name, JsonUtility.ToJson(partData));
        print(PlayerPrefs.GetString("PartData_" + transform.name));

    }

    private void Load() {

        if (!PlayerPrefs.HasKey("PartData_" + transform.name))
            return;

        PartData partData = (PartData)JsonUtility.FromJson(PlayerPrefs.GetString("PartData_" + transform.name), typeof(PartData));
        selectedHoodIndex = partData.partArray[0];
        selectedBumper_FIndex = partData.partArray[1];
        selectedBumper_RIndex = partData.partArray[2];
        selectedSpoilerIndex = partData.partArray[3];
        selectedSideskirtIndex = partData.partArray[4];
        selectedFenderIndex = partData.partArray[5];

    }

    public void UnlockPart(PartType partType) {

        List<Part> correctParts = new List<Part>();

        for (int i = 0; i < parts.Length; i++) {

            if (parts[i].partType == partType)
                correctParts.Add(parts[i]);

        }

        for (int i = 0; i < correctParts.Count; i++) {

            if (!PlayerPrefs.HasKey(transform.name + correctParts[i].part.name)) {

                CR_UIInformer.Instance.Display("New Part Unlocked", "Search more parts to unlock!");
                PlayerPrefs.SetInt(transform.name + correctParts[i].part.name, 1);
                break;

            } else {

                CR_UIInformer.Instance.Display("Unlocked Already", "This part has been unlocked already!");

            }

        }

    }

    public void EngineUpgrade() {

        if (engineLevel < 5)
            engineLevel++;

        PlayerPrefs.SetInt(transform.name + "Upgrade_Engine", engineLevel);

        CheckUpgrades();

    }

    public void HandlingUpgrade() {

        if (handlingLevel < 5)
            handlingLevel++;

        PlayerPrefs.SetInt(transform.name + "Upgrade_Handling", handlingLevel);

        CheckUpgrades();

    }

    public void BrakeUpgrade() {

        if (brakeLevel < 5)
            brakeLevel++;

        PlayerPrefs.SetInt(transform.name + "Upgrade_Brake", brakeLevel);

        CheckUpgrades();

    }

    public void NOSUpgrade() {

        if (nosLevel < 1)
            nosLevel++;

        PlayerPrefs.SetInt(transform.name + "Upgrade_NOS", nosLevel);

        CheckUpgrades();

    }

    public void CheckUpgrades() {

        CarController.Engine.maximumTorqueAsNM = Mathf.Lerp(defEngineTorque, maxEngineTorque, engineLevel / 5f);
        CarController.OtherAddonsManager.Nos.enabled = nosLevel == 1 ? true : false;

    }

}
