using UnityEngine;

public class TriangleSplitSphereColorScript : MonoBehaviour {

    private void Update() {
        GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 0.155f);
    }
}