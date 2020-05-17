
using UnityEngine;

public class EnableIfOwnsSupporterPackScript : MonoBehaviour {

    private void Start() {
        if (!Game.boughtSupporterPack) {
            gameObject.SetActive(false);
        }
    }
}
