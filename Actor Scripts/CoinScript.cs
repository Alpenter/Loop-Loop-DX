using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {
	
	bool got = false;

	AudioClip snd = null;
	GameObject gotEffect = null;

	void Awake(){
		gameObject.GetComponent<TrailRenderer>().material.color = Game.frontColor;
		snd = Resources.Load("SFX/coin") as AudioClip;
		gotEffect = Resources.Load("Coin Get") as GameObject;
        gameObject.GetComponent<MoveForward>().maxSpeed = Random.Range(-30, -40);
    }
	
	void Update () {
		gameObject.GetComponent<LookAtScript>().enabled = (!got);
		gameObject.GetComponent<MoveForward>().enabled = (!got);
		if(!got && Game.playerObj != null) {
			Vector3 playerPos = Game.playerObj.transform.position;
			float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.y), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
			if(distance <= 1f){
				Game.coins++;
                Game.currentCoinsCollectedInLevel++;
                Instantiate(gotEffect, transform.position, Quaternion.identity);
				PlaySound.NoLoopRandomPitch(snd, 0.5f, 1.5f);
				got = true;
			}
		} else {
			Destroy(gameObject, 1f);
		}
	}
}
