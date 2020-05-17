using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VektorBossScript : MonoBehaviour {

	//Which phase the "boss" is on.
	public enum Phase {
		Intro,
		GunPhase,
		DestroyedGunPhase,
		BrokeShield,
        Hop,
        PostHop,
        FlyAway,
	}
	public Phase ph = Phase.Intro;

	//Ints.
	int introTextIndex = 0;
	int breakShieldTextIndex = 0;
    int prehopIndex = 0;
	int noFloppyTextIndex = 0;
	int floppyTextIndex = 0;
	int crystalHealth = 35;
    int exploRateRounded = 0;

	//Floats.
	[HideInInspector] public float timeTillDeath = 45f;
	float crystalHurtCounter = 0f;
    float bezierTime = 0f;
    float flyCounter = 1.2f;
    float endCounter = 0.8f;
    float exploRate = 0f;
	readonly float crystalHurtTime = 0.2f;

	//Bools
	bool playedLaserSound = false;
	bool grabbedPlayerPosition = false;
    bool madeEscapeSphere = false;

	//Strings.
	string numToDisplay = "";

	//GameObjects.
	GameObject puppetHolder = null;
	GameObject faceObj = null;
	GameObject crystalHolderObj = null;
	GameObject crystalPuppetObj = null;
	GameObject rotatingPinwheel = null;
	GameObject stillPinwheel = null;
	GameObject introTextHolder = null;
	GameObject shieldBreakTextHolder = null;
    GameObject prehopTextHolder = null;
	GameObject noFloppyTextHolder = null;
	GameObject floppyTextHolder = null;
	GameObject timerObj = null;
	GameObject gunHolder = null;
	GameObject musicObj = null;
	GameObject crystalCrack = null;
	GameObject crystalExplo = null;
    GameObject vektorPickupEffect = null;
    GameObject vektorRejectEffect = null;
    GameObject escapeSphere = null;
    GameObject explosionParticlesObj = null;
    GameObject selfDestructSoundObj = null;
	List<GameObject> introTextBatches = new List<GameObject>();
	List<GameObject> breakShieldTextBatches = new List<GameObject>();
    List<GameObject> prehopTextBatches = new List<GameObject>();
	List<GameObject> noFloppyTextBatches = new List<GameObject>();
	List<GameObject> floppyTextBatches = new List<GameObject>();
	List<GameObject> guns = new List<GameObject>();

	//Colliders.
	Collider2D crystalCollider = null;

	//Colors!
	Color crackTargetColor = Color.clear;
	Color currentCrackColor = Color.clear;

	//Script components
	AnimationScript anim = null;

	//Audioclips.
	AudioClip glassBreakSnd = null;
	AudioClip laserSnd = null;
    AudioClip rejectSnd = null;
    AudioClip acceptSnd = null;
    AudioClip flyAwaySnd = null;

	//Find all needed gameObjects in Awake.
	void Awake(){
		//Puppets.
		puppetHolder = transform.Find("Puppet Holder").gameObject;
		faceObj = puppetHolder.transform.Find("Face").gameObject;
		crystalHolderObj = puppetHolder.transform.Find("Crystal").gameObject;
		crystalPuppetObj = crystalHolderObj.transform.Find("puppet").gameObject;
		rotatingPinwheel = puppetHolder.transform.Find("Rotating Pinwheel Holder").gameObject;
		stillPinwheel = puppetHolder.transform.Find("Still Pinwheel Holder").gameObject;
		timerObj = transform.Find("Time Left").gameObject;
		musicObj = transform.Find("Vektor Music").gameObject;
        selfDestructSoundObj = transform.Find("Self Destruct Sound").gameObject;
		crystalCrack = puppetHolder.transform.Find("Crystal Crack").gameObject;
        explosionParticlesObj = transform.Find("Explosion Particles").gameObject;
		crystalExplo = Resources.Load("Crystal Explosion Particles") as GameObject;
        vektorPickupEffect = Resources.Load("Vector Pickup Effect") as GameObject;
        vektorRejectEffect = Resources.Load("Vector Reject Effect") as GameObject;
        escapeSphere = Resources.Load("Escape Sphere") as GameObject;
        

		//Find script components.
		anim = faceObj.GetComponent<AnimationScript>();

		//object batch holders.
		introTextHolder = transform.Find("Intro Text Holder").gameObject;
		noFloppyTextHolder = transform.Find("No Floppy Text Holder").gameObject;
        prehopTextHolder = transform.Find("Prehop Text Holder").gameObject;
		floppyTextHolder = transform.Find("Floppy Text Holder").gameObject;
		shieldBreakTextHolder = transform.Find("Break Shield Text Holder").gameObject;
		gunHolder = transform.Find("Gun Holder").gameObject;

		//Populate the object batch lists.
		foreach (Transform ta in introTextHolder.gameObject.transform){ 
			introTextBatches.Add(ta.gameObject);
			ta.gameObject.SetActive(false);
		}
		foreach (Transform tb in noFloppyTextHolder.gameObject.transform){ 
			noFloppyTextBatches.Add(tb.gameObject);
			tb.gameObject.SetActive(false);
		}
		foreach (Transform tc in floppyTextHolder.gameObject.transform){ 
			floppyTextBatches.Add(tc.gameObject);
			tc.gameObject.SetActive(false);
		}
		foreach (Transform td in gunHolder.gameObject.transform){
			guns.Add(td.gameObject);
			VektorGunScript vgs = td.gameObject.GetComponent<VektorGunScript>();
			vgs.vbs = this;
		}
        foreach (Transform te in prehopTextHolder.gameObject.transform){ 
			prehopTextBatches.Add(te.gameObject);
			te.gameObject.SetActive(false);
		}
        foreach (Transform tf in shieldBreakTextHolder.gameObject.transform){ 
			breakShieldTextBatches.Add(tf.gameObject);
			tf.gameObject.SetActive(false);
		}

		//Find collider.
		crystalCollider = gameObject.GetComponent<Collider2D>();
		crystalCollider.enabled = false;

		//Load sounds.
		glassBreakSnd = Resources.Load("SFX/Glass Break") as AudioClip;
		laserSnd = Resources.Load("SFX/Vektor Big Laser") as AudioClip;
        rejectSnd = Resources.Load("SFX/Metal Clink") as AudioClip;
        acceptSnd = Resources.Load("SFX/Bullet Wipe Sound") as AudioClip;
        flyAwaySnd = Resources.Load("SFX/flyaway") as AudioClip;
	}
    
	//Basic setup stuff happens on start. (Moving things, starting off at the right position, color, etc.)
	void Start(){
		timerObj.transform.position = new Vector3(0, 25f, -3f);
		crystalCrack.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
        explosionParticlesObj.SetActive(false);
        selfDestructSoundObj.GetComponent<AudioSource>().volume = 0f;
	}

	void Update(){
        //Manage Vektor!
		if (Game.manager.GetComponent<GameManager>().splashCounter <= 0){
			VektorPhaseManagement();
		}

        //Activate the explosion particles and play the explosion sound.
        if (Game.brokeCore) {
            if(!explosionParticlesObj.GetComponent<ParticleSystem>().isPlaying) {
                explosionParticlesObj.GetComponent<ParticleSystem>().Play();
            }
            ParticleSystem.EmissionModule epm = explosionParticlesObj.GetComponent<ParticleSystem>().emission;
            exploRate += Time.deltaTime/1.5f;
            exploRateRounded = Mathf.RoundToInt(exploRate);
            epm.rateOverTime = exploRateRounded;

            //Raising volume of self destruct sound.
            if(selfDestructSoundObj.GetComponent<AudioSource>().volume < Game.soundVolume) {
                selfDestructSoundObj.GetComponent<AudioSource>().volume += Time.deltaTime/4f;
            }
            if(selfDestructSoundObj.GetComponent<AudioSource>().volume > Game.soundVolume) {
                selfDestructSoundObj.GetComponent<AudioSource>().volume = Game.soundVolume;
            }
        }

		//Color the crystal.
		float crystalAlpha = 0.333f;
		crystalPuppetObj.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, crystalAlpha);
		
		//Color the face.
		faceObj.GetComponent<MeshRenderer>().material.color = Game.frontColor;		
	}

	private void VektorPhaseManagement(){

		//Intro text scrolls through during both the intro and gun phase.
		if(ph == Phase.Intro || ph == Phase.GunPhase){
			//If the current text box has run it's course, increase the index by 1.
			if(introTextIndex < introTextBatches.Count){
				if(introTextBatches[introTextIndex] == null){
					introTextIndex++;
				}					
			}

			//Enable all text batches in order based on index.
			for(int i = 0; i < introTextBatches.Count; i++){
				if(i <= introTextIndex){
					if(introTextBatches[i] != null){ introTextBatches[i].SetActive(true); }
				} else {
					if(introTextBatches[i] != null){ introTextBatches[i].SetActive(false); }
				}
			}
		}



		switch(ph){//What to do during each phase.
			case Phase.Intro: //Intro phase.
				//animate happy face when at dialogue index 0.
				if(introTextIndex == 0){
					anim.framesOffset[0] = Vector2.zero;
					anim.framesOffset[1] = new Vector2(0.1666666f, 0f);	
				} else {
					anim.framesOffset[0] = new Vector2(0.3333334f, 0f);
					anim.framesOffset[1] = new Vector2(0.5f, 0f);
				}

				if(introTextBatches[2] == null){
					ph = Phase.GunPhase;
				}
				break;
			case Phase.GunPhase:
				//Move timer to proper pos.
				Vector3 timerPos = timerObj.transform.position;
				float smooth = 10f;
				timerPos = Vector3.Lerp(timerPos, new Vector3(0, 14.5f, -3f), smooth*Time.deltaTime);
				timerObj.transform.position = timerPos;

				//Manage the countdown!
				timeTillDeath -= Time.deltaTime;
				if(timeTillDeath > 0){ numToDisplay = Mathf.RoundToInt(timeTillDeath).ToString(); }
				else { numToDisplay = "0"; }
				timerObj.GetComponent<TextMesh>().text = numToDisplay;

				//What happens when guns go off.
				if(!playedLaserSound && timeTillDeath < 1f){
					PlaySound.NoLoop(laserSnd);
					playedLaserSound = true;
				}
				
				//Play the music.
				if(!musicObj.GetComponent<AudioSource>().isPlaying){ musicObj.GetComponent<AudioSource>().Play(); }
				musicObj.GetComponent<AudioSource>().volume = Game.musicVolume;

				//Check when all guns are missing or dead.
				if(gunHolder.transform.childCount == 0){
					//Crystal collider is enabled.
					crystalCollider.enabled = true;
					ph = Phase.DestroyedGunPhase;
				}
				break;
			case Phase.DestroyedGunPhase:
				//When to set the intro text holder active.
				introTextHolder.SetActive(false);
				for(int i = 0; i < introTextBatches.Count; i++){
					if(introTextBatches[i] != null){
						Destroy(introTextBatches[i]);
					}
				}

				//Make the :o face
				anim.framesOffset[0] = new Vector2(0.8333335f, 0f);	
				anim.framesOffset[1] = new Vector2(0.8333335f, 0f);	

				//Fade out the boss music.
				if(musicObj.GetComponent<AudioSource>().volume > 0){ musicObj.GetComponent<AudioSource>().volume -= Time.deltaTime; }
				if(musicObj.GetComponent<AudioSource>().volume < 0){ musicObj.GetComponent<AudioSource>().volume = 0f; }

				//Move timer to proper pos.
				Vector3 timerPos2 = timerObj.transform.position;
				float smooth2 = 10f;
				timerPos2 = Vector3.Lerp(timerPos2, new Vector3(0, 25f, -3f), smooth2*Time.deltaTime);
				timerObj.transform.position = timerPos2;

				//If the current text box has run it's course, increase the index by 1.
				if(breakShieldTextIndex < breakShieldTextBatches.Count){
					if(breakShieldTextBatches[breakShieldTextIndex] == null){
						breakShieldTextIndex++;
					}
				}

				//Color the crystal crack.
				if(crystalHurtCounter <= 0){
					crackTargetColor = Color.clear;
				} else {
					crystalHurtCounter -= Time.deltaTime;
					crackTargetColor = Color.white;
				}
				float colorSmooth = 5f;
				currentCrackColor = Color.Lerp(currentCrackColor, crackTargetColor, colorSmooth * Time.deltaTime);
				crystalCrack.GetComponent<MeshRenderer>().material.color = currentCrackColor;

				//Enable all text batches in order based on index.
				for(int i = 0; i < breakShieldTextBatches.Count; i++){
					if(i <= breakShieldTextIndex){
						if(breakShieldTextBatches[i] != null){ breakShieldTextBatches[i].SetActive(true); }
					} else {
						if(breakShieldTextBatches[i] != null){ breakShieldTextBatches[i].SetActive(false); }
					}
				}
				break;
            case Phase.BrokeShield: //Phase for after you have broken the shield...
                
                //Tell the game you broke the core.
                Game.brokeCore = true;
                
                //Start up the explosion particles.
                explosionParticlesObj.SetActive(true);


                //Make those pinwheels be gone!
                Vector3 rotatingPinwheelScale = rotatingPinwheel.transform.localScale;
                float toSmallSpeed = 4f;
                rotatingPinwheelScale = Vector3.Lerp(rotatingPinwheelScale, Vector3.zero, toSmallSpeed*Time.deltaTime);
                rotatingPinwheel.transform.localScale = rotatingPinwheelScale;
                stillPinwheel.transform.localScale = rotatingPinwheelScale;

                //Destroy all shield text if it didn't finish going through. (To prevent overlapping text.)
				shieldBreakTextHolder.SetActive(false);
				for(int i = 0; i < breakShieldTextBatches.Count; i++){
					if(breakShieldTextBatches[i] != null){
						Destroy(breakShieldTextBatches[i]);
					}
				}

                //Animate the face.
                anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.1666666f, 0f);
          
                //If the current text box has run it's course, increase the index by 1.
				if(prehopIndex < prehopTextBatches.Count){
					if(prehopTextBatches[prehopIndex] == null){
						prehopIndex++;
					}
				}

                //Enable all text batches in order based on index.
				for(int i = 0; i < prehopTextBatches.Count; i++){
					if(i <= prehopIndex){
						if(prehopTextBatches[i] != null){ prehopTextBatches[i].SetActive(true); }
					} else {
						if(prehopTextBatches[i] != null){ prehopTextBatches[i].SetActive(false); }
					}
				}

                if(prehopTextBatches[2] == null){
					ph = Phase.Hop;
				}
                break;
            case Phase.Hop:

                //Bezier move stuff.
                Vector3 facePos = faceObj.transform.position;
                Vector3 playerPos = Game.playerObj.transform.position;
                float StartPointX = 0f;
	            float StartPointY = 1f;
	            float ControlPointX = 20f;
	            float ControlPointY = 50f;
	            float EndPointX	= 0f;
	            float EndPointY = 0f;
	            float CurveX = 0f;
	            float CurveY = 0f;
	            

                if (!grabbedPlayerPosition) {
                    Vector3 dif = facePos - playerPos;
		            ControlPointX = dif.x;
		            ControlPointY = dif.y;
                    grabbedPlayerPosition = true;
                }

                EndPointX = playerPos.x;
			    EndPointY = playerPos.y;
			
			    bezierTime += Time.deltaTime;

                CurveX = (((1-bezierTime)*(1-bezierTime)) * StartPointX) + (2 * bezierTime * (1 - bezierTime) * ControlPointX) + ((bezierTime * bezierTime) * EndPointX);
			    CurveY = (((1-bezierTime)*(1-bezierTime)) * StartPointY) + (2 * bezierTime * (1 - bezierTime) * ControlPointY) + ((bezierTime * bezierTime) * EndPointY);
			    
                faceObj.transform.position = Vector3.Lerp(faceObj.transform.position, new Vector3(CurveX, CurveY, 0), 7f *Time.deltaTime);
			
			    if(bezierTime >= 1.11f){ //when vektor gets to the player.
                    if (Game.ownsFloppyDisk) { //If the player has the floppy disk... vektor gets inside.
                        Game.CheckAchievements(31);
                        faceObj.SetActive(false);
                        PlaySound.NoLoop(acceptSnd);
                        Instantiate(vektorPickupEffect, Game.playerObj.transform.position, Quaternion.identity);
                    } else { //If the player does not have the floppy disk, reject Vektor!
                        Instantiate(vektorRejectEffect, Game.playerObj.transform.position, Quaternion.identity);
                        PlaySound.NoLoop(rejectSnd);
                    }
                    ph = Phase.PostHop;
                }
                break;
            case Phase.PostHop:
                //Move face back to center. (It won't be visible if he was accepted.)
                Vector3 pos = faceObj.transform.position;
                float smoothee = 3f;
                pos = Vector3.Lerp(pos, new Vector3(0, 1, 5), smoothee*Time.deltaTime);
                faceObj.transform.position = pos;

                //Run through the remaining dialogue.
                if (Game.ownsFloppyDisk) { //what to do when the player has the floppy disk.
                    //If the current text box has run it's course, increase the index by 1.
				    if(floppyTextIndex < floppyTextBatches.Count){
					    if(floppyTextBatches[floppyTextIndex] == null){
						    floppyTextIndex++;
					    }
				    }

                    //Enable all text batches in order based on index.
				    for(int i = 0; i < floppyTextBatches.Count; i++){
					    if(i <= floppyTextIndex){
						    if(floppyTextBatches[i] != null){ floppyTextBatches[i].SetActive(true); }
					    } else {
						    if(floppyTextBatches[i] != null){ floppyTextBatches[i].SetActive(false); }
					    }
				    }

                    //When to jump!
                    if(floppyTextBatches[4] == null) {
                        ph = Phase.FlyAway;
                    }
                } else { //What to do when the player does NOT have the floppy disk.
                    //If the current text box has run it's course, increase the index by 1.
				    if(noFloppyTextIndex < noFloppyTextBatches.Count){
					    if(noFloppyTextBatches[noFloppyTextIndex] == null){
						    noFloppyTextIndex++;
					    }
				    }

                    //Enable all text batches in order based on index.
				    for(int i = 0; i < noFloppyTextBatches.Count; i++){
					    if(i <= noFloppyTextIndex){
						    if(noFloppyTextBatches[i] != null){ noFloppyTextBatches[i].SetActive(true); }
					    } else {
						    if(noFloppyTextBatches[i] != null){ noFloppyTextBatches[i].SetActive(false); }
					    }
				    }
                    if(noFloppyTextBatches[8] == null) {
                      
                        ph = Phase.FlyAway;
                    }
                }
                break;
            case Phase.FlyAway:
                Game.savedVektor = Game.ownsFloppyDisk;//You have saved vektor if you fly away while owning the floppy disk.
                
                if(flyCounter > 0) {
                    if (Game.savedVektor) {
                        Game.CheckAchievements(31);
                    }
                    flyCounter -= Time.deltaTime;
                } else {
                    if (!madeEscapeSphere) { 
                        PlaySound.NoLoop(flyAwaySnd);
                        Instantiate(escapeSphere, Game.playerObj.transform.position, Quaternion.identity);
                        Game.playerObj.SetActive(false);
                        madeEscapeSphere = true;
                    }
                    //Make the sad face
				    anim.framesOffset[0] = new Vector2(0.6666668f, 0f);	
				    anim.framesOffset[1] = new Vector2(0.6666668f, 0f);	

                    //End this.
                    endCounter -= Time.deltaTime;
                    if(endCounter <= 0) {
                        Game.manager.GetComponent<GameManager>().victory = true;
                    }
                }

                break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(crystalHealth < 1){
			Instantiate(crystalExplo, new Vector3(0, 0, 40), Quaternion.identity);
			PlaySound.NoLoop(glassBreakSnd);
			crystalCrack.SetActive(false);
			crystalHolderObj.SetActive(false);
			crystalCollider.enabled = false;
			ph = Phase.BrokeShield;
		} else {
			PlaySound.Damage();
			crystalHurtCounter = crystalHurtTime;
			crystalHealth--;
		}
	}
}