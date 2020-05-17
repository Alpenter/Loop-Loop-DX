using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemyBulletScript : MonoBehaviour {
	
	GameObject puppet;
	Vector3 pos;
	LayerMask wall = 1024;
	bool triggered = false;
	float disTime = 0.5f;
	
	void Awake(){
		puppet = transform.Find("puppet").gameObject;
		puppet.GetComponent<MeshRenderer>().material.color = Game.bulletColor;
	}
	
	void Start(){
		iTween.ColorTo(puppet, iTween.Hash(
			"r", Game.frontColor.r,
			"g", Game.frontColor.g,
			"b", Game.frontColor.b,
			"time", 0.2f,
			"easetype", iTween.EaseType.easeInOutSine,
			"looptype", iTween.LoopType.pingPong
		));
	}
	
	void Update () {
		pos = transform.position;
		
		if(pos.x < -37){ pos.x = 36.5f;  Destroy(gameObject); }
		if(pos.x > 37){ pos.x = -36.5f; Destroy(gameObject); }
		if(pos.y > 22f){ pos.y = -21.5f; Destroy(gameObject); }
		if(pos.y < -22f){ pos.y = 21.5f; Destroy(gameObject); }
	}
	
	  void FixedUpdate() {
        Collider2D hit = Physics2D.OverlapPoint(transform.position, wall);
        if (hit && !triggered) {
            PoofMe();
        }
    }

	public void PoofMe(){
		GetComponent<MoveForward>().enabled = false;
		Destroy(puppet.GetComponent<iTween>());
		GetComponent<Collider2D>().enabled = false;
		float scale = 3f;
		iTween.ScaleTo(puppet, iTween.Hash(
			"x", scale, "y", scale, "z", 1f,
			"time", disTime/1.5f,
			"easetype", iTween.EaseType.easeOutSine,
			"looptype", iTween.LoopType.none
		));
		iTween.ColorTo(puppet, iTween.Hash(
			"r", Game.frontColor.r,
			"g", Game.frontColor.g,
			"b", Game.frontColor.b,
			"a", 0f,
			"time", disTime,
			"easetype", iTween.EaseType.easeOutSine,
			"looptype", iTween.LoopType.none
		));
		Destroy(gameObject, disTime + 0.1f);
		triggered = true;
	}
	
	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player"){
			Destroy(gameObject, 0.05f);
		}
	}
}
