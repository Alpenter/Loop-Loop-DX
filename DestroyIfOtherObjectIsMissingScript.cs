using UnityEngine;

public class DestroyIfOtherObjectIsMissingScript : MonoBehaviour {
    public GameObject goToCheck = null;
    void Update() {
        if(goToCheck == null) {
            Destroy(gameObject);
        }
    }
}
