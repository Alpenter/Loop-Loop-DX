using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCutsceneStartControllerScript : MonoBehaviour {

    float waitCounter = 16f;
    FinalCutsceneScript fcs = null;
    AudioSource whiteNoise = null;

    private void Awake() {
        fcs = gameObject.GetComponent<FinalCutsceneScript>();
        fcs.enabled = false;
        whiteNoise = gameObject.GetComponent<AudioSource>();
        whiteNoise.volume = 0;
    }

    void Update() {
        //Increase white noise volume.
        if(whiteNoise.volume < Game.soundVolume) { whiteNoise.volume += Time.deltaTime/25f; }
        if(whiteNoise.volume > Game.soundVolume) { whiteNoise.volume = Game.soundVolume; }

        //Wait to start the text sequence.
        if(waitCounter > 0) {
            fcs.enabled = false;
            waitCounter -= Time.deltaTime;
        } else {
            fcs.enabled = true;
        }
    }
}
