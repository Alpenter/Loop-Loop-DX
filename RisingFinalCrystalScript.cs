using UnityEngine;

public class RisingFinalCrystalScript : MonoBehaviour {

    private void Awake() {
        Game.risingCrystalObj = this.gameObject;
    }
}
