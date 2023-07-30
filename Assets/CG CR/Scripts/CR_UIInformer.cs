using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CR_UIInformer : MonoBehaviour{

    private static CR_UIInformer instance;
    public static CR_UIInformer Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<CR_UIInformer>();

            return instance;

        }

    }

    public Animator animator;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    public void Display(string title, string desc){

        titleText.text = title;
        descText.text = desc;
        animator.SetBool("open", true);
        
    }

    void Update(){


        
    }

}
