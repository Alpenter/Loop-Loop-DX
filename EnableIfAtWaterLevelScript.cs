using UnityEngine;

public class EnableIfAtWaterLevelScript : MonoBehaviour {

    private void Update() {
        if (!Game.atWaterLevel) {
            Destroy(gameObject);
        } else {
        	if(gameObject.GetComponent<TextMesh>() != null){
        		TextMesh t = gameObject.GetComponent<TextMesh>();
        		if(Game.usingController){
    				t.text = "You unlocked the ability to pee" + '\n' + "by pressing and holding 'Y'!";
    			} else {
    				t.text = "You unlocked the ability to pee" + '\n' + "by pressing and holding 'P'!";
    			}
        	}
        }
    }
}
