using System;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class GameManager : MonoBehaviour {

	//Which gamemode the game is going to run.
	public enum GameMode {
		Adventure,
		BulletHell,
		FinalBoss,
		Legacy,
		LegacyBulletHell,
	}
	public GameMode currentMode;

	//For adventure mode, these are the possible level themes.
	public enum ZoneThemes {
		RedZone1,
		YellowZone2,
		GreenZone3,
		BlueZone4,
		PurpleZone5,
		WhiteZone6,
		FinalBossZone7,
	}
	public ZoneThemes zTheme;
	
	//Used for scrolling through colors of the rainbow, which color is being subtracted.
	public enum SubColorMode{
		subR,
		subG,
		subB,
	}
	SubColorMode sMode = SubColorMode.subR;

	//GameObjects.
	GameObject backgroundHolder = null; //The parent object of all the background settings for adventure mode.
	GameObject backgroundCamera = null; //The background Camera Gameobject. (The camera that renders only the background and nothing else.)
	GameObject mainCamera = null; //The main camera object. Renders everything going on in the foreground. (Player, walls, enemies, etc.)
	GameObject screenGrab = null; //The quad that scales down to zero after the player shoots themself and transitions to the next layer.
	GameObject pauseMenu = null; //The big pause menu object.
	GameObject splashScreen = null; //The big parent object of the boss splashscreen stuff.
	GameObject progress = null; //Big parent object of the bar that shows your progress through an adventure level.
	GameObject progressScale = null; //The child of 'progress' gameObject that scales the bar dependant on your progress through an adventure level.
	GameObject progressNumber = null; //The gameObject with the text mesh that displays the number/text of how far you are through the adventure level.
	GameObject bossHealthBar = null; //The boss health bar. 
	GameObject victoryObj = null; //The victory text and animation holder! Enable it and let it play with tweens! (Confetti included!)
	GameObject adventureWallHolder = null; //This holds the walls for the adventure mode.
	GameObject legacyWallHolder = null; //This holds all the wall layers for legacy mode.
	GameObject wallSetHolderCurrentlyUsed = null; //Which group of walls we are using for this level. (Zone 1 walls, or Zone 2 walls, or legacy walls, etc.)
	GameObject waterCamera = null; //The camera that renders the water distortion effects. (Renders in front of the background camera and behind the main camera.)
    GameObject numObj = null; //For legacy mode, this number is in the background to tell the player what layer they are on.
    GameObject backgroundQuad = null; //For legacy mode, this is a quad in the background to match the background color. (Used so that it's captured by the screen grab.)
    GameObject legacyMusicHolder = null; //Music for legacy mode.
    GameObject playerPrefab = null; //Which player is being loaded to be spawned.
    GameObject bHellBulObj = null; //Legacy bullet hell bullet.
    GameObject legacyWin = null; //Legacy win screen.
    GameObject bitCrushMusicObj = null; //When inside disruptor fields, the regular song fades out and this fades in.
    GameObject forceCamRatioObj = null; //This object contains the script that forces the camera ratio to 16:9. KEEP IT ACTIVE ALWAYS!!!
    GameObject activeBackground = null; //This is the currently active background object.
    GameObject legacyBulletHellWallHolderObj = null; //Holds the walls for legacy bullet hell mode.
    GameObject timeObj = null; //This is counter object for regular bullet hell mode. (Shows how long you've been alive.)
    GameObject singleBullet = null; //This is a bullet for bullet hell mode.
    GameObject xLeft, xRight, yUp, yDown; //These are the loop arrows for bullet hell mode.
    GameObject bulletHellMoneyBag = null; //Bullet hell money that flies by for the player to collect.
    GameObject laserReticle = null; //In Bullet Hell mode, this is the object that will kill you if you stay in the same place for too long.
    GameObject reticleShootEffect = null; //Reticle shoot effect that kills the player in bullet hell mode.
    GameObject reticlePuppet = null; //This is the puppet for the reticle, we edit it's color mostly.
    GameObject gamePadPauseMenuSelectorObj = null; //This is the object that selects things on the pause menu when you are using a gamepad.
    GameObject partyBatchObj = null; //This is the object that appears when party mode is on.
    GameObject smileyObj = null; //This is used for the smiley mode supporter mode. (It is the smiley face gameobject)
    GameObject laserHolderObj = null; //This is the gameObject that holds the amount of lasers the player has too shoot.
    GameObject noLoopWallBatch = null; //This gameObject shows that the player can no longer loop around the screen. (Only active halfway through the white zone.)
    [HideInInspector] public GameObject music = null; //The gameobject that holds the audio source that plays all the music. ! NOTE: MUSIC MANAGEMENT IS GAMEMODE SPECIFIC
    [HideInInspector] public GameObject swiper; //The thing that swipes in and out as the level loads in and out.
	List<GameObject> themeZones = new List<GameObject>(); //Background sets for each zone in adventure mode. (Zone 1 backgrounds, zone 2 backgrounds, etc. are here)
	List<GameObject> hearts = new List<GameObject>(); //The player's hearts.
    List<GameObject> playerLasers = new List<GameObject>(); //The player's laser shots.
    List<GameObject> zoneWallHolders = new List<GameObject>(); //Adventure mode wall holders. (Contains the objects that contains the walls for each zone.)
	List<GameObject> wallSets = new List<GameObject>(); //The object that contains the walls/layers.
	List<GameObject> musicChannels = new List<GameObject>(); //For legacy mode, the music channels for the layering effect.
	List<GameObject> bulletBatches = new List<GameObject>(); //These are a list of bullet batches for bullet hell mode.
	
	//Vector 3s.
	Vector3 moneyBagSpawnPosition = Vector3.zero; //This is the position where money bags will spawn in Bullet Hell Mode.
	Vector3 targetCamPos = Vector3.zero; //For lerping the camera, this is where the camera will want to be at.
	Vector3 targetCoverPosition = Vector3.zero; //For lerping the pause menu cover, this is where the cover will want to be at.
	Vector3 targetProgressPos = Vector3.zero; //For lerping the progress bar, this is where it will want to be at.
	Vector3 screenGrabStartScale = Vector3.zero; //BASICALLY, what the screen grab scale should be when you aren't supposed to see it. (zero)
	readonly Vector3 pauseCamPos = new Vector3(-30f, 0f, -30f); //Where the pause camera will go to when the game is paused.
	readonly Vector3 unPauseCamPos = new Vector3(0f, 0f, -30f); //Where the pause camera will go to when the game is NOT paused.
	readonly Vector3 pauseCoverPosition = new Vector3(-65f, 0f, -17f); //Where the pause menu cover will go when the game is paused.
	readonly Vector3 unPauseCoverPosition = new Vector3(-67f, 0f, -17f); //Where the pause menu cover will go when the game is NOT paused.
	readonly Vector3 showProgressPos = new Vector3(0f, 0.5f, 0f); //Where the level progress bar will go when the player is holding down the "Show Pregress" button.
	readonly Vector3 hideProgressPos = new Vector3(0, -1.5f, 0f); //Where the level progress bar will go when the player is NOT holding down the "Show Pregress" button.
	readonly Vector3 progressNumberPos = new Vector3(1f, 0.2f, 0f); //The position of the progress number text within the progress bar.
	
	//Vector 2s.
    Vector2 nearestEmptySpot = Vector2.zero; //Nearest empty spot in the next layer from where the player was on the previous layer.
	List<Vector2> emptySpots = new List<Vector2>(); //A list of empty spots scanned in when a player progresses to the next layer.

	//Floats.
    float deathTime = 3f; //How long it takes too die.
    float victoryHoldTime = 8f; //How long it takes the victory screen to play out.
    float endTime = 20f; //For legacy mode, how long the end will last.
    float nearestPosDistance = 500f; //Shortest distance the player is to a blank space on the next layer.
    float scrollingRedValue = 1f; //For the zone 6 bullet color scrolling from Blue to Magenta.
    float r = 1f, g = 0f, b = 0f; //For legacy Bullet Hell mode, color values for scrolling through the rainbow for the backgrounds.
    float winTime = 0f;//For legacy mode, this is how long to hold on the win screen before going back to the menu.
    float transCounter; //The counter that ticks down when transitioning between layers.
    float timeAlive; //Used in bullet hell modes... how long the player lives for.
    float tripCounter; //For legacy mode, during the win animation, this is how fast to spawn one of the colored squares.
    float rmsValue; //More audio stuff that I don't remember how it works.
    float dbValue; //More audio stuff that I don't remember how it works.
    float legacyBulletCounter; //The counter that determines when bullets spawn in legacy bullet hell mode.
	float bulletBatchSpawnTime = 5f; //How long it will take to spawn a bullet batch in bullet hell mode.
	float bulletBatchCounter = 5f; //The counter that actually counts down that spawns bullet hell batches.
	float singleBulletSpawnTime = 2.5f; //How long it will take to spawn a single bullet in bullet hell mode.
	float singleBulletCounter = 0; //The counter that actually counts down that spawns bullet hell bullets in bullet hell mode.
	float noMoveCounter = 0f; //In bullet hell mode, this will be used to determing how long the player has not been moving.
    float spawnMoneyBagCounter = 10f; //The counter that spawns money bags in bullet hell when the counter reaches 0.
    float menuSelectCounter = 0f; //The counter of how long it takes for the gamepad to move up and down the pause menu selecting things.
    float backToMenuTime = 0.85f; //How long it take to go back to the main menu from selecting "Back to menu" from the pause menu.
    private float[] samples; //More audio stuff that I don't remember how it works.
    readonly float transTime = 0.5f; //The time it takes to transition between layers.
	readonly float refValue = 0.1f; //Audio bloom reactive thing... idk what it is I wrote the Audio related stuff like 6 years ago cut me some slack. (I doubt I wrote it myself too.)
	readonly float intensity = 1f; //How intense the musically interactive stuff will be.
    readonly float negativeRMSValue; //More audio stuff that I don't remember how it works.
    readonly float legacyBulletTime = 3f; //Time that legacy bullets take to spawn.
    readonly float menuSelectTime = 0.2f; //The time the menu select counter goes to when resetting. (It takes 0.2 seconds to move up and down in the pause menu.)
	[HideInInspector] public float splashCounter = 4.2f; //How long the boss splashscreen plays out for.
    
    //Ints.
    int themeNum; //The index out of all possible themes. (Zone 1 would be: themeNum = 0; Zone 2 would be themeNum = 1; etc.)
    int level; //Which layer the player is currently on. (This starts at 50 if you're in legacy mode.)
    int levelToLoadOnDie; //What level will load when the player dies.
    int sat; //For legacy mode, determines if it's a bright or a darker saturation of colors for the theme.
    int pauseMenuIndex = 0; //What pause menu item you have selected with the gamepad.
	[HideInInspector] public int moneyBagsCollected = 0; //How many money bags the player collects in bullet hell mode.
	readonly int qSamples = 1024; //More audio stuff that I don't remember how it works.
	readonly int themeZoneLength = 10; //How many theme zones there are to chose from.
	readonly int bulletBatchCount = 13; //How many bullet batches there are for bullet hell mode.

	//Bools.
	[HideInInspector] public bool paused = false; //Bool that sets if the game is paused or not.
    [HideInInspector] public bool victory = false; //Set to true from outside boss scripts.
    [HideInInspector] public bool goingBackToMenu = false; //Bool that tells if you're going back to the menu.
	[HideInInspector] public bool showingBossSplashscreen = false; //Bool that tells if the boss splashscreen is currently being shown.
    [HideInInspector] public bool dead = false; //Bool that shows if the player has died yet.
    [HideInInspector] public bool gotHit = false; //Used to track the achievements for not getting hit in a level.
    bool startedWin = false; //When the legacy mode win animation has started.
    bool scanned = false; //Scanned the level for A*.
    bool playedSwipeTween = false; //Indicating the swiping out tween has played.
    bool increasingRed = false; //For the zone 6 bullet color scrolling from Blue to Magenta.
    bool playedJingle = false; //For legacy mode win screen, tells when the victory jingle has already played. Stops it from playing every frame.
    bool reticleKilledPlayer = false; //If the no move reticle has killed the player in Bullet Hell Mode.
    bool playerMinStats = false; //If the player has minimum stats for min stats achievements.
    bool checkedAdventureAchievements = false; //If the adventure achievements have been checked or not.
    bool needBitcrushTunes = false; //If the game is in need of the bitcrush channel.

    //AudioClips.
	AudioClip levelChangeSnd = null; //The audioclip that plays when the player advances a level by shooting themself.
	AudioClip victoryJingle = null; //The victory tune!
	AudioClip laserSound = null; //The reticle that kills the player when they dont move in Bullet Hell makes this sound.
    AudioClip menuSelectSound = null; //The sound the menu makes when you move the stick around at the pause menu.
    AudioClip menuActionSound = null; //The sound that plays when you do a menu action with the gamepad.

	//Colors.
	Color xTargetColor = Color.clear; //This is the color that the x axis loop arrow wants to be for bullet hell mode.
	Color yTargetColor = Color.clear; //This is the color that the y axis loop arrow wants to be for bullet hell mode.
	Color xCol = Color.clear; //This is the current color that the x axis loop is for bullet hell mode.
	Color yCol = Color.clear; //This is the current color that the y axis loop is for bullet hell mode.
	Color reticleTargetColor = Color.clear; //This is the current color that the bullet hell laser will smoothly transfer too.

	//LayerMasks.
	readonly LayerMask mainCamRenderLayer = 253735; //The layer that the main camera renders.
    readonly LayerMask wallLayer = 1024; //The layer of the walls.


	//Find stuff and apply stuff on Awake... (This function runs before the "Start()" function.)
	//Everything managed is IN ORDER of appearance in the heirarchy.
	void Awake() {
        //Set up static variables.
		Game.manager = this.gameObject; //staticly store this as the gamemanager object.
        Game.currentCoinsCollectedInLevel = 0; //Reset current coins collected to 0.
		Game.canShoot = true; //You can shoot at the start.
		Game.speed = 20f; //This is the speed of the player in legacy mode.
		Game.currentHealth = 3; //This sets the starting health for legacy mode.
		Game.brokeRecord = false; //You haven't broken any records at the start.
		Game.cameFromLegacy = (currentMode == GameMode.Legacy || currentMode == GameMode.LegacyBulletHell); //If you have come from a legacy mode or not.
		Game.currentDirection = Game.CurrentDirection.Up; //Bubbles will always start by going upward. (Blue zone specific.)
		Game.newMaxHealth = Game.healthRank+2; //Set starting health for adventure mode.
		Game.newCurrentHealth = Game.healthRank+2; //Set starting health for adventure mode.
        Game.brokeCore = false; //You start the game by breaking the vektor core.
        Game.enableGlitterTrail = false; //You don't start with the glitter trail!
        Game.atWardenLevel = (currentMode == GameMode.FinalBoss); //If you are at the final level or not. (Things behave differently then.)
        Game.loopBlocked = false; //The loop blocker is not enabled at the very start of a level.
        Game.atFinalBossFinalForm = false; //You are not at the final bosses final form at this state.
        Game.atFinalShot = false; //Taking the final shot at the final bosses life!
        Game.hitByFinalBoss = false; //You do not start off hit by the final boss.
        Game.wardenDead = false; //The warden doesn't start off dead.

		//Find and set the main camera render layer.
        Camera.main.cullingMask = mainCamRenderLayer;
        Camera.main.gameObject.transform.position = new Vector3(0, 0, -50); //Snap the main camera position.
		Camera.main.clearFlags = CameraClearFlags.Depth;

        //Find background holder and background camera object.
        backgroundHolder = transform.Find("Backgrounds").gameObject; //Find the background holder object.
		backgroundCamera = transform.Find("Background Camera").gameObject; //Find the background camera object.
		mainCamera = Camera.main.gameObject; //Finding the main camera object.
		for(int i = 0; i < themeZoneLength; i++){ //Fill up background list.
			//Backgrounds filled based off their "Zone #" name in heirarchy. (Bullet Hell is considered zone 8, Legacy = zone 10)
			GameObject o = backgroundHolder.transform.Find("Zone " + (i + 1).ToString()).gameObject;
			themeZones.Add(o);
		}

		//Find bullet storage and enemy storage.
		Game.bulletHolder = transform.Find("Bullets").gameObject;
		Game.enemyHolder = transform.Find("Enemies").gameObject;

		//Find and set screen grab stuff.
		screenGrab = transform.Find("Screen Grab").gameObject;
		screenGrabStartScale = screenGrab.transform.localScale;
		screenGrab.SetActive(false);

		//Wall holders, we find them and determine if they should be active.
		adventureWallHolder = transform.Find("Adventure Walls").gameObject; //This holds the walls for the adventure mode.
		legacyWallHolder = transform.Find("Legacy Walls").gameObject; //This holds all the wall layers for legacy mode.
		legacyBulletHellWallHolderObj = transform.Find("Legacy Bullet Hell Walls").gameObject; //Finding the walls for legacy bullet hell mode.
		adventureWallHolder.SetActive(currentMode == GameMode.Adventure);
		legacyWallHolder.SetActive(currentMode == GameMode.Legacy);
		legacyBulletHellWallHolderObj.SetActive(currentMode == GameMode.LegacyBulletHell);

		//Find heart holder and fill the heart list.
		GameObject heartsObj = transform.Find("Hearts").gameObject;
		foreach (Transform child in heartsObj.transform){
			hearts.Add(child.gameObject);
		}

		//Disable if it is a bullet hell mode.
		heartsObj.SetActive(!(currentMode == GameMode.BulletHell || currentMode == GameMode.LegacyBulletHell));

        //Find the player laser holder and fill the laser list.
        laserHolderObj = transform.Find("Player Lasers").gameObject;
        foreach(Transform child in laserHolderObj.transform) {
            playerLasers.Add(child.gameObject);
        }

        //Always start the game with 2 lasers.
        Game.currentLasers = 2;

        //Turn off laser holder obj. (It can only be turned on in adventure mode specific Update function.)
        laserHolderObj.SetActive(false);

		//Swipe in transition.
        swiper = transform.Find("Swipey").gameObject;
        swiper.transform.position = new Vector3(0, 0, -7);
        swiper.SetActive(true);
        iTween.MoveTo(swiper, iTween.Hash(
            "x", -100f,
            "y", 0f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.none
        ));

        //Managing the music setup stuff.
        music = transform.Find("Music").gameObject; //Find the music channel.
        legacyMusicHolder = transform.Find("Legacy Music").gameObject; //Find the legacy.
        bitCrushMusicObj = transform.Find("Bitcrush Music").gameObject; //Find the bitcrush audio channel.
        music.SetActive(currentMode != GameMode.Legacy);
        legacyMusicHolder.SetActive(currentMode == GameMode.Legacy);
        needBitcrushTunes = (currentMode == GameMode.Adventure && (zTheme == ZoneThemes.PurpleZone5 || zTheme == ZoneThemes.WhiteZone6 || zTheme == ZoneThemes.FinalBossZone7));
        bitCrushMusicObj.SetActive(needBitcrushTunes);
        bitCrushMusicObj.GetComponent<AudioSource>().volume = 0;

		//Find pause menu stuff.
		pauseMenu = transform.Find("Pause Menu").gameObject;
        gamePadPauseMenuSelectorObj = pauseMenu.transform.Find("Pad Selector").gameObject;
		paused = false;

		//Find progress stuff.
		progress = transform.Find("Progress").gameObject;
		progressScale = progress.transform.Find("scale").gameObject;
		progressNumber = progress.transform.Find("number").gameObject;
		progress.SetActive(currentMode == GameMode.Adventure);
		GameObject progressNumberPosObj = progressScale.transform.Find("text pos").gameObject; 	//Find and set proper progress number.
		progressNumberPosObj.transform.localPosition = progressNumberPos;	//Set the progress number position.

		//Find the splashScreen and disable it.
		splashScreen = transform.Find("Boss Splash Screen").gameObject;
		splashScreen.SetActive(false);

		//Find boss health bar and start it off disabled.
		bossHealthBar = transform.Find("Boss Health Bar").gameObject;
		bossHealthBar.SetActive(false);
		bossHealthBar.transform.position = new Vector3(1, 0, 0);

        //Find victory object and deactivate it immediately.
        victoryObj = transform.Find("Victory").gameObject;
        victoryObj.SetActive(false);
        victory = false;  //You are not victorious at the beginning...

		//Find the water camera.
        waterCamera = transform.Find("Water Camera").gameObject;

        //Find the legacy background number.
        numObj = transform.Find("Legacy Number").gameObject;
        numObj.SetActive(currentMode == GameMode.Legacy || currentMode == GameMode.LegacyBulletHell);

        //Find the legacy mode win screen object.
        legacyWin = transform.Find("Win").gameObject;
        legacyWin.SetActive(false);

        //Find the camera force ratio object and keep it enabled.
        forceCamRatioObj = transform.Find("Force Camera Ratios").gameObject;
		
		//Load level change sound and victory jingle.
		levelChangeSnd = Resources.Load("SFX/TeleportSFX") as AudioClip;
		victoryJingle = Resources.Load("SFX/Victory Jingle") as AudioClip;
        menuSelectSound = Resources.Load("SFX/menu select") as AudioClip;
        menuActionSound = Resources.Load("SFX/Button Click") as AudioClip;

		//Bulet hell timer object.
		timeObj = transform.Find("Time Alive").gameObject;
		timeObj.SetActive(currentMode == GameMode.BulletHell);

		//Find loop indicator arrows.
		GameObject loopHolderObj = transform.Find("Loop Indicators").gameObject;
		loopHolderObj.SetActive(currentMode == GameMode.BulletHell);
		xLeft = loopHolderObj.transform.Find("x left").gameObject;
		xRight = loopHolderObj.transform.Find("x right").gameObject;
		yUp = loopHolderObj.transform.Find("y up").gameObject;
		yDown = loopHolderObj.transform.Find("y down").gameObject;
		xLeft.GetComponent<MeshRenderer>().material.color = xCol; //Set color of loop indicators.
		xRight.GetComponent<MeshRenderer>().material.color = xCol;
		yUp.GetComponent<MeshRenderer>().material.color = yCol;
		yDown.GetComponent<MeshRenderer>().material.color = yCol;

		//Find the laser reticle.
		laserReticle = transform.Find("Laser Reticle").gameObject;
		laserReticle.SetActive(currentMode == GameMode.BulletHell);
		reticlePuppet = laserReticle.transform.Find("puppet").gameObject;

		//Find and disable the no loop wall batch.
		noLoopWallBatch = transform.Find("No Loop Walls").gameObject;
        Game.noLoopBlockObj = noLoopWallBatch;
		noLoopWallBatch.SetActive(false);

        //Load the party batch object and spawn it in if needed.
        if (Game.partyMode) {
            partyBatchObj = Resources.Load("Party Batch") as GameObject;
            Instantiate(partyBatchObj, new Vector3(0f, 0f, -7.5f), Quaternion.identity);
        }

        //Load the smiley object if needed.
        if (Game.smileyMode) { smileyObj = Resources.Load("Smiley") as GameObject; }
      
		//Apply the determined theme for the level.
		SetTheme();

		//Which things need to be set specifically for each game mode in Awake.
		switch(currentMode){
			//Adventure Mode independent code for the Awake function.
			case GameMode.Adventure:
				level = 0; //You start at level 0 in adventure mode.

				levelToLoadOnDie = 8; //Load scene 8 when you die on adventure mode.
				
				playerPrefab = Resources.Load("New Ship") as GameObject; //The player object used in this game mode is loaded here.

				//Finding the container that holds every wallset for each zone.
				wallSetHolderCurrentlyUsed = adventureWallHolder;
				foreach (Transform child in wallSetHolderCurrentlyUsed.transform) {
					zoneWallHolders.Add(child.gameObject);
				}
		        //Setting the wallset of the current zone to be used and active.
		        for(int i = 0; i < zoneWallHolders.Count; i++) {
		            zoneWallHolders[i].SetActive(i == themeNum);
		            if(i == themeNum) {
		                wallSetHolderCurrentlyUsed = zoneWallHolders[i];
		            } else {
                        Destroy(zoneWallHolders[i]); //Destroy unused wall holders.
                    }
		        }

		        //Now that we have the wallset master holder, find each subset of walls and add it to list, these are what changes out as the player progresses through the zone.
		        foreach(Transform child in wallSetHolderCurrentlyUsed.transform) {
		            wallSets.Add(child.gameObject);
		        }
				for(int i = 0; i < wallSets.Count; i++){
					if(wallSets[i].name == level.ToString()) {
						wallSets[i].SetActive(true);
					} else {
						wallSets[i].SetActive(false);
					} 
				}
				
				//Find and set up the boss splashscreen values. (Textures, names, etc.)
		        SetBossSplashScreen(themeNum);	
				break;
			case GameMode.BulletHell:
                Game.beenToBulletHell = true;
				level = 0;
				levelToLoadOnDie = 5; //Load scene 5 when you die in bullet hell mode.
				playerPrefab = Resources.Load("New Bullet Hell Ship") as GameObject;
				singleBullet = Resources.Load("Bullet Hell Bullets/New Bullet Hell Bullet") as GameObject;
				laserSound = Resources.Load("SFX/Laser") as AudioClip;
				bulletHellMoneyBag = Resources.Load("Bullet Hell Bullets/Bullet Hell Money Bag") as GameObject;
				reticleShootEffect = Resources.Load("Reticle Shoot") as GameObject;
				for (int i = 0; i < bulletBatchCount; i++){
					GameObject loadedBatch = Resources.Load("Bullet Hell Bullets/Bullet Batches/Batch " + i.ToString()) as GameObject;
					bulletBatches.Add(loadedBatch);
				}
				moneyBagsCollected = 0; //Reset the money bags collected to 0.
				break;
			//Final Boss independent code for the Awake function.
			case GameMode.FinalBoss:
				level = 0;//You are always at layer 0 at the final boss.

                levelToLoadOnDie = 8; //Load scene 8 when you die on the final boss.
				
				playerPrefab = Resources.Load("New Ship") as GameObject; //The player object used in this game mode is loaded here.
               
		        SetBossSplashScreen(themeNum);  //Find and set up the boss splashscreen values. (Textures, names, etc.)
				break;
			case GameMode.Legacy:
                Game.beenToLegacy = true;
				//If you are playing legacy mode, you start at level 50 and work your way down.
				level = 50;
				levelToLoadOnDie = 1; //Load scene 1 (The menu) when you die in legacy mode.
				playerPrefab = Resources.Load("Ship") as GameObject;

				//Setting up the walls for legacy mode.
				wallSetHolderCurrentlyUsed = legacyWallHolder;
				foreach (Transform child in wallSetHolderCurrentlyUsed.transform) {
					wallSets.Add(child.gameObject);
				}
				for(int i = 0; i < wallSets.Count; i++){
					if(wallSets[i].name == level.ToString()) {
						wallSets[i].SetActive(true);
					} else {
						wallSets[i].SetActive(false);
					}
				}

				//Populate the list of music channels for legacy mode.
				foreach (Transform child in legacyMusicHolder.transform) {
					musicChannels.Add(child.gameObject);
				}
				break;
			case GameMode.LegacyBulletHell:
                Game.beenToLegacyBulletHell = true;
				level = 0; 
				levelToLoadOnDie = 5; //Load scene 5 when you die in bullet hell mode.
				playerPrefab = Resources.Load("Bullet Hell Ship") as GameObject;
				bHellBulObj = Resources.Load("Enemies/Bullet Hell Bullet") as GameObject;
				break;
		}

        //Spawn the player
		Game.playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Game.playerObj.name = "Player";

        //Find background quad that will be colored for legacy mode.
        if(currentMode == GameMode.LegacyBulletHell || currentMode == GameMode.Legacy){
			backgroundQuad = activeBackground.transform.Find("puppet").gameObject;
		}
    }
	
	//On the first frame, but after awake.
	void Start () {
		
        //Set music audio channels for legacy mode.
        if(currentMode == GameMode.Legacy){
        	SetLagacyMusic();
    	} else { //If it isn't legacy mode, apply the proper tunes.
    		SetMusic();
    	}

		//Start off audio samples size.
		samples = new float[qSamples];
    
        //Find out if the player has the minimum stats.
        playerMinStats = (
            Game.healthRank <= 0 &&
            Game.shotSpeedRank <= 0 &&
            Game.speedRank <= 0 && 
            Game.shotSplitRank <= 0
        );
	}
	


	// Update is called once per frame
	void Update () {
		//When to have light shafts enabled.
		LightEffectEnableHandler();

		//This function handles the transition between layers. (Screenshotting, tweening, moving player to nearest empty space.)
		LayerTransitionHandler();
		
		//Managing the pause menu here.
		PauseMenuHandler();

		//Managing the hearts and laser counter.
		HeartLaserCounterHandler();
        
		//Musically interactive fancy stuff.
		MusicReactionHandler();

		//What happens when the player dies.
		DeathHandler();

		//Looking for controller!
        Game.ControllerCheck();
        
		//You can shoot yourself when there are no enemies.
		if(currentMode == GameMode.Legacy){ 
            Game.canShootSelf = (Game.enemyHolder.transform.childCount == 0);
        } else {
            Game.canShootSelf = (Game.enemyHolder.transform.childCount == 0 && level < wallSets.Count - 1);
        }

		//Game management specific to the game mode.
		GameModeSpecificManagement();

        //Returning to menu from the pause menu.
        if (goingBackToMenu) { BackToMainMenu(); }

        //Determine cursor visibility.
        CursorVision();

        //Managing SMILEY MODE!
        if (Game.smileyMode) { SmileyModeHandler(); }

        //Make the camratio obj always active.
        forceCamRatioObj.SetActive(true);

        //Do the debugs.
		if(Game.debug){ Debuggo(); }
    }

    //Setting the theme of the level.
	private void SetTheme() {

		switch (currentMode){
			case GameMode.Adventure:
				if(zTheme == ZoneThemes.RedZone1){
					Game.backColor = Color.red;
					Game.frontColor = Color.black;
					Game.bulletColor = Color.white;
		            Game.atWaterLevel = false;
					themeNum = 0;
				} else if(zTheme == ZoneThemes.YellowZone2){
					Game.backColor = Color.yellow;
					Game.frontColor = Color.black;
					Game.bulletColor = Color.red;
		            Game.atWaterLevel = false;
					themeNum = 1;
				} else if(zTheme == ZoneThemes.GreenZone3){
					Game.backColor = Color.black;
					Game.frontColor = Color.green;
					Game.bulletColor = Color.white;
		            Game.atWaterLevel = false;
					themeNum = 2;
				} else if(zTheme == ZoneThemes.BlueZone4){
					Game.backColor = new Color(0.22f, 0.22f, 1f, 1f);
					Game.frontColor = Color.black;
					Game.bulletColor = Color.white;
		            Game.atWaterLevel = true;
					themeNum = 3;
				} else if(zTheme == ZoneThemes.PurpleZone5){
					Game.backColor = Color.magenta;
					Game.frontColor = Color.black;
					Game.bulletColor = Color.white;
		            Game.atWaterLevel = false;
					themeNum = 4;
				} else if(zTheme == ZoneThemes.WhiteZone6){
					Game.backColor = Color.white;
					Game.frontColor = Color.black;
					Game.bulletColor = Color.magenta;
		            Game.atWaterLevel = false;
					themeNum = 5;
				} else if(zTheme == ZoneThemes.FinalBossZone7){
					Game.backColor = Color.black;
					Game.frontColor = Color.white;
					Game.bulletColor = Color.grey;
		            Game.atWaterLevel = false;
					themeNum = 6;
				}
			break;
			case GameMode.BulletHell:
				themeNum = 7;
				Color pink = new Color(1f, (1f/3f), 1f, 1f);

				//Setting the theme color.
				Color[] cPool = {
					Color.cyan,
					Color.green,
					Color.magenta,
					Color.red,
					Color.yellow,
                    Color.grey,
					pink,
				};
				int chosenColor = UnityEngine.Random.Range(0, cPool.Length);
				Game.backColor = cPool[chosenColor];
				Game.frontColor = Color.black;
				Game.bulletColor = Color.white;
				break;
			case GameMode.FinalBoss:
				themeNum = 6;
				Game.backColor = Color.black;
				Game.frontColor = Color.white;
				Game.bulletColor = Color.grey;
		        Game.atWaterLevel = false;
				break;
			case GameMode.Legacy:
				themeNum = 9;
				Game.backColor = Color.white;
				Game.frontColor = Color.black;
				Game.bulletColor = Color.black;
				break;
			case GameMode.LegacyBulletHell:
				themeNum = 9;
				Game.backColor = Color.red;
				Game.frontColor = Color.cyan;
				Game.bulletColor = Color.black;
			break;
		}
		
		//Applying camera and fog color.
        bool fogOn = (currentMode != GameMode.FinalBoss);
        RenderSettings.fog = fogOn;
		RenderSettings.fogColor = Game.backColor;
		backgroundCamera.GetComponent<Camera>().backgroundColor = Game.backColor;
		backgroundCamera.GetComponent<SunShafts>().sunColor = Game.backColor;
		
		//Enable and disable intended background.
		for(int i=0; i < themeZoneLength; i++){
			if(i == themeNum){
				themeZones[i].SetActive(true);
				activeBackground = themeZones[i];
                if(themeNum == 6) {
                    //Debug.Log("Got Stars!");
                    Game.finalBossStars = themeZones[i].transform.Find("stars").gameObject;
                }
			} else {
				themeZones[i].SetActive(false);
			}
		}
	}

	//This function determines if the light effects need to be enabled at the moment.	
    private void LightEffectEnableHandler(){ //They are enabled if they are turned on in the settings (Game.lightEffects = true)
    	if(currentMode == GameMode.Legacy || currentMode == GameMode.LegacyBulletHell){
    		backgroundCamera.GetComponent<SunShafts>().enabled = false;
    		Camera.main.gameObject.GetComponent<BloomOptimized>().enabled = false;
    	} else {
	    	//Sun shafts are enabled if the theme is not console green, final boss or white adventure zones.   	
	    	backgroundCamera.GetComponent<SunShafts>().enabled = (Game.lightEffects && themeNum != 2 && themeNum != 5 && themeNum != 6 && themeNum != 9);
	    	//Light bloom is enabled if it the game is not in legacy mode.
	    	if(Camera.main.gameObject != null){
				Camera.main.gameObject.GetComponent<BloomOptimized>().enabled = (Game.lightEffects && themeNum != 9);
			}
		}
    }

    //This function handles the transition between layers. (Screenshotting, tweening, moving player to nearest empty space.)
	private void LayerTransitionHandler(){
		if(transCounter > 0){ //When the transition counter is above zero...
			transCounter -= Time.deltaTime; //Subtract the transition counter by delta time.
			Game.canShoot = false; //You can not shoot during transition period.
			Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture(); //Take a screenshot of the screen.
			screenGrab.GetComponent<MeshRenderer>().material.mainTexture = screen; //Apply the screenshot as a texture on the screengrab quad.
			Destroy(screen, transTime + 0.1f); //Destroy the texture when the transition ends.
			for(int i = 0; i < wallSets.Count; i++){ //For every wallset...
				if(wallSets[i] != null) { //If the wallset is not 'null'...
                    if(i < level && currentMode != GameMode.Legacy) { //If the wallset won't ever be needed again...
                        Destroy(wallSets[i]); //Destroy the wallset.
                    } else { //If the wallset will be needed...
					    if(wallSets[i].name == level.ToString()) { //If the wallset number name matches the current level.
						    wallSets[i].SetActive(true); //Enable it.
					    } else { //If the wallset number is not matching the current level BUT is ahead...
						    wallSets[i].SetActive(false);//Keep the wall set inactive.
					    }
                    }
                }
			}
			if(!scanned){//If you haven't scanned the A* yet...
                nearestEmptySpot = Vector2.zero; //Reset the nearest empty spot variable.
                nearestPosDistance = 500f; //Reset this position too.
				emptySpots.Clear(); //Clear out the list of empty spaces.
				MovePlayerToClearSpace(); //This function keeps the player from being stuck in a wall.
				AstarPath.active.Scan(); //Scan that A* !
				scanned = true;//You have scanned for a frame already, do not scan anymore.
			}
			screenGrab.SetActive(true); //The screen grab quad should be enabled during transition.
		} else {
			Game.canShoot = true; //You can shoot when not in transition.
			screenGrab.SetActive(false); //Hide the screen grab.
			screenGrab.transform.localScale = screenGrabStartScale; //Reset the screen grab to it's start scale.
		}
	}

	//This function handles how the pause menu works.
	private void PauseMenuHandler(){
		//Pausing game.
		if(Input.GetButtonDown("Pause") && !showingBossSplashscreen && !victory){ paused = !paused; }
		
        //If you press the "B" button when having the pause menu open with the gamepad, that also closes it.
        if(paused && Input.GetButtonDown("Cancel")) { paused = false; }

		//If you are dead, you cannot pause the game.
		if(dead) { paused = false; }

		//When paused...
		if(paused && !goingBackToMenu){
			targetCamPos = pauseCamPos;
			targetCoverPosition = pauseCoverPosition;
			Time.timeScale = 0f;
            if (victory) { paused = false; } //You REALLY shouldn't be paused at the victory screen.
		} else { //When not paused...
			targetCamPos = unPauseCamPos;
			targetCoverPosition = unPauseCoverPosition;
			if(!goingBackToMenu){ Time.timeScale = 1f; } else { Time.timeScale = 0f; }
		}
		
		//Move camera and color the cover based on paused or not.
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 20f * Time.unscaledDeltaTime);
        waterCamera.transform.position = mainCamera.transform.position;
		pauseMenu.transform.position = Vector3.Lerp(pauseMenu.transform.position, targetCoverPosition, 20f * Time.unscaledDeltaTime);

        //When the gamepad selector should be visible.
        gamePadPauseMenuSelectorObj.SetActive(Game.usingController);

        //Controlling the menu with a gamepad.
        if (Game.usingController && paused) {
            //Positioning the gamepad selector.
            Vector3[] v0 = {
                new Vector3(10f, 8.9f, 0f),
                new Vector3(10f, 6.9f, 0f),
                new Vector3(10f, -0.1f, 0f),
                new Vector3(10f, -2.1f, 0f),
                new Vector3(10f, -4.1f, 0f),
                new Vector3(10f, -6.1f, 0f),
                new Vector3(10f, -8.1f, 0f),
                new Vector3(10f, -10.1f, 0f),
            };
            gamePadPauseMenuSelectorObj.transform.localPosition = v0[pauseMenuIndex];

            //moving with da stick.
            float f0 = Input.GetAxisRaw("Vertical [Gamepad]");
            float f1 = Input.GetAxisRaw("Horizontal [Gamepad]");
            float threshHold = 0.5f;
            if(menuSelectCounter > 0) {
                menuSelectCounter -= Time.unscaledDeltaTime;
            } else {
                if (f0 < -threshHold && pauseMenuIndex > 0) {
                    pauseMenuIndex--;
                    PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                    menuSelectCounter = menuSelectTime;
                }
               	if (f0 > threshHold && pauseMenuIndex < 7) {
                    pauseMenuIndex++;
                    PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                    menuSelectCounter = menuSelectTime;
                }
                if(f1 > threshHold) {
                    switch (pauseMenuIndex) {
                        case 2: //SFX up.
                            if(Game.soundVolume < 1f){
                                Game.soundVolume = Game.soundVolume + 0.1f; 
                                PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                                menuSelectCounter = menuSelectTime;
                            }
                            break;
                        case 3: //Music Up.
                            if(Game.musicVolume < 1f){ 
                                Game.musicVolume = Game.musicVolume + 0.1f;
                                PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                                menuSelectCounter = menuSelectTime;
                            }
                            break;
                    }
                }
                if(f1 < -threshHold) {
                    switch (pauseMenuIndex) {
                        case 2: //SFX Down.
                            if(Game.soundVolume > 0f){ 
                                Game.soundVolume = Game.soundVolume - 0.1f;
                                PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                                menuSelectCounter = menuSelectTime;
                            }
                            break;
                        case 3: //Music Down.
                            if(Game.musicVolume > 0f){ 
                                Game.musicVolume = Game.musicVolume - 0.1f; 
                                PlaySound.NoLoopRandomPitch(menuSelectSound, 1f, 1.5f);
                                menuSelectCounter = menuSelectTime;
                            }
                            break;
                    }
                }
            }
            //Pressing that lovely A button.
            if(Input.GetButtonDown("Submit") && pauseMenuIndex != 2 && pauseMenuIndex != 3){
                PauseMenuAction(pauseMenuIndex); 
            }
        }
	}

    //The actions of the pause menu.
    public void PauseMenuAction(int i) {
        PlaySound.NoLoop(menuActionSound);
        switch (i) {
            case 0: //Resume.
                paused = false;
                break;
            case 1: //Back to menu.
                iTween.MoveTo(swiper, iTween.Hash(
                    "x", 0f,
                    "y", 0f,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.easeInOutSine,
                    "looptype", iTween.LoopType.none,
                    "ignoretimescale", true
                ));
                goingBackToMenu = true;
                break;
            case 4: //Light Effects.
                Game.lightEffects = !Game.lightEffects;
                break;
            case 5: //Vsync.
                Game.vSync = !Game.vSync;
                break;
            case 6: //Fullscreen.
                Game.isFullscreen = !Game.isFullscreen;
                break;
            case 7: //Speech.
                Game.CheckAchievements(33);
            	Game.speechSounds = !Game.speechSounds;
            	break;
        }
        if(i == 5 || i == 6) {
            Game.ApplySettings();
        }
    }

    //Going back to the main menu from the pause menu.
    private void BackToMainMenu() {
        backToMenuTime -= Time.unscaledDeltaTime;
        if(backToMenuTime <= 0) {
            SceneManager.LoadScene(1);
        }
    }

	//This handles how hearts work. (Basically how many hearts to show based on player health.)
	private void HeartLaserCounterHandler(){
		//Heart activation uses different numbers based on different gamemodes...
		switch(currentMode){
			case GameMode.Adventure: case GameMode.FinalBoss://What hearts to make active for adventure mode.
				for(int i = 0; i < hearts.Count; i++){
					if(dead){
						hearts[i].SetActive(false);
					} else {
						if(i < Game.newCurrentHealth && !showingBossSplashscreen && !victory && !goingBackToMenu){
							hearts[i].SetActive(true);
						} else {
							hearts[i].SetActive(false);
						}
					}
				}
				break;
			case GameMode.Legacy: //What hearts to make active for adventure mode.
				//What hearts to make active.
				for(int i = 0; i < hearts.Count; i++){
					if(i < Game.currentHealth && level > -1){
						hearts[i].SetActive(true);
					} else {
						hearts[i].SetActive(false);
					}
				}
				break;
		}

        //Laser activation for different modes.
        if(currentMode == GameMode.Adventure && Game.ownsLasers) {
            for(int i = 0; i < playerLasers.Count; i++){
			    if(dead){
				    playerLasers[i].SetActive(false);
			    } else {
				    if(i < Game.currentLasers && !showingBossSplashscreen && !victory && !goingBackToMenu){
					    playerLasers[i].SetActive(true);
				    } else {
					    playerLasers[i].SetActive(false);
				    }
			    }
		    }
        } else {
            for(int i = 0; i < playerLasers.Count; i++){
                playerLasers[i].SetActive(false);
            }
        }

        if (level == wallSets.Count - 1) { laserHolderObj.transform.localPosition = new Vector3( -2f ,0f, 0f); }
        else { laserHolderObj.transform.localPosition = Vector3.zero; }
	}

	//This is the music reactive stuff.
	private void MusicReactionHandler(){
		if(currentMode == GameMode.Adventure || currentMode == GameMode.BulletHell || currentMode == GameMode.FinalBoss){
			//Set the bard staticly so that the "React to Music" script can also read from it too!
			Game.bardObj = music;

			//Getting volume levels of bard, but only if it is playing.
			if(music.GetComponent<AudioSource>().isPlaying){
				music.GetComponent<AudioSource>().GetOutputData(samples, 0);
				float sum = 0;
				for (int i = 0; i < qSamples; i++){
					sum += samples[i] * samples[i];
				}
				rmsValue = Mathf.Sqrt(sum / qSamples);
				dbValue = 20 * Mathf.Log10(rmsValue / refValue);
				if (dbValue < -160) { dbValue = -160; }
				
				//scale the hearts.
				Vector3 heartScale = new Vector3(1+((rmsValue*intensity)/Game.lightValue), 1+((rmsValue*intensity)/Game.lightValue), 1);
				for (int i = 0; i < hearts.Count; i++){
					hearts[i].transform.localScale = heartScale;
				}

                //Scale the player lasers, same as the hearts.
			    for(int i = 0; i < playerLasers.Count; i++) {
                    playerLasers[i].transform.localScale = heartScale;
                }

				//Set bloom to the volume of the music.
				mainCamera.gameObject.GetComponent<BloomOptimized>().intensity = (rmsValue*intensity)/Game.lightValue;
			} else { //...when music is not playing.
				//scale the hearts.
				for (int i = 0; i < hearts.Count; i++){
					hearts[i].transform.localScale = Vector3.one;
				}
			}
		} else { //This is what we do the the heart scales when we are in a legacy mode.
			for (int i = 0; i < hearts.Count; i++){ //scale the hearts.
				hearts[i].transform.localScale = Vector3.one;
			}
		}
	}

	//Doing the debugs.
	private void Debuggo(){
        //Debug for skipping a layer.
		if(Input.GetKeyDown(KeyCode.O) && level < wallSets.Count - 1){
			foreach (Transform child in Game.enemyHolder.transform){ Destroy(child.gameObject); }
			ChangeLayer();
		}
	}

	//What happens when the player dies.
	private void DeathHandler(){
        if (dead){ 	//When you are dead...
            music.GetComponent<AudioSource>().pitch -= Time.deltaTime/2f;
            if(music.GetComponent<AudioSource>().pitch < 0) { music.GetComponent<AudioSource>().pitch = 0f; }
            deathTime -= Time.deltaTime;
            if(deathTime <= 1.1f && !playedSwipeTween) {
                iTween.MoveTo(swiper, iTween.Hash(
                    "x", 0f,
                    "y", 0f,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.easeInOutSine,
                    "looptype", iTween.LoopType.none
                ));
                playedSwipeTween = true;
            }
            if(deathTime <= 0f){
                Game.SaveGame();
                Game.levelDiedOn = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(levelToLoadOnDie);
            }
        }
	}

	//Game management specific to the game mode.
	private void GameModeSpecificManagement(){
		switch(currentMode){
			case GameMode.Adventure:
				//What to do when you first are at the boss.
				if(showingBossSplashscreen){
					splashCounter -= Time.unscaledDeltaTime;
					splashScreen.SetActive(true);
					if(splashCounter <= 0){
						showingBossSplashscreen = false;
					}
				}

                //Disable if is any mode other than adventure OR player does not own lasers.
                laserHolderObj.SetActive(currentMode == GameMode.Adventure && Game.ownsLasers);

				//Applying music volume.
		        if (!victory) {
		        	//If the music needs the bitcrush tunes...
                    if (needBitcrushTunes) {
                    	//Make music quieter as you approach the white zone boss.
                    	if(level >= 34){
                    		if(level == 34){ music.GetComponent<AudioSource>().volume = Game.musicVolume/2f; }
                    		if(level == 35){ music.GetComponent<AudioSource>().volume = Game.musicVolume/3f; }
                    		if(level == 36){ music.GetComponent<AudioSource>().volume = Game.musicVolume/4f; }
                    		if(level == 37){ music.GetComponent<AudioSource>().volume = Game.musicVolume/6f; }
                    		if(level == 38){ music.GetComponent<AudioSource>().volume = Game.musicVolume/7f; }
                    		if(level == 39){ music.GetComponent<AudioSource>().volume = Game.musicVolume/8f; }
                    		if(level == 40){ music.GetComponent<AudioSource>().volume = Game.musicVolume/9f; }
                    		if(level == 41){ music.GetComponent<AudioSource>().volume = Game.musicVolume/10f; }
                    		if(level == 42){ //This is how the music behaves at the white zone boss level.
                    			music.GetComponent<AudioSource>().volume = 0f;
                    		}
                    		bitCrushMusicObj.GetComponent<AudioSource>().volume = 0f;
                    	} else {
	                        if (Game.inDisruptor) { //Play the bitcrush music when in a disruptor...
	                            music.GetComponent<AudioSource>().volume = 0;
	                            bitCrushMusicObj.GetComponent<AudioSource>().volume = Game.musicVolume;
	                        } else { //Play the regular music when not in a disruptor.
	                            music.GetComponent<AudioSource>().volume = Game.musicVolume;
	                            bitCrushMusicObj.GetComponent<AudioSource>().volume = 0;
	                        }
	                    }
                    } else { //If the music does not need the bitcrush tunes...
                        music.GetComponent<AudioSource>().volume = Game.musicVolume;
                        bitCrushMusicObj.GetComponent<AudioSource>().volume = 0;
                    }
		        } else { //Fade out the music when the victory screen is showing.
		            float v = music.GetComponent<AudioSource>().volume;
		            v -= Time.deltaTime;
		            if (v <= 0f) { v = 0f; }
		            music.GetComponent<AudioSource>().volume = v;
                    bitCrushMusicObj.GetComponent<AudioSource>().volume = 0;
		        }

				//Showing and hiding the progress bar.
				if(Input.GetButton("Show Progress") && level < wallSets.Count - 1){
					targetProgressPos = showProgressPos;
				} else {
					targetProgressPos = hideProgressPos;
				}
				progress.transform.position = Vector3.Lerp(progress.transform.position, targetProgressPos, 11f * Time.unscaledDeltaTime);
				
				//Scaling the progress bar.
				float piece = (75f/wallSets.Count);
				float scale = (level+1)*piece;
		        if(scale == 0f) { scale = 0.1f; }//Stop that stupid annoying ass bug.
				progressScale.transform.localScale = new Vector3(scale, 1.2f, 1f);
				
				//Tracking the progress text.
				string s = " Layer " + (level+1).ToString() +"/"+wallSets.Count.ToString();
				if(level > wallSets.Count/2) {
					progressNumber.GetComponent<TextMesh>().anchor = TextAnchor.MiddleRight;
					progressNumber.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
				} else {
					progressNumber.GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
					progressNumber.GetComponent<MatchFrontColorScript>().matchBackInstead = true;
				}
				progressNumber.GetComponent<TextMesh>().text = s;
				
		        //Manage the custom color change of the Bullet Color for the White zone 6. (Scrolling from blue to majenta)
		        if(themeNum == 5 && !Game.brokeCore) {
		            if (!increasingRed) {
		                scrollingRedValue -= Time.deltaTime;
		                if(scrollingRedValue <= 0f) { increasingRed = true; }
		            } else {
		                scrollingRedValue += Time.deltaTime;
		                if(scrollingRedValue >= 1f) { increasingRed = false; }
		            }
		            if(scrollingRedValue > 1f) { scrollingRedValue = 1f; }
		            if(scrollingRedValue < 0f) { scrollingRedValue = 0f; }
		            Game.bulletColor = new Color(scrollingRedValue, 0f, 1f, 1f);
		        }

		        //Enable the victory object when you won the level!
		        victoryObj.SetActive(victory && !dead);

		        //what do when victory hmm?
		        if (victory && !dead){
                    //What level did you just beat?
                    switch (zTheme) {
                        case ZoneThemes.RedZone1:
                            Game.unlockedShop = true;
                            Game.beatFirstLevel = true;
                            break;
                        case ZoneThemes.YellowZone2:
                            Game.unlockedBulletHell = true;
                            Game.beatYellow = true;
                            break;
                        case ZoneThemes.GreenZone3:
                            Game.unlockedLegacy = true;
                            Game.unlockedLegacyBulletHell = true;
                            Game.beatGreen = true;
                            break;
                        case ZoneThemes.BlueZone4:
                            Game.beatBlue = true;
                            break;
                        case ZoneThemes.PurpleZone5:
                            Game.beatPurple = true;
                            break;
                        case ZoneThemes.WhiteZone6:
                            Game.beatWhite = true;
                            break;
                        case ZoneThemes.FinalBossZone7:
                            Game.beatFinalBoss = true;
                            break;
                    }

                    //If the steam is active...
                    if (Game.steamy) { 
                        CheckAdventureAchievements(); //Check for adventure achievements.
                    }

		            Game.SaveGame(); //Save that you beat that level, along with every other thingy.

		            //Subtract victory time.
		            victoryHoldTime -= Time.deltaTime;
		            
		            //When victory time left is below 2 seconds, play the swipe out animation. 
		            if(victoryHoldTime < 2f && !playedSwipeTween){
		                iTween.MoveTo(swiper, iTween.Hash(
		                    "x", 0f,
		                    "y", 0f,
		                    "time", 0.75f,
		                    "easetype", iTween.EaseType.easeInOutSine,
		                    "looptype", iTween.LoopType.none
		                ));
		                playedSwipeTween = true;
		            }

		            //When victory hold time is 0 or less, go back to menu.
		            if(victoryHoldTime <= 0f) {
		                SceneManager.LoadScene(1);
		            }
		        }

		        //When to enable the no looping wall batch.
		        Game.loopBlocked = (zTheme == ZoneThemes.WhiteZone6 && level > 22);
		        noLoopWallBatch.SetActive(Game.loopBlocked);

                //When to enable the final rising crystals.
                if(Game.risingCrystalObj != null) {
                    ParticleSystem pts = Game.risingCrystalObj.GetComponent<ParticleSystem>();
                    bool shouldEnablePTS = (zTheme == ZoneThemes.WhiteZone6 && level > 36);
                    if (shouldEnablePTS && !pts.isPlaying) { pts.Play(); }
                    if (!shouldEnablePTS && pts.isPlaying) { pts.Stop(); }
                }
                
                //Make everything spooky when you break the core.
                if(zTheme == ZoneThemes.WhiteZone6 && Game.brokeCore) {
                    float colorSmooth = 2f;
                    Game.bulletColor = Color.Lerp(Game.bulletColor, Color.red, colorSmooth*Time.deltaTime);
                    Game.backColor = Color.Lerp(Game.backColor, Color.black, colorSmooth*Time.deltaTime);
                    Game.frontColor = Color.Lerp(Game.frontColor, Color.white, colorSmooth*Time.deltaTime);
                    backgroundCamera.GetComponent<Camera>().backgroundColor = Game.backColor;
                }

                //When the glitter should be active.
                Game.enableGlitterTrail = (zTheme == ZoneThemes.WhiteZone6 && !Game.brokeCore);

				break;
			case GameMode.FinalBoss:

                //The no loop walls are active when game says so. (Controlled by Warden Boss script.)
                noLoopWallBatch.SetActive(Game.loopBlocked);

                if (Game.wardenDead) {
                    music.GetComponent<AudioSource>().volume -= Time.deltaTime/4f;
                    if(music.GetComponent<AudioSource>().volume <= 0) {
                        music.GetComponent<AudioSource>().volume = 0;
                    }
                } else {
                    music.GetComponent<AudioSource>().volume = Game.musicVolume;
                }

                //The bullet color changes too white at the bosses final phase.
                if (Game.atFinalBossFinalForm) {
                    Game.bulletColor = Color.white;
                } else {
                    Game.bulletColor = Color.grey;
                }
                    
				break;
			case GameMode.BulletHell:
				//Set position of background.
				Vector3 backPos = new Vector3(Game.playerObj.transform.position.x/15f, 0, Game.playerObj.transform.position.y/15f);
				activeBackground.transform.position = Vector3.Lerp(activeBackground.transform.position, backPos, 1f*Time.deltaTime);
				
				//Increase time alive.
				if(!dead){ timeAlive += Time.deltaTime; }
				TimeSpan timeSpan = TimeSpan.FromSeconds(timeAlive);
				string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
				timeObj.GetComponent<TextMesh>().text = timeText;
				Game.maxTimeAlive = timeAlive;
				if(Game.maxTimeAlive > Game.highMaxTimeAlive){
					Game.highMaxTimeAlive = Game.maxTimeAlive;
				}
				
				//Set music volume.
				music.GetComponent<AudioSource>().volume = Game.musicVolume;

				//spawning MoneyBags.
				float moneyBagSpawnTime = 15f;
				if(spawnMoneyBagCounter > 0){ 
					spawnMoneyBagCounter -= Time.deltaTime;
				} else { 
					int randDir = UnityEngine.Random.Range(0, 5);
					int xSpawn = 40;
					int ySpawn = 25;
					float xRandomRange = 32f;
					float yRandomRange = 17.5f;
				
					switch (randDir){
						case 0: //Going up.
							moneyBagSpawnPosition = new Vector3(UnityEngine.Random.Range(-xRandomRange, xRandomRange), -ySpawn, 0);
							GameObject go0 = Instantiate(bulletHellMoneyBag, moneyBagSpawnPosition, Quaternion.identity) as GameObject;
							go0.transform.parent = Game.bulletHolder.transform;
							BulletHellMoneyBagScript bhmbs0 = go0.GetComponent<BulletHellMoneyBagScript>();
							bhmbs0.goWhere = BulletHellMoneyBagScript.MoveType.GoUp;
							break;
						case 1: //Going down.
							moneyBagSpawnPosition = new Vector3(UnityEngine.Random.Range(-xRandomRange, xRandomRange), ySpawn, 0);
							GameObject go1 = Instantiate(bulletHellMoneyBag, moneyBagSpawnPosition, Quaternion.identity) as GameObject;
							go1.transform.parent = Game.bulletHolder.transform;
							BulletHellMoneyBagScript bhmbs1 = go1.GetComponent<BulletHellMoneyBagScript>();
							bhmbs1.goWhere = BulletHellMoneyBagScript.MoveType.GoDown;
							break;
						case 2: //Going left.
							moneyBagSpawnPosition = new Vector3(-xSpawn, UnityEngine.Random.Range(-yRandomRange, yRandomRange), 0);
							GameObject go2 = Instantiate(bulletHellMoneyBag, moneyBagSpawnPosition, Quaternion.identity) as GameObject;
							go2.transform.parent = Game.bulletHolder.transform;
							BulletHellMoneyBagScript bhmbs2 = go2.GetComponent<BulletHellMoneyBagScript>();
							bhmbs2.goWhere = BulletHellMoneyBagScript.MoveType.GoLeft;
							break;
						case 3: //Going right.
							moneyBagSpawnPosition = new Vector3(xSpawn, UnityEngine.Random.Range(-yRandomRange, yRandomRange), 0);
							GameObject go3 = Instantiate(bulletHellMoneyBag, moneyBagSpawnPosition, Quaternion.identity) as GameObject;
							go3.transform.parent = Game.bulletHolder.transform;
							BulletHellMoneyBagScript bhmbs3 = go3.GetComponent<BulletHellMoneyBagScript>();
							bhmbs3.goWhere = BulletHellMoneyBagScript.MoveType.GoRight;
							break;
					}
					spawnMoneyBagCounter = moneyBagSpawnTime;
				}

				//Spawning bulletbatches.
				if(bulletBatchCounter > 0){
					bulletBatchCounter -= Time.deltaTime;
				} else {
					GameObject o = Instantiate(bulletBatches[UnityEngine.Random.Range(0, bulletBatches.Count)], Vector3.zero, Quaternion.identity) as GameObject;
					o.transform.parent = Game.bulletHolder.transform;
					Destroy(o, 2f);//Destroy the bullet batch after 2 seconds, it will be empty at this point.
					bulletBatchCounter = bulletBatchSpawnTime;
				}
				
				//Spawning single bullets.
				if(singleBulletCounter > 0){
					singleBulletCounter -= Time.deltaTime;
				} else {
					GameObject o = Instantiate(singleBullet, new Vector3(UnityEngine.Random.Range(-36f, 36f), UnityEngine.Random.Range(-21f, 21f), 0), Quaternion.identity) as GameObject;
					o.transform.parent = Game.bulletHolder.transform;
					NewBulletHellBulletScript b = o.GetComponent<NewBulletHellBulletScript>();
					b.normalizeMe = true;
					Vector2 v = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
					b.velocity = v.normalized;
					b.canLoop = false;
					singleBulletCounter = singleBulletSpawnTime;
				}
				
				//Bullet spawn time gets faster and faster.
				if(timeAlive < 30f){
					singleBulletSpawnTime = 3f;
					bulletBatchSpawnTime = 6.5f;
				} else if(timeAlive > 30f && timeAlive < 60f){
					singleBulletSpawnTime = 2.5f;
					bulletBatchSpawnTime = 6f;
				} else if(timeAlive > 60f && timeAlive < 90f){
					singleBulletSpawnTime = 2f;
					bulletBatchSpawnTime = 5.5f;
				} else if(timeAlive > 90f && timeAlive < 120f){
					singleBulletSpawnTime = 1.5f;
					bulletBatchSpawnTime = 5f;
				} else if(timeAlive > 120f && timeAlive < 150f){
					singleBulletSpawnTime = 1f;
					bulletBatchSpawnTime = 4.5f;
				} else if(timeAlive > 150f && timeAlive < 180f){
					singleBulletSpawnTime = 0.5f;
					bulletBatchSpawnTime = 4f;
				}  else if(timeAlive > 180f){
					singleBulletSpawnTime = 0.25f;
					bulletBatchSpawnTime = 3.5f;
				}
				
				//Setting position of loop indicators.
				yDown.transform.position = new Vector3(Game.playerObj.transform.position.x, -19f, 0f);
				yUp.transform.position = new Vector3(Game.playerObj.transform.position.x, 19f, 0f);
				xLeft.transform.position = new Vector3(-34.7f, Game.playerObj.transform.position.y, 0f);
				xRight.transform.position = new Vector3(34.7f, Game.playerObj.transform.position.y, 0f);
				
				//Setting Color of loop indicators.
				float xd = Vector3.Distance(new Vector3(Game.playerObj.transform.position.x, 0, 0), Vector3.zero);
				float yd = Vector3.Distance(new Vector3(0, Game.playerObj.transform.position.y, 0), Vector3.zero);
				//Debug.Log("XD: " + xd.ToString() + " YD: " + yd.ToString());
				if(xd > 20){ xTargetColor = Color.white; } else { xTargetColor = new Color(1f, 1f, 1f, 0f); }
				if(yd > 10){ yTargetColor = Color.white; } else { yTargetColor = new Color(1f, 1f, 1f, 0f); }
				xCol = Color.Lerp(xCol, xTargetColor, 5f * Time.deltaTime);
				yCol = Color.Lerp(yCol, yTargetColor, 5f * Time.deltaTime);
				
				//Apply color of loop indicators.
				xLeft.GetComponent<MeshRenderer>().material.color = xCol;
				xRight.GetComponent<MeshRenderer>().material.color = xCol;
				yUp.GetComponent<MeshRenderer>().material.color = yCol;
				yDown.GetComponent<MeshRenderer>().material.color = yCol;
				
				//When paused...
				if(paused && !goingBackToMenu){
					Time.timeScale = 0f;
					targetCamPos = pauseCamPos;
					targetCoverPosition = pauseCoverPosition;
				} else { //When not paused...
					if(!goingBackToMenu){ Time.timeScale = 1f; } else { Time.timeScale = 0f; }
					targetCamPos = unPauseCamPos;
					targetCoverPosition = unPauseCoverPosition;
				}
				
				//Move camera and color the cover based on paused or not.
				mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 20f * Time.unscaledDeltaTime);
				pauseMenu.transform.position = Vector3.Lerp(pauseMenu.transform.position, targetCoverPosition, 20f * Time.unscaledDeltaTime);
				
				//Here we will manage the laser killing reticle.
				if(Game.playerObj != null && !reticleKilledPlayer && !dead){
					Vector2 pv = Game.playerObj.GetComponent<Rigidbody2D>().velocity;
					float mt = 2.2f;
					float timeTillReveal = 4.5f;
					float timeTillPew = 7f;
					bool moving = (pv.x > mt || pv.y > mt || pv.x < -mt || pv.y <-mt);
					//Debug.Log("Moving: " + moving.ToString());
					if(moving){ //if you are moving, keep the counter locked at 0.
						noMoveCounter = 0f;
					} else {
						laserReticle.transform.position = new Vector3(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y, Game.playerObj.transform.position.z + 5);
					 	noMoveCounter += Time.deltaTime;
					}
					if(noMoveCounter >= timeTillReveal){ reticleTargetColor = Color.white; }
				 	else { reticleTargetColor = new Color(1, 1, 1, 0); }
				 	float smooth = 10f;
				 	reticlePuppet.GetComponent<MeshRenderer>().material.color = Color.Lerp(reticlePuppet.GetComponent<MeshRenderer>().material.color, reticleTargetColor, smooth*Time.deltaTime);
					if(noMoveCounter >= timeTillPew){
						PlaySound.NoLoop(laserSound);
						Vector3 reticleShootPos = new Vector3(laserReticle.transform.position.x, laserReticle.transform.position.y, Game.playerObj.transform.position.z - 4);
						Instantiate(reticleShootEffect, reticleShootPos, Quaternion.identity);
						Game.playerObj.GetComponent<BulletHellShipScript>().KillMe();
						reticleKilledPlayer = true;
					}
				}
				break;
			case GameMode.Legacy:

				if(transCounter > 0){
					numObj.GetComponent<TextMesh>().text = level.ToString();
					if(sat == 0){ numObj.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1f/3f); }
					if(sat == 1){ numObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f/3f); }
				} else {
					if(level > -1){ Game.canShoot = true; }
				}

				//Manage the legacy audio channels.
				if(level > 0){
					musicChannels[0].GetComponent<AudioSource>().volume = Game.musicVolume; 
					if(level <= 40 && level > 1){musicChannels[1].GetComponent<AudioSource>().volume = Game.musicVolume; } else { musicChannels[1].GetComponent<AudioSource>().volume = 0f; }
					if(level <= 30 && level > 2){ musicChannels[2].GetComponent<AudioSource>().volume = Game.musicVolume; } else { musicChannels[2].GetComponent<AudioSource>().volume = 0f; }
					if(level <= 20 && level > 3){ musicChannels[3].GetComponent<AudioSource>().volume = Game.musicVolume; } else { musicChannels[3].GetComponent<AudioSource>().volume = 0f; }
				} else {
					musicChannels[0].GetComponent<AudioSource>().volume = 0f;
					musicChannels[1].GetComponent<AudioSource>().volume = 0f;
					musicChannels[2].GetComponent<AudioSource>().volume = 0f;
					musicChannels[3].GetComponent<AudioSource>().volume = 0f;
				}

				//If the win condition for legacy mode has been met...
				if(level <= -1){
					endTime -= Time.deltaTime; //The time it takes to end goes down over time.
					Game.speed = 0f; //The player can not move.
					Game.canShoot = false; //The player can not shoot.
					tripCounter -= Time.deltaTime; //The trip counter goes down over time. (How often the colored squares spawn.)
					if(tripCounter <= 0f){//If the trip counter goes below 0...
						GameObject trip = Resources.Load("Trip") as GameObject; //Load the trip square object.
						GameObject tripInst = Instantiate(trip, new Vector3(0f, 0f, -9f), Quaternion.identity) as GameObject; //Create the trip square.
						Color col = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f); //Create random color.
						Camera.main.backgroundColor = col; //Callor the camera to the randomly generated color.
						tripInst.GetComponent<MeshRenderer>().material.color = col; //Apply the random color to the trip square.
						float tripTime = 1f; //This float dictates how long it takes for a trip square to scale down to 0.
						iTween.ScaleTo(tripInst, iTween.Hash( //Scale the trip square to 0.
							"x", 0f, 
							"y", 0f,
							"z", 0f,
							"easetype", iTween.EaseType.linear,
							"time", 1f,
							"delay", endTime/5f 
						));
						iTween.MoveTo(tripInst, iTween.Hash( //Move the trip square along the z axis. (So that new spawned squares are in front of older ones.)
							"z", -11f,
							"easetype", iTween.EaseType.linear,
							"time", 1f,
							"delay", endTime/5f
						));
						Destroy(tripInst, (tripTime - endTime/5f)+(endTime+5f));//Destroy the trip square when it won't be visible.
						tripCounter = endTime/5f;//This makes the trip squares spawn faster and faster as the end of the victory animation gets closer to the end.
					}
					if(endTime <= -1){ 
						if(!startedWin){
							legacyWin.SetActive(true);
							iTween.ScaleTo(legacyWin, iTween.Hash(
								"x", 1f, 
								"y", 1f,
								"z", 1f,
								"easetype", iTween.EaseType.linear,
								"time", 1f 
							));
							Game.CheckAchievements(40); //Check legacy mode achievement for completion.
							if(Game.currentHealth >= 5){ Game.CheckAchievements(41); } //Check achievement for beating legacy mode with all 5 hearts.
							startedWin = true;
						} else {
							winTime -= Time.deltaTime;
							if(winTime <= -0.5f && !playedJingle){
								PlaySound.NoLoop(victoryJingle);
								playedJingle = true;
							}
							if(winTime <= -5 && !playedSwipeTween){	
								Vector3 swiperPos = swiper.transform.position;
								swiperPos.z = -28.5f;
								swiper.transform.position = swiperPos;
								iTween.MoveTo(swiper, iTween.Hash(
				                    "x", 0f,
				                    "y", 0f,
				                    "time", 0.75f,
				                    "easetype", iTween.EaseType.easeInOutSine,
				                    "looptype", iTween.LoopType.none
				                ));
								playedSwipeTween = true;
							}
							if(winTime <= -6){
								SceneManager.LoadScene(1);
							}
						}
					}
				}
				break;
			case GameMode.LegacyBulletHell:
					//Timer counter doody.
					numObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f/3f);
					if(!dead){ timeAlive += Time.deltaTime; }
					int secs = Mathf.RoundToInt(timeAlive);
					numObj.GetComponent<TextMesh>().text = secs.ToString();
					Game.maxTimeAliveLegacy = secs;
					if(secs > Game.highMaxTimeAliveLegacy){
						Game.brokeRecord = true;
						Game.highMaxTimeAliveLegacy = secs;
					}

					//Spawning of bullets here.
					legacyBulletCounter -= Time.deltaTime;
					if(legacyBulletCounter <= 0){
						Vector3 bPos = Vector3.zero;
						int screenSide = UnityEngine.Random.Range(0, 4);
						if(screenSide == 0){bPos = new Vector3(UnityEngine.Random.Range(-37f, 37f), 21, 0); } 	//Top
						if(screenSide == 1){bPos = new Vector3(UnityEngine.Random.Range(-37f, 37f), -21, 0); }	//Bottom
						if(screenSide == 2){bPos = new Vector3(-37, UnityEngine.Random.Range(-21f, 21f), 0); }	//Left
						if(screenSide == 3){bPos = new Vector3(37, UnityEngine.Random.Range(-21f, 21f), 0); }	//Right
						GameObject bHellBulInst = (GameObject) Instantiate(bHellBulObj, bPos, Quaternion.identity);
						Quaternion rot = Quaternion.identity;
						Vector3 vRot = new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f));
						rot.eulerAngles = vRot;
						bHellBulInst.transform.localRotation = rot;
						bHellBulInst.transform.parent = Game.bulletHolder.transform;
						legacyBulletCounter = legacyBulletTime;
					}

					//Set music volume.
					music.GetComponent<AudioSource>().volume = Game.musicVolume;

					//Colors are here.
					float slowdown = 25f;

					//From r to g.
					if(sMode == SubColorMode.subR){ 
						r -= Time.deltaTime/slowdown;
						g += Time.deltaTime/slowdown;
						if(r <= 0f){ r = 0f; }
						if(g >= 1f){ g = 1f; }
						if(r == 0f && g == 1f){ 
							sMode = SubColorMode.subG;
						}
					}
					//From g to b.
					if(sMode == SubColorMode.subG){
						g -= Time.deltaTime/slowdown;
						b += Time.deltaTime/slowdown;
						if(g <= 0f){ g = 0f; }
						if(b >= 1f){ b = 1f; }
						if(g == 0f && b == 1f){
							sMode = SubColorMode.subB;
						}
					}
					//From b to r.
					if(sMode == SubColorMode.subB){
						b -= Time.deltaTime/slowdown;
						r += Time.deltaTime/slowdown;
						if(b <= 0f){ b = 0f; }
						if(r >= 1f){ r = 1f; }
						if(b == 0f && r == 1f){
							sMode = SubColorMode.subR;
						}
					}
					Game.backColor = new Color(r/2.5f, g/2.5f, b/2.5f, 1f);
					Game.frontColor = new Color(1f - Game.backColor.r, 1f-Game.backColor.g, 1f-Game.backColor.b, 1f);
					backgroundQuad.SetActive(true);
					backgroundQuad.GetComponent<MeshRenderer>().material.color = Game.backColor;
				break;
		}
		
	}

	//Managing boss health bar. (Called from bosses script.)
	public void BossHealthBar(int health, float divider){
		bool showBossBar = (level >= wallSets.Count - 1 && !showingBossSplashscreen && health > 0);
		bossHealthBar.SetActive(showBossBar);
		if(showBossBar){ bossHealthBar.transform.position = Vector3.Lerp(bossHealthBar.transform.position, Vector3.zero, 3f*Time.deltaTime); }
		bossHealthBar.transform.localScale = Vector3.Lerp(bossHealthBar.transform.localScale, new Vector3(1f, (divider*health),1f), 3f*Time.deltaTime);
	}
	
	public void ChangeLayer (){
		switch(currentMode){
			case GameMode.Adventure:
		        if (level < wallSets.Count - 1) {
		            GameObject circ = Resources.Load("New Wait Radius") as GameObject;
		            GameObject circInst = Instantiate(circ, Vector3.zero, Quaternion.identity);
		            circInst.GetComponent<MeshRenderer>().material.color = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.25f);
		            circInst.transform.parent = Game.playerObj.transform;
		            circInst.transform.localPosition = new Vector3(0, -0.5f, 0);
		            if (Game.playerObj != null) { Game.playerObj.GetComponent<NewShipScript>().startCounter = 1.5f; }
		            PlaySound.NoLoopRandomPitch(levelChangeSnd, 0.5f, 1.5f);
		            scanned = false;
		            //Destroy bullets.
		            foreach (Transform child in Game.bulletHolder.transform) { Destroy(child.gameObject); }
		            //if(screenGrab.GetComponent<iTween>() != null){Destroy(screenGrab.GetComponent<iTween>());}
		            screenGrab.SetActive(true);
		            iTween.ScaleTo(screenGrab, iTween.Hash(
		                "x", 0,
		                "y", 0,
		                "z", 0.5f,
		                "time", transTime - 0.1f,
		                "easetype", iTween.EaseType.linear,
		                "ignoretimescale", true
		            ));
		            level++;
		            if (level == wallSets.Count - 1) { showingBossSplashscreen = true; }
		            if (zTheme == ZoneThemes.RedZone1 && level == 3) { music.GetComponent<AudioSource>().Play(); }
		            transCounter = transTime;
	        	}
	        	break;
        	case GameMode.Legacy:
				GameObject circLegacy = Resources.Load("Wait Radius") as GameObject;
				GameObject circLegacyInst = Instantiate(circLegacy, Vector3.zero, Quaternion.identity);
				circLegacyInst.transform.parent = Game.playerObj.transform;
				circLegacyInst.transform.localPosition = new Vector3(0, -0.5f, 0);
				if(Game.playerObj != null){ Game.playerObj.GetComponent<SpaceShipScript>().startCounter = 1.5f; }
				PlaySound.NoLoopRandomPitch(levelChangeSnd, 0.5f, 1.5f);
				scanned = false;
				//Destroy bullets.
				foreach (Transform child in Game.bulletHolder.transform){ Destroy(child.gameObject); }
				//if(screenGrab.GetComponent<iTween>() != null){Destroy(screenGrab.GetComponent<iTween>());}
				screenGrab.SetActive(true);
				iTween.ScaleTo(screenGrab, iTween.Hash(
					"x", 0,
					"y", 0,
					"z", 0.5f,
					"time", transTime - 0.1f,
					"easetype", iTween.EaseType.linear
				));
				if(level > 1){
					sat = UnityEngine.Random.Range(0, 2);
					if(level < 5){ sat = 1; }
					if(sat == 0){ Game.backColor = new Color(UnityEngine.Random.Range(0.6f, 1f), UnityEngine.Random.Range(0.6f, 1f), UnityEngine.Random.Range(0.6f, 1f)); }
					if(sat == 1){ Game.backColor = new Color(UnityEngine.Random.Range(0f, 0.3f), UnityEngine.Random.Range(0f, 0.3f), UnityEngine.Random.Range(0f, 0.3f)); }
				} else {
					Game.backColor = Color.black;
				}
				Camera.main.backgroundColor = Game.backColor;
				backgroundQuad.GetComponent<MeshRenderer>().material.color = Game.backColor;
				Game.frontColor = new Color(1f - Game.backColor.r, 1f-Game.backColor.g, 1f-Game.backColor.b, 1f);
				level--;

				//Checking progress based legacy achievements.
				if(level <= 25){ Game.CheckAchievements(38); }
				if(level <= 10){ Game.CheckAchievements(39); }

				if(level < Game.lowestLayer){
					Game.lowestLayer = level;
				}
				transCounter = transTime;
        		break;
        }
	}

    //Where we set up the music to play.
    private void SetMusic() {
        switch (themeNum) {
            case 0: //Zone 1 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/2 - Crisp Red") as AudioClip;
                break;
            case 1: //Zone 2 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/3 - Sweet Yellow") as AudioClip;
                break;
            case 2: //Zone 3 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/4 - Console Green") as AudioClip;
                break;
            case 3: //Zone 4 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/5 - Deep Blue") as AudioClip;
                break;
            case 4: //Zone 5 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/6 - Lilac Light Show") as AudioClip;
                bitCrushMusicObj.GetComponent<AudioSource>().clip = Resources.Load("Music/6b - Lilac Light Show (Bitcrush)") as AudioClip;
                break;
            case 5: //Zone 6 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/7 - Crystal Clear White") as AudioClip;
                bitCrushMusicObj.GetComponent<AudioSource>().clip = Resources.Load("Music/7b - Crystal Clear White (Bitcrush)") as AudioClip;
                break;
            case 6: //Zone 7 music.
                music.GetComponent<AudioSource>().clip = Resources.Load("Music/8 - The Warden's Unrelenting Grasp") as AudioClip;
                break;
            case 7: //Setting the bullet hell music.
            	music.GetComponent<AudioSource>().clip = Resources.Load("Music/BONUS - Deep Blue (Bullet Hell Mix)") as AudioClip;
            	break;
            case 9: //Legacy zone music.
            	music.GetComponent<AudioSource>().clip = Resources.Load("Music/Full Mix") as AudioClip;
            	break;
        }
		if(themeNum != 0){
			music.GetComponent<AudioSource>().Play();
		}
        if (needBitcrushTunes) {
            bitCrushMusicObj.GetComponent<AudioSource>().Play();
        }
    }

    //Setting up the music for legacy mode.
    private void SetLagacyMusic(){
		foreach (Transform child in legacyMusicHolder.transform) {
			musicChannels.Add(child.gameObject);
		}
	}

    private void SetBossSplashScreen(int zone) {
        GameObject bossStuffObj = splashScreen.transform.Find("Boss Stuff").gameObject;
        GameObject bossPortraitObj = bossStuffObj.transform.Find("Portrait").gameObject;
        GameObject midText = bossStuffObj.transform.Find("and their ship").gameObject;
        GameObject bossNameObj = bossStuffObj.transform.Find("Name").gameObject;
        GameObject bossShipNameObj = bossStuffObj.transform.Find("Ship Name").gameObject;
        
        switch (zone) {
            case 0: //Boss of zone 1.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Reinforcer Redd Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "Reinforcer Redd";
                midText.GetComponent<TextMesh>().text = "and their ship";
                bossShipNameObj.GetComponent<TextMesh>().text = "Spike!";
                break;
            case 1: //Boss of zone 2.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Chef Cheese Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "Chef Cheese";
                midText.GetComponent<TextMesh>().text = "and their ship";
                bossShipNameObj.GetComponent<TextMesh>().text = "The Grillmaster 5000!";
                break;
            case 2: //Boss of zone 3.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Cotton Junior Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "Cotton Jr.";
                midText.GetComponent<TextMesh>().text = "";
                bossShipNameObj.GetComponent<TextMesh>().text = "";
                //Setting proper colors for green zone. (Match foreground instead of background.)
                bossNameObj.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                bossPortraitObj.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                bossNameObj.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                midText.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                bossShipNameObj.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                GameObject vsTextObj = splashScreen.transform.Find("VS.").gameObject;
                GameObject carlStuff = splashScreen.transform.Find("Commander Carl Stuff").gameObject;
                GameObject carlPortrait = carlStuff.transform.Find("Carl Portrait").gameObject;
                GameObject carlName = carlStuff.transform.Find("Commander Carl Name").gameObject;
                vsTextObj.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                carlPortrait.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                carlName.GetComponent<MatchFrontColorScript>().matchBackInstead = false;
                break;
            case 3: //Boss of zone 4.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Void Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "The Void";
                midText.GetComponent<TextMesh>().text = "";
                bossShipNameObj.GetComponent<TextMesh>().text = "";
                break;
            case 4: //Boss of zone 5.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Punisher Plum Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "Punisher Plum";
                midText.GetComponent<TextMesh>().text = "";
                bossShipNameObj.GetComponent<TextMesh>().text = "";
                break;
            case 5: //Boss of Zone 6.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Vektor Boss Splash") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "Vektor...";
                midText.GetComponent<TextMesh>().text = "";
                bossShipNameObj.GetComponent<TextMesh>().text = "";
                break;
            case 6: //Boss of zone 7.
                bossPortraitObj.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/Punisher Plum Splash Texture") as Texture;
                bossNameObj.GetComponent<TextMesh>().text = "The Warden";
                midText.GetComponent<TextMesh>().text = "";
                bossShipNameObj.GetComponent<TextMesh>().text = "";
                break;
        }

        //...Oh yeah and set the victory screen colors here too, cus why not.
        GameObject zoneClear = victoryObj.transform.Find("Zone Clear Text").gameObject;
        GameObject gainedCoins = victoryObj.transform.Find("Gained Coins").gameObject;
        GameObject splashText = victoryObj.transform.Find("Splash Text").gameObject;
        if(zTheme == ZoneThemes.WhiteZone6) {
            zoneClear.GetComponent<TextMesh>().color = Color.white;
            gainedCoins.GetComponent<TextMesh>().color = Color.white;
            splashText.GetComponent<TextMesh>().color = Color.clear;
        } else {
            zoneClear.GetComponent<TextMesh>().color = Game.frontColor;
            gainedCoins.GetComponent<TextMesh>().color = Game.frontColor;
            splashText.GetComponent<TextMesh>().color = Game.frontColor;
        }
    }
	
    //Managing the visibility of the cursor.
    private void CursorVision() {
        //Cursor visibility for bullet hell mode.
        if(currentMode == GameMode.BulletHell) {
            if (Game.usingController) {
                Cursor.visible = false;
            } else {
                Cursor.visible = paused;
            }
        } else {
            Cursor.visible = !Game.usingController;
        }
    }

    //Smiley mode handler.
    private void SmileyModeHandler() {
        foreach(Transform enemyT in Game.enemyHolder.transform) {
            GameObject enemy = enemyT.gameObject;
            //Putting smileys on new enemies.
            if(enemy.GetComponent<NewEnemyScript>() != null) {
                NewEnemyScript nes = enemy.GetComponent<NewEnemyScript>();
                if (!nes.smiley) {
                    GameObject smileyInst = Instantiate(smileyObj, enemy.transform.position, Quaternion.identity) as GameObject;
                    smileyInst.transform.parent = enemy.transform;
                    nes.smiley = true;
                }
            }

            //Putting smileys on old enemies.
            if(enemy.GetComponent<EnemyScript>() != null) {
                EnemyScript es = enemy.GetComponent<EnemyScript>();
                if (!es.smiley) {
                    GameObject smileyInst = Instantiate(smileyObj, enemy.transform.position, Quaternion.identity) as GameObject;
                    smileyInst.transform.parent = enemy.transform;
                    es.smiley = true;
                }
            }
        }
    }

    //That function that moves the player to the nearest empty space so that you dont spawn in a wall and get stuck.
	private void MovePlayerToClearSpace(){
        //GameObject debugObj = Resources.Load("Debug/Debug Quad") as GameObject;
		int mapWidth = 72;
        int mapHeight = 40;
        int startWidth = -(72/2);
        int startHeight = -(40/2);

        //Find all empty spots.
        for(int x = startWidth; x < (mapWidth/2); x++) {
            for(int y = startHeight; y < (mapHeight/2); y++) {
                bool empty = !Physics2D.OverlapPoint(new Vector2(x, y), wallLayer);
                if (empty) {
                    Vector2 spot = new Vector2(x, y);
                    //Instantiate(debugObj, spot, Quaternion.identity);
                    emptySpots.Add(spot);
                }
            }
        }

        //Move player to nearest empty spot.
        for(int i = 0; i < emptySpots.Count; i++) {
            Vector2 playerPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
            float dist = Vector2.Distance(emptySpots[i], playerPos);
            if(dist < nearestPosDistance) {
                nearestEmptySpot = emptySpots[i];
                nearestPosDistance = dist;
            }
        }
        Game.playerObj.transform.position = nearestEmptySpot;
	}

    private void CheckAdventureAchievements() {
        if (!checkedAdventureAchievements) { 
            switch (zTheme) {
                case ZoneThemes.RedZone1:
                    Game.CheckAchievements(0); //Beat zone 1.
                    if (!gotHit) { Game.CheckAchievements(8); } //Beat zone 1 without getting hit.
                    break;
                case ZoneThemes.YellowZone2: 
                    Game.CheckAchievements(1); //Beat zone 2.
                    if (!gotHit) { Game.CheckAchievements(9); } //Beat zone 2 without getting hit.
                    if(playerMinStats) { Game.CheckAchievements(15); } //Beat zone 2 with minimum stats.
                    break;
                case ZoneThemes.GreenZone3:
                    Game.CheckAchievements(2); //Beat zone 3.
                    if (!gotHit) { Game.CheckAchievements(10); } //Beat zone 3 without getting hit.
                    if (playerMinStats) { Game.CheckAchievements(16); } //Beat zone 3 with min stats.
                    break;
                case ZoneThemes.BlueZone4:
                    Game.CheckAchievements(3); //Beat zone 4.
                    if (!gotHit) { Game.CheckAchievements(11); }//Beat zone 4 without getting hit.
                    if (playerMinStats) { Game.CheckAchievements(17); } //Beat zone 4 with min stats.
                    break;
                case ZoneThemes.PurpleZone5:
                    Game.CheckAchievements(4); //Beat zone 5.
                    if (!gotHit) { Game.CheckAchievements(12); } //Beat zone 5 without taking damage.
                    if (playerMinStats) { Game.CheckAchievements(18); }//Beat zone 5 with min stats.
                    break;
                case ZoneThemes.WhiteZone6:
                    Game.CheckAchievements(5); //Beat zone 6.
                    break;
            }

            //Checking for the splitshot achievement.
            if(Game.shotSplitRank == 1 && zTheme != ZoneThemes.RedZone1 && zTheme != ZoneThemes.FinalBossZone7){
                Game.CheckAchievements(20);
            }
            checkedAdventureAchievements = true;
        }
    }
}