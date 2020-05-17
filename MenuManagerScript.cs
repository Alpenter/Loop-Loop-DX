using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using Steamworks;

public class MenuManagerScript : MonoBehaviour {
	
	//Enum for what menu we are at.
	public enum Menu {
		Main,
		Settings,
		Stats,
		Adventure,
		Shop,
		DeleteSave,
	}
	public Menu currentMenu = Menu.Main;

	//Enum for what type of save we are deleting.
	public enum DeleteType{
		All,
		Adventure,
	}
	[HideInInspector] public DeleteType dType = DeleteType.All;

	//Ints.
	int selectedTheme = 0; //Which random color theme we have selected.
	int gamePadMainIndex = 0; //Which menu item we are at with the the gamepad at the main menu.
	int gamePadSettingsIndex = 0; //Which menu item we are at with the the gamepad at the settings menu.
	int gamePadShopIndex = 0; //Which menu item we are at with the the gamepad at the shop menu.
    int settingMax = 10; //If you have supporter, it's 10. If not, it's 7.
	int lvl = 0; //Which level is going to be loaded.
    readonly public int rank1Price = 100; //How much something costs at rank 1.
	readonly public int rank2Price = 250; //How much something costs at rank 2.
	readonly public int rank3Price = 500; //How much something costs at rank 3. 
	readonly public int floppyPrice = 1000; //How much the floppy disk costs.
    readonly int maxRank = 3; //The max rank of powerups.
	[HideInInspector] public int shopIndex = 0; //Which index we are at in the shop. This determines which shop text is active.
	[HideInInspector] public int camState = 0; //Where the camera will be.
	[HideInInspector] public int selectorMouseIndex = 0; //Where the selector will be for mouse controls.
	[HideInInspector] public int resLength, currentRes; //How many resolutions there are, and the current one.
	[HideInInspector] public List<int> lockedMains = new List<int>();

	//GameObjects.
	GameObject swiper = null; //The swipey level transition object.
	GameObject selector = null; //The selector.
	GameObject shopMusic = null; //The shop music object.
    GameObject gamePadSelector = null; //The selector in the shop for gamepad.
    GameObject coinSpurt = null; //The coin spurt for a redund.
    [HideInInspector] public GameObject adventurePadSelector = null; //The thing that selects levels in adventure mode.
    [HideInInspector] public GameObject menuMusic = null; //The menu music object.
    GameObject coinDrop = null; //This is the coin particle effect that spawns when you buy something at the shop. 
    public GameObject[] shopTexts; //The list of shop texts. (These are enabled based off of 'shopIndex' integer.)
	List<GameObject> pics = new List<GameObject>(); // The list of objescts that serve as the images that represent what you have selected on the main menu.
	
	//Bools
	bool reset = false; //If the player is holding down the reset controls button.
    bool padLocked = false; //If something is locked with gamepad controls, displayed the "?" image.
    bool vektorDead = false; //If vektor has died.
	[HideInInspector] public bool atRightSettingsMenu = false; //When you are at the right side of the settings menu, it changes the scale of the selector.
	[HideInInspector] public bool startedSwipe = false; //If the swipe tween has been played or not.
	[HideInInspector] public bool onThing = false; //Bool that tracks if you are moused over a button or not.
	[HideInInspector] public bool adventureCam = false; //Is this camera the adventure cam? If so, it tweens to the adventure map positon.
	
	//Floats.
	float loadTime = 0.6f; //How long it takes for the levels to transition out. (Basically waiting for the swipe object to finish it's animation.)
	float resetTime = 2f; //How long it takes to reset the settings the the default.
	float startTime = 0.8f; //How long it takes for the start swipe tween to play out.
	float counter = 0f; //The iTime counter.
    float musicPitch = 1f;
    [HideInInspector] public float adventureDelayCounter = 0f;
	readonly float iTime = 0.2f; //How it takes for the selector to move when using the gamepad.
    readonly float adventureDelayTime = 0.4f;
    [HideInInspector] public float backFromAdventureTweenCounter; 
	
	
	//Colors.
	public Color[] backColorPool; //The possible back colors, chosen by the theme index.
	public Color[] frontColorPool; //The possible front colors, chosen by the theme index.
	[HideInInspector] public Color backColor = Color.black; //What the background color is.

	//AudioClips.
	AudioClip snd = null; //Sound for pressing button.
	AudioClip overSnd = null; //When you select something on the menu with the controller.
    AudioClip cashSnd = null; //The sound that plays when you buy something.

