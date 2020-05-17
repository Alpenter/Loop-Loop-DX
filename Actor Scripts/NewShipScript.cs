using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewShipScript : MonoBehaviour {
	
    readonly int knockBackMultiplier = 20;

	float x, y, shootCounter, shootTime = 0.05f, hurtCounter, speed, peeTime, angle, laserShootCounter, laserHoldCounter, deflectCounter, finalShootCounter, hurtyYPos;
    readonly float knockBack = 5f, hurtTime = 10f, laserShootTime = 1.5f, laserHoldTime = 1.3f, deflectTime = 0.2f, finalShootTime = 0.05f;
	[HideInInspector] public float startCounter = 1.5f;
	
    [HideInInspector] public bool dead = false;
    bool playedExitDisruptorSound = true;
    bool playedEnterDisruptorSound = false;
    bool moving = false;
    [HideInInspector] public bool looking = false;
    bool checkedPeeAchievement = false;
    
	GameObject puppet, bullet, deathExplo, dbShotPuppet1, dbShotPuppet2, disruptorParticlesObj, peeSoundObj, laserPrefab, currentLaser, deflectEffect, swearObj;
	GameObject p0, p1, p2, pLaser;
    GameObject glitterObj = null;
	GameObject finalBulletBatch = null;
    GameObject finalTrail = null;
    GameObject loopPuppet = null;

	Color shipColor;
	
	Vector2 mousePos;
	Vector2 deflectForceVec;
	[HideInInspector]
	public Vector2 vel;
	Vector3 pos;
	
    Quaternion finalShipRot = Quaternion.identity;
    //ghjgjh
	AudioClip shoot = null;
	AudioClip enterDisruptorSnd = null;
	AudioClip exitDisruptorSnd = null;
    AudioClip laserSnd = null;
    AudioClip wallDeflectSnd = null;
    AudioClip[] hitSndPool = new AudioClip[4];

    ParticleSystem pee = null;
    
    readonly LayerMask disruptorLayer = 131072;

    GameManager man = null;

	void Awake(){
        //You never begin in a disruptor field.
        Game.inDisruptor = false;

        //Find necessary children objects.
		puppet = transform.Find("puppet").gameObject; //Find puppet.
		disruptorParticlesObj = transform.Find("disruptor particles").gameObject; //Find puppet.
		Game.playerObj = this.gameObject; //Store player object staticly.
		bullet = Resources.Load("New Bullet") as GameObject; //Load bullet.
		deathExplo = Resources.Load("Break Effects/New Player Break Effect") as GameObject;//Load death explosion.
        laserPrefab = Resources.Load("Player Laser") as GameObject;
        GameObject peeObj = transform.Find("Pee").gameObject;
        pee = peeObj.GetComponent<ParticleSystem>();
        peeSoundObj = transform.Find("pee sound").gameObject;
        deflectEffect = Resources.Load("Deflect Effect") as GameObject;
        swearObj = Resources.Load("Carl Swear Word") as GameObject;
        glitterObj = transform.Find("glitter").gameObject;
        finalBulletBatch = Resources.Load("Final Player Bullet Batch") as GameObject;
        finalTrail = transform.Find("final trail").gameObject;
        loopPuppet = puppet.transform.Find("loop puppet").gameObject;

		//Finding shoot points.
		p0 = puppet.transform.Find("point 0").gameObject;
		p1 = puppet.transform.Find("point 1").gameObject;
		p2 = puppet.transform.Find("point 2").gameObject;
		pLaser = puppet.transform.Find("laser point").gameObject;

		//Apply stats based on rank.
		ApplyStats();
		
		//Load sounds.
		shoot = Resources.Load("SFX/Shoot") as AudioClip;
        enterDisruptorSnd = Resources.Load("SFX/Enter Disruptor Sound") as AudioClip;
        exitDisruptorSnd = Resources.Load("SFX/Exit Disruptor Sound") as AudioClip;
        laserSnd = Resources.Load("SFX/Player Laser Shoot") as AudioClip;
        wallDeflectSnd = Resources.Load("SFX/Wall Bounce") as AudioClip;
        for(int i = 0; i < hitSndPool.Length; i++){
        	hitSndPool[i] = Resources.Load("SFX/carlhit" + i.ToString()) as AudioClip;
        }
	}

    private void Start() {
        man = Game.manager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update () {
        if (Game.atFinalBossFinalForm) {
            //This is how the player is handled during the final phase of the final boss.
            ControlPlayerFinal();
        } else {
            //This is how the player is handled regularly.
		    ControlPlayerRegularly();
        }

        //Subracting hurt counter and flash when damaged.
		if(hurtCounter > 0){
			hurtCounter -= Time.deltaTime*10f;
			int flash = Mathf.RoundToInt(hurtCounter);
			if(flash == 10 || flash == 8 || flash == 6 || flash == 4 || flash == 2){ shipColor = Game.bulletColor; }
			else { shipColor = Game.frontColor; }
		} else {
			shipColor = Game.frontColor;
		}

        //Apply colors.
		puppet.GetComponent<MeshRenderer>().material.color = shipColor;
		dbShotPuppet1.GetComponent<MeshRenderer>().material.color = shipColor;
		dbShotPuppet2.GetComponent<MeshRenderer>().material.color = shipColor;

        //Managing your bladder. (Only if you've beaten the blue zone and are not at the final boss final form.)
        if (Input.GetButton("Pee") && Game.beatBlue && !Game.atFinalBossFinalForm) {
            peeSoundObj.GetComponent<AudioSource>().volume = Game.soundVolume;
            peeTime += Time.deltaTime;
            if(peeTime >= 30f && !checkedPeeAchievement) {
                Game.CheckAchievements(45);
                checkedPeeAchievement = true;
            }
            if (!pee.isPlaying) { pee.Play(); }
        } else {
            checkedPeeAchievement = false;
            peeSoundObj.GetComponent<AudioSource>().volume = 0f;
            if (pee.isPlaying) { pee.Stop(); }
            peeTime = 0;
        }
	}
	
    //This is how the player is handled during the final phase of the final boss.
    void ControlPlayerFinal() {
        //Start counter is always 0.
        startCounter = 0;

        //Hide the loop puppet.
        loopPuppet.SetActive(false);

        //Turn on the final trail.
        finalTrail.SetActive(true);
        if (!finalTrail.GetComponent<ParticleSystem>().isPlaying) {
            finalTrail.GetComponent<ParticleSystem>().Play();
        }

        //The x position the player is locked at.
        float xLockPos = -30.5f;
        
        //Set pos and vel vectors.
		pos = new Vector3(xLockPos, transform.position.y, 0);
		vel = GetComponent<Rigidbody2D>().velocity;

        //Reading moving up and down controls without a controller.
        if (Game.usingController) {
            y = -Input.GetAxis("Vertical [Gamepad]");
        } else { //Reading vertical input from a controller.
            y = Input.GetAxisRaw("Vertical");
        }

        //Lerp the vel vector to match the y input.
        float speed = 23f;
		vel = Vector2.Lerp(vel, new Vector2(0, y * speed), Time.deltaTime * 8f);

        //Loop the player around vertically.
		if(pos.y > 22f){ pos.y = -21.5f; }
		if(pos.y < -22f){ pos.y = 21.5f; }

        //Apply position and velocity.
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vel;

        //Keep ship at proper rotation.
        finalShipRot.eulerAngles = new Vector3(0, 0, -90);
        puppet.transform.rotation = finalShipRot;

        //Shooting bullets.
        float bulletXSpawn = 0.5f;
        if(finalShootCounter > 0) { finalShootCounter -= Time.deltaTime; }
        if (Game.usingController) {
            //Looking with the right stick.
            Vector2 lookVec = new Vector2(Input.GetAxisRaw("Gamepad Right Stick X"), -Input.GetAxisRaw("Gamepad Right Stick Y"));
            float thresh = 0.15f;
            if(lookVec.x > thresh || lookVec.y > thresh || lookVec.x < -thresh || lookVec.y < -thresh) {
                looking = true;
                angle = Mathf.Atan2(lookVec.x, lookVec.y) * Mathf.Rad2Deg;
            } else { looking = false; }

            //If you are holding the right stick in any direction, shoot!
            if(looking && finalShootCounter <= 0){
			    PlaySound.NoLoop(shoot);
                Vector3 bulletSpawnPos = new Vector3(pos.x + bulletXSpawn, pos.y, 0);
                Instantiate(finalBulletBatch, bulletSpawnPos, Quaternion.identity);
                finalShootCounter = finalShootTime;
		    }
        } else {
            if(Input.GetButton("Shoot") && finalShootCounter <= 0){
			    PlaySound.NoLoop(shoot);
                Vector3 bulletSpawnPos = new Vector3(pos.x + bulletXSpawn, pos.y, 0);
                Instantiate(finalBulletBatch, bulletSpawnPos, Quaternion.identity);
                finalShootCounter = finalShootTime;
		    }
        }
    }

    //This is how the player is handled regularly.
    void ControlPlayerRegularly() {
        if(startCounter > 0){ startCounter -= Time.deltaTime; }
		
		//Set pos and vel.
		pos = new Vector3(transform.position.x, transform.position.y, 0);
		vel = GetComponent<Rigidbody2D>().velocity;

        //If the laser hold counter is less than or equal to 0, move normally.
        if(laserHoldCounter <= 0 && deflectCounter <= 0) { 
            //Tracking x and y input for keyboard.
            if (Game.usingController) {
                x = Input.GetAxis("Horizontal [Gamepad]");
                y = -Input.GetAxis("Vertical [Gamepad]");
            } else {
		        x = Input.GetAxisRaw("Horizontal");
		        y = Input.GetAxisRaw("Vertical");

                //Track the position of the mouse relative to the screen.
		        mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		        if(Camera.main.gameObject != null){
		        	mousePos = Camera.main.ScreenToWorldPoint(mousePos);
	        	} else {
	        		mousePos = Vector2.zero;
	        	}
            }

		    //Move around.
		    vel = Vector2.Lerp(vel, new Vector2(x * speed, y * speed), Time.deltaTime * 8f);

            //Looping around the screen. (You can do this when you aren't being pushed by the laser.)
            if(Game.loopBlocked){ //If the player is blocked from looping... knock him back!
            	if(deflectCounter > 0){
            		deflectCounter -= Time.deltaTime;
            	} else {
            		if(pos.x < -36f){ WallDeflect("Left"); }
		    		if(pos.x > 36f){ WallDeflect("Right"); }
		    		if(pos.y > 21f){ WallDeflect("Up"); }
		    		if(pos.y < -21f){ WallDeflect("Down"); }
		    	}
        	} else { //If the player is NOT blocked from looping, go around the screen as normal.
        		if(pos.x < -37f){ pos.x = 36.5f; }
		    	if(pos.x > 37f){ pos.x = -36.5f; }
		    	if(pos.y > 22f){ pos.y = -21.5f; }
		    	if(pos.y < -22f){ pos.y = 21.5f; }
        	}
		    

            //Apply rotation.
		    puppet.transform.localRotation = Quaternion.Slerp(puppet.transform.localRotation, MyRotation(), Time.deltaTime * 25f);
        } else if(laserHoldCounter > 0) { //If the laser hold counter is greater than zero... push the player back with it.
            laserHoldCounter -= Time.deltaTime;
            if(currentLaser != null && laserHoldCounter < 0.8f) { 
                //Push back the player.
                float pushBack = (speed*laserHoldCounter);
                GameObject midPoint = currentLaser.GetComponent<PlayerLaserScript>().midPoint;
                Vector2 bPoint = new Vector2(transform.position.x, transform.position.y) - new Vector2(midPoint.transform.position.x, midPoint.transform.position.y);
			    bPoint.Normalize();
			    vel = new Vector2((bPoint.x*pushBack), (bPoint.y*pushBack));

                //Position the laser.
                currentLaser.transform.position = pLaser.transform.position;
            } else { vel = Vector2.zero; }
            //If you go out of bounds of screen while being pushed by the laser, keep me on the screen and don't loop.
            if(pos.x < -36f){ pos.x = -36f; }
		    if(pos.x > 36f){ pos.x = 36f; }
		    if(pos.y > 21f){ pos.y = 21f; }
		    if(pos.y < -21f){ pos.y = -21f; }
        } else if(deflectCounter > 0){
        	deflectCounter -= Time.deltaTime;
        	vel = deflectForceVec;
        } else {
        	vel = Vector2.zero;
        }
		
		
		
	
        //Figuring out if the player is in the disruptor or not.
        Game.inDisruptor = Physics2D.OverlapPoint(new Vector2(pos.x, pos.y), disruptorLayer);

        //Playing the disruptor sound effects.
        if(Game.inDisruptor && !playedEnterDisruptorSound) {
            PlaySound.NoLoop(enterDisruptorSnd);
            disruptorParticlesObj.GetComponent<ParticleSystem>().Play();
            playedExitDisruptorSound = false;
            playedEnterDisruptorSound = true;
        } else if (!Game.inDisruptor && !playedExitDisruptorSound) {
            PlaySound.NoLoop(exitDisruptorSnd);
            disruptorParticlesObj.GetComponent<ParticleSystem>().Stop();
            playedEnterDisruptorSound = false;
            playedExitDisruptorSound = true;
        }

        //Always setting the color of the disruptor particle Object.
        disruptorParticlesObj.GetComponent<Renderer>().material.color = Game.bulletColor;
        
		//Shooting bullets.
		if(shootCounter > 0){ shootCounter -= Time.deltaTime; }

        //Shooting bullets with keyboard controls.
        if (!Game.usingController) { 
		    if(Input.GetButton("Shoot") && Game.canShoot && Game.gotGun && !Game.inDisruptor){
			    Shoot();
		    }
        } else { //Shooting bullets with gamepad controls.
            if(looking && Game.canShoot && Game.gotGun && !Game.inDisruptor){
			    Shoot();
		    }
        }

        //Laser shooting.
        if(laserShootCounter > 0) { laserShootCounter -= Time.deltaTime; }

        //Shoot lasers when you got them. (Keyboard)
        if(Game.ownsLasers && Input.GetButtonDown("Shoot Laser") && laserShootCounter <= 0 && !Game.usingController && Game.currentLasers > 0 && !Game.inDisruptor) {
            ShootLaser();
        }
        //Shoot lasers when you got them. (Gamepad)
        if (Game.usingController) {
            float lPull = Input.GetAxisRaw("Shoot Laser [Gamepad]");
            if(lPull > 0.65f && laserShootCounter <= 0 && Game.ownsLasers && Game.currentLasers > 0 && !Game.inDisruptor) {
                ShootLaser();
            }
            //Debug.Log("Left Trigger: " + lPull.ToString());
        }
        
		//Apply position.
		GetComponent<Rigidbody2D>().velocity = vel;
		transform.position = pos;

        //Managing the loop puppet.
        loopPuppet.SetActive(Game.canShootSelf);
        loopPuppet.GetComponent<MeshRenderer>().material.color = Game.backColor;

        //Managing the glitter.
        moving = Moving(vel);
        if (moving) { 
            if (Game.enableGlitterTrail && !glitterObj.GetComponent<ParticleSystem>().isPlaying) {
                glitterObj.GetComponent<ParticleSystem>().Play();
            }
            if(!Game.enableGlitterTrail && glitterObj.GetComponent<ParticleSystem>().isPlaying) {
                glitterObj.GetComponent<ParticleSystem>().Stop();
            }
        } else {
            if(glitterObj.GetComponent<ParticleSystem>().isPlaying) {
                glitterObj.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

	//Applying stats based on ranks of stuff.
	void ApplyStats(){
			
		//Shot Speed.
		if(Game.shotSpeedRank == 0){ shootTime = 0.2f; }
		else if(Game.shotSpeedRank == 1){ shootTime = 0.15f; }
		else if(Game.shotSpeedRank == 2){ shootTime = 0.1f; }
		else if(Game.shotSpeedRank == 3){ shootTime = 0.05f; }
		
		//Speed
		if(Game.speedRank == 0){ speed = 15f; }
		else if(Game.speedRank == 1){ speed = 17.5f; }
		else if(Game.speedRank == 2){ speed = 20f; }
		else if(Game.speedRank == 3){ speed = 22.5f; }
		
		//Shot split.
		GameObject o = puppet.transform.Find("double shot puppet").gameObject;
		o.SetActive(Game.shotSplitRank > 0);
		dbShotPuppet1 = o.transform.Find("puppet 1").gameObject;
		dbShotPuppet2 = o.transform.Find("puppet 2").gameObject;
	}
	
	
	void OnCollisionEnter2D(Collision2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){
            Vector2 v = GetHurtTouchPoint(hurty);
            HurtMe(v); 
        }
	}	
	void OnCollisionStay2D(Collision2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){
            Vector2 v = GetHurtTouchPoint(hurty);
            HurtMe(v); 
        }
	}
	
    void OnTriggerEnter2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){
            Vector2 v = GetHurtTouchPointTrigger(hurty);
            HurtMe(v); 
        }
	}	
	void OnTriggerStay2D(Collider2D hurty){
		if(hurty.gameObject.tag == "Murder" && hurtCounter <= 0){
            Vector2 v = GetHurtTouchPointTrigger(hurty);
            HurtMe(v); 
        }
	}
    
    private Vector2 GetHurtTouchPoint(Collision2D hurty) {
        if (Game.atFinalBossFinalForm) {
            ContactPoint2D contact = hurty.GetContact(0);
            Vector2 v = new Vector2(0, contact.point.y);
            return v;
        } else {
            ContactPoint2D contact = hurty.GetContact(0);
            return contact.point;
        }
        
    }

    private Vector2 GetHurtTouchPointTrigger(Collider2D hurty) {
        if (Game.atFinalBossFinalForm) {
            Vector3 pos = transform.position;
            hurtyYPos = pos.y - hurty.gameObject.transform.position.y;
            return new Vector2(0, hurtyYPos);
        } else {
            Vector2 hPos = new Vector2(hurty.gameObject.transform.position.x, hurty.gameObject.transform.position.y);
            return hPos;
        }
    }

	private void HurtMe(Vector2 touchPoint){
		if(hurtCounter <= 0 && startCounter <= 0){
			if(Game.newCurrentHealth > 1){
                Game.hitByFinalBoss = true;
                PlaySound.NoLoop(hitSndPool[UnityEngine.Random.Range(0, hitSndPool.Length)]);
		        Instantiate(swearObj, transform.position, Quaternion.identity);
                man.gotHit = true;
				Vector2 bPoint = new Vector2(transform.position.x, transform.position.y) - touchPoint;
				bPoint.Normalize();
				GetComponent<Rigidbody2D>().AddForce(new Vector2((bPoint.x*knockBack)*knockBackMultiplier, (bPoint.y*knockBack)*knockBackMultiplier), ForceMode2D.Impulse);
				Game.newCurrentHealth--;
				hurtCounter = hurtTime;
			} else if(!dead) {
				KillMe();
			}
		}
	}
	
    void Shoot() {
        if(shootCounter <= 0 && startCounter <= 0.65f){
			Game.newShotCount++;
			PlaySound.NoLoop(shoot);
			//How many bullets and where they come from based on your shot split rank.
			switch(Game.shotSplitRank){
				case 0:
					GameObject bulletInst = Instantiate(bullet, p0.transform.position, p0.transform.rotation) as GameObject;
					bulletInst.transform.parent = Game.bulletHolder.transform;
					break;
				case 1:
					GameObject bulletInst1 = Instantiate(bullet, p1.transform.position, p1.transform.rotation) as GameObject;
					GameObject bulletInst2 = Instantiate(bullet, p2.transform.position, p2.transform.rotation) as GameObject;
					bulletInst1.transform.parent = Game.bulletHolder.transform;
					bulletInst2.transform.parent = Game.bulletHolder.transform;
					break;
				case 2:
					GameObject bulletInst3 = Instantiate(bullet, p0.transform.position, p0.transform.rotation) as GameObject;
					GameObject bulletInst4 = Instantiate(bullet, p1.transform.position, p1.transform.rotation) as GameObject;
					GameObject bulletInst5 = Instantiate(bullet, p2.transform.position, p2.transform.rotation) as GameObject;
					bulletInst3.transform.parent = Game.bulletHolder.transform;
					bulletInst4.transform.parent = Game.bulletHolder.transform;
					bulletInst5.transform.parent = Game.bulletHolder.transform;
					break;
			}
			shootCounter = shootTime;
		}
    }

    void ShootLaser() {
        PlaySound.NoLoop(laserSnd);
        Vector3 v = pLaser.transform.rotation.eulerAngles;
        Vector3 v2 = new Vector3(v.x, v.y, v.z + 90);
        Quaternion rot = Quaternion.Euler(v2);
        currentLaser = Instantiate(laserPrefab, pLaser.transform.position, rot) as GameObject;
        currentLaser.transform.parent = Game.bulletHolder.transform;
        Game.currentLasers--;
        laserHoldCounter = laserHoldTime;
        laserShootCounter = laserShootTime;
    }

	void KillMe(){
        man.dead = true;
		Game.newDeaths++;
		Instantiate(deathExplo, transform.position, Quaternion.identity);
        Destroy(gameObject);
	}

	private void WallDeflect(string direction){
		int zp = 5;
		float deflectKnockBack = 35f;
		switch(direction){
			case "Left":
				Vector3 rotVecLeft = new Vector3(0, 0, 270);
				Quaternion rotLeft = Quaternion.Euler(rotVecLeft);
				Instantiate(deflectEffect, new Vector3(-35.6f, transform.position.y, zp), rotLeft);
				deflectForceVec = new Vector2(deflectKnockBack, 0);
				break;
			case "Right":
				Vector3 rotVecRight = new Vector3(0, 0, 90);
				Quaternion rotRight = Quaternion.Euler(rotVecRight);
				Instantiate(deflectEffect, new Vector3(35.6f, transform.position.y, zp), rotRight);
				deflectForceVec = new Vector2(-deflectKnockBack, 0);
				break;
			case "Up":
				Vector3 rotVecUp = new Vector3(0, 0, 180);
				Quaternion rotUp = Quaternion.Euler(rotVecUp);
				Instantiate(deflectEffect, new Vector3(transform.position.x, 20.2f, zp), rotUp);
				deflectForceVec = new Vector2(0, -deflectKnockBack);
				break;
			case "Down":
				Instantiate(deflectEffect, new Vector3(transform.position.x, -20.2f, zp), Quaternion.identity);
				deflectForceVec = new Vector2(0, deflectKnockBack);
				break;
		}
		PlaySound.NoLoop(wallDeflectSnd);
		deflectCounter = deflectTime;
	}
	
	private Quaternion MyRotation() {
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

    private Quaternion WaterDistortionRotation() {
        Vector3 dif = new Vector2(vel.x, vel.y) - Vector2.zero;
        dif.Normalize();
        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = new Vector3(0, 0, (angle - 90));
        return rot;
    }

    private bool Moving(Vector2 v) {
        bool m = false;
        float limit = 0.75f;
        if(v.y > limit || v.y < -limit || v.x > limit || v.x < -limit) { m = true; }
        else { m = false; }
        return m;
    }
}
