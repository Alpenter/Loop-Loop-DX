using UnityEngine;

public class CottonJrScript : MonoBehaviour {

    public enum Phase { ComingIn, Phase1, Phase2, Transition1, Phase3, Transition2, Phase4, Death }
    Phase p = Phase.ComingIn;

    GameObject introPoofEffect, form1And2Holder, form1Puppet, textObj, rainObj, coin, form3Puppet, form4Holder, form4Body, form4Face, vektorText, puppetToColor;
    GameObject leftPoof, rightPoof, upPoof, downPoof, wallHolder, upWall, downWall, leftWall, rightWall, arrow;

    public GameObject[] attacks;

    int health = 300, moneys = 150;
    int form4Attack = 0;

    float introWait = 9f, openMouthWait = 3f, textAwayTime = 2f;
    float totalMoveTime = (2f + 1f + 1.5f + 2f) + 1.1f;
    readonly float heartSpawnTime = 2f, hitTime = 0.15f, lightingSpawnTime = 0.5f;
    float heartSpawnCounter = 2f, hitCounter, lightingSpawnCounter = 1f, totalForm4Counter = 0f; //1f;
    readonly float form4ToStartTime = 2f, form4SwipeTime = 1.5f, form4BackTime = 2f;
    float victoryWaitTime = 3f;

    bool dead = false, madeCoins = false, gotNewVel = false, allowForm4HitboxManagement = false, triggeredTextTween = false, attackedForm4 = false;
    bool playedLaughSnd = false;

    Vector2 frame = Vector2.zero;
    Vector2 vel, newVel;
    Vector3 puppet3TargetScale = Vector3.zero;
    Vector3 pos = Vector3.zero;
    Vector3 startPos, midPos, targetPos, facePos;
    Vector3 leftWallPos, rightWallPos, upWallPos, downWallPos;

    BoxCollider2D hitBox = null;

    Rigidbody2D rig = null;

    AnimationScript anim = null;
    AnimationScript animForm3 = null;

    Color targetColor = Color.green;

    AudioClip killSound, giggleSnd, sadSnd, chargeSnd, laughSnd;
    AudioClip[] smooches = new AudioClip[3];

    //Before anything else...
    void Awake() {
        //Find needed GameObjects.
        introPoofEffect = transform.Find("Intro Poof Effect").gameObject;
        form1And2Holder = transform.Find("Form 1 and 2").gameObject;
        form1Puppet = form1And2Holder.transform.Find("puppet").gameObject;
        textObj = form1And2Holder.transform.Find("love text").gameObject;
        rainObj = form1And2Holder.transform.Find("rain").gameObject;
        form3Puppet = transform.Find("Form 3 Puppet").gameObject;
        form4Holder = transform.Find("Form 4 Holder").gameObject;
        form4Body = form4Holder.transform.Find("Body").gameObject;
        form4Face = form4Body.transform.Find("Face").gameObject;
        vektorText = transform.Find("vektor text").gameObject;
        arrow = transform.Find("Arrow").gameObject;

        //Find the poofs.
        downPoof = form4Body.transform.Find("Bottom Poofs").gameObject;
        upPoof = form4Body.transform.Find("Top Poofs").gameObject;
        leftPoof = form4Body.transform.Find("Left Poofs").gameObject;
        rightPoof = form4Body.transform.Find("Right Poofs").gameObject;

        //Find the wall holder and walls for form 4.
        wallHolder = transform.Find("Wall Set").gameObject;
        wallHolder.transform.parent = null;
        leftWall = wallHolder.transform.Find("Left Wall").gameObject;
        rightWall = wallHolder.transform.Find("Right Wall").gameObject;
        upWall = wallHolder.transform.Find("Up Wall").gameObject;
        downWall = wallHolder.transform.Find("Down Wall").gameObject;

        //Set what objects to be active and inactive at the start.
        introPoofEffect.SetActive(false);
        form1And2Holder.SetActive(false);
        form3Puppet.SetActive(false);
        form3Puppet.transform.localScale = new Vector3(0, 0, 1); //Set the start scale of form 3 puppet as 0.
        form4Holder.SetActive(false);
        vektorText.SetActive(false);
        arrow.SetActive(false);
        arrow.transform.localScale = Vector3.zero;
        arrow.GetComponent<LookAtScript>().enabled = false;

        //Find hitbox.
        hitBox = GetComponent<BoxCollider2D>();
        hitBox.enabled = false;

        //Find animators.
        anim = form1Puppet.GetComponent<AnimationScript>();
        animForm3 = form3Puppet.GetComponent<AnimationScript>();
        animForm3.enabled = false;

        //Find rigidBody.
        rig = GetComponent<Rigidbody2D>();

        //Load coins.
        coin = Resources.Load("Coin") as GameObject;
    }

