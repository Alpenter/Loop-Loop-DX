using UnityEngine;

public class SetInvisibleOnStartScript : MonoBehaviour {
    private void Awake() {
        GetComponent<Renderer>().enabled = false;
    }
}
