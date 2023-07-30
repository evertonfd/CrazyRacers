using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CR_Prop : MonoBehaviour{

    public Rigidbody rigid;
    public int propScore = 100;
    public bool destroyed = false;
    public AudioClip[] clip;

    // Start is called before the first frame update
    void Start(){

        rigid = GetComponent<Rigidbody>();
        rigid.Sleep();
        
    }

    private void OnCollisionEnter(Collision collision) {

        StartCoroutine(SetDestroyed());
        Destroy(gameObject, 5);

        if (clip.Length > 1)
            RCCP_AudioSource.NewAudioSource(gameObject, "", 10f, 50f, .1f, clip[Random.Range(0, clip.Length)], false, true, true);

    }

    private IEnumerator SetDestroyed() {

        yield return new WaitForSeconds(.1f);
        destroyed = true;

    }

}
