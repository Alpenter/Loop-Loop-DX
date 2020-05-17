using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutsceneTextScript : MonoBehaviour {

    private void Update() {
        if (Game.usingController) {
            GetComponent<TextMesh>().text = "Hold 'Y' to skip.";
        } else {
            GetComponent<TextMesh>().text = "Hold 'Space' to skip.";
        }
    }
}
