using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CR_CinemachineTargetGroupManager : MonoBehaviour {

    public CinemachineTargetGroup targetGroup;
    public int currentTarget = 0;

    void Update() {

        for (int i = 0; i < targetGroup.m_Targets.Length; i++) {

            if (i != currentTarget)
                targetGroup.m_Targets[i].weight = 0f;

        }

        targetGroup.m_Targets[currentTarget].weight = Mathf.Lerp(targetGroup.m_Targets[currentTarget].weight, 1f, Time.deltaTime * 5f);

    }

    public void SetTarget(int target) {

        currentTarget = target;

    }

}
