using UnityEngine;

public class LoopGunScript : MonoBehaviour {
	
	bool got = false;
	AudioClip cockLol = null;
	GameObject puppet, shine;
	
	void Awake(){ 
		cockLol = Resources.Load("SFX/gun cock") as AudioClip; 
		puppet = transform.Find("puppet").gameObject;
		shine = transform.Find("shine").gameObject;
	}
		
	void Update () {
        if(Game.playerObj != null) { 
		    float d = Vector2.Distance(new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y), new Vector2(transform.position.x, transform.position.y));
		    if(d < 1.7f && !got){
			    Destroy(gameObject.GetComponent<iTween>());
			    Destroy(shine.GetComponent<iTween>());
			    Destroy(puppet);
			    iTween.ColorTo(shine, iTween.Hash(
				    "a", 0f,
				    "time", 0.5f,
				    "looptype", iTween.LoopType.none,
				    "easetype", iTween.EaseType.easeInOutSine
			    ));
			    iTween.ScaleTo(shine, iTween.Hash(
				    "x", 7f, "y", 7f,
				    "time", 0.5f,
				    "looptype", iTween.LoopType.none,
				    "easetype", iTween.EaseType.easeInOutSine
			    ));
			    Destroy(gameObject, 3f);
			    PlaySound.NoLoop(cockLol);
			    Game.gotGun = true;
			    got = true;
            }
		}
	}
}
