using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class EnemyScript : MonoBehaviour {
	
	public enum Enemy {
		Square,
		Circle,
		Triangle,
		Splitter,
		Hexagon,
	}
	public Enemy enemy;
	
	List<GameObject> puppets = new List<GameObject>();
	GameObject breakEffect, breakInst, heartPickup;
	
	int hp = 0;
	int healthChance = 0;
	
	float hitCounter = 0f;
	float hitTime = 0.1f;
	float speed = 10f;
	float updateRate = 2;
	float moveLerp = 10f;
	float knockBack = 5f;
	float dashTime = 2.25f, dashCounter;

	bool madeAtk = false;
	[HideInInspector] public bool smiley = false;

	Vector2 force, targetForce;
	RaycastHit2D playerRay;
	Rigidbody2D rig;
	LayerMask wall = 1024;
	
	//Pathfinding
	private Seeker seeker;
	public Path path;
	[HideInInspector]
    public bool pathIsEnded = false;
    float nextWayPointDistance = 3;
    private int currentWaypoint = 0;
	
    //Sounds.
    List<AudioClip> hitSounds = new List<AudioClip>();
    List<AudioClip> killSounds = new List<AudioClip>();

	void Awake(){ 
		dashCounter = dashTime;
		rig = GetComponent<Rigidbody2D>();
		heartPickup = Resources.Load("Heart Pickup") as GameObject;
		healthChance = Random.Range(0, 10);
	}
	
	void Start(){
		foreach (Transform child in gameObject.transform) {
			puppets.Add(child.gameObject);
		}
		seeker = GetComponent<Seeker>();
		seeker.StartPath(transform.position, Game.playerObj.transform.position, OnPathComplete);
        StartCoroutine(UpdatePath());
		//Debug.Log("wall layer: "+wall.value);
		
        //Load hit sounds.
        for(int i = 0; i < 10; i++){
        	AudioClip a = Resources.Load("SFX/Monster Hit "+i.ToString()) as AudioClip;
        	hitSounds.Add(a);
        }

        //Load kill sound.
        for(int i = 0; i < 3; i++){
        	AudioClip a = Resources.Load("SFX/Enemy Kill "+i.ToString()) as AudioClip;
        	killSounds.Add(a);
        }

		//Set enemy stats.
		if(enemy == Enemy.Square){
			hp = 8;
			speed = 10f;
			knockBack = 5f;
			breakEffect = Resources.Load("Break Effects/Square Enemy Break Effect") as GameObject;
		}
		
		if(enemy == Enemy.Circle){
			hp = 5;
			speed = 5f;
			knockBack = 30f;
			moveLerp = 1f;
			breakEffect = Resources.Load("Break Effects/Circle Enemy Break Effect") as GameObject;
		}
		
		if(enemy == Enemy.Triangle){
			hp = 30;
			speed = 40;
			knockBack = 0;
			moveLerp = 3f;
			breakEffect = Resources.Load("Break Effects/Triangle Enemy Break Effect") as GameObject;
		}
		
		if(enemy == Enemy.Splitter){
			hp = 20;
			speed = 7.5f;
			knockBack = 0;
			moveLerp = 4f;
			breakEffect = Resources.Load("Break Effects/Square Enemy Break Effect") as GameObject;
		}
		
		if(enemy == Enemy.Hexagon){
			hp = 12;
			speed = 6f;
			knockBack = 1f;
			moveLerp = 2f;
			breakEffect = Resources.Load("Break Effects/Hex Enemy Break Effect") as GameObject;			
		}
	}
	
	void Update(){
		//Set proper parent.
		transform.parent = Game.enemyHolder.transform;
		
		//Do my AI, only if a player is present.
		if(Game.playerObj != null){
			MyAI();
		}
		
		//Flash me when im hit.
		if(hitCounter > 0f){
			hitCounter -= Time.deltaTime;
			for(int i = 0; i < puppets.Count; i++){
				puppets[i].GetComponent<InvertColor>().enabled = false;
				puppets[i].GetComponent<MeshRenderer>().material.color = Color.white;
			}
		} else {
			for(int i = 0; i < puppets.Count; i++){
				puppets[i].GetComponent<InvertColor>().enabled = true;
			}
		}
		
		//Screen loop.
		Vector3 pos = transform.position;
		
		//Other side of screen.
		if(pos.x < -37){ pos.x = 36.5f;  }
		if(pos.x > 37){ pos.x = -36.5f; }
		if(pos.y > 22f){ pos.y = -21.5f; }
		if(pos.y < -22f){ pos.y = 21.5f; }
	
		transform.position = pos;
	}
	
	//When I'm hit by a bullet.
	void OnTriggerEnter2D(Collider2D bullet){
		if(bullet.gameObject.tag == "Bullet"){
			Vector2 bPoint = new Vector2(bullet.gameObject.transform.position.x, bullet.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
			if(hp > 1){
				AudioClip a = hitSounds[Random.Range(0, hitSounds.Count)];
				PlaySound.NoLoop(a);
				bullet.gameObject.GetComponent<BulletScript>().PoofMe();
				hp--;
				hitCounter = hitTime;
				bPoint.Normalize();
				GetComponent<Rigidbody2D>().AddForce(new Vector2(((bPoint.x*knockBack)*-1)*speed/3, ((bPoint.y*knockBack)*-1)*speed/3), ForceMode2D.Impulse);
			} else {
				if(enemy == Enemy.Splitter){
					GameObject sq = Resources.Load("Enemies/Square Enemy") as GameObject;
					GameObject sq1 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
					GameObject sq2 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
					Vector2 sqForce = new Vector2((bPoint.y*5)*speed/3, (bPoint.x*5)*speed/3);
					sq1.GetComponent<Rigidbody2D>().AddForce(sqForce,  ForceMode2D.Impulse);
					sq2.GetComponent<Rigidbody2D>().AddForce(-sqForce,  ForceMode2D.Impulse);
				}
				AudioClip a = killSounds[Random.Range(0, killSounds.Count)];
				PlaySound.NoLoop(a);
				if(healthChance == 3){ Instantiate(heartPickup, transform.position, Quaternion.identity); }
				breakInst = (GameObject) Instantiate(breakEffect, transform.position, Quaternion.identity);
				breakInst.transform.parent = Game.bulletHolder.transform;
				Destroy(breakInst, 5f);
				Destroy(gameObject);
			}
		}
	}
	
	private void MyAI(){
		
		//Square + Circle AI.
		if(enemy == Enemy.Square || enemy == Enemy.Circle || enemy == Enemy.Hexagon){
			//Cast ray to player.
			playerRay = Physics2D.Linecast(transform.position, Game.playerObj.transform.position, wall);
			
			//If the player ray isn't interrupted by a wall...
			if(!playerRay){
				targetForce = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
				targetForce.Normalize();
				targetForce = new Vector2(targetForce.x * speed, targetForce.y * speed);
				if(enemy == Enemy.Square){ moveLerp = 3f; }
			} else { // If you are interrupted by a wall.
				AStarAI();
			}
			
			if(enemy == Enemy.Square) {
				//Setting rotation of my bby.
				puppets[0].GetComponent<RotateScript>().rotationSpeed = (targetForce.x/2) * -1;
			}
			
			//Apply force with that smooth lerp bby.
			GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, targetForce, moveLerp * Time.deltaTime);
		}
		
		//Triangle AI.
		if(enemy == Enemy.Triangle){
			//Cast ray to player.
			playerRay = Physics2D.Linecast(transform.position, Game.playerObj.transform.position, wall);
			float qSlerp = 0f;
			//If the player ray isn't interrupted by a wall...
			if(!playerRay){
				qSlerp = 6f;
				targetForce = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
				targetForce.Normalize();
				dashCounter -= Time.deltaTime;
				if(dashCounter <= 0){
					GetComponent<Rigidbody2D>().AddForce(new Vector2(targetForce.x * speed, targetForce.y * speed), ForceMode2D.Impulse);
					dashCounter = dashTime;
				}
				float slowdown = 20f;
				Vector2 vel = rig.velocity;
				if(vel.x > 0){ vel.x -= Time.deltaTime*slowdown; }
				if(vel.x < 0){ vel.x += Time.deltaTime*slowdown; }
				if(vel.y > 0){ vel.y -= Time.deltaTime*slowdown; }
				if(vel.y < 0){ vel.y += Time.deltaTime*slowdown; }
				rig.velocity = vel;
			} else {
				AStarAI();
				qSlerp = 2.5f;
				//Apply force with that smooth lerp bby.
				GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, targetForce, moveLerp * Time.deltaTime);
			}
			Quaternion w = Quaternion.identity;
			float angle = Mathf.Atan2(targetForce.y, targetForce.x) * Mathf.Rad2Deg;
			w.eulerAngles = new Vector3(0, 0, (angle - 90));
			puppets[0].transform.rotation = Quaternion.Slerp(puppets[0].transform.rotation, w, qSlerp * Time.deltaTime);
		}
		
		//Splitter AI.
		if(enemy == Enemy.Splitter){
			//Cast ray to player.
			playerRay = Physics2D.Linecast(transform.position, Game.playerObj.transform.position, wall);
			
			//If the player ray isn't interrupted by a wall...
			if(!playerRay){
				targetForce = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
				targetForce.Normalize();
				targetForce = new Vector2(targetForce.x * speed, targetForce.y * speed);
			} else {
				AStarAI();
			}
			//Apply force with that smooth lerp bby.
			GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, targetForce, moveLerp * Time.deltaTime);
		}
		
		//Hexagon AI.
		if(enemy == Enemy.Hexagon){
			float trigD = 8f;
			float d = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y));
			if(d < trigD && !madeAtk){
				GameObject atkObj = Resources.Load("Hex Blast") as GameObject;
				GameObject atkInst = (GameObject) Instantiate(atkObj, transform.position, Quaternion.identity);
				iTween.ScaleTo(atkInst, iTween.Hash(
					"x", 3f,
					"y", 3f,
					"z", 3f,
					"time", 0.2f,
					"easetype", iTween.EaseType.easeInOutSine,
					"looptype", iTween.LoopType.pingPong
				));
				atkInst.transform.parent = gameObject.transform;
				Destroy(atkInst, 0.39f);
				madeAtk = true;
			}
			if(d > trigD){
				madeAtk = false;
			}
		}
	}
	
	public void OnPathComplete(Path p) {
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
    }
	
	void AStarAI(){
		//A*!!!!!!
		//Debug.Log("Doing A*");
		if(enemy == Enemy.Square){ moveLerp = 5f; }
		if (path == null) { return; }
		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded) {
				return;
			}
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;
		
		//Direction to the next waypoint;
		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

		//Move the AI.
		targetForce = new Vector2(dir.x, dir.y);
		targetForce.Normalize();
		targetForce = new Vector2(targetForce.x * speed, targetForce.y * speed);

		float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if (dist < nextWayPointDistance)
		{
			currentWaypoint++;
			return;
		}	
	}
	
	IEnumerator UpdatePath(){
		if(Game.playerObj != null){ seeker.StartPath(transform.position, Game.playerObj.transform.position, OnPathComplete); }
		yield return new WaitForSeconds(1f / updateRate);
		StartCoroutine(UpdatePath());
    }

}
