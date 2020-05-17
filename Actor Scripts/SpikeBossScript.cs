using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBossScript : MonoBehaviour {
	
	public GameObject[] faces;
	GameObject body, spikes, deathParticles, boom, coin;
	public GameObject[] walls;

	int targetFace = 0;
	int health = 50;
    readonly int moneys = 50;

    float hurtCounter, deathCounter = 3f;
    readonly float hurtTime = 0.15f, speed = 3f, deathTime = 2f;
	
	bool dead = false;
	bool playedDeadTween = false;
	bool playedSecondTween = false;
    bool madeCoins = false;

	Vector2 dif;

    AudioClip boomSnd = null;

	void Start(){
		body = transform.Find("Body Holder").gameObject;
		spikes = body.transform.Find("Spikes").gameObject;
		deathParticles = transform.Find("explo particles").gameObject;
		boom = transform.Find("death explo").gameObject;
        coin = Resources.Load("Coin") as GameObject;
        boomSnd = Resources.Load("SFX/Spike Explosion") as AudioClip;
	}
	
	void Update () {
		
		//Set position and parents of the object and body.
		body.transform.parent = Game.enemyHolder.transform;
		body.transform.position = gameObject.transform.position;
		gameObject.transform.parent = Game.enemyHolder.transform;
		
		//Manage the health bar.
		float divider = 0.02f;
		Game.manager.GetComponent<GameManager>().BossHealthBar(health, divider);
		if(health < 0){ health = 0; }
		
		//Hold boss until not in Splash Screen.
		if(Game.manager.GetComponent<GameManager>().splashCounter <= 0 && !dead){
			
			//Move towards player slowly.
			if(Game.playerObj != null){ dif = new Vector2(transform.position.x, transform.position.y) - new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y); }
			Vector2 vel = -(dif.normalized)*speed;
			GetComponent<Rigidbody2D>().velocity = vel;
			
			//Scale up the less health he has.
			float scale =  ((50-health)*0.325f)+ 4f; 
			Vector3 targetScale = new Vector3(scale, scale, 1f);
			transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 5f*Time.deltaTime);
			body.transform.localScale = transform.localScale;
			
			//Make the hurt face when hit.
			if(hurtCounter > 0){
				hurtCounter -= Time.deltaTime;
				targetFace = 1;
			} else {
                if (Game.smileyMode) {
                    targetFace = 2;
                } else { 
				    targetFace = 0;
                }
			}
			
			//Apply what face to show. [1= normal; 2 = hurt;]
			for(int i=0; i < faces.Length; i++){
				faces[i].SetActive(i == targetFace);
			}
		} else if(dead){ //When dead.
			if(!playedDeadTween){
				iTween.MoveTo(gameObject, iTween.Hash(
					"x", 0f, "y", 0f, "z", 0f,
					"time", deathTime,
					"easetype", iTween.EaseType.easeInOutSine,
					"looptype", iTween.LoopType.none
				));
				iTween.ScaleTo(spikes, Vector3.zero, deathTime);
				deathParticles.GetComponent<ParticleSystem>().Play();
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				playedDeadTween = true;
			}
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(4f, 4f, 1f), 5f*Time.deltaTime);
			faces[0].SetActive(false);
			faces[1].SetActive(true);
            faces[2].SetActive(false);
			body.transform.localScale = transform.localScale;
            GetComponent<CircleCollider2D>().enabled = false;
			body.GetComponent<Collider2D>().enabled = false;
			if(deathCounter > 0){ deathCounter -= Time.deltaTime; }
			if(deathCounter <= 0 && !playedSecondTween){
				iTween.MoveTo(gameObject, iTween.Hash(
					"x", 0f, "y", -27f, "z", 0f,
					"time", 1f,
					"easetype", iTween.EaseType.linear,
					"looptype", iTween.LoopType.none
				));
				iTween.ScaleTo(boom, iTween.Hash(
					"x", 20f, "y", 20f, "z", 1f,
					"time", 1f,
					"delay", 1f,
					"easetype", iTween.EaseType.linear,
					"looptype", iTween.LoopType.none
				));
				iTween.ColorTo(boom, iTween.Hash(
					"a", 0f,
					"time", 0.75f,
					"delay", 1.25f,
					"easetype", iTween.EaseType.linear,
					"looptype", iTween.LoopType.none
				));
                Game.manager.GetComponent<GameManager>().victory = true;
                PlaySound.NoLoop(boomSnd);
				playedSecondTween = true;
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D playerBullet){
		if(playerBullet.gameObject.tag == "Bullet"){
            playerBullet.gameObject.GetComponent<BulletScript>().PoofMe();
            if (health >= 1){
				PlaySound.Damage();
				health--;
				hurtCounter = hurtTime;
			} else {
                if (!madeCoins) {
                    for (int i = 0; i < moneys; i++) {
                        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                        Instantiate(coin, transform.position, rot);
                    }

                    for (int j = 0; j < walls.Length; j++) {
                        iTween.ScaleTo(walls[j], iTween.Hash(
					        "y", 0f, "z", 0f,
					        "time", 1f,
					        "delay", 1f,
					        "easetype", iTween.EaseType.linear,
					        "looptype", iTween.LoopType.none
				        ));
                    }

                    madeCoins = true;
                }
				dead = true;
			}
		}

        if(playerBullet.gameObject.tag == "Player Laser") {
            health = 0;
            if (!madeCoins) {
                for (int i = 0; i < moneys; i++) {
                    Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    Instantiate(coin, transform.position, rot);
                }

                for (int j = 0; j < walls.Length; j++) {
                    iTween.ScaleTo(walls[j], iTween.Hash(
					    "y", 0f, "z", 0f,
					    "time", 1f,
					    "delay", 1f,
					    "easetype", iTween.EaseType.linear,
					    "looptype", iTween.LoopType.none
				    ));
                }
                madeCoins = true;
            }
			dead = true;
		}
    }
}
