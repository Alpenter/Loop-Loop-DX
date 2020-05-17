using UnityEngine;

public class WhiteZoneBackgroundGlitterScript : MonoBehaviour {

    ParticleSystem ps = null;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (Game.brokeCore) {
            ParticleSystem.MainModule psm = ps.main;
            psm.startColor = Color.clear;            
        }
    }

}