    private void Start() {
        //Load sounds.
        killSound = Resources.Load("SFX/CottonKill") as AudioClip;
    	giggleSnd  = Resources.Load("SFX/CottonIntro") as AudioClip;
    	sadSnd = Resources.Load("SFX/CottonSad") as AudioClip;
    	chargeSnd = Resources.Load("SFX/CottonCharge") as AudioClip;
    	laughSnd = Resources.Load("SFX/CottonDistantLaugh") as AudioClip;
    	for(int i = 0; i < smooches.Length; i++){
    		smooches[i] = Resources.Load("SFX/CottonShoot" + i.ToString()) as AudioClip;
    	}
    }

    // Update is called once per frame
    void Update() {

        //Manage health bar.
        float divider = 0.0033f;
        Game.manager.GetComponent<GameManager>().BossHealthBar(health, divider);
        if (health < 0) { health = 0; } //You can't go negative health!

        //Set proper parent.
        transform.parent = Game.enemyHolder.transform;
        
        //Flash me when I'm hit.
        if (hitCounter > 0f) {
            hitCounter -= Time.deltaTime;
            targetColor = Game.bulletColor;
        } else {
            targetColor = Game.frontColor;
        }
        float smoothness = 5f;
        if (puppetToColor != null){
            puppetToColor.GetComponent<MeshRenderer>().material.color = Color.Lerp(puppetToColor.GetComponent<MeshRenderer>().material.color, targetColor, smoothness * Time.deltaTime);
        }

        if (Game.manager.GetComponent<GameManager>().splashCounter <= 0 && !dead) {
            //Transition phase.
            if (p == Phase.ComingIn){
                //Keep hitbox disabled.
                hitBox.enabled = false;

                //Enable introPoofEffect.
                introPoofEffect.SetActive(true);

                //Setting animation frames.
                anim.framesOffset[0] = Vector2.zero;
                anim.framesOffset[1] = new Vector2(0.2f, 0f);

                //Subtract the amount of time needed for the introduction scene.
                introWait -= Time.deltaTime;

                //Set the Form 1 and 2 puppet to be active when there are 1.9 seconds left in the intro.
                bool active = (introWait <= 1.9f);
                form1And2Holder.SetActive(active);

                //When the intro cutscene is over, go to Phase 1!
                if (introWait <= 0f) {
                	PlaySound.NoLoop(giggleSnd);
                 	p = Phase.Phase1;
                }
            }
            else if (p == Phase.Phase1) { //Phase 1!
			
				//When to get rid of love text.
				if(textAwayTime > 0){
					textAwayTime -= Time.deltaTime;	
				} else {
					if(!triggeredTextTween){
						iTween.ScaleTo(textObj, iTween.Hash(
							"x", 0f, "y", 0f, "z", 1f,
							"time", 1f,
							"easetype", iTween.EaseType.easeInOutSine,
							"looptype", iTween.LoopType.none
						));
						triggeredTextTween = true;
					}
				}
				
                //Kill the poof effect object, it is no longer needed.
                if (introPoofEffect != null) { Destroy(introPoofEffect); }

                //Keep hitbox enabled and set it's params.
                hitBox.enabled = true;
                hitBox.size = new Vector2(3.5f, 1.6f);
                hitBox.offset = new Vector2(0f, -0.1f);

                //Setting animation frames.
                anim.framesOffset[0] = Vector2.zero;
                anim.framesOffset[1] = new Vector2(0.1f, 0f);

                //Setting the puppet to Color when being hit.
                puppetToColor = form1Puppet;

                //Spawning the hearts.
                heartSpawnCounter -= Time.deltaTime;
                if (heartSpawnCounter <= 0) {
                	PlaySound.NoLoop(smooches[Random.Range(0, smooches.Length)]);
                    Instantiate(attacks[0], new Vector3(transform.position.x, transform.position.y, 25), Quaternion.identity);
                    heartSpawnCounter = heartSpawnTime;
                }

                //When to go to phase 2.
                if (health < 250) {
                	PlaySound.NoLoop(sadSnd);
                    GoPhaseTwo();
                }
            }
            else if (p == Phase.Phase2) {
                //Keep hitbox enabled and set it's params.
                hitBox.enabled = true;
                hitBox.size = new Vector2(3.5f, 1.6f);
                hitBox.offset = new Vector2(0f, -0.1f);

                //Setting animation frames.
                anim.framesOffset[0] = new Vector2(0.3f, 0f);
                anim.framesOffset[1] = new Vector2(0.4f, 0f);

                //Lightning spawning.
                lightingSpawnCounter -= Time.deltaTime;
                if (lightingSpawnCounter <= 0){
                    float x = Random.Range(-32f, 32f);
                    Instantiate(attacks[1], new Vector3(x, 0, 0), Quaternion.identity);
                    lightingSpawnCounter = lightingSpawnTime;
                }

                //When to start transition phase.
                if (health < 150) {
                    health = 149;
                    p = Phase.Transition1;
                }
            }
            else if (p == Phase.Transition1) { //Transition to phase 3!
                //Disable the hitbox when transforming, also enable form 3 puppet.
                hitBox.enabled = false;
                form3Puppet.SetActive(true);

                //Waiting to open the mouth.
                openMouthWait -= Time.deltaTime;
                if (openMouthWait > 0)
                { //Keep mouth closed until wait time is over.
                    anim.framesOffset[0] = new Vector2(0.5f, 0f);
                    anim.framesOffset[1] = new Vector2(0.6f, 0f);
                } else {
                    //Play open mouth frames.
                    anim.framesOffset[0] = new Vector2(0.7f, 0f);
                    anim.framesOffset[1] = new Vector2(0.8f, 0f);
                }

                //Updating scale of the form 3 puppet to come out of the mouth!
                if (openMouthWait > -2) { puppet3TargetScale = new Vector3(0, 0, 1); }
                else { puppet3TargetScale = new Vector3(15, 15, 1); }
                float smuth = 3f;
                form3Puppet.transform.localScale = Vector3.Lerp(form3Puppet.transform.localScale, puppet3TargetScale, smuth * Time.deltaTime);

                //Animate the form3puppet a bit.
                if (openMouthWait > -4f) { frame = Vector2.zero; }
                else if (openMouthWait < -4f && openMouthWait > -5f) { frame = new Vector2(0.2f, 0f); }
                else if (openMouthWait < -5f && openMouthWait > -6f) { frame = new Vector2(0.4f, 0f); }
                else if (openMouthWait < -6f && openMouthWait > -6.25f) { frame = new Vector2(0.2f, 0f); }
                else if (openMouthWait < -6.25f && openMouthWait > -6.5f) { frame = new Vector2(0.4f, 0f); }
                else if (openMouthWait < -6.5f && openMouthWait > -6.75f) { frame = new Vector2(0.2f, 0f); }
                else if (openMouthWait < -7f) { frame = new Vector2(0.4f, 0f); }
                form3Puppet.GetComponent<MeshRenderer>().material.mainTextureOffset = frame;

                //Go to form 3!
                if (openMouthWait < -8f) {
                	PlaySound.NoLoop(chargeSnd);
                    p = Phase.Phase3;
                }
            }
            else if (p == Phase.Phase3) {
                //Destroy the unneeded puppets.
                if (form1And2Holder != null) { Destroy(form1And2Holder); }

                //Determine position and velocity.
                pos = transform.position;
                vel = rig.velocity;

                //Setting hitbox properties.
                hitBox.enabled = (health > 50);
                hitBox.offset = Vector2.zero;
                hitBox.size = new Vector2(11f, 11f);

                //Setting which puppet to be colored when hit.
                puppetToColor = form3Puppet;

                //Enable that chomp animation.
                animForm3.enabled = true;

                //Manage flip of puppet.
                if (vel.x < 0) { form3Puppet.transform.localScale = new Vector3(15, 15, 1); }
                else if (vel.x > 0) { form3Puppet.transform.localScale = new Vector3(-15, 15, 1); }

                //Go towards player!
                if (!gotNewVel && Game.playerObj != null) {
                    Vector2 pPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
                    Vector2 dif = new Vector2(pos.x, pos.y) - pPos;
                    float speed = 20f;
                    newVel = dif.normalized * speed;
                    vel = -newVel;
                    gotNewVel = true;
                }

                //Going around randomly when offscreen!
                if (pos.x > 46 || pos.x < -46 || pos.y > 31 || pos.y < -31) {

                    //To phase 4!
                    if (health < 51) {
                        arrow.SetActive(true);
                        iTween.ScaleTo(arrow, iTween.Hash(
                            "x", 2, "y", 2, "z", 2,
                            "time", 2f,
                            "delay", 4f,
                            "looptype", iTween.LoopType.none,
                            "easetype", iTween.EaseType.easeInOutSine
                        ));
                        arrow.transform.parent = null;
                        arrow.transform.position = new Vector3(0, 0, 50);
                        vel = Vector2.zero;
                        form3Puppet.SetActive(false);
                        p = Phase.Phase4;
                    }

                    //1 == top, 2 == bottom, 3 == left, 4 == right.
                    int newSide = Random.Range(1, 4);
                    float z = 0f;
                    if (newSide == 1) {
                        pos = new Vector3(Random.Range(45f, -45f), 30, z);
                        gotNewVel = false;
                    }
                    else if (newSide == 2) {
                        pos = new Vector3(Random.Range(45f, -45f), -30, z);
                        gotNewVel = false;
                    }
                    else if (newSide == 3) {
                        pos = new Vector3(-45f, Random.Range(-30f, 30f), z);
                        gotNewVel = false;
                    }
                    else if (newSide == 4) {
                        pos = new Vector3(45f, Random.Range(-30f, 30f), z);
                        gotNewVel = false;
                    }
                }

                //Updating position and velocity.
                rig.velocity = vel;
                transform.position = pos;
            }
            else if (p == Phase.Phase4) {
                //Keeping position and velocity locked at zero.
                transform.position = Vector3.zero;
                rig.velocity = Vector2.zero;

                //Vektor says a thing to keep the player distracted from Cotton's large appear time.
                if(vektorText != null) {
                    vektorText.SetActive(true);
                }

                //Total time it takes to do an attack.
                totalMoveTime = (2f + 1f + 1.5f + 2f) + 1.1f;
                
                //Constantly goes down, when it reaches 0, it picks a new target to attack from.
                //The counter should be long enough to wait for one swipe attack to finish.
                totalForm4Counter -= Time.deltaTime;
                if (totalForm4Counter <= 0){
                   
                    attackedForm4 = true;

                    arrow.GetComponent<LookAtScript>().enabled = true;

                    if (form4Attack < 3) { form4Attack++; } else { form4Attack = 0; }
                    float z = 0;
                    if(form4Attack == 0) { startPos = new Vector3(-90, 0, z); midPos = new Vector3(-75, 0, z); targetPos = new Vector3(-24, 0, z); facePos = new Vector3(0.45f, 0f, -2f); } //Left
                    else if(form4Attack == 1) { startPos = new Vector3(90, 0, z); midPos = new Vector3(75, 0, z); targetPos = new Vector3(24, 0, z); facePos = new Vector3(-0.45f, 0f, -2f); } //Right
                    else if(form4Attack == 2) { startPos = new Vector3(0, 73, z); midPos = new Vector3(0, 62, z); targetPos = new Vector3(0, 37, z); facePos = new Vector3(0f, -0.47f, -2f); } //Up
                    else if(form4Attack == 3) { startPos = new Vector3(0, -73, z); midPos = new Vector3(0, -62, z); targetPos = new Vector3(0, -37, z); facePos = new Vector3(0f, 0.47f, -2f); } //Down
                    form4Face.transform.localPosition = facePos;
                    form4Body.transform.localPosition = startPos;

                    //Do the tweens!
                    iTween.MoveTo(form4Body, iTween.Hash("x", midPos.x, "y", midPos.y, "z", midPos.z, "time", form4ToStartTime, "looptype", iTween.LoopType.none, "easeType", iTween.EaseType.easeInOutSine));
                    iTween.MoveTo(form4Body, iTween.Hash("x", targetPos.x, "y", targetPos.y, "z", targetPos.z, "time", form4SwipeTime, "delay", (form4ToStartTime + 1.5f), "looptype", iTween.LoopType.none, "easeType", iTween.EaseType.easeOutBounce));
                    iTween.MoveTo(form4Body, iTween.Hash("x", startPos.x, "y", startPos.y, "z", startPos.z, "time", form4BackTime, "delay", (form4ToStartTime + 1.5f + form4SwipeTime), "looptype", iTween.LoopType.none, "easeType", iTween.EaseType.easeInOutSine));

                    //Enable the form holder and allow for box management.
                    form4Holder.SetActive(true);
                    allowForm4HitboxManagement = true;
                    totalForm4Counter = totalMoveTime;
                }
                
                //Manage the outer walls!
                if(form4Attack == 1 || attackedForm4) { leftWallPos = new Vector3(0f, 0f, 10f); } else { leftWallPos = new Vector3(-20f, 0f, 10f); }
                if(form4Attack == 0 || attackedForm4) { rightWallPos = new Vector3(0f, 0f, 10f); } else { rightWallPos = new Vector3(20f, 0f, 10f); }
                if(form4Attack == 3 || attackedForm4) { upWallPos = new Vector3(0f, 0f, 10f); } else { upWallPos = new Vector3(0f, 20f, 10f); }
                if(form4Attack == 2 || attackedForm4) { downWallPos =new Vector3(0f, 0f, 10f); } else { downWallPos = new Vector3(0f, -20f, 10f); }
                float wallSmooth = 2f;
                leftWall.transform.position = Vector3.Lerp(leftWall.transform.position, leftWallPos, wallSmooth*Time.deltaTime);
                rightWall.transform.position = Vector3.Lerp(rightWall.transform.position, rightWallPos, wallSmooth*Time.deltaTime);
                upWall.transform.position = Vector3.Lerp(upWall.transform.position, upWallPos, wallSmooth*Time.deltaTime);
                downWall.transform.position = Vector3.Lerp(downWall.transform.position, downWallPos, wallSmooth*Time.deltaTime);

                //Play laugh sound if it hasn't played already.
                if(!playedLaughSnd){
                	PlaySound.NoLoop(laughSnd);
            		playedLaughSnd = true;
            	}

                //Managing the hitbox for Phase 4.
                hitBox.enabled = allowForm4HitboxManagement;
                if (allowForm4HitboxManagement) {
                    hitBox.offset = form4Body.transform.localPosition;
                    hitBox.size = form4Body.transform.localScale;
                }

                //Puppet color.
                puppetToColor = form4Body;

                //Color the poofies.
                ParticleSystemRenderer dr = downPoof.GetComponent<ParticleSystemRenderer>();
                ParticleSystemRenderer ur = upPoof.GetComponent<ParticleSystemRenderer>();
                ParticleSystemRenderer lr = leftPoof.GetComponent<ParticleSystemRenderer>();
                ParticleSystemRenderer rr = rightPoof.GetComponent<ParticleSystemRenderer>();
                dr.material.color = puppetToColor.GetComponent<MeshRenderer>().material.color;
                ur.material.color = puppetToColor.GetComponent<MeshRenderer>().material.color;
                lr.material.color = puppetToColor.GetComponent<MeshRenderer>().material.color;
                rr.material.color = puppetToColor.GetComponent<MeshRenderer>().material.color;
            }
        } else if(p == Phase.Death) {
            if(form4Holder!= null){ Destroy(form4Holder); }
            hitBox.enabled = false;
            victoryWaitTime -= Time.deltaTime;
            if(victoryWaitTime < 0f) {
                Game.manager.GetComponent<GameManager>().victory = true;
            }
        }
    }

