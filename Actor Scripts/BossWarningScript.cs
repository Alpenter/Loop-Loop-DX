using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWarningScript : MonoBehaviour {
	
	GameObject leftP, rightP, textObj;
	float speed = 1f;
	bool started = false;
	
	void Awake () {
		leftP = transform.Find("left puppet").gameObject;
		rightP = transform.Find("right puppet").gameObject;
		textObj = transform.Find("text").gameObject;
	}
	
	void Start(){
		//Setting start colors.
		leftP.GetComponent<MeshRenderer>().material.color = Game.frontColor;
		rightP.GetComponent<MeshRenderer>().material.color = Game.frontColor;
		textObj.GetComponent<TextMesh>().color = Game.frontColor;
	}
	
	void Update(){
		if(!started){
			//Tweening colors.
			iTween.ColorTo(leftP, iTween.Hash(
				"r", Game.frontColor.r,"g", Game.frontColor.g, "b", Game.frontColor.b, "a", 0.5f,
				"easetype", iTween.EaseType.easeInOutSine,
				"looptype", iTween.LoopType.pingPong,
				"time", speed
			));
			iTween.ColorTo(rightP, iTween.Hash(
				"r", Game.frontColor.r,"g", Game.frontColor.g, "b", Game.frontColor.b, "a", 0.5f,
				"easetype", iTween.EaseType.easeInOutSine,
				"looptype", iTween.LoopType.pingPong,
				"time", speed
			));
			iTween.ColorTo(textObj, iTween.Hash(
				"r", Game.frontColor.r,"g", Game.frontColor.g, "b", Game.frontColor.b, "a", 0.5f,
				"easetype", iTween.EaseType.easeInOutSine,
				"looptype", iTween.LoopType.pingPong,
				"time", speed
			));
			started = true;
		}
	}
}
