using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZoneSceneScript : MonoBehaviour {
	
	int part = 0;
	float textBoxTime = 7f, counter = 7f;
	bool looped = false;
	bool tweenedGun = false;
	public GameObject[] textBoxes;
	GameObject gun = null;
	
	void Start(){
		gun = transform.Find("Loop Gun").gameObject;
		for(int i = 0; i < textBoxes.Length; i++){
			textBoxes[i].SetActive(i == 0);
		}
	}
	
	void Update () {
		if(counter > 0){ counter -= Time.deltaTime; }
		if(counter <= 0){
			if(looped){
				part++;
				counter = textBoxTime;
			}
		}
		
		Vector3 pos = Game.playerObj.transform.position;
		if(pos.x < -36.5f){ looped = true; }
		if(pos.x > 36.5f){ looped = true; }
		if(pos.y > 21.5f){ looped = true; }
		if(pos.y < -21.5f){ looped = true; }
		
		if(part >= 2 && !tweenedGun && gun != null){
			iTween.MoveTo(gun, iTween.Hash(
				"x", 0, "y", 0, "z", 5,
				"looptype", iTween.LoopType.none,
				"easeType", iTween.EaseType.easeInOutSine,
				"time", 7.5f
			));
			tweenedGun = true;
		}
		
		for(int i = 0; i < textBoxes.Length; i++){
			if(i <= part){
				if(textBoxes[i] != null){ textBoxes[i].SetActive(true); }
			} else {
				if(textBoxes[i] != null){ textBoxes[i].SetActive(false); }
			}
		}
	}
}
