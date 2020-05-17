using UnityEngine;

public class PeeScript : MonoBehaviour {
    private void Update() {
        if(Game.playerObj != null) {
            Vector3 pPos = Game.playerObj.transform.position;
            transform.position = new Vector3(pPos.x, pPos.y, pPos.z + 5);
        }
    }
}
