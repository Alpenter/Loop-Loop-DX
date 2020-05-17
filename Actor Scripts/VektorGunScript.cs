using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VektorGunScript : MonoBehaviour {

	List<GameObject> puppets = new List<GameObject>();
	GameObject explosionParticles = null;
	GameObject reticle = null;
	GameObject laserObj = null;

	float hitCounter = 0f;
	readonly float deathTime = 1f;
	readonly float hitTime = 0.1f;

	int health = 20;
	
	bool dead = false;
	bool tweenedMe = false;

	Color targetColor = Color.black;
	Color currentColor = Color.black;

	Vector3 targetPosition;
	public Vector3 offScreenPosition;
	public Vector3 onScreenPosition;

    AudioClip exploSnd = null;

	[HideInInspector]
	public VektorBossScript vbs = null;
    
	void Awake(){
		//Load in explosion particles.
		explosionParticles = Resources.Load("Vektor Gun Explosion Particles") as GameObject;

        //Load explo sound.
        exploSnd = Resources.Load("SFX/Gun Break") as AudioClip;

		//Fill up them puppets bby.
		GameObject puppetHolder = transform.Find("Puppet Holder").gameObject;
		foreach(Transform child in puppetHolder.transform){
			puppets.Add(child.gameObject);
		}

		//Find the reticle and laser.
		reticle = transform.Find("Reticle").gameObject;
		laserObj = transform.Find("Laser").gameObject;
		laserObj.SetActive(false);
		laserObj.transform.localScale = new Vector3(1, 0, 1);

		//Start off the proper target position.
		targetPosition = offScreenPosition;
	}

	void Update(){

		//Move to where I need to be.
		Vector3 pos = transform.position;
		if(vbs.ph == VektorBossScript.Phase.GunPhase){
			targetPosition = onScreenPosition;
		} else { 
 			targetPosition = offScreenPosition;
		}
		float smoothy = 6f;
		pos = Vector3.Lerp(pos, targetPosition, smoothy*Time.deltaTime);
		transform.position = pos;

		//Manage the reticle scale.
		if(dead || vbs.ph != VektorBossScript.Phase.GunPhase){
			reticle.transform.localScale = new Vector3(250f, 0f, 1f);
		} else {
			reticle.transform.localScale = new Vector3(250f, 0.5f, 1f);
		}

		//Color the reticle.
		float alph = 0.5f;
		reticle.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, alph);

		//Manage the laser.
		if(vbs.timeTillDeath <= 0){
			laserObj.SetActive(true);
			if(!tweenedMe){
				iTween.ScaleTo(laserObj, iTween.Hash(
					"x", 1f, "y", 8f, "z", 1f,
					"time", 0.75f,
					"easetype", iTween.EaseType.easeInOutSine,
					"looptype", iTween.LoopType.none
				));
				tweenedMe = true;
			}
		}

		//Flash me when I'm hit.
		if(hitCounter > 0f){
			hitCounter -= Time.deltaTime;
			targetColor = Game.bulletColor;
		} else {
			targetColor = Game.frontColor;
		}
		float smooth = 5f;
		currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime*smooth);
		for(int i = 0; i < puppets.Count; i++){
			puppets[i].GetComponent<MeshRenderer>().material.color = currentColor;
		}

	}


	void OnTriggerEnter2D(Collider2D bullet){
		if(bullet.gameObject.tag == "Bullet" && !dead){
			if(health <= 0){
                PlaySound.NoLoop(exploSnd);
				Instantiate(explosionParticles, gameObject.transform.position, gameObject.transform.rotation);
				for(int i = 0; i < puppets.Count; i++){
					iTween.ScaleTo(puppets[i], iTween.Hash(
						"x", 0, "y", 0, "z", 0,
						"time", deathTime - 0.1f,
						"looptype", iTween.LoopType.none,
						"easetype", iTween.EaseType.easeInOutSine
					));
				}
				iTween.ScaleTo(reticle, iTween.Hash(
					"y", 0f,
					"time", deathTime - 0.1f,
					"looptype", iTween.LoopType.none,
					"easetype", iTween.EaseType.easeInOutSine
				));
				GetComponent<Collider2D>().enabled = false;
				Destroy(gameObject, deathTime);
				dead = true;
			} else {
				PlaySound.Damage();
				health--;
				hitCounter = hitTime;
			}
		}
	}


}
