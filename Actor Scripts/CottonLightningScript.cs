using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonLightningScript : MonoBehaviour {
    
    float waitTime = 2f;
    GameObject puppet, particles;
    bool playedSnd = false;
    AudioClip snd = null;
    int flip;

    // Start is called before the first frame update
    void Awake() {
        puppet = transform.Find("puppet").gameObject;
        particles = transform.Find("particles").gameObject;
        snd = Resources.Load("SFX/Lightning Crash") as AudioClip;
        flip = Random.Range(0, 1);
        if(flip == 0) { puppet.transform.localScale = new Vector3(41, 3, 1); }
        else if(flip == 1) { puppet.transform.localScale = new Vector3(41, -3, 1); }
    }

    // Update is called once per frame
    void Update() {
        waitTime -= Time.deltaTime;
        if(waitTime <= 0) {
            gameObject.tag = "Murder";
            puppet.SetActive(true);
            if (!playedSnd) {
                particles.GetComponent<ParticleSystem>().Stop();
                PlaySound.NoLoopRandomPitch(snd, 0.5f, 1.5f);
                playedSnd = true;
            }
        } else {
            gameObject.tag = "Untagged";
            puppet.SetActive(false);
        }

        if(waitTime <= -0.25f) { Destroy(gameObject); }
    }
}