	//Resolutions
	List<Resolution> resList = new List<Resolution>(); //The list of possible resolutions.

	//Camera Effects.
	Bloom bloom = null; //The light bloom of the camera.

	void Awake () {
        Cursor.visible = true;

        Game.manager = this.gameObject;

		Time.timeScale = 1;
		
		coinDrop = Resources.Load("Coin Drop") as GameObject;
        coinSpurt = Resources.Load("Coin Spurt") as GameObject;

		selectedTheme = Random.Range(0, 7);
		
		GameObject picHolder = transform.Find("Picture Holder").gameObject;
		foreach (Transform child in picHolder.transform) {
			pics.Add(child.gameObject);
		}
		
		Game.backColor = backColorPool[selectedTheme];
		Game.frontColor = frontColorPool[selectedTheme];
		Camera.main.backgroundColor = backColorPool[selectedTheme];
		
		selector = transform.Find("Selector").gameObject;
		gamePadSelector = transform.Find("Gamepad Shop Selector").gameObject;
        swiper = transform.Find("Swipey").gameObject;
        adventurePadSelector = transform.Find("Adventure Pad Selector").gameObject;

		iTween.MoveTo(swiper, iTween.Hash(
			"x", -23f,
			"y", 0f,
			"time", 0.5f,
			"easetype", iTween.EaseType.easeInOutSine,
			"looptype", iTween.LoopType.none
		));
		
		//Find light bloom.
		bloom = Camera.main.gameObject.GetComponent<Bloom>();
		
		//Find shopMusic.
		shopMusic = transform.Find("shop music").gameObject;
		shopMusic.SetActive(false);

        //Find menuMusic.
        menuMusic = transform.Find("menu music").gameObject;
        menuMusic.SetActive(true);
		
		//Load button click sound.
		snd = Resources.Load("SFX/Button Click") as AudioClip;
		overSnd = Resources.Load("SFX/menu select") as AudioClip;
        cashSnd = Resources.Load("SFX/Shop Coin Drop") as AudioClip;

		//If you chose to go to the store when you died.
		if(Game.startAtShop){
			currentMenu = Menu.Shop;
		} else {
			currentMenu = Menu.Main;
		}

        //Reset final boss bool.
        Game.atFinalBossFinalForm = false;
	}
	
	void Start(){
		//Get the resolutions.
		Resolution[] resolutions = Screen.resolutions;
        foreach (var res in resolutions) {
			resList.Add(res);
            //Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);
        }
		//Set maximum res.
		resLength = resList.Count;
		currentRes = resList.Count;
	}
	
