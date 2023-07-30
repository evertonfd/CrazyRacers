using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCCP_CameraSettings : MonoBehaviour {

    public float distance = 6f;
    public float height = 1.5f;

    // Start is called before the first frame update
    private IEnumerator Set() {

        yield return new WaitForFixedUpdate();

        if (RCCP_SceneManager.Instance.activePlayerCamera) {

            RCCP_SceneManager.Instance.activePlayerCamera.TPSDistance = distance;
            RCCP_SceneManager.Instance.activePlayerCamera.TPSHeight = height;

        }

    }

    // Update is called once per frame
    void OnEnable() {

        StartCoroutine(Set());

    }
}