    //Getting hit by a bullet!
    void OnTriggerEnter2D(Collider2D playerBullet){
        if (playerBullet.gameObject.tag == "Bullet"){
            playerBullet.gameObject.GetComponent<BulletScript>().PoofMe();
            if (health >= 1){
                PlaySound.Damage();
                health--;
                hitCounter = hitTime;
            } else {
                Die();
            }
        }

        if(playerBullet.gameObject.tag == "Player Laser") {
            switch (p) {
                case Phase.Phase1:
                    GoPhaseTwo();
                    break;
                case Phase.Phase2:
                    health = 149;
                    p = Phase.Transition1;
                    break;
                case Phase.Phase3:
                    health = 50;
                    break;
                case Phase.Phase4:
                    Die();
                    break;
            }
        }
    }

    private void GoPhaseTwo() {
        health = 249;
        rainObj.GetComponent<ParticleSystem>().Play();
        rainObj.transform.parent = null;
        p = Phase.Phase2;
    }

    private void Die() {
        if (!madeCoins){
            Instantiate(attacks[2], Vector3.zero, Quaternion.identity);
            PlaySound.NoLoop(killSound);
            for (int i = 0; i < moneys; i++){
                Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                Instantiate(coin, transform.position, rot);
            }
            iTween.ScaleTo(wallHolder, iTween.Hash("x", 2, "y", 2, "z", 1, "time", 5f, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.none));
            Destroy(wallHolder, 5f);
            madeCoins = true;
        }      
        p = Phase.Death;
        dead = true;
    }
}
