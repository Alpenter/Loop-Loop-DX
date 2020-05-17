using UnityEngine;

public class PlumBatchScript : MonoBehaviour {
    
    int livingPlums = 1;

    private void Awake() {
        Game.plumBatch = this.gameObject;
    }

    private void Update() {
        livingPlums = gameObject.transform.childCount;
        if (livingPlums <= 0) {
            Game.manager.GetComponent<GameManager>().victory = true;
        }
    }
}
