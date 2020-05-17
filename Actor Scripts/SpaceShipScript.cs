using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipScript : MonoBehaviour {
	
	float x, y, r, g, b, shootCounter, shootTime = 0.05f, knockBack = 5f, hurtCounter, hurtTime = 10f, angle;
	[HideInInspector] public float startCounter = 1.5f;
	GameObject puppet, wepPoint, bullet, bulletInst, deathExplo;
	Vector2 mousePos, vel;
	Vector3 pos;
	Color shipColor;
	AudioClip shoot;
	public enum Ship {
		Normal,
		BulletHell,
	}
	public Ship ship;
	bool dead = false;
    bool looking = false;
	//LayerMask enemyLayer = 2048;
	
	void Awake(){
		puppet = transform.Find("puppet").gameObject;
		shipColor = puppet.GetComponent<MeshRenderer>().material.color;
		Game.playerObj = this.gameObject;

		if(ship == Ship.Normal){
			wepPoint = puppet.transform.Find("point").gameObject;
			bullet = Resources.Load("Bullet") as GameObject;
			//Debug.Log("enemy layer: " + enemyLayer.value);
			
			//Load sounds.
			shoot = Resources.Load("SFX/Shoot") as AudioClip;

			//Load explo.
			deathExplo = Resources.Load("Break Effects/Player Break Effect") as GameObject;
		}

		if(ship == Ship.BulletHell){
			//Load explo.
			deathExplo = Resources.Load("Break Effects/Bullet Hell Player Break Effect") as GameObject; 
		}
	}
	
	void Update(){
		
		//Set pos and vel.
		pos = new Vector3(transform.position.x, transform.position.y, 0);
		vel = GetComponent<Rigidbody2D>().velocity;
		
		//Tracking x and y input for keyboard.
        if(Game.usingController) {
            x = Input.GetAxis("Horizontal [Gamepad]");
            y = -Input.GetAxis("Vertical [Gamepad]");
        } else {
            x = Input.GetAxis("Horizontal");
		    y = Input.GetAxis("Vertical");
        }
		
		//Move around.
		vel = Vector3.Lerp(vel, new Vector2(x * Game.speed, y * Game.speed), Time.deltaTime * 8f); 
		
		if(ship == Ship.Normal){
	        //Track the position of the mouse relative to the screen.
	        mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
			
			//Apply rotation.
			puppet.transform.localRotation = Quaternion.Slerp(puppet.transform.localRotation, myRotation(), Time.deltaTime * 25f);
			
			//Other side of screen.
			if(pos.x < -37f){ pos.x = 36.5f; }
			if(pos.x > 37f){ pos.x = -36.5f; }
			if(pos.y > 22f){ pos.y = -21.5f; }
			if(pos.y < -22f){ pos.y = 21.5f; }
		    
            //Shooting bullets with keyboard controls.
            if (!Game.usingController) { 
		        if(Input.GetButton("Shoot") && Game.canShoot){
			        Shoot();
		        }
            } else { //Shooting bullets with gamepad controls.
                if(looking && Game.canShoot){
			        Shoot();
		        }
            }
		}

		//Flip colors for ship
		r = 1f - Camera.main.backgroundColor.r;
		g = 1f - Camera.main.backgroundColor.g;
		b = 1f - Camera.main.backgroundColor.b;

		//Subracting hurt counter and flash when damaged.
		if(hurtCounter > 0){
			hurtCounter -= Time.deltaTime*10f;
			int flash = Mathf.RoundToInt(hurtCounter);
			if(flash == 10 || flash == 8 || flash == 6 || flash == 4 || flash == 2){ shipColor = Color.white; }
			else { shipColor = new Color(r, g, b); }
		} else {
			shipColor = new Color(r, g, b);
		}

		//Start counter goes down when start of next level.
		if(startCounter > 0){ startCounter -= Time.deltaTime; }
		
		puppet.GetComponent<MeshRenderer>().material.color = shipColor;
		
		//Apply position.
		GetComponent<Rigidbody2D>().velocity = vel;
		transform.position = pos;
	}
	
	private Quaternion myRotation() {
		Quaternion w = Quaternion.identity;
		if (Game.usingController) {
			Vector2 lookVec = new Vector2(Input.GetAxisRaw("Gamepad Right Stick X"), -Input.GetAxisRaw("Gamepad Right Stick Y"));
            float thresh = 0.15f;
            if(lookVec.x > thresh || lookVec.y > thresh || lookVec.x < -thresh || lookVec.y < -thresh) {
                looking = true;
                angle = Mathf.Atan2(lookVec.x, lookVec.y) * Mathf.Rad2Deg;
            } else { looking = false; }

            Vector2 moveVec = GetComponent<Rigidbody2D>().velocity;
            if((moveVec.x > thresh || moveVec.y > thresh || moveVec.x < -thresh || moveVec.y < -thresh) && !looking) {
                angle = Mathf.Atan2(-moveVec.x, moveVec.y) * Mathf.Rad2Deg;
            }

			w.eulerAngles = new Vector3(0, 0, (angle));
		} else {
			Vector2 dif = mousePos - new Vector2(transform.position.x, transform.position.y);
			dif.Normalize();
			angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
			w.eulerAngles = new Vector3(0, 0, (angle - 90));
		}
		return w;
	}
	
	void OnTriggerEnter2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){ HurtMe(hurty.gameObject); }
	}	
	void OnTriggerStay2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){ HurtMe(hurty.gameObject); }
	}
	
	private void HurtMe(GameObject obj){
		if(hurtCounter <= 0 && startCounter <= 0){
			if(ship == Ship.Normal){
				if(Game.currentHealth > 1){
					Vector2 bPoint = new Vector2(transform.position.x, transform.position.y) - new Vector2(obj.transform.position.x, obj.transform.position.y);
					bPoint.Normalize();
					GetComponent<Rigidbody2D>().AddForce(new Vector2((bPoint.x*knockBack)*Game.speed, (bPoint.y*knockBack)*Game.speed), ForceMode2D.Impulse);
					Game.currentHealth--;
					hurtCounter = hurtTime;
				} else if(!dead) {
					KillMe();
				}
			} else if(ship == Ship.BulletHell) {
				if(!dead){
					KillMe();
				}
			}
		}
	}

	void KillMe(){
		if(ship == Ship.Normal){ Game.deaths++; }
		Game.manager.GetComponent<GameManager>().dead = true;
		GameObject obj = Instantiate(deathExplo, transform.position, Quaternion.identity) as GameObject;
		if(ship == Ship.BulletHell){
			ParticleSystemRenderer pr = obj.GetComponent<ParticleSystemRenderer>();
        	pr.material.color = Game.frontColor;
		}
		
		Destroy(gameObject);
		dead = true;
	}

    //Shooting bullets.
	private void Shoot() { 
		shootCounter -= Time.deltaTime;
		if(shootCounter <= 0){
			Game.shots++;
			PlaySound.NoLoop(shoot);
			bulletInst = (GameObject) Instantiate(bullet, wepPoint.transform.position, Quaternion.identity);
			bulletInst.transform.parent = Game.bulletHolder.transform;
			bulletInst.transform.localRotation = puppet.transform.localRotation;
			shootCounter = shootTime;
		}
	}
}
