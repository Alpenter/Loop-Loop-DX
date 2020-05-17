using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WardenScript : MonoBehaviour {

    public enum Phase {
        IntroPhase,
        ColorTransition,
        PhaseOneRed,
        PhaseTwoYellow,
        PhaseThreeGreen,
        PhaseFourBlue,
        PhaseFivePurple,
        TransitionToFinalPhase,
        FinalPhase,
        Death,
    }
    Phase currentPhase = Phase.IntroPhase;
    
    public enum SwitchType {
        RedToYellow,
        YellowToGreen,
        GreenToBlue,
        BlueToPurple,
    }
    public SwitchType switchToColor = SwitchType.RedToYellow;
    
    //GameObjects.
    GameObject introHolder = null;
    GameObject savedVektorText = null;
    GameObject deadVektorText = null;
    GameObject wardenIntroText = null;
    GameObject prepareWardenText = null;
    GameObject wallBorderAppearanceEffect = null;
    GameObject formOneComingInParticles = null;
    GameObject formOneAppearParticles = null;
    GameObject formOneHolder = null;
    GameObject face = null;
    GameObject backCapeBatch = null;
    GameObject frontCapeBatch = null;
    GameObject crystalBatch = null;
    GameObject crystalRotateController = null;
    GameObject crystalBreakPrefab = null;
    GameObject formChangeParticles = null;
    GameObject yellowFormBulletPrefab = null;
    GameObject yellowBulletHolder = null;
    GameObject greenLightningPrefab = null;
    GameObject blueSuckParticles = null;
    GameObject bombPrefab = null;
    GameObject bombInst = null;
    GameObject bombExplo = null;
    GameObject blueFormShield = null;
    GameObject blueCenterCoverParticles = null;
    GameObject purpleCrystalIntroText = null;
    GameObject purpleCrystalHolder = null;
    GameObject purpleCrystalCrack = null;
    GameObject bigPurpleCrystalPrefab = null;
    GameObject bigPurpleCrystalBreakEffect = null;
    GameObject finalTransitionTextOne = null;
    GameObject finalFade = null;
    GameObject attackNotice = null;
    GameObject formTwoHolder = null;
    GameObject finalFlash = null;
    GameObject finalFacePuppet = null;
    GameObject finalHurtPuppet = null;
    GameObject frontArmIdlePuppet = null;
    GameObject frontArmAttackPuppet = null;
    GameObject spitPrefab = null;
    GameObject gearBullet = null;
    GameObject laserChargePrefab = null;
    GameObject wardenLaser = null;
    GameObject finalFormLoopBlockers = null;
    GameObject otherPurposeBullet = null;
    GameObject finalFormHealthBar = null;
    GameObject wardenFinalAttackHolder = null;
    GameObject finalExplo = null;
    GameObject[] blueFormWalls = new GameObject[7];
    List<GameObject> frontCapePuppets = new List<GameObject>();
    List<GameObject> backCapePuppets = new List<GameObject>();
    List<GameObject> crystals = new List<GameObject>();
    List<GameObject> crystalPuppets = new List<GameObject>();
    List<GameObject> crystalMatchPoints = new List<GameObject>();

    //Integers.
    int redHealth = 75;
    int yellowHealth = 85;
    int greenHealth = 90;
    int blueHealth = 3;
    int firstPurpleCrystalHealth = 70;
    int blueChunksSpawned = 0;
    int finalPhaseHealth = 700;
    int finalFormAttackIndex = 0; // 0 = Normal Spewing | 1 = gear spewing | 2 = Laser Hand |  
    readonly int maxAttack = 3; //4 possible attacks (0 to 3)

    //Floats.
    float wardenAppearCounter = 6f;
    float currentRedFormMovementSpeed = 0f;
    float hurtCounter = 0f;
    float yellowBulletShootCounter = 1.5f;
    float colorTransitionCounter = 5f;
    float greenCrystalMoveCounter = 0.5f;
    float greenCrystalHoldCounter = 2.1f;
    float blueCrystalYtargetPos = 0f;
    float suckForce = 0f;
	float blueChunkSpawnCounter = 0f;
    float finalTransitionBandaidCounter = 1f;
    float finalPhaseHurtCounter = 0f;
    float spitHoldCounter = 0f;
    float finalPhaseAttackCounter = 0f;
    float spitSpawnCounter = 0f;
    float gearSpawnCounter = 0f;
    float laserChargeCounter = 0f;
    float laserFireCounter = 0f;
    float loopBlockAttackStartCounter = 0f;
    float loopBlockAttackSpewCounter = 0f;
    float endCounter = 10f;
    readonly float loopBlockAttackSpewTime = 6f;
    readonly float loopBlockAttackStartTime = 3.5f;
    readonly float laserChargeTime = 2.5f;
    readonly float laserFireTime = 5f;
    readonly float gearSpawnTime = 0.8f;
    readonly float spitSpawnTime = 0.15f;
    readonly float finalPhaseAttackTime = 3f;
    readonly float spitHoldTime = 3.5f;
    readonly float finalPhaseHurtTime = 0.2f;
    readonly float finalTransitionBandaidTime = 1f;
    readonly float chunklifeTime = 7f;
    readonly float blueChunkSpawnTime = 2f;
    readonly float targetSuckForce = 2f;
    readonly float greenCrystalMoveTime = 0.5f;
    readonly float greenCrystalHoldTime = 2.1f;
    readonly float colorTransitionTime = 5f;
    readonly float wardenAppearMusicTime = 17.57f;
    readonly float hurtTime = 0.1f;
    readonly float yellowBulletShootTime = 0.65f;
    readonly float finalFadeLifeTime = 0.25f;

    //Bools.
    bool wardenIntroSpeaking = false;
    bool madeWardenAppearSound = false;
    bool madeWallBorderAppearEffect = false;
    bool canColorTransition = false;
    bool releasedCrystalsFromRotate = false;
    bool tweenedGreenCrystals = false;
    bool madeGreenLightning = false;
    bool gotNewVel = false;
    bool tweenedBlueCrystals = false;
    bool tweenedPurpleCrystal = false;
    bool setFinalMusic = false;
    bool playedFlash = false;
    bool switchedFinalPhaseAttack = false;
    bool madeLaserChargePrefab = false;
    bool madeWardenLaserPrefab = false;
    bool movedLoopBlockerOffScreen = false;
    bool movedLoopBlockerOnScreen = false;
    bool playedLaserchargeSound = false;
    bool playedLaserFireSound = false;
    bool destroyedFinalAttacks = false;
    bool checkedAchievos = false;

    //Vectors.
    Vector2 faceFrame = Vector2.zero;
    Vector2 vel = Vector2.zero;
    Vector2 newVel = Vector2.zero;
    Vector3 chunkSpawnPlace = Vector3.zero; //Place blue chunk walls spawn.
    Vector3 chunkRotVec = Vector3.zero; //Rotation vector of blue chunk walls.

    //Quaternions.
    Quaternion chunkRot = Quaternion.identity; //Rotation quaternion of blue chunk walls.

    //Colors.
    Color hurtColor = Color.white;
    Color colorPhaseSwitchFrom = Color.red;
    Color colorPhaseSwitchTo = Color.yellow;   
    readonly Color32 blueColor = new Color(100, 100, 255, 255);

    //Colliders.
    PolygonCollider2D formOneCollider = null;
    CapsuleCollider2D firstPurpleCrystalCollider = null;

    //Trail Renderers
    TrailRenderer blueCrystalTrail = null;
    TrailRenderer purpleCrystalTrail = null;

    //Audio Sources and Clips.
    [HideInInspector] public AudioSource musicSource = null;
    AudioClip wardenAppearSound = null;
    AudioClip noLoopWallAppearSound = null;
    AudioClip crystalBreakSound = null;
    AudioClip yellowShootSound = null;
    AudioClip laserChargeSound = null;
    AudioClip laserFireSound = null;
    AudioClip wardenScreech = null;

    private void Awake() {
        //Set this script staticly.
        Game.warden = this;

        //You can loop at the start.
        Game.loopBlocked = false; 

        //Find/Load all needed intro phase gameObjects.
        introHolder = transform.Find("Intro Holder").gameObject;
        savedVektorText = introHolder.transform.Find("Saved Vektor Text Batch").gameObject;
        deadVektorText = introHolder.transform.Find("Dead Vektor Text Batch").gameObject;
        wardenIntroText = introHolder.transform.Find("Warden Text Batch").gameObject;
        prepareWardenText = introHolder.transform.Find("Prepare Warden Text Batch").gameObject;
        wallBorderAppearanceEffect = introHolder.transform.Find("Wall Border Appearance Effect").gameObject;
        formOneComingInParticles = introHolder.transform.Find("Coming In Particles").gameObject;
        formOneAppearParticles = introHolder.transform.Find("Appear Particles").gameObject;
        
        //Find/Load in all phase one gameObjects.
        formOneHolder = transform.Find("Form One Holder").gameObject;
        face = formOneHolder.transform.Find("face puppet").gameObject;
        backCapeBatch = formOneHolder.transform.Find("back cape batch").gameObject;
        frontCapeBatch = formOneHolder.transform.Find("front cape batch").gameObject;
        crystalBatch = formOneHolder.transform.Find("Crystal Batch").gameObject;
        crystalRotateController = formOneHolder.transform.Find("Crystal Rotate Controller").gameObject;
        crystalBreakPrefab = Resources.Load("Warden Crystal Explo") as GameObject;
        formChangeParticles = formOneHolder.transform.Find("Form Change Particles").gameObject;
        yellowFormBulletPrefab = Resources.Load("Crystal Flame Bullet") as GameObject;
        greenLightningPrefab = Resources.Load("Warden Lightning Attack") as GameObject;
        blueSuckParticles = formOneHolder.transform.Find("blue suck particles").gameObject;
        bombPrefab = Resources.Load("Crystal Blue Bomb") as GameObject;
        bombExplo = Resources.Load("Crystal Bomb Kablooey") as GameObject;
        blueCenterCoverParticles = formOneHolder.transform.Find("Blue Center Cover Particles").gameObject;
        blueFormShield = formOneHolder.transform.Find("blue form shield").gameObject;
        purpleCrystalIntroText = formOneHolder.transform.Find("Purple Crystal Intro Text").gameObject;
        purpleCrystalCrack = formOneHolder.transform.Find("Purple Crystal Crack").gameObject;
        bigPurpleCrystalPrefab = Resources.Load("Warden Purple Stuff/Purple Crystal Big") as GameObject;
        attackNotice = formOneHolder.transform.Find("attack notice").gameObject;
        bigPurpleCrystalBreakEffect = Resources.Load("Warden Purple Stuff/Big Purple Crystal Explosion Particles") as GameObject;
        finalTransitionTextOne = gameObject.transform.Find("Warden Final Transition Text").gameObject;
        finalFade = gameObject.transform.Find("Final Fade").gameObject;
        finalFlash = gameObject.transform.Find("Final Flash").gameObject;
        foreach(Transform child in frontCapeBatch.transform) {
            frontCapePuppets.Add(child.gameObject);
        }
        foreach(Transform child in backCapeBatch.transform) {
            backCapePuppets.Add(child.gameObject);
        }
        foreach(Transform child in crystalBatch.transform) {
            GameObject crystal = child.gameObject;
            crystals.Add(crystal);
            GameObject p = crystal.transform.Find("puppet").gameObject;
            crystalPuppets.Add(p);
        }
        foreach(Transform child in crystalRotateController.transform) {
            crystalMatchPoints.Add(child.gameObject);
        }
        for(int i = 0; i < blueFormWalls.Length; i++) {
            blueFormWalls[i] = Resources.Load("Warden Blue Walls/Warden Wall Group " + i.ToString()) as GameObject;
        }
        
        //Find form two holder stuff.
        formTwoHolder = transform.Find("Form Two Holder").gameObject;
        finalFacePuppet = formTwoHolder.transform.Find("Face Puppet").gameObject;
        finalHurtPuppet = formTwoHolder.transform.Find("Hurt Puppet").gameObject;
        frontArmIdlePuppet = formTwoHolder.transform.Find("Front Arm Idle Puppet").gameObject;
        frontArmAttackPuppet = formTwoHolder.transform.Find("Front Arm Attacking Puppet").gameObject;
        spitPrefab = Resources.Load("Purpose Bullet") as GameObject;
        gearBullet = Resources.Load("Gear Bullet") as GameObject;
        laserChargePrefab = Resources.Load("Warden Laser Charge Batch") as GameObject;
        wardenLaser = Resources.Load("Warden Laser") as GameObject;
        finalFormLoopBlockers = formTwoHolder.transform.Find("Loop Blockers").gameObject;
        otherPurposeBullet = Resources.Load("Other Purpose Bullet") as GameObject;
        finalFormHealthBar = formTwoHolder.transform.Find("Warden Health Bar").gameObject;
        finalExplo = formTwoHolder.transform.Find("Final Explo").gameObject;

        //Get the colliders.
        formOneCollider = GetComponent<PolygonCollider2D>();
        formOneCollider.enabled = false;
        firstPurpleCrystalCollider = GetComponent<CapsuleCollider2D>();
        firstPurpleCrystalCollider.enabled = false;

        //Get the trail renderers.
        blueCrystalTrail = crystals[3].GetComponent<TrailRenderer>();
        purpleCrystalTrail = crystals[4].GetComponent<TrailRenderer>();
        blueCrystalTrail.time = 0f;
        purpleCrystalTrail.time = 0f;
        blueCrystalTrail.enabled = false;
        purpleCrystalTrail.enabled = false;

        //Load sound effects.
        wardenAppearSound = Resources.Load("SFX/Bullet Wipe Sound") as AudioClip;
        noLoopWallAppearSound = Resources.Load("SFX/Bullet Wipe Sound") as AudioClip;
        crystalBreakSound = Resources.Load("SFX/Glass Break") as AudioClip;
        yellowShootSound = Resources.Load("SFX/Fire Shoot") as AudioClip;
        laserChargeSound = Resources.Load("SFX/Warden Laser Charge Sound") as AudioClip;
        laserFireSound = Resources.Load("SFX/Warden Big Laser") as AudioClip;
        wardenScreech = Resources.Load("SFX/Warden Screech") as AudioClip;
    }

    private void Start() {
        //No explo me at start.
        finalExplo.SetActive(false);

        //Find the music source.
        GameObject musicSourceObj = Game.manager.GetComponent<GameManager>().music;
        musicSource = musicSourceObj.GetComponent<AudioSource>();

        //Start purple crystal crack off correctly.
        purpleCrystalCrack.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
        purpleCrystalCrack.SetActive(false);

        //Create the yellow bullet holder and purple crystal holder.
        yellowBulletHolder = new GameObject() {
            name = "Yellow Bullet Holder"
        };
        purpleCrystalHolder = new GameObject() {
            name = "Purple Crystal Holder"
        };
        Game.wardenPurpleCrystalHolder = purpleCrystalHolder;
        wardenFinalAttackHolder = new GameObject() {
            name = "Warden Final Attack Holder"
        };

        //Set final form loop blockers too start off screen.
        finalFormLoopBlockers.transform.position = new Vector3(100, 0, 0);
    }

    private void Update() {
        
        //Lock the velocity of the form 2 rigidbody here to be 0... just because.
        if(formTwoHolder != null) { 
            formTwoHolder.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        

        //When the attack notice is active.
        attackNotice.SetActive(currentPhase == Phase.ColorTransition);

        //If you are at any phase other than the switching color phase, you can't switch colors.
        if(currentPhase != Phase.ColorTransition) {
            colorTransitionCounter = colorTransitionTime;
            canColorTransition = false; 
        }

        //Don't do the music stuff here if it is in the intro phase, the music is handled there already.
        if(currentPhase != Phase.IntroPhase && currentPhase != Phase.FinalPhase && currentPhase != Phase.TransitionToFinalPhase) { 
            if(musicSource.time > 74.1f) {
                musicSource.time = 35f;         
            }
        }

        //How things work based on the phase of the boss.
        switch (currentPhase) {
            case Phase.IntroPhase:
                //Turn off the blue form shield.
                blueFormShield.SetActive(false);

                //Velocity is zero.
                vel = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = vel;
                
                //The warden speaks after Carl or Vektor speaks OR if the player gets too close to the wall.
                if (!wardenIntroSpeaking) {    
                    //Which character should speak at the very start?
                    if (Game.savedVektor) { //If vektor is saved, vektor speaks.
                        if(savedVektorText != null){ savedVektorText.SetActive(true); }
                        if(deadVektorText != null){ Destroy(deadVektorText); } //Destroy unneeded text.
                    } else { //If vektor is dead, carl speaks.
                        if(deadVektorText != null){ deadVektorText.SetActive(true); }
                        if(savedVektorText != null){ Destroy(savedVektorText); } //Destroy unneeded text.
                    }

                    //If the player gets too close to the edge of the screen, this also skips the intro text.
                    Vector3 playerIntroPos = Game.playerObj.transform.position;
                    float xLim = 30f;
                    float yLim = 16f;
                    if(playerIntroPos.x > xLim) { wardenIntroSpeaking = true; }
                    if(playerIntroPos.y > yLim) { wardenIntroSpeaking = true; }
                    if(playerIntroPos.x < -xLim) { wardenIntroSpeaking = true; }
                    if(playerIntroPos.y < -yLim) { wardenIntroSpeaking = true; }

                    //If the player waits long enough, the warden speaks now.
                    if(musicSource.time >= wardenAppearMusicTime) { wardenIntroSpeaking = true; }
                } else { //If it is the wardens turn to talk...
                    //The first texts are DESTROYED!!!!
                    if(deadVektorText != null) { Destroy(deadVektorText); }
                    if(savedVektorText != null) { Destroy(savedVektorText); }

                    //Make that loop wall appear!
                    Game.loopBlocked = true;
                    if (!madeWallBorderAppearEffect && wallBorderAppearanceEffect != null) {
                        musicSource.time = wardenAppearMusicTime; //Set the music time.
                        PlaySound.NoLoop(noLoopWallAppearSound);
                        wallBorderAppearanceEffect.SetActive(true);
                        Destroy(wallBorderAppearanceEffect, 5f);
                        madeWallBorderAppearEffect = true;
                    }

                    //Set warden text to be active.
                    if(wardenIntroText != null){ 
                        wardenIntroText.SetActive(true); 
                    } else { //When the warden text is missing...
                        wardenAppearCounter -= Time.deltaTime; //Subtract the counter for how long it takes him to appear.
                        formOneComingInParticles.SetActive(true);
                        //If his appear counter is below zero, he has appeared!
                        if(wardenAppearCounter <= 0) {
                            if (!madeWardenAppearSound) {
                                PlaySound.NoLoop(wardenAppearSound);
                                madeWardenAppearSound = true;
                            }
                            AnimateFace(); //Turn the face puppet to look at the player.
                            formOneCollider.enabled = true; //Turn on the collider.
                            formOneAppearParticles.SetActive(true);
                            formOneHolder.SetActive(true);
                            if(prepareWardenText == null) {
                                currentPhase = Phase.PhaseOneRed;
                            } else {
                                prepareWardenText.SetActive(true);
                            }
                        }
                    }
                }
                break;
            case Phase.ColorTransition: //Phase that switches between colors during form one of the boss.
                //Velocity is zero.
                vel = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = vel;
                
                //Move to the center of the screen.
                Vector3 colorTransitionPos = transform.position;
                float toCenterSpeed = 7f;
                Vector3 currentPos = Vector3.MoveTowards(colorTransitionPos, Vector3.zero, toCenterSpeed*Time.deltaTime);
                transform.position = currentPos;
                
                //Animate the face.
                AnimateFace();
                
                //Keep the colliders disabled during transition. (So that the player can't be damaged unfairly when the boss changes.)
                formOneCollider.enabled = false;
                for(int i = 0; i < crystals.Count; i++) { 
                    if(crystals[i] != null) {
                        crystals[i].GetComponent<Collider2D>().enabled = false;
                    }
                }

                //Different behaviours for switching between the phases.
                switch (switchToColor) {
                    case SwitchType.RedToYellow:
                        //Set which colors to go from.
                        colorPhaseSwitchFrom = Color.red;
                        colorPhaseSwitchTo = Color.yellow;

                        //When the boss is close enough to the center, that is when you change colors.
                        crystalRotateController.transform.position = currentPos; //Crystals move to the center as well.
                        
                        //Scale the crystal rotate controller to one.
                        Vector3 crystalRotateControllerScale = crystalRotateController.transform.localScale;
                        crystalRotateControllerScale = Vector3.Lerp(crystalRotateControllerScale, Vector3.one, toCenterSpeed*Time.deltaTime);
                        crystalRotateController.transform.localScale = crystalRotateControllerScale;


                        //Reverse rotation of the rotate controller.
                        if(crystalRotateController.GetComponent<RotateScript>().rotationSpeed > -2){
                            crystalRotateController.GetComponent<RotateScript>().rotationSpeed -= Time.deltaTime*1.5f;
                        }
                        if (crystalRotateController.GetComponent<RotateScript>().rotationSpeed < -2) {
                            crystalRotateController.GetComponent<RotateScript>().rotationSpeed = -2;
                        }

                        //The color transitions when crystals get to the next rotation.
                        canColorTransition = crystalRotateController.GetComponent<RotateScript>().rotationSpeed <= -2;

                        //Move crystals to proper position.
                        Vector3 yellowCrystalTargetPosition = new Vector3(4, 4, 0);
                        Vector3 purpleCrystalTargetPosition = new Vector3(-4, 4, 0);
                        float crystalRepositionSmoothness = 5f;
                        crystalMatchPoints[1].transform.localPosition = Vector3.Lerp(crystalMatchPoints[1].transform.localPosition, yellowCrystalTargetPosition, crystalRepositionSmoothness*Time.deltaTime);
                        crystalMatchPoints[4].transform.localPosition = Vector3.Lerp(crystalMatchPoints[4].transform.localPosition, purpleCrystalTargetPosition, crystalRepositionSmoothness*Time.deltaTime);

                        //When the counter is all the way down, we go to the yellow phase from the red one.
                        if(colorTransitionCounter <= 0) {
                            currentPhase = Phase.PhaseTwoYellow;
                        }
                        break;
                    case SwitchType.YellowToGreen:
                        //Set which colors to go from.
                        colorPhaseSwitchFrom = Color.yellow;
                        colorPhaseSwitchTo = Color.green;
                        
                        //Crystal rotate controller goes to a speed of 0.
                        if(crystalRotateController.GetComponent<RotateScript>().rotationSpeed > 0){
                            crystalRotateController.GetComponent<RotateScript>().rotationSpeed -= Time.deltaTime*2;
                        }
                        if(crystalRotateController.GetComponent<RotateScript>().rotationSpeed < 0) {
                            crystalRotateController.GetComponent<RotateScript>().rotationSpeed = 0;
                        }

                        //When the crystal rotate controller has reached 0, unparent all the match points.
                        if(crystalRotateController.GetComponent<RotateScript>().rotationSpeed <= 0) {
                            for(int i = 0; i < crystalMatchPoints.Count; i++) {
                                crystalMatchPoints[i].transform.parent = null;
                            }
                            for(int j = 0; j < crystals.Count; j++) {
                                if(crystals[j] != null) { 
                                    crystals[j].transform.parent = null;
                                }
                            }
                            releasedCrystalsFromRotate = true;
                        }
                        //Colors can transition when all yellow bullets are gone.
                        canColorTransition = yellowBulletHolder.transform.childCount == 0 && releasedCrystalsFromRotate;

                        //When counter has gone to 0, go to green phase.
                        if(colorTransitionCounter <= 0) {
                            currentPhase = Phase.PhaseThreeGreen;
                        }
                        break;
                    case SwitchType.GreenToBlue:
                        //Set which colors to go from.
                        colorPhaseSwitchFrom = Color.green;
                        colorPhaseSwitchTo = new Color(1/2.5f, 1/2.5f, 1f, 1f);

                        //Reparrent match points to spin controller.
                        if (!tweenedBlueCrystals) {    
                            crystalRotateController.transform.parent = null;
                            crystalRotateController.transform.position = Vector2.zero;
                            for(int i = 0; i < crystalMatchPoints.Count; i++) {
                                crystalMatchPoints[i].transform.parent = crystalRotateController.transform;
                                if(i == 3) { blueCrystalYtargetPos = 5.5f; }
                                else if(i == 4) { blueCrystalYtargetPos = -5.5f; }
                                iTween.MoveTo(crystalMatchPoints[i], iTween.Hash(
                                    "x", 0f,
                                    "y", blueCrystalYtargetPos,
                                    "z", 0f,
                                    "time", colorTransitionCounter - 0.1f,
                                    "easetype", iTween.EaseType.easeInOutSine,
                                    "looptype", iTween.LoopType.none
                                ));
                            }
                            tweenedBlueCrystals = true;
                        }
                        
                        //Freeze the velocity back to zero.
                        vel = Vector2.zero;
                        GetComponent<Rigidbody2D>().velocity = vel;
                        
                        //Color transition is triggered when close enough to the center.
                        Vector2 posBler = new Vector2(transform.position.x, transform.position.y);
                        float dFromCenter = Vector2.Distance(Vector2.zero, posBler);
                        if(dFromCenter <= 0.5f) {
                            canColorTransition = true;
                        }

                        //When to go to blue phase!
                        if(colorTransitionCounter <= 0 && dFromCenter <= 0.25f) {
                            blueFormShield.SetActive(true); //Turn on the blue form shield at this point.
                            blueFormShield.transform.parent = null;
                            currentPhase = Phase.PhaseFourBlue;
                        }
                        break;
                    case SwitchType.BlueToPurple:
                        //Set which colors to go from.
                        colorPhaseSwitchFrom = new Color(1/2.5f, 1/2.5f, 1f, 1f);;
                        colorPhaseSwitchTo = Color.magenta;
                        
                        //Stop the suck particles.
                        if (blueSuckParticles.GetComponent<ParticleSystem>().isPlaying) {
                            blueSuckParticles.GetComponent<ParticleSystem>().Stop();
                        }

                        //Stop the center cover particles.
                        if (blueCenterCoverParticles.GetComponent<ParticleSystem>().isPlaying) {
                            blueCenterCoverParticles.GetComponent<ParticleSystem>().Stop();
                        }
                        
                        //Turn off the blue form shield at this point.
                        blueFormShield.GetComponent<CircleCollider2D>().enabled = false; 

                        //Freeze the velocity back to zero.
                        vel = Vector2.zero;
                        GetComponent<Rigidbody2D>().velocity = vel;

                        //Parent purple crystal.
                        crystals[4].transform.parent = purpleCrystalHolder.transform;

                        //Move the last crystal to the warden position.
                        if (!tweenedPurpleCrystal) {
                            iTween.MoveTo(crystalMatchPoints[4], iTween.Hash(
                                "x", 0, "y", 0, "z", 0,    
                                "time", 0.75f,
                                "easetype", iTween.EaseType.easeInOutSine,
                                "looptype", iTween.LoopType.none
                            ));
                            iTween.ScaleTo(crystals[4], iTween.Hash(
                                "x", 6, "y", 6, "z", 6,
                                "time", colorTransitionTime - 0.1f,
                                "easetype", iTween.EaseType.easeInOutSine,
                                "looptype", iTween.LoopType.none
                            ));
                            iTween.ColorTo(blueFormShield, iTween.Hash(
                                "a", 0f,
                                "time", colorTransitionTime - 3.5f,
                                "easetype", iTween.EaseType.easeInOutSine,
                                "looptype", iTween.LoopType.none
                            ));
                            iTween.ScaleTo(face, iTween.Hash(
                                "x", 0, "y", 0, "z", 0,
                                "time", colorTransitionTime - 0.1f,
                                "easetype", iTween.EaseType.easeInOutSine,
                                "looptype", iTween.LoopType.none
                            ));
                            tweenedPurpleCrystal = true;
                        }

                        //This will smoothly make the trail go away.
                        purpleCrystalTrail.time = colorTransitionCounter;

                        //You can color transition right away.
                        canColorTransition = true;

                        //When to go to purple phase!
                        if(colorTransitionCounter <= 0 && yellowBulletHolder.transform.childCount == 0) {
                            currentPhase = Phase.PhaseFivePurple;
                        }
                        break;
                }

                //Color based on if it's time to color.
                if (!canColorTransition) {
                    ColorFormOne(colorPhaseSwitchFrom);
                } else {
                    ColorFormOne(colorPhaseSwitchTo);
                }

                //Form change particles become active when color transition is allowed.
                if (!formChangeParticles.GetComponent<ParticleSystem>().isPlaying) {
                    formChangeParticles.GetComponent<ParticleSystem>().Play();
                }
                
                //Subract the time it takes for the color transitions to take place.
                colorTransitionCounter -= Time.deltaTime;

                //Color the attack indicator based on color transition counter.
                float sizeIncreaser = 1.1f;
                attackNotice.transform.localScale = new Vector3(colorTransitionCounter + sizeIncreaser, colorTransitionCounter + sizeIncreaser, 1f);
                if(switchToColor == SwitchType.RedToYellow || switchToColor == SwitchType.YellowToGreen) {
                    float alp = 1f - (colorTransitionCounter/colorTransitionTime);
                    Color colorToBe = new Color(colorPhaseSwitchTo.r, colorPhaseSwitchTo.g, colorPhaseSwitchTo.b, alp);
                    attackNotice.GetComponent<MeshRenderer>().material.color = colorToBe;
                } else {
                    attackNotice.GetComponent<MeshRenderer>().material.color = Color.clear;
                }
                break;
            case Phase.PhaseOneRed:
                //Turn the face puppet to look at the player.
                AnimateFace(); 

                //Color puppets to red.
                ColorFormOne(Color.red);

                //Velocity is zero.
                vel = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = vel;

                //Manage the scale of the crystal spinner.
                Vector3 crystalCircleScale = crystalRotateController.transform.localScale;
                float crystalCircleSize = 1f + ((70 - redHealth)/20f);
                Vector3 targetCrystalCircleScale = new Vector3(crystalCircleSize, crystalCircleSize, 1);
                float circleSizerSmoothness = 3f;
                crystalCircleScale = Vector3.Lerp(crystalCircleScale, targetCrystalCircleScale, circleSizerSmoothness*Time.deltaTime);
                crystalRotateController.transform.localScale = crystalCircleScale;
                crystalRotateController.transform.parent = null;
                crystalBatch.transform.parent = null;
                
                //Move slowly toward the player.
                if(Game.playerObj != null) { 
                    float targetRedFormSpeed = 5f;
                    if(currentRedFormMovementSpeed < targetRedFormSpeed) { currentRedFormMovementSpeed += Time.deltaTime; }
                    if(currentRedFormMovementSpeed > targetRedFormSpeed) { currentRedFormMovementSpeed = targetRedFormSpeed; }
                    Vector3 playerPos = Game.playerObj.transform.position;
                    Vector3 pos = transform.position;
                    Vector3 movePos = Vector3.MoveTowards(pos, playerPos, currentRedFormMovementSpeed*Time.deltaTime);
                    crystalRotateController.transform.position = movePos;
                    pos = movePos;
                    transform.position = movePos;
                }
                break;
            case Phase.PhaseTwoYellow:
                //Color the puppets yellow.
                ColorFormOne(Color.yellow);
                
                //Velocity is zero.
                vel = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = vel;
                
                //Re-enable the colliders.
                formOneCollider.enabled = true;
                for(int i = 0; i < crystals.Count; i++) { 
                    if(crystals[i] != null) {
                        crystals[i].GetComponent<Collider2D>().enabled = true;
                    }
                }

                yellowBulletShootCounter -= Time.deltaTime;
                if(yellowBulletShootCounter <= 0) {
                    PlaySound.NoLoop(yellowShootSound);
                    for(int i = 0; i < crystalMatchPoints.Count; i++) {
                        if(i != 0) { //Don't spawn a crystal at the where there isn't a crystal. (0 was the red crystal.)
                            Vector2 dif = Vector2.zero - new Vector2(crystalMatchPoints[i].transform.position.x, crystalMatchPoints[i].transform.position.y);
			                dif.Normalize();
			                float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
			                Quaternion yellowBulletRot = Quaternion.Euler(new Vector3(0, 0, (angle)));
                            GameObject obj = Instantiate(yellowFormBulletPrefab, crystalMatchPoints[i].transform.position, yellowBulletRot) as GameObject;
                            obj.transform.parent = yellowBulletHolder.transform;
                        }
                    }
                    yellowBulletShootCounter = yellowBulletShootTime;
                }
                break;
            case Phase.PhaseThreeGreen:
                //Reenable the collider.
                formOneCollider.enabled = true;

                //Color him.
                ColorFormOne(Color.green);
                
                //X position size crystals can randomly move too.
                float crystalPositionVariance = 34.5f;

                //Move the crystals around and spawn lightning.
                if (!tweenedGreenCrystals) { 
                    for(int i = 0; i < crystalMatchPoints.Count; i++) { //Tween crystal to it's place.
                        iTween.MoveTo(crystalMatchPoints[i], iTween.Hash(
                            "x", Random.Range(-crystalPositionVariance, crystalPositionVariance),
                            "y", 18.4f,
                            "time", greenCrystalMoveTime - 0.1f,
                            "easetype", iTween.EaseType.easeInOutSine,
                            "looptype", iTween.LoopType.none
                        ));
                    }
                    greenCrystalMoveCounter = greenCrystalMoveTime;
                    greenCrystalHoldCounter = greenCrystalHoldTime;
                    madeGreenLightning = false;
                    tweenedGreenCrystals = true;
                } else {
                    greenCrystalMoveCounter -= Time.deltaTime;   
                    if(greenCrystalMoveCounter <= 0) {
                        if (!madeGreenLightning) {
                            for(int i = 0; i < crystalMatchPoints.Count; i++) {
                                if(i > 1) {
                                    Vector3 lightningPos = new Vector3(crystalMatchPoints[i].transform.position.x, 0, 5);
                                    Instantiate(greenLightningPrefab, lightningPos, Quaternion.identity);
                                }
                            }
                            madeGreenLightning = true;
                        }
                        greenCrystalHoldCounter -= Time.deltaTime;
                        if (greenCrystalHoldCounter <= 0) {
                            tweenedGreenCrystals = false;
                        }
                    }
                }

                //Manage the way the warden looks when moving around.
                if (vel.x < 0) { faceFrame = Vector2.zero; }
                else if (vel.x > 0) { faceFrame = new Vector2(0.75f, 0f); }
                face.GetComponent<MeshRenderer>().material.mainTextureOffset = faceFrame;

                //Move the warden around.
                vel = GetComponent<Rigidbody2D>().velocity;
                Vector3 greenWardenPosition = transform.position;

                if (!gotNewVel && Game.playerObj != null) {
                    Vector2 pPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
                    Vector2 dif = new Vector2(greenWardenPosition.x, greenWardenPosition.y) - pPos;
                    float speed = 25f;
                    newVel = dif.normalized * speed;
                    vel = -newVel;
                    gotNewVel = true;
                }
                
                //If the warden goes off screen, reposition him.
                //Going around randomly when offscreen!
                int xLimit = 48;
                int yLimit = 33;
                if ( greenWardenPosition.x > xLimit ||  greenWardenPosition.x < -xLimit ||  greenWardenPosition.y > yLimit ||  greenWardenPosition.y < -yLimit) {
                    //1 == top, 2 == bottom, 3 == left, 4 == right.
                    int newSide = Random.Range(1, 4);
                    float z = 0f;
                    if (newSide == 1) {
                        greenWardenPosition = new Vector3(Random.Range(47f, -47f), 32, z);
                        gotNewVel = false;
                    }
                    else if (newSide == 2) {
                        greenWardenPosition = new Vector3(Random.Range(47f, -47f), -32, z);
                        gotNewVel = false;
                    }
                    else if (newSide == 3) {
                        greenWardenPosition = new Vector3(-47f, Random.Range(-32f, 32f), z);
                        gotNewVel = false;
                    }
                    else if (newSide == 4) {
                        greenWardenPosition = new Vector3(47f, Random.Range(-32f, 32f), z);
                        gotNewVel = false;
                    }
                }

                //Updating position and velocity.
                GetComponent<Rigidbody2D>().velocity = vel;
                transform.position = greenWardenPosition;
                break;
            case Phase.PhaseFourBlue:
                //Turn the face puppet to look at the player.
                AnimateFace(); 

                //Velocity is zero.
                vel = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = vel;
                
                //Color puppets to red.
                ColorFormOne(new Color(0.3f, 0.3f, 1f, 1f));
                
                //build up the spin of the blue crystals to be really fast.
                float blueCrystalTargetSpinSpeed = 7.5f;
                crystalRotateController.GetComponent<RotateScript>().rotationSpeed += Time.deltaTime*1.5f;
                if(crystalRotateController.GetComponent<RotateScript>().rotationSpeed > blueCrystalTargetSpinSpeed) {
                    crystalRotateController.GetComponent<RotateScript>().rotationSpeed = blueCrystalTargetSpinSpeed;
                }

                //Increase lifetime of trails.
                blueCrystalTrail.enabled = true;
                purpleCrystalTrail.enabled = true;
                float lifeTime = blueCrystalTrail.time;
                if(lifeTime < 0.4f) { lifeTime += Time.deltaTime/2f; }
                if(lifeTime > 0.4f) { lifeTime = 0.4f; }
                blueCrystalTrail.time = lifeTime;
                purpleCrystalTrail.time = lifeTime;
                
                //Play the suck particles.
                if (!blueSuckParticles.GetComponent<ParticleSystem>().isPlaying) {
                    blueSuckParticles.GetComponent<ParticleSystem>().Play();
                }

                //Play the center cover particles.
                if (!blueCenterCoverParticles.GetComponent<ParticleSystem>().isPlaying) {
                    blueCenterCoverParticles.GetComponent<ParticleSystem>().Play();
                }

                blueChunkSpawnCounter -= Time.deltaTime;
		        if(blueChunkSpawnCounter <= 0){
			        //Spawn a bomb every 4 chunks.
			        if(blueChunksSpawned >= 4){
				        bombInst = Instantiate(bombPrefab, chunkSpawnPlace, Quaternion.identity) as GameObject;
                        bombInst.GetComponent<CrystalBombScript>().warden = this;
                        bombInst.transform.parent = yellowBulletHolder.transform;

				        //Move Bomb.
				        iTween.MoveTo(bombInst, iTween.Hash(
					        "x", 0,
					        "y", 0,
					        "z", 0,
					        "time", 10f,
					        "easeType", iTween.EaseType.linear
				        ));
				        //Scale Bomb.
				        iTween.ScaleTo(bombInst, iTween.Hash(
					        "x", 0,
					        "y", 0,
					        "z", 0,
					        "time", 0.5f,
					        "easeType", iTween.EaseType.linear,
					        "delay", 9.5f
				        ));
				        Destroy(bombInst, 10.5f);
				        blueChunksSpawned = 0;
			        }
			
			        //0 == left. 1 == right. 2 == up. 3 == down.
			        int chosenSide = Random.Range(0, 4);
			        int chosenChunk = Random.Range(0, blueFormWalls.Length);
			        int chosenRot = Random.Range(0, 4);
			
			        //Setting spawn place for chunks.
			        if(chosenSide == 0){ chunkSpawnPlace = new Vector2(-60, Random.Range(-40f, 40f)); }
			        if(chosenSide == 1){ chunkSpawnPlace = new Vector2(60, Random.Range(-40f, 40f)); }
			        if(chosenSide == 2){ chunkSpawnPlace = new Vector2(Random.Range(-60, 60), 40); }
			        if(chosenSide == 3){ chunkSpawnPlace = new Vector2(Random.Range(-60, 60), -40); }
			
			        //Setting the chunk rotation.
			        if(chosenRot == 0){chunkRotVec = Vector3.zero;}
			        if(chosenRot == 1){chunkRotVec = new Vector3(0, 0, 180);}
			        if(chosenRot == 2){chunkRotVec = new Vector3(0, 0, 90);}
			        if(chosenRot == 3){chunkRotVec = new Vector3(0, 0, -90);}
			        chunkRot.eulerAngles = chunkRotVec;
			
			        //Spawn chunk.
                    GameObject chunkInst = Instantiate(blueFormWalls[chosenChunk], chunkSpawnPlace, chunkRot) as GameObject;
                    chunkInst.transform.parent = yellowBulletHolder.transform;

			        //Move Chunk.
			        iTween.MoveTo(chunkInst, iTween.Hash(
				        "x", 0,
				        "y", -0.2f,
				        "z", 0,
				        "time", chunklifeTime,
				        "easeType", iTween.EaseType.linear
			        ));
			
			        //Scale Chunk.
			        iTween.ScaleTo(chunkInst, iTween.Hash(
				        "x", 0,
				        "y", 0,
				        "z", 0,
				        "time", 0.5f,
				        "easeType", iTween.EaseType.linear,
				        "delay", chunklifeTime - 0.5f
			        ));
			
			
			        //Destroy chunk.
			        Destroy(chunkInst, chunklifeTime);
			
			        //Add to chunks spawned.
			        blueChunksSpawned++;
			
			        //Reset clock.
			        blueChunkSpawnCounter = blueChunkSpawnTime;
		        }
                break;
            case Phase.PhaseFivePurple:
                //Carl say a thing!
                if(purpleCrystalIntroText != null) {
                    purpleCrystalIntroText.SetActive(true);
                }
                
                //Disable the purple crystals collider.
                if(crystals[4] != null){ crystals[4].GetComponent<PolygonCollider2D>().enabled = false; }

                //Enable this gameObjects collider.
                if(firstPurpleCrystalHealth > 1){ firstPurpleCrystalCollider.enabled = true; }

                //Hide everything!
                face.GetComponent<MeshRenderer>().material.color = Color.clear;
                for(int i = 0; i < frontCapePuppets.Count; i++) { frontCapePuppets[i].GetComponent<SkinnedMeshRenderer>().material.color = Color.clear; }
                for(int j = 0; j < backCapePuppets.Count; j++) { backCapePuppets[j].GetComponent<SkinnedMeshRenderer>().material.color = Color.clear; }

                //Turn of the flame.
                Color clrlol = face.GetComponent<MeshRenderer>().material.color;
                if(formChangeParticles.GetComponent<ParticleSystem>().isPlaying){
                    formChangeParticles.GetComponent<ParticleSystem>().Stop();
                }
               
                //Enable the purple crystal crack.
                purpleCrystalCrack.SetActive(true);
                
                //Color the purple crystal crack.
                float crackColorSmooth = 4f;
                if(hurtCounter > 0) {
                    hurtCounter -= Time.deltaTime;
                    purpleCrystalCrack.GetComponent<MeshRenderer>().material.color = Color.Lerp(purpleCrystalCrack.GetComponent<MeshRenderer>().material.color, Color.black, crackColorSmooth*Time.deltaTime);
                } else {
                    purpleCrystalCrack.GetComponent<MeshRenderer>().material.color = Color.Lerp(purpleCrystalCrack.GetComponent<MeshRenderer>().material.color, Color.clear, crackColorSmooth*Time.deltaTime);
                }

                //Go to the final form transition when all purple crystals are broken.
                if(purpleCrystalHolder.transform.childCount == 0) {
                    finalTransitionBandaidCounter -= Time.deltaTime;
                    if(finalTransitionBandaidCounter <= 0) {
                        currentPhase = Phase.TransitionToFinalPhase;
                    }
                } else {
                    finalTransitionBandaidCounter = finalTransitionBandaidTime;
                }
                break;
            case Phase.TransitionToFinalPhase:
                //Set the music.
                if(!setFinalMusic) {
                    musicSource.time = 79.72f;
                    setFinalMusic = true;
                }

                if (finalTransitionTextOne != null) {
                    finalTransitionTextOne.SetActive(true);
                } else {
                    Color fc = finalFade.GetComponent<MeshRenderer>().material.color;
                    fc.a += Time.deltaTime/1.5f;
                    finalFade.GetComponent<MeshRenderer>().material.color = fc;
                    if(fc.a > 1) { fc.a = 1f; }
                    if(musicSource.time > 90.1f) {
                        Destroy(finalFade, finalFadeLifeTime);
                        currentPhase = Phase.FinalPhase;
                    }
                }
                break;
            case Phase.FinalPhase:
                //Looping the final phase music.
                if(musicSource.time >= 133.771f) {
                    musicSource.time = 90.1f;
                }

                //Final form health bar management.
                //Debug.Log(finalPhaseHealth.ToString());
                float why = finalPhaseHealth*0.0014285714285714f;
                Vector3 hScale = new Vector3(1, why, 1);
                finalFormHealthBar.transform.localScale = hScale;

                //Play the flash!
                if (!playedFlash) {
                    iTween.ScaleTo(finalFlash, iTween.Hash(
                        "y", 45f, "time", finalFadeLifeTime - 0.05f,
                        "easetype", iTween.EaseType.easeInOutSine,
                        "looptype", iTween.LoopType.none
                    ));
                    iTween.ColorTo(finalFlash, iTween.Hash(
                        "a", 0f, "time", 0.3f, "delay", finalFadeLifeTime,
                        "easetype", iTween.EaseType.easeInOutSine,
                        "looptype", iTween.LoopType.none
                    ));
                    playedFlash = true;
                }
                //You can loop around the screen now.
                Game.loopBlocked = false;

                //You are at the final bosses final phase.
                Game.atFinalBossFinalForm = true;
                
                //Set active holders.
                formOneHolder.SetActive(false);
                formTwoHolder.SetActive(true);

                //Manage hurt look.
                if(finalPhaseHurtCounter > 0) {
                    finalPhaseHurtCounter -= Time.deltaTime;
                }
                finalHurtPuppet.SetActive(finalPhaseHurtCounter > 0);
                finalHurtPuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset;
                

                //Time to attack!
                if(finalPhaseAttackCounter > 0) { //If the counter is greater than 0...
                    finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = Vector2.zero;
                    if (!switchedFinalPhaseAttack) { //Switch the final phase attack.
                        if(finalFormAttackIndex < maxAttack) {
                            finalFormAttackIndex++;
                        } else {
                            finalFormAttackIndex = 0;
                        }
                        switchedFinalPhaseAttack = true; //Mark that the attack has been switched.
                    }
                    finalPhaseAttackCounter -= Time.deltaTime; //Attack wait time goes down.

                    //Keep attack counters reset here as well.
                    spitHoldCounter = spitHoldTime;
                    spitSpawnCounter = spitSpawnTime;
                    gearSpawnCounter = gearSpawnTime;
                    laserChargeCounter = laserChargeTime;
                    laserFireCounter = laserFireTime;
                    loopBlockAttackStartCounter = loopBlockAttackStartTime;
                    loopBlockAttackSpewCounter = loopBlockAttackSpewTime;
                    
                    //Keep bools reset here.
                    madeLaserChargePrefab = false;
                    madeWardenLaserPrefab = false;
                    movedLoopBlockerOnScreen = false;
                    playedLaserchargeSound = false;
                    playedLaserFireSound = false;

                    //Reset default puppets too show.
                    frontArmAttackPuppet.SetActive(false);
                    frontArmIdlePuppet.SetActive(true);

                    //If needed, move the screen loop blockers back off-screen.
                    if (!movedLoopBlockerOffScreen) {
                        iTween.MoveTo(finalFormLoopBlockers, iTween.Hash(
                            "x", 100, "y", 0, "z", 0,
                            "time", 3f,
                            "easetype", iTween.EaseType.easeInOutSine,
                            "looptype", iTween.LoopType.none
                        ));
                        movedLoopBlockerOffScreen = true;
                    }

                } else { // This else statement contains the attacking code.
                    switchedFinalPhaseAttack = false; //Mark that the attack has NOT been switched so that it can be reswitched when the attack is finished.

                    //This is the code for final form attack one. (Spewing the bullets from mouth.)
                    switch(finalFormAttackIndex){
                        case 0: //Spewing bullets from mouth attack.
                            if(spitHoldCounter > 0) { 
                                finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.5f, 0f);
                                spitHoldCounter -= Time.deltaTime;
                                spitSpawnCounter -= Time.deltaTime;
                                if (spitSpawnCounter < 0f && spitHoldCounter > 0.5f) {
                                    PlaySound.NoLoop(yellowShootSound);
                                    float randY = Random.Range(-5f, 0f);
                                    Vector3 spitSpawnPos = new Vector3(18f, randY, 3);
                                    GameObject go = Instantiate(spitPrefab, spitSpawnPos, Quaternion.identity) as GameObject;
                                    go.transform.parent = wardenFinalAttackHolder.transform;
                                    spitSpawnCounter = spitSpawnTime;
                                }
                            } else { //When the spit time is over, reset the attack doohicky.
                                finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = Vector2.zero;
                                finalPhaseAttackCounter = finalPhaseAttackTime;
                            }
                            break;
                        case 1: //Spewing gear bullets from mouth attack.
                            if(spitHoldCounter > 0) { 
                                finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.5f, 0f);
                                spitHoldCounter -= Time.deltaTime;
                                gearSpawnCounter -= Time.deltaTime;
                                if(gearSpawnCounter < 0f && spitHoldCounter > 0.5f) {
                                    PlaySound.NoLoop(yellowShootSound);
                                    float randY = Random.Range(-5f, 0f);
                                    Vector3 gearSpawnPos = new Vector3(18f, randY, 3);
                                    GameObject go = Instantiate(gearBullet, gearSpawnPos, Quaternion.identity) as GameObject;
                                    go.transform.parent = wardenFinalAttackHolder.transform;
                                    gearSpawnCounter = gearSpawnTime;
                                }
                            } else { //When the spit time is over, reset the attack doohicky.
                                finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = Vector2.zero;
                                finalPhaseAttackCounter = finalPhaseAttackTime;
                            }
                            break;
                        case 2: //Hand laser attack.
                            //Play the laser charge sound/
                            if (!playedLaserchargeSound) {
                                PlaySound.NoLoop(laserChargeSound);
                                playedLaserchargeSound = true;
                            }

                            //Show the attacking arm and hide the idle arm.
                            frontArmAttackPuppet.SetActive(true);
                            frontArmIdlePuppet.SetActive(false);

                            //Make the laser charge thingy.
                            if (!madeLaserChargePrefab) {
                                Instantiate(laserChargePrefab, new Vector3(4.5f, -7.5f, 0f), Quaternion.identity);
                                madeLaserChargePrefab = true;
                            }

                            //Charge the laser.
                            laserChargeCounter -= Time.deltaTime;
                            if(laserChargeCounter <= 0) {
                                if (!playedLaserFireSound) {
                                    PlaySound.NoLoop(laserFireSound);
                                    playedLaserFireSound = true;
                                }
                                laserFireCounter -= Time.deltaTime;
                                if (!madeWardenLaserPrefab) {
                                    GameObject go = Instantiate(wardenLaser, new Vector3(4.5f, -7.5f, 0f), Quaternion.identity) as GameObject;
                                    go.transform.parent = wardenFinalAttackHolder.transform;
                                    madeWardenLaserPrefab = true;
                                }
                                if(laserFireCounter <= 0) {
                                    finalPhaseAttackCounter = finalPhaseAttackTime;
                                }
                            }
                            break;
                        case 3: //Attack with the big wall and bullets coming from the top and bottom of the screen.
                            //Set this back to false so the loop blockers can be tweened off screen again when this attack is over.
                            movedLoopBlockerOffScreen = false;

                            //Move the loop blockers on screen.
                            if (!movedLoopBlockerOnScreen) {
                                iTween.MoveTo(finalFormLoopBlockers, iTween.Hash(
                                    "x", 0, "y", 0, "z", 0,
                                    "time", loopBlockAttackStartTime,
                                    "easetype", iTween.EaseType.easeInOutSine,
                                    "looptype", iTween.LoopType.none
                                ));
                                movedLoopBlockerOnScreen = true;
                            }

                            loopBlockAttackStartCounter -= Time.deltaTime;
                            if(loopBlockAttackStartCounter <= 0) {
                                loopBlockAttackSpewCounter -= Time.deltaTime;
                                spitSpawnCounter -= Time.deltaTime;
                                if(spitSpawnCounter <= 0) {
                                    int rand = Random.Range(0, 2);
                                    float xPos = Random.Range(-15f, 0.2f);
                                    float yPos = 22f;
                                    switch (rand) {
                                        case 0: //Bullet from the bottom.
                                            GameObject o1 = Instantiate(otherPurposeBullet, new Vector3(xPos, -yPos, 0), Quaternion.identity) as GameObject;
                                            o1.GetComponent<OtherPurposeBulletScript>().start = OtherPurposeBulletScript.StartSide.Down;
                                            o1.transform.parent = wardenFinalAttackHolder.transform;
                                            break;
                                        case 1: //Bullet from the top.
                                            GameObject o2 = Instantiate(otherPurposeBullet, new Vector3(xPos, yPos, 0), Quaternion.identity) as GameObject;
                                            o2.GetComponent<OtherPurposeBulletScript>().start = OtherPurposeBulletScript.StartSide.Up;
                                            o2.transform.parent = wardenFinalAttackHolder.transform;
                                            break;
                                    }
                                    spitSpawnCounter = spitSpawnTime;
                                }
                                if (loopBlockAttackSpewCounter <= 0) {
                                    finalPhaseAttackCounter = finalPhaseAttackTime;
                                }
                            }
                            break;
                    } 
                }
                //When the warden is out of health, go to the death phase.
                if(finalPhaseHealth <= 0) {
                    PlaySound.NoLoop(wardenScreech);
                    currentPhase = Phase.Death;
                }
                break;
            case Phase.Death: //What happens when the warden dies...
                //Warden marked as dead.
                Game.wardenDead = true;
                Game.beatFinalBoss = true;

                //Destroy all objects under the attack holder.
                if (!destroyedFinalAttacks) { 
                    foreach(Transform child in wardenFinalAttackHolder.transform) {
                        Destroy(child.gameObject);
                    }
                    destroyedFinalAttacks = true;
                }
                
                //If needed, move the screen loop blockers back off-screen.
                if (!movedLoopBlockerOffScreen) {
                    iTween.MoveTo(finalFormLoopBlockers, iTween.Hash(
                        "x", 100, "y", 0, "z", 0,
                        "time", 3f,
                        "easetype", iTween.EaseType.easeInOutSine,
                        "looptype", iTween.LoopType.none
                    ));
                    movedLoopBlockerOffScreen = true;
                }

                //Keep mouth open!
                finalHurtPuppet.SetActive(true);
                finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.5f, 0f);
                finalHurtPuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = finalFacePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset;
                
                //Enable that final explosion!
                finalExplo.SetActive(true);

                //Disable the boss collider.
                formTwoHolder.GetComponent<PolygonCollider2D>().enabled = false;

                //Waiting to end and go to the next scene.
                endCounter -= Time.deltaTime;
                if(endCounter <= 0) {
                    SceneManager.LoadScene(15);
                }

                //Check achievements for beating the boss.
                if (!checkedAchievos) {
                    Game.CheckAchievements(6);
                    if (!Game.hitByFinalBoss) {
                        Game.CheckAchievements(14);
                    }
                    checkedAchievos = true;
                }
                break;
        }
    }

    private void FixedUpdate() {
        //Suck in the player.
        if (!Game.manager.GetComponent<GameManager>().paused && currentPhase == Phase.PhaseFourBlue) { 
            //Slowly raising the suckforce to where it's supposed to be. (So that the player doesn't need to instantly react when loading into the boss.)
            float slowdown = 1.7f;
            if(suckForce < targetSuckForce) { suckForce += Time.fixedDeltaTime/slowdown; }
            if(suckForce > targetSuckForce) { suckForce = targetSuckForce; }

		    Vector2 playerP = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
		    Vector2 centerDistance = new Vector2(transform.position.x, transform.position.y);
		    Vector2 forceToApply = playerP - centerDistance;
		    forceToApply.Normalize();
		    forceToApply = new Vector2(-forceToApply.x*suckForce, -forceToApply.y*suckForce);
		    Game.playerObj.GetComponent<Rigidbody2D>().velocity += forceToApply;
        }
    }

    private void AnimateFace() {
        if(Game.playerObj != null) { 
            Vector3 playerPos = Game.playerObj.transform.position;
            Vector3 pos = transform.position;
            float forwardSize = 3f;
            if(currentPhase == Phase.ColorTransition && canColorTransition) {
                faceFrame = new Vector2(0.5f, 0f);
            } else {
                if (playerPos.x > pos.x - forwardSize && playerPos.x < pos.x + forwardSize) {
                    faceFrame = new Vector2(0.25f, 0f);
                } else if(playerPos.x < pos.x - forwardSize) {
                    faceFrame = Vector2.zero;
                } else if(playerPos.x > pos.x + forwardSize) {
                    faceFrame = new Vector2(0.75f, 0f);
                }
            }
            face.GetComponent<MeshRenderer>().material.mainTextureOffset = faceFrame;
        }
    }

    private void BreakCrystal(int crystalToBreak, Color colorCrystalExplosion) {
        for (int i = 0; i < crystals.Count; i++) {
            if(i == crystalToBreak && crystals[i] != null) {
                PlaySound.NoLoop(crystalBreakSound);
                GameObject obj = Instantiate(crystalBreakPrefab, crystals[i].transform.position, Quaternion.identity) as GameObject;
                GameObject smallCrystal = obj.transform.Find("Small Crystal").gameObject;
                GameObject bigCrystal = obj.transform.Find("Big Crystal").gameObject;
                GameObject smallCrystalPuppet = smallCrystal.transform.Find("puppet").gameObject;
                GameObject bigCrystalPuppet = bigCrystal.transform.Find("puppet").gameObject;
                smallCrystalPuppet.GetComponent<MeshRenderer>().material.color = colorCrystalExplosion;
                bigCrystalPuppet.GetComponent<MeshRenderer>().material.color = colorCrystalExplosion;
                Destroy(crystals[i]);
            }
        }
    }

    private void ColorFormOne(Color colorIWannaBe) {
        //Color the face.
        Color currentColor = face.GetComponent<MeshRenderer>().material.color;
        float colorSmooth = 2.5f;
        if(hurtCounter > 0) {
            hurtCounter -= Time.deltaTime;
            currentColor = Color.Lerp(currentColor, hurtColor, colorSmooth*Time.deltaTime);
        } else {
            currentColor = Color.Lerp(currentColor, colorIWannaBe, colorSmooth*Time.deltaTime);
        }
        face.GetComponent<MeshRenderer>().material.color = currentColor;

        //Color the front cape batch.
        Color frontCapeColor = currentColor/2;
        for(int i = 0; i < frontCapePuppets.Count; i++) {
            frontCapePuppets[i].GetComponent<SkinnedMeshRenderer>().material.color = frontCapeColor;
        }

        //Color the back cape batch.
        Color backCapeColor = currentColor/3;
        for(int j = 0; j < backCapePuppets.Count; j++) {
            backCapePuppets[j].GetComponent<SkinnedMeshRenderer>().material.color = backCapeColor;
        }

        //Color the crystals.
        Color crystalTargetColor = currentColor;
        for(int k = 0; k < crystalPuppets.Count; k++) {
            if(crystalPuppets[k] != null) { 
                crystalPuppets[k].GetComponent<MeshRenderer>().material.color = crystalTargetColor;
            }
        }

        //Color the flame.
        Color clrlol = face.GetComponent<MeshRenderer>().material.color;
        ParticleSystem.MainModule pms =  formChangeParticles.GetComponent<ParticleSystem>().main;
        pms.startColor = new Color(clrlol.r, clrlol.g, clrlol.b, 0.425f);

        //the stars in the background should match the color of the boss.
        Color starColor = Color.Lerp(Game.finalBossStars.GetComponent<Renderer>().material.color, colorIWannaBe, colorSmooth*Time.deltaTime);
        Game.finalBossStars.GetComponent<Renderer>().material.color = starColor;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Bullet") {
            switch (currentPhase) {
                case Phase.PhaseOneRed:
                    redHealth--;
                    PlaySound.Damage();
                    if(redHealth <= 1) {
                        BreakCrystal(0, Color.red);
                        switchToColor = SwitchType.RedToYellow;
                        currentPhase = Phase.ColorTransition;
                    }
                    break;
                case Phase.PhaseTwoYellow:
                    PlaySound.Damage();
                    yellowHealth--;
                    if(yellowHealth <= 1) {
                        foreach(Transform child in yellowBulletHolder.transform) {
                            GameObject yellowBullet = child.gameObject;
                            yellowBullet.GetComponent<CircleCollider2D>().enabled = false;
                            yellowBullet.GetComponent<MoveForward>().enabled = false;
                            GameObject particleObj = yellowBullet.transform.Find("Particle System").gameObject;
                            particleObj.GetComponent<ParticleSystem>().Stop();
                            GameObject rotatePuppet = yellowBullet.transform.Find("puppet rotate controller").gameObject;
                            rotatePuppet.SetActive(false);
                            Destroy(yellowBullet, 1f);
                        }
                        BreakCrystal(1, Color.yellow);
                        switchToColor = SwitchType.YellowToGreen;
                        currentPhase = Phase.ColorTransition;
                    }
                    break;
                case Phase.PhaseThreeGreen:
                    PlaySound.Damage();
                    greenHealth--;
                    if(greenHealth <= 1) {
                        crystalRotateController.GetComponent<RotateScript>().rotationSpeed = 0;
                        BreakCrystal(2, Color.green);
                        switchToColor = SwitchType.GreenToBlue;
                        currentPhase = Phase.ColorTransition;
                    }
                    break;
                case Phase.PhaseFivePurple:
                    PlaySound.Damage();
                    firstPurpleCrystalHealth--;
                    if (firstPurpleCrystalHealth <= 1) {
                        crystals[4].SetActive(false);
                        Destroy(crystals[4], 0.1f);
                        firstPurpleCrystalCollider.enabled = false;
                        PlaySound.NoLoop(crystalBreakSound);
                        Instantiate(bigPurpleCrystalBreakEffect, Vector3.zero, Quaternion.identity);
                        Vector3 dif = transform.position - collision.gameObject.transform.position;
                        Vector3 bulletPoint = dif.normalized;
                        int purpleCrystalSpeed = 7;
			            GameObject sq1 = Instantiate(bigPurpleCrystalPrefab, transform.position, Quaternion.identity) as GameObject;
			            GameObject sq2 = Instantiate(bigPurpleCrystalPrefab, transform.position, Quaternion.identity) as GameObject;
			            Vector2 force = new Vector2((bulletPoint.y)*purpleCrystalSpeed, (bulletPoint.x)*purpleCrystalSpeed);
                        sq1.transform.parent = purpleCrystalHolder.transform;
			            sq2.transform.parent = purpleCrystalHolder.transform;
                        sq1.GetComponent<Rigidbody2D>().velocity = force;
			            sq2.GetComponent<Rigidbody2D>().velocity = -force;
                        formOneHolder.SetActive(false);
                    }
                    break;
            }
            hurtCounter = hurtTime;
        }
    }

    public void BlueHurtMe() {
        PlaySound.Damage();
		Instantiate(bombExplo, transform.position, Quaternion.identity);
		//showHurtCounter = showHurtTime;
		if(blueHealth > 1){
			blueHealth--;
		} else {
            BreakCrystal(3, blueColor);
            switchToColor = SwitchType.BlueToPurple;
            currentPhase = Phase.ColorTransition;
		}
    }

    public void HurtFinalPhase() {
        finalPhaseHealth--;
        PlaySound.Damage();
        finalPhaseHurtCounter = finalPhaseHurtTime;
    }
}