	void Update () {
        //Debug for unlocking everything for steam test.
        if (Game.debug) {
            bool s = Input.GetKey(KeyCode.S);
            bool t = Input.GetKey(KeyCode.T);
            bool e = Input.GetKey(KeyCode.E);
            bool a = Input.GetKey(KeyCode.A);
            bool m = Input.GetKey(KeyCode.M);
            if (s && t && e && a && m) {
                Game.UnlockAll();
                SceneManager.LoadScene(0);
            }
        }

		//Check for controller usage.
		Game.ControllerCheck();
		//Debug.Log("Using Controller: " + Game.usingController);

		//When to show the cursor.
        Cursor.visible = !Game.usingController;

        //Setting the scale of the selector.
        if(atRightSettingsMenu){
        	selector.transform.localScale = new Vector3(-10f, 0.5f, 1f);
    	} else {
        	selector.transform.localScale = new Vector3(5f, 0.5f, 1f);
    	}

        //When the gamepad selector is visible.
        gamePadSelector.SetActive(Game.usingController);

		//when menu is fully open.
		if(startTime >= 0){
			startTime -= Time.deltaTime;
		}
		
		//Light bloom active if light effects are on.
		bloom.enabled = Game.lightEffects;
		
        //The adventure gamepad selector should have a velocity of 0 if not at the adventure menu, or ot using a controller.
        if(currentMenu != Menu.Adventure || !Game.usingController) {
            adventurePadSelector.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
		
		//Setting the camera positions.
		Vector3[] cPos = {
			new Vector3(0, 0, -30),//0
			new Vector3(0, -20, -30), //1
			new Vector3(20, 0, -30), //2
			new Vector3(20, -20, -30), //3
			new Vector3(60, 0, -30), //4
		};
        //If we are the adventure cam, scroll up to the map, else, behave normally.
        if (adventureCam && backFromAdventureTweenCounter <= 0) { Camera.main.gameObject.transform.position = Vector3.Lerp(Camera.main.gameObject.transform.position, new Vector3(0, 20, -10), 3f*Time.deltaTime); }
        else if(!adventureCam && backFromAdventureTweenCounter <= 0){
            Camera.main.gameObject.transform.position = cPos[camState];
        }
        if(backFromAdventureTweenCounter > 0f) { backFromAdventureTweenCounter -= Time.deltaTime; }
        
        //You have been to the shop... when you go there lel.
		if(currentMenu == Menu.Shop) { Game.beenToShop = true; }
        else { shopIndex = 0; }

		//Set what shop texts to set active if you have coins.
        vektorDead = (Game.beatWhite && !Game.savedVektor);

        if (vektorDead) {
            shopMusic.GetComponent<AudioSource>().pitch = 1f;
            shopMusic.GetComponent<AudioSource>().volume = 0;
            for (int p = 0; p < shopTexts.Length; p++){
			    shopTexts[p].SetActive(false);
			}
        } else { 
		    if(Game.coins >= 0){
			    shopMusic.GetComponent<AudioSource>().pitch = 1f;
                for(int o = 0; o < shopTexts.Length; o++){
				    if(o == shopIndex){
					    shopTexts[o].SetActive(true);
				    } else {
					    shopTexts[o].SetActive(false);
				    }
			    }
		    } else { //When you have no money, only display debt text.
		    shopMusic.GetComponent<AudioSource>().pitch = 0.5f;
			    for (int j = 0; j < shopTexts.Length; j++){
				    if(j == 9){
					    shopTexts[j].SetActive(true);
				    } else {
					    shopTexts[j].SetActive(false);
				    }
			    }
		    }
        }
		
		//When to set shop music active.
		shopMusic.SetActive(camState == 3);
		if(!vektorDead){ shopMusic.GetComponent<AudioSource>().volume = Game.musicVolume; }

        //When to set menu music active.
        menuMusic.SetActive(camState != 3);
        if (!startedSwipe) {
            menuMusic.GetComponent<AudioSource>().volume = Game.musicVolume;
        } else {
            if (menuMusic.GetComponent<AudioSource>().volume > 0) { menuMusic.GetComponent<AudioSource>().volume -= Time.deltaTime*1.5f; }
        }
        //Pitch down the menu music if you are at adventure mode with the final boss visible.
        float pitchSpeed = 1f;
        musicPitch = menuMusic.GetComponent<AudioSource>().pitch;
        if(Game.beatWhite && currentMenu == Menu.Adventure) {
            //Debug.Log("going!");
            musicPitch = Mathf.Lerp(musicPitch, 0.5f, pitchSpeed*Time.deltaTime);
        } else {
            musicPitch = Mathf.Lerp(musicPitch, 1f, pitchSpeed*Time.deltaTime);
        }
        menuMusic.GetComponent<AudioSource>().pitch = musicPitch; //Apply the pitch.

        //Resetting the settings that need resetting of the settings.
		if(Input.GetButton("Reset Controls") && !reset){
			resetTime -= Time.deltaTime;
			if(resetTime <= 0){
				Game.ResetSettings();
				//Debug.Log("Reset");
				reset = true;
			}
		} else {
			resetTime = 2f;
			reset = false;
		}

        //The delay before you can select a level in adventure mode is controlled here.
        if(currentMenu == Menu.Adventure) {
            if (adventureDelayCounter > -1){ adventureDelayCounter -= Time.deltaTime; }
        } else {
            adventureDelayCounter = adventureDelayTime;
        }

		//Debug reset level.
		if(Input.GetKeyDown(KeyCode.O) && Game.debug){
			SceneManager.LoadScene(1);
		}
		
		//How the menu is interacted with by keyboard and mouse.
		if(!Game.usingController){
			
			//Setting the selector positions.
			Vector3[] sPos = {
				new Vector3(-6.5f, 2f, 0), //1
				new Vector3(-6.5f, 1.5f, 0), //2
				new Vector3(-6.5f, 1f, 0), //3
				new Vector3(-6.5f, 0.5f, 0), //4
				new Vector3(-6.5f, -1f, 0), //5
				new Vector3(-6.5f, -2.5f, 0), //6
				new Vector3(-6.5f, -3f, 0), //7
				new Vector3(-6.5f, -3.5f, 0), //8
				new Vector3(-6.5f, -4f, 0), //9
				new Vector3(-6.5f, -4.5f, 0), //10
				new Vector3(-6.5f, -24.6f, 0), //11
				new Vector3(13.5f, -3.6f, 0), //12
				new Vector3(13.5f, -22.45f, 0), //13
				new Vector3(53.6f, -4.6f, 0), //14
				new Vector3(24f, 1.95f, 0), //15
				new Vector3(24f, 1.18f, 0), //16
			};
			selector.transform.position = sPos[selectorMouseIndex];
			
			//When you are at the right settings menu.
			atRightSettingsMenu = (currentMenu == Menu.Settings);

			//Selector is visible when on a thing.
			selector.SetActive(onThing && !startedSwipe && startTime < 0);
			
            //what pics to set active.
            for (int i = 0; i < pics.Count; i++){
			    if(i == selectorMouseIndex && onThing && startTime < 0){
				    pics[i].SetActive(true);
			    } else {
				    pics[i].SetActive(false);
			    }
		    }

			//Clicking.
			if(Input.GetMouseButtonDown(0) && onThing && startTime < 0){
				MenuAction(selectorMouseIndex);
			}
		} else { //How the menu is controlled using a controller.
			//Selector is always active when using a gamepad.
			selector.SetActive(true);

			//Get the controller inputs.
			float x = Input.GetAxisRaw("Horizontal [Gamepad]");
			float y = Input.GetAxisRaw("Vertical [Gamepad]");
			float threshHold = 0.5f;

			if (counter > 0) { counter -= Time.deltaTime; }
                
			switch(currentMenu){
				case Menu.Main:
					Vector3[] sPos = {
						new Vector3(-6.5f, 2f, 0), //1
						new Vector3(-6.5f, 1.5f, 0), //2
						new Vector3(-6.5f, 1f, 0), //3
						new Vector3(-6.5f, 0.5f, 0), //4
						new Vector3(-6.5f, -1f, 0), //5
						new Vector3(-6.5f, -2.5f, 0), //6
						new Vector3(-6.5f, -3f, 0), //7
						new Vector3(-6.5f, -3.5f, 0), //8
						new Vector3(-6.5f, -4f, 0), //9
						new Vector3(-6.5f, -4.5f, 0), //10
						new Vector3(-6.5f, -24.6f, 0), //11
						new Vector3(13.5f, -3.6f, 0), //12
						new Vector3(13.5f, -22.45f, 0), //13
					};
					selector.transform.position = sPos[gamePadMainIndex];
                    
                    //What pics to set active.
                    for (int i = 0; i < pics.Count; i++){
			            padLocked = lockedMains.Contains(i);
                        if(i == gamePadMainIndex && startTime < 0 && !padLocked){
				            pics[i].SetActive(true);
			            } else {
				            pics[i].SetActive(false);
			            }
		            }

					if (y < -threshHold && gamePadMainIndex > 0 && counter <= 0) { gamePadMainIndex--; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
               		if (y > threshHold && gamePadMainIndex < 9 && counter <= 0) { gamePadMainIndex++; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
					if(Input.GetButtonDown("Submit") && !lockedMains.Contains(gamePadMainIndex)){ MenuAction(gamePadMainIndex); }
					break;
				case Menu.Settings:

                    if (Game.boughtSupporterPack) {
                        settingMax = 10;
                    } else {
                        settingMax = 7;
                    }

					Vector3[] settingSelectorPos = {
						new Vector3(13.5f, 2.24f, 0), //0 sfx
						new Vector3(13.5f, 1.68f, 0), //1 music
						new Vector3(13.5f, 0.17f, 0), //2 light effects
						new Vector3(13.5f, -0.45f, 0), //3 vsync
						new Vector3(13.5f, -2.03f, 0), //4 fullscreen
						new Vector3(13.5f, -2.61f, 0), //5 resolution
						new Vector3(24f, 1.95f, 0), //6 delete adventure mode progress.
						new Vector3(24f, 1.18f, 0), //7 delete all progress.
						new Vector3(24f, -1.55f, 0), //8 supporter benefits smiley mode.
						new Vector3(24f, -2.16f, 0), //9 supporter benefits party mode.
						new Vector3(24f, -2.79f, 0), //10 supporter benefits toot mode.
					};
					selector.transform.position = settingSelectorPos[gamePadSettingsIndex];
					//How the menu works when you are at the left side.
					if(!atRightSettingsMenu){ 
						//Clamping index.
						if(gamePadSettingsIndex < 0){ gamePadSettingsIndex = 0; }
						if(gamePadSettingsIndex > 5){ gamePadSettingsIndex = 5; }
						//Movement.
						if (y < -threshHold && gamePadSettingsIndex > 0 && counter <= 0) { gamePadSettingsIndex--; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
               			if (y > threshHold && gamePadSettingsIndex < 5 && counter <= 0) { gamePadSettingsIndex++; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
               			if (Input.GetButtonDown("Right Bumper")){ PlaySound.NoLoop(overSnd); gamePadSettingsIndex = 6; atRightSettingsMenu = true; }
					} else { //How the menu works when you are at the right side.
						//Clamping index.
						if (gamePadSettingsIndex < 6){ gamePadSettingsIndex = 6; }
						if (gamePadSettingsIndex > settingMax){ gamePadSettingsIndex = settingMax; }
						//Movement.
						if (y < -threshHold && gamePadSettingsIndex > 6 && counter <= 0) { gamePadSettingsIndex--; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
               			if (y > threshHold && gamePadSettingsIndex < settingMax && counter <= 0) { gamePadSettingsIndex++; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
						if (Input.GetButtonDown("Left Bumper")){ PlaySound.NoLoop(overSnd); gamePadSettingsIndex = 0; atRightSettingsMenu = false; }
					}
					//For tick box settings.
					if(Input.GetButtonDown("Submit")) {
						PlaySound.NoLoop(snd);
						switch(gamePadSettingsIndex){
							case 2: //Light effects.
								SettingAction(4);
								break;
							case 3: //VSync.
								SettingAction(5);
								break;
							case 4: //Fullscreen.
								SettingAction(6);
								break;
							case 6:
								MenuAction(14);
								break;
							case 7:
                                MenuAction(15);
								break;
							case 8:
								SettingAction(9);
								break;
							case 9:
								SettingAction(10);
								break;
                            case 10:
                                SettingAction(11);
                                break;
						}
					}

					//For settings requiring horizontal movement.
					if(x > threshHold && counter <= 0) { //Increasing value of music, sfx, and resolution.
						switch(gamePadSettingsIndex){
							case 0: //Sfx up.
								SettingAction(1);
								PlaySound.NoLoop(snd);
								break;
							case 1:	//Music up.
								SettingAction(3);
								PlaySound.NoLoop(snd);
								break;
							case 5: //Resolution up.
								SettingAction(8);
								PlaySound.NoLoop(snd);
								break;
						}
						counter = iTime;
					}
					if(x < -threshHold && counter <= 0){ //Decreasing value of musicm sfx, and resolution.
						switch(gamePadSettingsIndex){
							case 0: //Sfx down.
								SettingAction(0);
								PlaySound.NoLoop(snd);
								break;
							case 1:	//Music down.
								SettingAction(2);
								PlaySound.NoLoop(snd);
								break;
							case 5: //Resolution down.
								SettingAction(7);
								PlaySound.NoLoop(snd);
								break;
						}
						counter = iTime;
					}

					//Backing out of the menu.
					if(Input.GetButtonDown("Cancel")) { MenuAction(11); }
					break;
				case Menu.Stats:
					if(Input.GetButtonDown("Cancel")) { MenuAction(10); }
					break;
				case Menu.Adventure:
                    Vector2 v0 = adventurePadSelector.GetComponent<Rigidbody2D>().velocity;
                    Vector3 p0 = adventurePadSelector.transform.position;
                    float smooth = 17.5f;
                    float selectSpeed = 5f;
                    v0 = Vector2.Lerp(v0, new Vector2(x, -y)*selectSpeed, smooth*Time.deltaTime);
                    float maxX = 8.5f;
                    float maxY = 25.4f;
                    float minX = -9.2f;
                    float minY = 15.5f;
                    if(p0.x < minX) { p0.x = minX; }
                    if(p0.x > maxX) { p0.x = maxX; }
                    if(p0.y > maxY) { p0.y = maxY; }
                    if(p0.y < minY) { p0.y = minY; }
                    p0.z = -7.5f;
                    adventurePadSelector.transform.position = p0;
                    adventurePadSelector.GetComponent<Rigidbody2D>().velocity = v0;
                    
                    //When the swipe starts, hide the cursor.
                    adventurePadSelector.SetActive(!startedSwipe);
                    

                    if(Input.GetButtonDown("Cancel")) { MenuAction(16); }
					break;
				case Menu.Shop:

                    //The postions of the shop box.
                    Vector3[] shopBoxPos = {
                        new Vector3(13, -18, 0), //0 - Health.
                        new Vector3(20, -18, 0), //1 - Fire Rate.
                        new Vector3(27, -18, 0), //2 - Floppy Disk.
                        new Vector3(16.5f, -21, 0), //3 - Speed
                        new Vector3(23.5f, -21, 0), //4 - Shot Splits.
                    };
                    gamePadSelector.transform.position = shopBoxPos[gamePadShopIndex];

                    //Moving the stick around in the shop menu. (Up and Down).
                    if (y > threshHold && gamePadShopIndex == 0 && counter <= 0) { gamePadShopIndex = 3; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
                    if (y > threshHold && gamePadShopIndex == 1 && counter <= 0) { gamePadShopIndex = 4; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
               		if (y > threshHold && gamePadShopIndex == 2 && counter <= 0) { gamePadShopIndex = 4; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
                    if (y < -threshHold && gamePadShopIndex == 3  && counter <= 0) { gamePadShopIndex = 0; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
                    if (y < -threshHold && gamePadShopIndex == 4  && counter <= 0) { gamePadShopIndex = 1; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
					//Moving the stick around in the shop menu. (Horizontal).
                    if (x < -threshHold && gamePadShopIndex > 0  && counter <= 0) { gamePadShopIndex--; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }
                    if (x > threshHold && gamePadShopIndex < 4  && counter <= 0) { gamePadShopIndex++; PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); counter = iTime; }

                    //This sets the shop text.
                    switch (gamePadShopIndex) {
                        case 0:
                            shopIndex = 2;
                            break;
                        case 1:
                            shopIndex = 4;
                            break;
                        case 2:
                            shopIndex = 7;
                            break;
                        case 3:
                            shopIndex = 5;
                            break;
                        case 4:
                            shopIndex = 6;
                            break;
                    }

                    //Pressing that A button.
                    if(Input.GetButtonDown("Submit") && Game.coins >= 0){
                        switch (gamePadShopIndex) {
                            case 0: //Buying heart.
                                if(Game.healthRank < maxRank) { ShopAction(Game.healthRank, "Health"); }
                                break;
                            case 1:
                                if(Game.shotSpeedRank < maxRank) { ShopAction(Game.shotSpeedRank, "Shot Speed"); }
                                break;
                            case 2:
                                if (!Game.ownsFloppyDisk) { ShopAction(4, "Floppy"); }
                                break;
                            case 3:
                                if(Game.speedRank < maxRank) { ShopAction(Game.speedRank, "Speed"); } 
                                break;
                            case 4:
                                if (Game.shotSplitRank < (maxRank - 1)) { ShopAction(Game.shotSplitRank, "Shot Split"); }
                                break;
                        }
                        //ShopAction(0, "temp");
                    }

                    //Refund!
                    bool refundable = RefundCheck();
                    if(Input.GetButtonDown("Delete Save") && refundable) {
                        Refund();
                    }

                    if(Input.GetButtonDown("Cancel")) { MenuAction(12); }
					break;
				case Menu.DeleteSave:
                    if(Input.GetButtonDown("Cancel")) { MenuAction(13); }
					break;

			}
		}

		//Swiping out to a level.
		if(startedSwipe){
			loadTime -= Time.deltaTime;
			if(loadTime <= 0){
				if(lvl == 420){
					Application.Quit();
				} else {
					SceneManager.LoadScene(lvl);
				}
			}
		}
	}
	
	public void ApplyResolution(){
		Screen.SetResolution(resList[currentRes].width, resList[currentRes].height, Game.isFullscreen);
	}
	
	public void LoadLevel(int i){
		if(!startedSwipe){
			iTween.MoveTo(swiper, iTween.Hash(
				"x", 0f,
				"y", 0f,
				"time", 0.5f,
				"easetype", iTween.EaseType.easeInOutSine,
				"looptype", iTween.LoopType.none
			));
			lvl = i;
			startedSwipe = true;
		}
	}
	
    //Function that spawns the shop coins and plays the sound.
	public void SpawnCoinDrop(){
        PlaySound.NoLoop(cashSnd);
		Instantiate(coinDrop, new Vector3(20, -15.1f, 30), Quaternion.identity);
	}

	public void MenuAction(int i){
		switch(i){
			case 0: //Play Adventure.
				if(Game.beatFirstLevel){
					currentMenu = Menu.Adventure;
					adventureCam = true;
					camState = 0;
				} else {
				 	LoadLevel(6);
				} 
				break;
			case 1: //Play Bullet Hell.
				LoadLevel(4);
				break;
			case 2:	//Play Legacy Mode.
				LoadLevel(2);
				break;
			case 3: //Play Legacy Bullet Hell Mode.
				LoadLevel(3);
				break;
			case 4: //The Ship Shop.
				camState = 3;
				currentMenu = Menu.Shop;
				break;
			case 5: //Statistics.
				camState = 1;
				currentMenu = Menu.Stats;
				break;
			case 6: //Options.
				camState = 2;
				currentMenu = Menu.Settings;
				break;
			case 7: //Purchase Soundtrack.
                if (Game.steamy) { 
                    SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/1194020/LoopLoop_DX_Official_Soundtrack/");
                }
				break;
			case 8: //Purchase Supporter Pack.
                if (Game.steamy) { 
                    SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/1194021/LoopLoop_DX_Supporter_Pack/");
                }
				break;
			case 9: //Quit.
				LoadLevel(420);
				break;
			case 10: //Back from statistics.
				camState = 0;
				currentMenu = Menu.Main;
				break;
			case 11: //Back from options.
				gamePadSettingsIndex = 0;
                atRightSettingsMenu = false;
				camState = 0;
				currentMenu = Menu.Main;
				break;
			case 12: //Back from shop.
				camState = 0;
			 	currentMenu = Menu.Main;
			 	break;
		 	case 13: //Back from delete menu.
		 		camState = 2;
		 		currentMenu = Menu.Settings;
		 		break;
	 		case 14: //Delete Adventure.
	 			camState = 4;
				dType = DeleteType.Adventure;
				currentMenu = Menu.DeleteSave;
				break;
			case 15: //Delete ALLLLL
				camState = 4;
				dType = DeleteType.All;
				currentMenu = Menu.DeleteSave;
				break;
            case 16: //Back from adventure.
				currentMenu = MenuManagerScript.Menu.Main;
                PlaySound.NoLoop(snd);
                float camTime = 1.5f;
                backFromAdventureTweenCounter = camTime + 0.1f;
                iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", 0, "y", 0, "time", camTime, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.none));
                adventureCam = false;
                break;

		}
		PlaySound.NoLoop(snd); //Play button click sound.
	}

	//The setting menu actions.
	public void SettingAction(int i){
		switch(i){
			case 0: //SFX Down
				if(Game.soundVolume > 0f){ Game.soundVolume = Game.soundVolume - 0.1f; }
				break;
			case 1: //SFX Up
				if(Game.soundVolume < 1f){ Game.soundVolume = Game.soundVolume + 0.1f; }
				break;
			case 2: //Music Down.
				if(Game.musicVolume > 0f){ Game.musicVolume = Game.musicVolume - 0.1f; }
				break;
			case 3: //Music Up.
				if(Game.musicVolume < 1f){ Game.musicVolume = Game.musicVolume + 0.1f; }
				break;
			case 4: //Light Effects.
				Game.lightEffects = !Game.lightEffects;
				break;
			case 5: //VSync.
				Game.vSync = !Game.vSync;
				break;
			case 6: //Fullscreen.
				Game.isFullscreen = !Game.isFullscreen;
				ApplyResolution();
				break;
			case 7: //Resolution down.
				if(currentRes > 0){ currentRes--; }
				ApplyResolution();
				break;
			case 8: //Resolution up.
				if(currentRes < resLength){ currentRes++; }
				ApplyResolution();
				break;
			case 9: //Smiley mode.
                if (Game.boughtSupporterPack) { 
				    Game.smileyMode = !Game.smileyMode;
                } else {
                    Game.smileyMode = false;
                }
				break;
			case 10: //Party mode.
                if (Game.boughtSupporterPack) { 
				    Game.partyMode = !Game.partyMode;
                } else {
                    Game.partyMode = false;
                }
				break;
            case 11: //Toot mode.
                if (Game.boughtSupporterPack) { 
                    Game.tootMode = !Game.tootMode;
                } else {
                    Game.tootMode = false;
                }
                break;
		}
	}

    //Store actions.
    public void ShopAction(int rank, string thingIBought) {

        //This unlocks the "Made your first purchase achievement."
        Game.CheckAchievements(21); 
        
        switch (rank) {
            case 0:
                Game.coins = Game.coins - rank1Price;
                SpawnCoinDrop();
                break;
            case 1:
                Game.coins = Game.coins - rank2Price;
                SpawnCoinDrop();
                break;
            case 2:
                Game.coins = Game.coins - rank3Price;
                SpawnCoinDrop();
                break;
            case 4:
                Game.coins = Game.coins - floppyPrice;
                SpawnCoinDrop();
                break;
        }
        
        //Check if you are in debt, and unlock the debt achievement if you are.
        if(Game.coins < 0) {
            Game.CheckAchievements(23);
        }

        switch (thingIBought) {
            case "Health": //Purchasing health.
                Game.healthRank++;
                break;
            case "Shot Speed": //Purchasing shot speed.
                Game.shotSpeedRank++;
                break;
            case "Speed": //Purchasing speed.
                Game.speedRank++;
                break;
            case "Shot Split": //Purchasing shot split.
                Game.shotSplitRank++;
                break;
            case "Floppy":
                Game.ownsFloppyDisk = true;
                break;
        }

        //Check if you own the floppy disk without being in debt and unlocking the achievement for it.
        if(Game.ownsFloppyDisk && Game.coins >= 0) {
            Game.CheckAchievements(24);
        }

        //Check if you own everything and unlock the achievement.
        if( Game.healthRank >= 3 &&
            Game.shotSpeedRank >= 3 &&
            Game.speedRank >= 3 && 
            Game.shotSplitRank >= 2 &&
            Game.ownsFloppyDisk) { Game.CheckAchievements(25); }


        Game.SaveGame();
    }
    
    public void Refund() {

        //This unlocks the "First Refund." Achievement.
        Game.CheckAchievements(22); 

        //Calculate how much money needs to be returned.
		int moneyToReturn = MoneySpent();

		//..give the money back...
		Game.coins = Game.coins + moneyToReturn;
				
		//...reset all the ranks...
		Game.healthRank = 0;
		Game.shotSpeedRank = 0;
		Game.speedRank = 0;
		Game.shotSplitRank = 0;
		Game.ownsFloppyDisk = false;
	    
        //Here we make the coin spurt and color it, etc.
		Quaternion rot = Quaternion.Euler(-90, 0, 0);
		GameObject obj = Instantiate(coinSpurt, new Vector3(20, -23.5f, 0f), rot) as GameObject;
		ParticleSystemRenderer rr = obj.GetComponent<ParticleSystemRenderer>();
        rr.material.color = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.333f);
        PlaySound.NoLoop(cashSnd);

		//...and save the game.
		Game.SaveGame();
    }


    //Is the player allowed to refund?
    public bool RefundCheck(){
		if(Game.healthRank > 0 || Game.shotSpeedRank > 0 || Game.speedRank > 0 || Game.shotSplitRank > 0 || Game.ownsFloppyDisk){
			return true;
		} else {
			return false;
		}
	}
	
    //Calculate how much money the player has spent for the refund option.
	public int MoneySpent(){
		int m = 0;
		
		//Add floppy disk price.
		if(Game.ownsFloppyDisk){ m = m + floppyPrice; }
		
		//Add health price.
		if(Game.healthRank == 1){ m = m + rank1Price; }
		else if(Game.healthRank == 2){ m = m + (rank1Price + rank2Price); }
		else if(Game.healthRank == 3){ m = m + (rank1Price + rank2Price + rank3Price); }
		else { m = m + 0; }
		
		//Add shot speed price.
		if(Game.shotSpeedRank == 1){ m = m + rank1Price; }
		else if(Game.shotSpeedRank == 2){ m = m + (rank1Price + rank2Price); }
		else if(Game.shotSpeedRank == 3){ m = m + (rank1Price + rank2Price + rank3Price); }
		else { m = m + 0; }
		
		//Add speed price.
		if(Game.speedRank == 1){ m = m + rank1Price; }
		else if(Game.speedRank == 2){ m = m + (rank1Price + rank2Price); }
		else if(Game.speedRank == 3){ m = m + (rank1Price + rank2Price + rank3Price); }
		else { m = m + 0; }
		
		//Add shot split price.
		if(Game.shotSplitRank == 1){ m = m + rank1Price; }
		else if(Game.shotSplitRank == 2){ m = m + (rank1Price + rank2Price); }
		else if(Game.shotSplitRank == 3){ m = m + (rank1Price + rank2Price + rank3Price); } //Wait this shouldnt even be possible lol.
		else { m = m + 0; }
		
		//Return that numbah, baby!
		return m;
	}
}