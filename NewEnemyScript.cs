using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class NewEnemyScript : MonoBehaviour {

	public enum Enemy { //Enum for types of enemy.
		Square, //Square enemy.
		Circle, //Circle enemy.
		Triangle, //Triangle enemy.
		Splitter, //Splitter enemy.
		Hexagon, //Hexagon enemy.
		VennDiagram, //Double circle splitter enemy.
		Donut, //Laser shooting donut enemy.
		MoneyCircle, //Money circle enemy.
		Shooter, //Enemy that shoots bullets.
        Urchin, //Enemy that sucks you in.
       	Swimmer, //Enemy that goes through walls and just swims forward.
	}
	public Enemy enemy; //The enemy that I am!

	public enum ShooterType{ //The types of shooter enemies.
		Regular, //4-way vertical and horizontal.
		Diagonal, //4-way diagonals.
		EightWay, //8-way.
	}
	public ShooterType shooterType;//The type of shooter I am!
	
	public enum SwimmerType { //The types of swimmer enemies.
		Up, //Swims upward.
		Down, //Swims downward. 
		Left, //Swims left.
		Right, //Swims right.
	}
	public SwimmerType swimmerDir; //Which way I swim!

    //Where the money bag is for what color zone achievement to unlock.
    public enum MoneyBag {
        None,
        Red,
        Yellow,
        Green,
        Blue,
        Purple,
        White,
    }
    public MoneyBag bagColor = MoneyBag.None;

	//GameObjects.
	GameObject breakEffect = null; //The prefab to spawn when the enemy dies.
	GameObject breakInst = null;//The instance of the prefab that spawns when the enemy dies.
	GameObject heartPickup = null; //The heart pickup that the enemy has a chance to drop.
	GameObject bullet = null; //The bullets that the shooter enemy spawns.
	GameObject coin = null; //The coin gameObject that spawns when the player dies.
	GameObject subPuppet = null; //The puppet that is a child of puppet. (That way, it can behave and be colored differently.)
	GameObject trailObj = null; //The trail gameObject that the triangle uses.
	GameObject laserPickup = null; //The laser pickup that enemies sometimes drop for the player to pickup.
	GameObject urchinOutline = null; //The outline that shows when urchins will start to suck you in.
	List<GameObject> puppets = new List<GameObject>(); //The puppets. (Children underneath me.)

	//Ints
	int hp = 0; //How much health the enemy has.
	int healthChance = 0; //The number that determines if a health pickup will spawn.
    int laserChance = 0; //The number that determines if a laser pickup will spawn.
	int coins = 0; //How many coins the enemy will spawn when they die.
	
	//Floats
	float hitCounter = 0f; //The counter that counts down after being hit. (This being higher than 0 colors the puppet to the damage color.)
	float speed = 10f; //How fast the enemy moves.
	float moveLerp = 10f; //The lerp value of the enemy movement.
	float knockBack = 5f; //The knocback force of the enemy.
	float dashCounter = 0f; //The counter that counts down for the triangle enemy dash.
	float shootCounter = 0f; //The counter that counts down for the shooter enemy to shoot.
	float laserChargeCounter = 2f; //The counter that counts down for the donut enemy to shoot it's laser.
	float laserHoldCounter = 0.5f; //The counter that holds the laser still before it shoots, giving 0.5f seconds of indication it is about to shoot!
	float dist = 0f; //How far the player is away from me.
	float coinTime = 15.5f; //This counts down and is how long the money bag has to live.
	float targetTrailLifeTime = 1f; //How long the trail lifetime is for the triangle enemy.
	private readonly float hitTime = 0.1f; 
	private readonly float updateRate = 2f;
	private readonly float dashTime = 2.25f;
	private readonly float shootTime = 2f;
	private readonly float laserChargeTime = 2f;
	private readonly float laserHoldTime = 0.5f;
	
	//Bools.
    bool madeAtk = false;
	bool needSupPuppet = false;
    [HideInInspector] public bool smiley = false;

    //Vectors.
	Vector2 force, targetForce;
	Vector3 linePlayerPointPos;
	Vector3 targetLaserPoint;
	
	RaycastHit2D playerRay;
	Rigidbody2D rig;
	LayerMask wall = 1024;
	Color targetColor, laserColor;

	//Pathfinding;
	private Seeker seeker;
	public Path path;
	[HideInInspector]
	public bool pathIsEnded = false;
	readonly float nextWaypointDistance = 3;
	private int currentWaypoint = 0;
	
	//Sounds
	List<AudioClip> hitSounds = new List<AudioClip>();
	List<AudioClip> killSounds = new List<AudioClip>();
	AudioClip laserSnd = null;
	
	void Awake(){
		dashCounter = dashTime;
		rig = GetComponent<Rigidbody2D>();
		coin = Resources.Load("Coin") as GameObject;
		heartPickup = Resources.Load("New Heart Pickup") as GameObject;
        laserPickup = Resources.Load("Laser Pickup") as GameObject;
		healthChance = Random.Range(0, 10);
        laserChance = Random.Range(0, 9);
	}
	
	void Start () {
		foreach(Transform child in gameObject.transform){
			puppets.Add(child.gameObject);
		}
		seeker = GetComponent<Seeker>();
		seeker.StartPath(transform.position, Game.playerObj.transform.position, OnPathComplete);
		StartCoroutine(UpdatePath());
		
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
		
		//Load laser sound
		laserSnd = Resources.Load("SFX/Laser") as AudioClip;

        //Set enemy stats.
        switch (enemy) {
            case Enemy.Square:
                hp = 8;
			    speed = 10f;
			    knockBack = 5f;
			    coins = 15;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Square Enemy Break Effect") as GameObject;
                break;
            case Enemy.Circle:
                hp = 5;
			    speed = 5f;
			    knockBack = 16f;
			    moveLerp = 1f;
			    coins = 15;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Circle Enemy Break Effect") as GameObject;
                break;
            case Enemy.Triangle:
                hp = 20;
			    speed = 40;
			    knockBack = 0;
			    moveLerp = 3f;
			    coins = 20;
                needSupPuppet = true;
                trailObj = puppets[0].transform.Find("trail").gameObject;
			    breakEffect = Resources.Load("Break Effects/New Triangle Enemy Break Effect") as GameObject;
                break;
            case Enemy.Splitter:
                hp = 20;
			    speed = 7.5f;
			    knockBack = 0;
			    moveLerp = 4f;
			    coins = 0;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Square Enemy Break Effect") as GameObject;
                break;
            case Enemy.Hexagon:
			    hp = 12;
			    speed = 6.5f;
			    knockBack = 2f;
			    moveLerp = 2f;
			    coins = 25;
                needSupPuppet = true;
			    breakEffect = Resources.Load("Break Effects/New Hex Enemy Break Effect") as GameObject;
                break;
            case Enemy.VennDiagram:
                hp = 20;
			    speed = 7.5f;
			    knockBack = 0;
			    moveLerp = 4f;
			    coins = 0;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Square Enemy Break Effect") as GameObject;
                break;
            case Enemy.Donut:
                hp = 15;
			    speed = 0;
			    knockBack = 0;
			    moveLerp = 0f;
			    coins = 25;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Bullet Hell Player Break Effect") as GameObject;
                break;
            case Enemy.MoneyCircle:
			    hp = 1;
			    speed = 0;
			    knockBack = 0;
			    moveLerp = 4f;
			    coins = 50;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Circle Enemy Break Effect") as GameObject;
                break;
            case Enemy.Shooter:
                hp = 9;
			    speed = 0;
			    knockBack = 0;
			    moveLerp = 0;
			    coins = 20;
                needSupPuppet = true;
                shootCounter = shootTime; //Start shoot counter at themax value.
			    breakEffect = Resources.Load("Break Effects/New Square Enemy Break Effect") as GameObject;
			    bullet = Resources.Load("New Enemies/Shooter Bullet") as GameObject;
                break;
            case Enemy.Urchin:
                hp = 12;
                speed = 0;
                knockBack = 0;
                moveLerp = 0;
                coins = 25;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Circle Enemy Break Effect") as GameObject;
                break;
            case Enemy.Swimmer:
                hp = 6;
			    speed = 7.5f;
			    knockBack = 30f;
			    moveLerp = 1f;
			    coins = 10;
                needSupPuppet = false;
			    breakEffect = Resources.Load("Break Effects/New Circle Enemy Break Effect") as GameObject;
			    break;
        }

        //Look for the subpuppet only for enemies that need it.
        if(needSupPuppet) {
            subPuppet = puppets[0].transform.Find("sub puppet").gameObject;
        }
	}
	
	//On every frame...
	void Update () {
        //Set proper parent.
        if (enemy == Enemy.MoneyCircle) {
            transform.parent = Game.bulletHolder.transform;
        } else {
            transform.parent = Game.enemyHolder.transform;
        }

        //Do my AI, only if player is present.
        if (Game.playerObj != null && enemy != Enemy.Urchin){ //Only the urchin will use fixed update for AI.
			MyAI();
		}
		
		//Flash me when I'm hit.
		if(hitCounter > 0f){
			hitCounter -= Time.deltaTime;
			targetColor = Game.bulletColor;
		} else {
			targetColor = Game.frontColor;
		}

		//Lerp that color!
		float smoothness = 5f;
		for (int i = 0; i < puppets.Count; i++){ puppets[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(puppets[i].GetComponent<MeshRenderer>().material.color, targetColor, smoothness*Time.deltaTime); }
		
		//Screen loop.
		Vector3 pos = transform.position;
        if(enemy == Enemy.Triangle) {
            if(pos.x < -35.5f){ pos.x = -35.5f;  }
		    if(pos.x > 35.5f){ pos.x = 35.5f; }
		    if(pos.y > 20.5f){ pos.y = 20.5f; }
		    if(pos.y < -20.5f){ pos.y = -20.5f; }
        } else {
            if(pos.x < -37f){ pos.x = 36.5f;  }
		    if(pos.x > 37f){ pos.x = -36.5f; }
		    if(pos.y > 22f){ pos.y = -21.5f; }
		    if(pos.y < -22f){ pos.y = 21.5f; }
        }
		transform.position = pos;
	}
	
	//On every tick of the physics engine.
	void FixedUpdate(){
		//The urchin only uses fixed update for AI.
		if (Game.playerObj != null && enemy == Enemy.Urchin){ //Only the urchin will use fixed update for AI.
			MyAI();
		}
	}
	
	//When I'm hit by a bullet.
	void OnTriggerEnter2D(Collider2D bullet){
		if(Game.playerObj != null){
	        Vector2 playerPoint = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
			Vector2 bulletPoint = new Vector2(bullet.gameObject.transform.position.x, bullet.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
            if(bullet.gameObject.tag == "Bullet" || (enemy == Enemy.MoneyCircle && bullet.gameObject.tag == "Player")){
				if(hp > 1){
					AudioClip a = hitSounds[Random.Range(0, hitSounds.Count)];
					PlaySound.NoLoop(a);
					bullet.gameObject.GetComponent<BulletScript>().PoofMe();
					hp--;
					hitCounter = hitTime;
					playerPoint.Normalize();
                    if(enemy == Enemy.Hexagon || enemy == Enemy.Circle) {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(((bulletPoint.x*knockBack)*-1)*speed/3, ((bulletPoint.y*knockBack)*-1)*speed/3), ForceMode2D.Impulse);
                    } else {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(((playerPoint.x*knockBack)*-1)*speed/3, ((playerPoint.y*knockBack)*-1)*speed/3), ForceMode2D.Impulse);
                    }
				} else {
	                Die(playerPoint, bulletPoint);
				}
			}

	        if(bullet.gameObject.tag == "Player Laser" && enemy != Enemy.MoneyCircle) {
	            Die(playerPoint, bulletPoint);
	        }
	    }
	}
	
    private void Die(Vector2 playerPoint, Vector2 bulletPoint) {
        //What to do special when the urchin dies.
        if(enemy == Enemy.Urchin) {
            float destroyTime = 1;
            GameObject newParent = new GameObject();
            newParent.name = "Urchin Death Effect";
            GameObject bubbles = puppets[0].transform.Find("bubbles").gameObject;
            GameObject vortex = puppets[0].transform.Find("vortex").gameObject;
            bubbles.transform.parent = Game.bulletHolder.transform;
            bubbles.transform.localScale = Vector3.one;
            Destroy(bubbles, 5f);
            urchinOutline.transform.parent = newParent.transform;
            vortex.transform.parent = newParent.transform;
            bubbles.GetComponent<ParticleSystem>().Stop();
            iTween.ColorTo(urchinOutline, iTween.Hash("a", 0f, "time", destroyTime, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
            LowerDistortionOvertimeScript dts = vortex.AddComponent<LowerDistortionOvertimeScript>();
            dts.speed = 25;
            Destroy(newParent, destroyTime);
        }
        //What to do special when the splitter dies.
		if(enemy == Enemy.Splitter){
			GameObject sq = Resources.Load("New Enemies/New Square Enemy") as GameObject;
			GameObject sq1 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
			GameObject sq2 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
			Vector2 sqForce = new Vector2((bulletPoint.y)*speed/0.5f, (bulletPoint.x)*speed/0.5f);
			sq1.GetComponent<Rigidbody2D>().AddForce(sqForce, ForceMode2D.Impulse);
			sq2.GetComponent<Rigidbody2D>().AddForce(-sqForce, ForceMode2D.Impulse);
		}
        //What to do special when the venn diagram dies.
		if(enemy == Enemy.VennDiagram){
			GameObject sq = Resources.Load("New Enemies/New Circle Enemy") as GameObject;
			GameObject sq1 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
			GameObject sq2 = (GameObject) Instantiate(sq, transform.position, Quaternion.identity);
			Vector2 sqForce = new Vector2((bulletPoint.y)*speed/0.5f, (bulletPoint.x)*speed/0.5f);
			sq1.GetComponent<Rigidbody2D>().AddForce(sqForce, ForceMode2D.Impulse);
			sq2.GetComponent<Rigidbody2D>().AddForce(-sqForce, ForceMode2D.Impulse);
		}

        //Spawn the coins.
		for(int i = 0; i < coins; i++){
			Quaternion coinRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
			Instantiate(coin, transform.position, coinRotation); 
		}

        //Check for moneybag achievements.
        if(enemy == Enemy.MoneyCircle) {
            switch (bagColor) {
                case MoneyBag.Red:
                    Game.CheckAchievements(32);
                    break;
                case MoneyBag.Yellow:
                    Game.CheckAchievements(34);
                    break;
                case MoneyBag.Green:
                    Game.CheckAchievements(35);
                    break;
                case MoneyBag.Blue:
                    Game.CheckAchievements(36);
                    break;
                case MoneyBag.Purple:
                    Game.CheckAchievements(37);
                    break;
                case MoneyBag.White:
                    Game.CheckAchievements(13);
                    break;
            }
            Game.SaveGame(); //Save the achievements you just unlocked.
        }
        //Other one-line things to do on death.
		AudioClip a = killSounds[Random.Range(0, killSounds.Count)]; //Load a random death sound.
        Game.enemiesKilled++; //Increase the amount of enemies you've killed by one in the statistics.
		PlaySound.NoLoop(a); //Play the death sound.
		if(healthChance == 3){ Instantiate(heartPickup, transform.position, Quaternion.identity); }
        if(Game.ownsLasers && laserChance == 3 && healthChance != 3) { Instantiate(laserPickup, transform.position, Quaternion.identity); }
		breakInst = (GameObject) Instantiate(breakEffect, transform.position, Quaternion.identity);
		breakInst.transform.parent = Game.bulletHolder.transform;
		Destroy(breakInst, 5f);
		Destroy(gameObject);
    }

	private void MyAI(){
		
        //Cast ray to player.
		playerRay = Physics2D.Linecast(transform.position, new Vector3(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y - 0.25f, Game.playerObj.transform.position.z), wall);

		//Square + Circle AI.
		if(enemy == Enemy.Square || enemy == Enemy.Circle || enemy == Enemy.Hexagon){
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
			float qSlerp = 0f;
            trailObj.GetComponent<TrailRenderer>().material.color = Game.bulletColor; //Color the trail.
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
                //Managing subpuppet color.
                float alph = dashCounter/2;
                if(alph < 0) { alph = 0; }
                if(alph > 1) { alph = 1; }
                Color subCol = new Color(Game.backColor.r, Game.backColor.g, Game.backColor.b, (1f-alph));
                subPuppet.GetComponent<MeshRenderer>().material.color = subCol;
                subPuppet.transform.localScale = new Vector3(alph, alph, 1);
                
                //Managing trail lifetime.
                targetTrailLifeTime = dashCounter;
                //Slowing down the triangle, since he isn't dashing anymore.
				float slowdown = 30f;
				Vector2 vel = rig.velocity;
				if(vel.x > 0){ vel.x -= Time.deltaTime*slowdown; }
				if(vel.x < 0){ vel.x += Time.deltaTime*slowdown; }
				if(vel.y > 0){ vel.y -= Time.deltaTime*slowdown; }
				if(vel.y < 0){ vel.y += Time.deltaTime*slowdown; }
				rig.velocity = vel;
			} else {
                Color subCol = subPuppet.GetComponent<MeshRenderer>().material.color;
                if(subCol.a > 0f) {
                    subCol.a -= Time.deltaTime;
                } else if(subCol.a < 0f) {
                    subCol.a = 0;
                }
                subPuppet.GetComponent<MeshRenderer>().material.color = subCol;
                dashCounter = dashTime;
				AStarAI();
				qSlerp = 6f;
                targetTrailLifeTime = 0.5f;
				//Apply force with that smooth lerp bby.
				GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, targetForce, moveLerp * Time.deltaTime);
			}
            float smooth = 5f;
            Mathf.Lerp(trailObj.GetComponent<TrailRenderer>().time, targetTrailLifeTime, smooth * Time.deltaTime);
			Quaternion w = Quaternion.identity;
			float angle = Mathf.Atan2(targetForce.y, targetForce.x) * Mathf.Rad2Deg;
			w.eulerAngles = new Vector3(0, 0, (angle - 90));
			puppets[0].transform.rotation = Quaternion.Slerp(puppets[0].transform.rotation, w, qSlerp * Time.deltaTime);
		}
		
		//Splitter AI.
		if(enemy == Enemy.Splitter || enemy == Enemy.VennDiagram){
			
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
            Vector3 scale = new Vector3((trigD/d), (trigD/d), 1f);
            if(scale.x > 1f) { scale.x = 1f; } else if(scale.x < 0f) { scale.x = 0f; }
            if(scale.y > 1f) { scale.y = 1f; } else if(scale.y < 0f) { scale.y = 0f; }
            subPuppet.transform.localScale = scale;
            subPuppet.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, (trigD/d));
            if(d < trigD && !madeAtk){
				GameObject atkObj = Resources.Load("New Hex Blast") as GameObject;
				GameObject atkInst = Instantiate(atkObj, transform.position, Quaternion.identity) as GameObject;
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
		
		//Shooter AI.
		if(enemy == Enemy.Shooter){
			//Quaternions for shooter bullet rotation.
			Quaternion u = transform.localRotation * Quaternion.identity;
			Quaternion ul = transform.localRotation * Quaternion.Euler(0, 0, 45);
			Quaternion l = transform.localRotation * Quaternion.Euler(0, 0, 90);
			Quaternion ur = transform.localRotation * Quaternion.Euler(0, 0, 135);
			Quaternion d = transform.localRotation * Quaternion.Euler(0, 0, 180);
			Quaternion dr = transform.localRotation * Quaternion.Euler(0, 0, (180+45));
			Quaternion r = transform.localRotation * Quaternion.Euler(0, 0, 270);
			Quaternion dl = transform.localRotation * Quaternion.Euler(0, 0, (270+45));
			//Subtract that counter!
			shootCounter -= Time.deltaTime;

            //Manage the subpuppet color and scale.
            float f = (shootCounter/2f)+0.2f;
            if(f > 1f) { f = 1f;}
            subPuppet.transform.localScale = new Vector3(f-0.2f, f-0.2f, 1);
            subPuppet.GetComponent<MeshRenderer>().material.color = new Color(Game.backColor.r, Game.backColor.g, Game.backColor.b, 1f-f);

            //When the shootcounter goes below 0...
			if(shootCounter <= 0){
                //Make the bullets.
				switch(shooterType){
                    case ShooterType.Regular:
                        GameObject go1 = Instantiate(bullet, transform.position, u) as GameObject; go1.transform.parent = Game.bulletHolder.transform;
					    GameObject go2 = Instantiate(bullet, transform.position, d) as GameObject; go2.transform.parent = Game.bulletHolder.transform;
					    GameObject go3 = Instantiate(bullet, transform.position, l) as GameObject; go3.transform.parent = Game.bulletHolder.transform;
					    GameObject go4 = Instantiate(bullet, transform.position, r) as GameObject; go4.transform.parent = Game.bulletHolder.transform;
                        break;
                    case ShooterType.Diagonal:
                        GameObject go5 = Instantiate(bullet, transform.position, ul) as GameObject; go5.transform.parent = Game.bulletHolder.transform;
					    GameObject go6 = Instantiate(bullet, transform.position, ur) as GameObject; go6.transform.parent = Game.bulletHolder.transform;
					    GameObject go7 = Instantiate(bullet, transform.position, dr) as GameObject; go7.transform.parent = Game.bulletHolder.transform;
					    GameObject go8 = Instantiate(bullet, transform.position, dl) as GameObject; go8.transform.parent = Game.bulletHolder.transform;
				        break;
                    case ShooterType.EightWay:
                        GameObject go9 = Instantiate(bullet, transform.position, u) as GameObject; go9.transform.parent = Game.bulletHolder.transform;
					    GameObject go10 = Instantiate(bullet, transform.position, d) as GameObject; go10.transform.parent = Game.bulletHolder.transform;
					    GameObject go11 = Instantiate(bullet, transform.position, l) as GameObject; go11.transform.parent = Game.bulletHolder.transform;
					    GameObject go12 = Instantiate(bullet, transform.position, r) as GameObject; go12.transform.parent = Game.bulletHolder.transform;
					    GameObject go13 = Instantiate(bullet, transform.position, ul) as GameObject; go13.transform.parent = Game.bulletHolder.transform;
					    GameObject go14 = Instantiate(bullet, transform.position, ur) as GameObject; go14.transform.parent = Game.bulletHolder.transform;
					    GameObject go15 = Instantiate(bullet, transform.position, dr) as GameObject; go15.transform.parent = Game.bulletHolder.transform;
					    GameObject go16 = Instantiate(bullet, transform.position, dl) as GameObject; go16.transform.parent = Game.bulletHolder.transform;
                        break;
                }
				shootCounter = shootTime;
			}
		}

        //Donut AI.
        if (enemy == Enemy.Donut) {
            GameObject pupil = transform.Find("pupil").gameObject;
            GameObject pupilCharge = transform.Find("pupil charge").gameObject;
            GameObject lineObj = pupil.transform.Find("laser").gameObject;
            GameObject linePuppet = lineObj.transform.Find("laser puppet").gameObject;
            GameObject spark = lineObj.transform.Find("laser sparks").gameObject;

            //Manage the position of the pupil charge.
            pupilCharge.transform.position = new Vector3(pupil.transform.position.x, pupil.transform.position.y, -1f);

            //Rotate laser to player, but only when not firing.
            if (!madeAtk) {
                Vector3 dir = transform.position - new Vector3(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y - 0.25f, Game.playerObj.transform.position.z);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                pupil.transform.rotation = Quaternion.Slerp(pupil.transform.rotation, newRotation, 5f * Time.deltaTime);
            }

            //The direction the ray will look, at the player.
            RaycastHit2D hit = Physics2D.Raycast(pupil.transform.position, -pupil.transform.right, 100, wall);
            if (hit) {
                dist = hit.distance;
            } else { dist = 100f; }
            lineObj.transform.localScale = new Vector3(dist + 0.5f, 1, 1);

            //Setting texture scale of the sparks.
            spark.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(dist, 1f);
            spark.SetActive(madeAtk);

            //If looking at a player.
            if (!playerRay) {
                Vector3 pPos = transform.position - Game.playerObj.transform.position;
                Vector3 nPPos = -(pPos.normalized) / 1.5f;
                if (!madeAtk) { pupil.transform.localPosition = Vector3.Lerp(pupil.transform.localPosition, nPPos, 4 * Time.deltaTime); }
                laserChargeCounter -= Time.deltaTime;
                if (laserChargeCounter <= 0) {
                    laserHoldCounter -= Time.deltaTime;
                    pupilCharge.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b,  laserHoldCounter*2);
                    pupilCharge.transform.localScale = new Vector3(laserHoldCounter*2f, laserHoldCounter*2, 1f);
                    laserColor = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b);
                    if (!madeAtk) {
                        PlaySound.NoLoop(laserSnd);
                        madeAtk = true;
                    }
                    if (laserHoldCounter <= 0) {
                        laserChargeCounter = laserChargeTime;
                    }
                } else {
                    pupilCharge.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b,  1f-(laserChargeCounter/2f));
                    pupilCharge.transform.localScale = new Vector3(1f-(laserChargeCounter/2f), 1f-(laserChargeCounter/2f), 1f);
                    madeAtk = false;
                    laserHoldCounter = laserHoldTime;
                    laserColor = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.5f);
                }
            } else {//If can't see the player.
                madeAtk = false;
                laserChargeCounter = laserChargeTime;
                pupil.transform.localPosition = Vector3.Lerp(pupil.transform.localPosition, Vector3.zero, 4 * Time.deltaTime);
                laserColor = Color.clear;
            }
            linePuppet.GetComponent<MeshRenderer>().material.color = laserColor;
            if (laserChargeCounter <= 0) { lineObj.tag = "Murder"; }
            else { lineObj.tag = "Untagged"; }
		}
		
		//Money Circle AI.
		if(enemy == Enemy.MoneyCircle){
			coinTime -= Time.deltaTime;
			GameObject n = transform.Find("text").gameObject;
			n.GetComponent<TextMesh>().text = Mathf.RoundToInt(coinTime).ToString();
			GetComponent<Collider2D>().enabled = (coinTime > 0f);
			if(coinTime <= 0f){
				GameObject succ = Resources.Load("Suck Effect") as GameObject;
				GameObject succInst = Instantiate(succ, new Vector3(transform.position.x, transform.position.y, transform.position.z + 4), Quaternion.identity);
				succInst.transform.parent = Game.bulletHolder.transform;
				breakInst = (GameObject) Instantiate(breakEffect, transform.position, Quaternion.identity);
				breakInst.transform.parent = Game.bulletHolder.transform;
				Destroy(succInst, 5f);
				Destroy(breakInst, 5f);
				Destroy(gameObject);
			}
		}

        //Urchin AI.
        if(enemy == Enemy.Urchin) {
            float suckPower = 1.25f;//How hard i suck.

            //Coloring the urchin outline.
            if(urchinOutline == null){
            	urchinOutline = puppets[0].transform.Find("visible radius").gameObject;
            	return;
            } else if(urchinOutline != null && hp > 0){
            	Color lineC = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 0.35f);
            	urchinOutline.GetComponent<LineRenderer>().material.color = lineC;
            }

            //If the player is present... suck him!
		    if(Game.playerObj){
			    Vector2 playerPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
			    Vector2 urchinDistance = new Vector2(transform.position.x, transform.position.y);
			    float triggerRadius = 15f;
			    float d = Vector2.Distance(playerPos, urchinDistance);
			    //Debug.Log(d.ToString());
			    if(d < triggerRadius){
				    Vector2 forceToApply = (playerPos - urchinDistance);
				    forceToApply.Normalize();
				    forceToApply = new Vector2(-forceToApply.x*suckPower, -forceToApply.y*suckPower);
				    //Debug.Log(forceToApply.ToString());
				    Game.playerObj.GetComponent<Rigidbody2D>().velocity += forceToApply;
			    }
		    }
        }

        //Swimmer AI.
        if(enemy == Enemy.Swimmer){
        	switch(swimmerDir){
        		case SwimmerType.Up:
        			targetForce = new Vector2(0, speed);
        			break;
        		case SwimmerType.Down:
        			targetForce = new Vector2(0, -speed);
        			break;
        		case SwimmerType.Left:
        			targetForce = new Vector2(-speed, 0);
        			break;
        		case SwimmerType.Right:
        			targetForce = new Vector2(speed, 0);
        			break;
        	}
        	
        	//Apply force.
			GetComponent<Rigidbody2D>().velocity = targetForce;
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
		if (dist < nextWaypointDistance){
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