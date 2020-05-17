using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellShipScript : MonoBehaviour {
	
	GameObject puppet, deathExplo, wipeIndicator, num, bulletWipePoof;
	public GameObject wipeEffect, wipeParticles;
	
	GameManager gm = null;

	float x, y;
	float timeSinceWiped = 0f;
	
	Vector2 vel;
	Vector3 pos;
	
	Color shipColor;
	
	bool dead = false;
	bool canWipe = false;

	AudioClip wipe = null;

	void Awake () {
		shipColor = Color.black;
		puppet = transform.Find("puppet").gameObject;
		wipeIndicator = transform.Find("wipe indicator").gameObject;
		num = transform.Find("num").gameObject;
		bulletWipePoof = Resources.Load("Bullet Hell Bullets/Bullet Wipe Poof") as GameObject;

		//Load death explosion.
		deathExplo = Resources.Load("Break Effects/New Bullet Hell Player Break Effect") as GameObject;
		
		//Load wipe sound.
		wipe = Resources.Load("SFX/Bullet Wipe Sound") as AudioClip;

		//Start off being able to wipe the screen.
		canWipe = true;
	}

	void Start() {
		gm = Game.manager.GetComponent<GameManager>();
	}
	
	void Update () {
		//Set pos and vel.
		pos = new Vector3(transform.position.x, transform.position.y, 0);
		vel = GetComponent<Rigidbody2D>().velocity;
		
		//Move if you aren't dead.
		if(!dead){
            //Tracking x and y input for keyboard.
            if (Game.usingController) {
                x = Input.GetAxis("Horizontal [Gamepad]");
                y = -Input.GetAxis("Vertical [Gamepad]");
            } else { 
			    x = Input.GetAxis("Horizontal");
			    y = Input.GetAxis("Vertical");
			}

			//Move around.
			vel = Vector3.Lerp(vel, new Vector2(x * Game.speed, y * Game.speed), Time.deltaTime * 10.5f); 
			
			//Show puppet when alive.
			puppet.SetActive(true);
			
		} else { //If you are dead, lock velocity, hide puppet and disable collider..
			vel = Vector3.zero;
			puppet.SetActive(false);
			GetComponent<Collider2D>().enabled = false;
		}
		
		//Wait for 60 seconds to be able to wipe again.
		if(timeSinceWiped < 60f && !canWipe){
			timeSinceWiped += Time.deltaTime;
		} else {
			canWipe = true;
		}
		num.SetActive(!canWipe && !dead);
		int timeLeft = 60 - Mathf.RoundToInt(timeSinceWiped);
		num.GetComponent<TextMesh>().text = timeLeft.ToString();

		//When you can wipe....
		if(canWipe){
			timeSinceWiped = 0f; //Reset counter.
			if(Input.GetButtonDown("Wipe") && !dead && !gm.paused){
				//Debug.Log("WIPE ME LIKE A DIRTY BABY SHAT HIS DIPER");
				PlaySound.NoLoop(wipe);
				WipeScreen();
			}
		}
		
		//When to show wipe indicator.
		wipeIndicator.SetActive(canWipe && !dead);
		
		//Loop around the screen.
		if(pos.x < -36.5f){ pos.x = 36f; }
		if(pos.x > 36.5f){ pos.x = -36f; }
		if(pos.y > 21.5f){ pos.y = -21f; }
		if(pos.y < -21.5f){ pos.y = 21f; }
			
		//Match set color.
		puppet.GetComponent<MeshRenderer>().material.color = shipColor;
		
		//Apply position.
		GetComponent<Rigidbody2D>().velocity = vel;
		transform.position = pos;
	}
	
	//Detecting getting hit by a bullet.
	void OnTriggerEnter2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && !dead){ KillMe(); }
	}	
	void OnTriggerStay2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && !dead){ KillMe(); }
	}
	
	public void KillMe(){
		Game.manager.GetComponent<GameManager>().dead = true;
		Instantiate(deathExplo, transform.position, Quaternion.identity);
		dead = true;
	}
	
	void WipeScreen(){
		Instantiate(wipeEffect, transform.position, Quaternion.identity);
		Instantiate(wipeParticles, new Vector3(0, -24, -5), Quaternion.identity);
		foreach (Transform child in Game.bulletHolder.transform) {
			Instantiate(bulletWipePoof, child.position, Quaternion.identity);
			Destroy(child.gameObject);
		}
		canWipe = false;
	}
}
