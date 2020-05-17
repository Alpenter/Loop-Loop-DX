using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlSwearWordScript : MonoBehaviour {

	float lifeTime = 0.75f;

	void Awake(){
		string[] texts = {
			"Shit!",
			"Ow!",
			"Fuck!",
			"Dangit!",
			"Heck!",
			"Damn!",
			"Bitch!",
			"Ouch!",
			"Owie!",
			"Piss!",
			"Cunt!",
			"Oof!",
			"Blargh!",
			"!!!",
			"Asshole!",
			"Poopy-Head!",
			"Stinky!",
			"Bummer!",
			"Zoowee-Mama!",
		};
		GetComponent<TextMesh>().text = texts[Random.Range(0, texts.Length)];
		GetComponent<TextMesh>().color = Game.bulletColor;
		iTween.ColorTo(gameObject, iTween.Hash(
			"a", 0f,
			"time", 0.25f,
			"delay", 0.5f,
			"easetype", iTween.EaseType.linear,
			"looptype", iTween.LoopType.none
		));
		Destroy(gameObject, lifeTime + 0.1f);
	}

	void FixedUpdate(){
		Vector3 pos = transform.position;
		float speed = 1.5f;
		pos.y += Time.fixedDeltaTime*speed;
		transform.position = pos;
	}
}
