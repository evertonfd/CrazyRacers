using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_ModificationPartAnimation : MonoBehaviour{

    private Vector3 defPosition = new Vector3(-1f, -1f, -1f);

    public Vector3 movingFrom = new Vector3(0f, 10f, 0f);
    public bool moving = false;
    public bool on = true;

    private void OnEnable() {

        if(defPosition == new Vector3(-1f, -1f, -1f))
            defPosition = transform.localPosition;  

    }

    public void Animate_On() {

        moving = true;
        on = true;
        transform.localPosition = defPosition;
        transform.localPosition += movingFrom;

    }

    public void Animate_Off() {

        moving = true;
        on = false;
        transform.localPosition = defPosition;

    }

    private void Update() {

        if (moving) {

            if(on)
                transform.localPosition = Vector3.Lerp(transform.localPosition, defPosition, Time.deltaTime * 5f);
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, defPosition + movingFrom, Time.deltaTime * 5f);

        }

        //if (Vector3.Distance(transform.localPosition, defPosition) <= .05f)
        //    moving = false;
        //else
        //    moving = true;

    }

}
