using UnityEngine;

public class BulletHellWipeTutorialTextScript : MonoBehaviour {
    string t = "";
    private void Update() {
        if (Game.usingController) {
            t = "Overwhelmed? Press 'Y'.";
        } else {
            t = "Overwhelmed? Press 'SPACE'.";
        }
        gameObject.GetComponent<TextMesh>().text = t;
    }
}