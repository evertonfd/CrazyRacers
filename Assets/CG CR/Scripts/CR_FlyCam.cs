using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_FlyCam : MonoBehaviour{

    public Camera cam;
    public bool tracking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!RCCP_SceneManager.Instance.activePlayerVehicle)
            return;

        if(tracking)
            cam.transform.LookAt(RCCP_SceneManager.Instance.activePlayerVehicle.transform);

        cam.enabled = tracking;

    }

    private void OnTriggerEnter(Collider other) {

        CR_PlayerManager rCCP_CarController = other.GetComponentInParent<CR_PlayerManager>();

        if (!rCCP_CarController)
            return;

        if (rCCP_CarController.CarController.speed < 20f)
            return;

        StartCoroutine(ToggleView());

    }

    private IEnumerator ToggleView() {

        tracking = true;
        yield return new WaitForSeconds(1);
        tracking = false;

    }

}
