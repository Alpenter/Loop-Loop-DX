using UnityEngine;
using Steamworks;

public static class Game {
	
	//New In-Game stuff.
	public static GameObject playerObj = null;
	public static GameObject manager = null;
    public static GameObject plumBatch = null;
    public static GameObject bardObj = null;
    public static GameObject risingCrystalObj = null;
    public static GameObject noLoopBlockObj = null;
    public static GameObject finalBossStars = null;
    public static GameObject wardenPurpleCrystalHolder = null;
	public static bool cameFromLegacy = false;
	public static bool brokeRecord = false;
	public static bool atWaterLevel = false;
    public static bool inDisruptor = false;
    public static bool usingController = false;
    public static bool startAtShop = false;
    public static bool boughtSupporterPack = false;
    public static bool loopBlocked = false;
    public static bool brokeCore = false;
    public static bool enableGlitterTrail = false;
    public static bool atWardenLevel = false;
    public static bool atFinalBossFinalForm = false;
    public static bool atFinalShot = false;
    public static bool hitByFinalBoss = false;
    public static bool wardenDead = false;
	public static int newCurrentHealth = 2;
	public static int newMaxHealth = 2;
    public static int currentLasers = 2; //Always start with two lasers.
    public static int maxLasers = 3; //Never have more than 3 lasers.
	public static float maxTimeAlive = 0;
	public static float lightValue = 1.3f;
	public static WardenScript warden = null;
    public static Color bulletColor = Color.white;
	public enum CurrentDirection { Left, Up, Down, Right }
    public static CurrentDirection currentDirection = CurrentDirection.Up;
    
	//Legacy in game stuff.
	public static GameObject bulletHolder = null;
	public static GameObject enemyHolder = null;
	public static int currentHealth = 3;
	public static int maxHealth = 5;
    public static int currentCoinsCollectedInLevel = 0;
    public static int levelDiedOn = 0;
	public static float speed = 20f;
	public static bool canShoot = false;
	public static bool canShootSelf = false;
	public static Color frontColor = Color.white;
	public static Color backColor = Color.black;
	public static float maxTimeAliveLegacy = 0;
	
	//Progress. (Stuff to save.)
	//Legacy stuff.
	public static int lowestLayer = 50;
	public static int deaths = 0;
	public static int shots = 0;
	public static float highMaxTimeAliveLegacy = 0;
	
	//New stuff.
	public static int coins = 0;
	public static int healthRank = 0;
	public static int shotSpeedRank = 0;
	public static int speedRank = 0;
	public static int shotSplitRank = 0;
	public static float highMaxTimeAlive = 0;
	public static int newShotCount = 0;
	public static int newDeaths = 0;
	public static bool gotGun = false;
	public static bool beatFirstLevel = false;
	public static bool beatYellow = false;
	public static bool beatGreen = false;
	public static bool beatBlue = false;
	public static bool beatPurple = false;
	public static bool beatWhite = false;
	public static int maxEndlessLayer = 0;
    public static bool beatFinalBoss = false;
    public static int enemiesKilled = 0;
	public static bool ownsFloppyDisk = false;
	public static int maxMoneyBagsCollected = 0;
	public static bool unlockedShop = false;
	public static bool unlockedBulletHell = false;
	public static bool unlockedLegacy = false;
	public static bool unlockedLegacyBulletHell = false;
	public static bool beenToShop = false;
	public static bool beenToBulletHell = false;
	public static bool beenToLegacy = false;
	public static bool beenToLegacyBulletHell = false;
    public static bool ownsLasers = false;
    public static bool savedVektor = false;
    public static bool beatGame = false;
    public static bool sawGoodEnding = false;

	//Settings. (Save these too.)
	public static float soundVolume = 1f;
	public static float musicVolume = 1f;
	public static bool lightEffects = true;
	public static bool vSync = true;
	public static bool isFullscreen = true;
	public static bool smileyMode = false;
	public static bool partyMode = false;
    public static bool tootMode = false;
	public static bool speechSounds = false;

	//Achievements.
	public static bool[] unlockedAchievements = new bool[49];

	//Version number.
	public static string versionNumber = "V 1.0.4";
    
	//Debug
	public static bool debug = false;

	//Are we using steam? (This is set from the SplashScreenManager script.)
	public static bool steamy = false;

	//Saving and loading game progress.
	public static void SaveGame(){
		//Save game progress.
		PlayerPrefs.SetInt("a", lowestLayer);
		PlayerPrefs.SetInt("b", deaths);
		PlayerPrefs.SetInt("c", shots);
		PlayerPrefs.SetFloat("d", highMaxTimeAliveLegacy);
		PlayerPrefs.SetInt("e", coins);
		PlayerPrefs.SetInt("f", healthRank);
		PlayerPrefs.SetInt("g", shotSpeedRank);
		PlayerPrefs.SetInt("h", speedRank);
		PlayerPrefs.SetInt("i", shotSplitRank);
		PlayerPrefsX.SetBool("j", beatFirstLevel);
		PlayerPrefs.SetInt("k", newShotCount);
		PlayerPrefs.SetInt("l", newDeaths);
		PlayerPrefsX.SetBool("m", gotGun);
		PlayerPrefsX.SetBool("n", beatYellow);
		PlayerPrefsX.SetBool("o", beatGreen);
		PlayerPrefsX.SetBool("p", beatBlue);
		PlayerPrefsX.SetBool("q", beatPurple);
		PlayerPrefsX.SetBool("r", beatWhite);
		PlayerPrefs.SetInt("s", maxEndlessLayer);
        PlayerPrefsX.SetBool("t", beatFinalBoss);
        PlayerPrefs.SetInt("u", enemiesKilled);
		PlayerPrefsX.SetBool("v", ownsFloppyDisk);
		PlayerPrefs.SetInt("w", maxMoneyBagsCollected);
		PlayerPrefsX.SetBool("x", unlockedShop);
		PlayerPrefsX.SetBool("y", unlockedBulletHell);
		PlayerPrefsX.SetBool("aa", unlockedLegacy);
		PlayerPrefsX.SetBool("ab", unlockedLegacyBulletHell);
		PlayerPrefsX.SetBool("ac", beenToShop);
		PlayerPrefsX.SetBool("ad", beenToBulletHell);
		PlayerPrefsX.SetBool("af", beenToLegacy);
		PlayerPrefsX.SetBool("ag", beenToLegacyBulletHell);
        PlayerPrefsX.SetBool("ah", ownsLasers);
        PlayerPrefsX.SetBool("ai", savedVektor);
        PlayerPrefsX.SetBool("aj", beatGame);
        PlayerPrefsX.SetBool("ak", sawGoodEnding);

		//Save achievements.
		for(int i = 0; i < unlockedAchievements.Length; i++){
			PlayerPrefsX.SetBool("t"+i.ToString(), unlockedAchievements[i]);
		}
	}
	
	//Load game.
	public static void LoadGame(){
		//Load game progress
		lowestLayer = PlayerPrefs.GetInt("a", 50);
		deaths = PlayerPrefs.GetInt("b", 0);
		shots = PlayerPrefs.GetInt("c", 0);
		highMaxTimeAliveLegacy = PlayerPrefs.GetFloat("d", 0);
		coins = PlayerPrefs.GetInt("e", 0);
		healthRank = PlayerPrefs.GetInt("f", 0);
		shotSpeedRank = PlayerPrefs.GetInt("g", 0);
		speedRank = PlayerPrefs.GetInt("h", 0);
		shotSplitRank = PlayerPrefs.GetInt("i", 0);
		beatFirstLevel = PlayerPrefsX.GetBool("j", false);
		newShotCount = PlayerPrefs.GetInt("k", 0);
		newDeaths = PlayerPrefs.GetInt("l", 0);
		gotGun = PlayerPrefsX.GetBool("m", false);
		beatYellow = PlayerPrefsX.GetBool("n", false);
		beatGreen = PlayerPrefsX.GetBool("o", false);
		beatBlue = PlayerPrefsX.GetBool("p", false);
		beatPurple = PlayerPrefsX.GetBool("q", false);
		beatWhite = PlayerPrefsX.GetBool("r", false);
		maxEndlessLayer = PlayerPrefs.GetInt("s", 0);
        beatFinalBoss = PlayerPrefsX.GetBool("t", false);
        enemiesKilled = PlayerPrefs.GetInt("u", 0);
		ownsFloppyDisk = PlayerPrefsX.GetBool("v", false);
		maxMoneyBagsCollected = PlayerPrefs.GetInt("w", 0);
		unlockedShop = PlayerPrefsX.GetBool("x", false);
		unlockedBulletHell = PlayerPrefsX.GetBool("y", false);
		unlockedLegacy = PlayerPrefsX.GetBool("aa", false);
		unlockedLegacyBulletHell = PlayerPrefsX.GetBool("ab", false);
		beenToShop = PlayerPrefsX.GetBool("ac", false);
		beenToBulletHell = PlayerPrefsX.GetBool("ad", false);
		beenToLegacy = PlayerPrefsX.GetBool("af", false);
		beenToLegacyBulletHell = PlayerPrefsX.GetBool("ag", false);
		ownsLasers = PlayerPrefsX.GetBool("ah", false);
        savedVektor = PlayerPrefsX.GetBool("ai", false);
        beatGame = PlayerPrefsX.GetBool("aj", false);
        sawGoodEnding = PlayerPrefsX.GetBool("ak", false);

		//Load achievements.
		for(int i = 0; i < unlockedAchievements.Length; i++){
			unlockedAchievements[i] = PlayerPrefsX.GetBool("t"+i.ToString(), false);
		}
	}
	
	//Delete adventure mode progress.
	public static void DeleteAdventureSave(){
		coins = 0;
		healthRank = 0;
		shotSpeedRank = 0;
		speedRank = 0;
		shotSplitRank = 0;
		beatFirstLevel = false;
		newShotCount = 0;
		newDeaths = 0;
		gotGun = false;
		beatYellow = false;
		beatGreen = false;
		beatBlue = false;
		beatPurple = false;
		beatWhite = false;
        beatFinalBoss = false;
        enemiesKilled = 0;
		ownsFloppyDisk = false;
		unlockedShop = false;
		beenToShop = false;
        ownsLasers = false;
        savedVektor = false;
        beatGame = false;
        sawGoodEnding = false;
        SaveGame();
	}

	//Delete Save.
	public static void DeleteSave(){
		//Reset all save progress to default values.
		lowestLayer = 50;
		deaths = 0;
		shots = 0;
		highMaxTimeAlive = 0;
		highMaxTimeAliveLegacy = 0;
		coins = 0;
		healthRank = 0;
		shotSpeedRank = 0;
		speedRank = 0;
		shotSplitRank = 0;
		beatFirstLevel = false;
		newShotCount = 0;
		newDeaths = 0;
		gotGun = false;
		beatYellow = false;
		beatGreen = false;
		beatBlue = false;
		beatPurple = false;
		beatWhite = false;
		maxEndlessLayer = 0;
        beatFinalBoss = false;
        enemiesKilled = 0;
		ownsFloppyDisk = false;
		maxMoneyBagsCollected = 0;
		unlockedShop = false;
		unlockedBulletHell = false;
		unlockedLegacy = false;
		unlockedLegacyBulletHell = false;
		beenToShop = false;
		beenToBulletHell = false;
		beenToLegacy = false;
		beenToLegacyBulletHell = false;
        ownsLasers = false;
        savedVektor = false;
        beatGame = false;
        sawGoodEnding = false;

		//Save the fact that we just deleted the game.
        SaveGame();
	}

    //Unlocking max everything for steam test.
    public static void UnlockAll() {
		lowestLayer = 50;
		coins = 9999999;
		healthRank = 3;
		shotSpeedRank = 3;
		speedRank = 3;
		shotSplitRank = 2;
		beatFirstLevel = true;
		gotGun = true;
		beatYellow = true;
		beatGreen = true;
		beatBlue = true;
		beatPurple = true;
		beatWhite = false;
        beatFinalBoss = false;
        enemiesKilled = 0;
		ownsFloppyDisk = true;
		unlockedShop = true;
		unlockedBulletHell = true;
		unlockedLegacy = true;
		unlockedLegacyBulletHell = true;
        ownsLasers = true;
        SaveGame();
    }
	
	//Saving and loading game settings.
	public static void SaveSettings(){
		PlayerPrefs.SetFloat("sa", soundVolume);
		PlayerPrefs.SetFloat("sb", musicVolume);
		PlayerPrefsX.SetBool("sc", lightEffects);
		PlayerPrefsX.SetBool("sd", vSync);
		PlayerPrefsX.SetBool("se", isFullscreen);
		PlayerPrefsX.SetBool("sf", smileyMode);
		PlayerPrefsX.SetBool("sg", partyMode);
        PlayerPrefsX.SetBool("sh", tootMode);
        PlayerPrefsX.SetBool("si", speechSounds);
	}
	
	//Loading the game settings.
	public static void LoadSettings(){
		musicVolume = PlayerPrefs.GetFloat("sa", 1f);
		soundVolume = PlayerPrefs.GetFloat("sb", 1f);
		lightEffects = PlayerPrefsX.GetBool("sc", true);
		vSync = PlayerPrefsX.GetBool("sd", true);
		isFullscreen = PlayerPrefsX.GetBool("se", true);
		smileyMode = PlayerPrefsX.GetBool("sf", false);
		partyMode = PlayerPrefsX.GetBool("sg", false);
        tootMode = PlayerPrefsX.GetBool("sh", false);
        speechSounds = PlayerPrefsX.GetBool("si", false);
	}
	
	//Reset to default settings.
	public static void ResetSettings(){
		musicVolume = 1f;
		soundVolume = 1f;
		lightEffects = true;
		vSync = true;
		isFullscreen = true;
		smileyMode = false;
		partyMode = false;
        tootMode = false;
        speechSounds = false;
		ApplySettings();
		SaveSettings();
	}
	
	public static void ApplySettings(){
		//Apply vSync.
		if(vSync){ QualitySettings.vSyncCount = 1; }
		else { QualitySettings.vSyncCount = 0; }
	
		//Apply antiAiliasing.
		QualitySettings.antiAliasing = 0;
		
		//Apply fullscreen.
		Screen.fullScreen = isFullscreen;
	}

	public static void CheckAchievements(int i){
        Debug.Log("Checking Achievement #"+i.ToString());
        bool unlockedAll = true;
		if(steamy){
			switch(i){
				case 0: //Beat zone 1.
					SteamUserStats.SetAchievement("beat1");
					break;
				case 1: //Beat zone 2.
					SteamUserStats.SetAchievement("beat2");
					break;
				case 2: //Beat zone 3.
					SteamUserStats.SetAchievement("beat3");
					break;
				case 3: //Beat zone 4.
					SteamUserStats.SetAchievement("beat4");
					break;
				case 4: //Beat zone 5.
					SteamUserStats.SetAchievement("beat5");
					break;
				case 5: //Beat zone 6.
					SteamUserStats.SetAchievement("beat6");
					break;
				case 6: //Beat the warden.
					SteamUserStats.SetAchievement("beatwarden");
					break;
				case 7: //Get the good ending.
					SteamUserStats.SetAchievement("goodend");
					break;
				case 8: //Beat zone 1 without taking damage.
					SteamUserStats.SetAchievement("nodamage1");
					break;
				case 9: //Beat zone 2 without taking damage.
					SteamUserStats.SetAchievement("nodamage2");
					break;
				case 10: //Beat zone 3 without taking damage.
					SteamUserStats.SetAchievement("nodamage3");
					break;
				case 11: //Beat zone 4 without taking damage.
					SteamUserStats.SetAchievement("nodamage4");
					break;
				case 12: //Beat zone 5 without taking damage.
					SteamUserStats.SetAchievement("nodamage5");
					break;
				case 13: //Getting the money bag(s) in Zone 6.
					SteamUserStats.SetAchievement("whitebag");
					break;
				case 14: //Beat the warden without taking damage.
					SteamUserStats.SetAchievement("nodamagewarden");
					break;
				case 15: //Beat zone 2 with minimum stats.
					SteamUserStats.SetAchievement("min2");
					break;
				case 16: //Beat zone 3 with minimum stats.
					SteamUserStats.SetAchievement("min3");
					break;
				case 17: //Beat zone 4 with minimum stats.
					SteamUserStats.SetAchievement("min4");
					break;
				case 18: //Beat zone 5 with minimum stats.
					SteamUserStats.SetAchievement("min5");
					break;
				case 19: //Get the laser weapon.
					SteamUserStats.SetAchievement("laser");
					break;
				case 20: //Beat any zone while have the two-shot split. (Excluding zone 1!)
					SteamUserStats.SetAchievement("wizard");
					break;
				case 21: //Make your first purchase at the ship shop.
					SteamUserStats.SetAchievement("firstbuy");
					break;
				case 22: //Acquire your first refund at the 'Ship Shop'.
					SteamUserStats.SetAchievement("refund");
					break;
				case 23: //Go into debt.
					SteamUserStats.SetAchievement("debt");
					break;
				case 24: //Own the floppy disk without being in debt.
					SteamUserStats.SetAchievement("floppy");
					break;
				case 25: //Buy everything from the Ship shop while remaining out of debt.
					SteamUserStats.SetAchievement("allbought");
					break;
				case 26: //Survive in bullet hell mode for 30 seconds.
					SteamUserStats.SetAchievement("bh1");
					break;
				case 27: //Survive in bullet hell mode for 1 minute.
					SteamUserStats.SetAchievement("bh2");
					break;
				case 28: //Survive in bullet hell mode for 1 minute and 30 seconds.
					SteamUserStats.SetAchievement("bh3");
					break;
				case 29: //Survive in bullet hell mode for 2 minutes.
					SteamUserStats.SetAchievement("bh4");
					break;
				case 30: //Survive in bullet hell mode for 2 minutes and 30 seconds.
					SteamUserStats.SetAchievement("bh5");
					break;
				case 31: //Save Vektor.
					SteamUserStats.SetAchievement("saved");
					break;
				case 32: //Getting the back in the red zone.
					SteamUserStats.SetAchievement("redbag");
					break;
				case 33: //Turn on speech sounds.
					SteamUserStats.SetAchievement("wakawaka");
					break; 
				case 34: //Getting the bag in the yellow zone.
					SteamUserStats.SetAchievement("yellowbag");
					break;
				case 35: //Getting the bag in the green zone.
					SteamUserStats.SetAchievement("greenbag");
					break;
				case 36: //Getting the bag in the blue zone.
					SteamUserStats.SetAchievement("bluebag");
					break;
				case 37: //Getting the bag in the purple zone.
					SteamUserStats.SetAchievement("purplebag");
					break;
				case 38: //Make it down to layer 25 in Legacy Mode.
					SteamUserStats.SetAchievement("legacy1");
					break;
				case 39: //Make it down to layer 10 in Legacy Mode.
					SteamUserStats.SetAchievement("legacy2");
					break;
				case 40: //Beat legacy mode.
					SteamUserStats.SetAchievement("legacy3");
					break;
				case 41: //Beat legacy mode with all 5 hearts.
					SteamUserStats.SetAchievement("legacy4");
					break;
				case 42: //Survive for 15 seconds in 'Legacy Bullet Hell Mode'.
					SteamUserStats.SetAchievement("bhl1");
					break;
				case 43: //Survive for 30 seconds in 'Legacy Bullet Hell Mode'.
					SteamUserStats.SetAchievement("bhl2");
					break;
				case 44: //Survive for 60 seconds in 'Legacy Bullet Hell Mode'.
					SteamUserStats.SetAchievement("bhl3");
					break;
				case 45: //Pee for a minute straight.
					SteamUserStats.SetAchievement("pee");
					break;
				case 46: //Console Easter egg for kitboga.
					SteamUserStats.SetAchievement("password");
					break;
				case 47: //Getting 4 money bags in 'Bullet Hell Mode'.
					SteamUserStats.SetAchievement("bhmoney1");
					break;
				case 48: //Getting 8 money bags in 'Bullet Hell Mode'.
					SteamUserStats.SetAchievement("bhmoney2");
					break;
			}

            //store that this achievement is unlocked.
            unlockedAchievements[i] = true;

            //Unlock the everything achievement.
            for(int p = 0; p <= 48; p++) {
                //Debug.Log("Unlocked " + p.ToString() + "?: " + unlockedAchievements[p].ToString());
                if (!unlockedAchievements[p]) {
                    unlockedAll = false;
                }
            }
            if (unlockedAll) { //If all achievements have been unlocked, unlock the final one.
                //unlockedAchievements[49] = true;
                SteamUserStats.SetAchievement("all");
            }

            
            SaveGame();
			SteamUserStats.StoreStats();
			SteamAPI.RunCallbacks();
		}
	}

	public static void ControllerCheck(){
		//Checking for movement of the mouse.
		float mouseX = Input.GetAxisRaw("Mouse X");
		float mouseY = Input.GetAxisRaw("Mouse Y");
		float mouseThreshold = 0.9f;
		bool movedMouse = (mouseX > mouseThreshold || mouseX < -mouseThreshold || mouseY > mouseThreshold || mouseY < -mouseThreshold);

		//Checking for keyboard buttons.
		bool touchedKeys = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
			Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.X) ||
			Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
			Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)
		);

		//Checking if you move the gamepad axis.
		float leftAxisX = Input.GetAxisRaw("Horizontal [Gamepad]");
		float leftAxisY = Input.GetAxisRaw("Vertical [Gamepad]");
		float rightAxisX = Input.GetAxisRaw("Gamepad Right Stick X");
		float rightAxisY = Input.GetAxisRaw("Gamepad Right Stick Y");
		float padThreshold = 0.45f;
		bool movedAxis = (leftAxisX > padThreshold || leftAxisX < -padThreshold || leftAxisY > padThreshold || leftAxisY <-padThreshold ||
			rightAxisX > padThreshold || rightAxisX < -padThreshold || rightAxisY > padThreshold || rightAxisY <-padThreshold
		);

		//Checking if you touch a game pad button.
		bool touchedPad = (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1) ||  Input.GetKeyDown(KeyCode.Joystick1Button2) ||
			Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button4) ||  Input.GetKeyDown(KeyCode.Joystick1Button5) ||
			Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.Joystick1Button7) ||  Input.GetKeyDown(KeyCode.Joystick1Button8) ||
			Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.Joystick1Button10) ||  Input.GetKeyDown(KeyCode.Joystick1Button11) ||
			Input.GetKeyDown(KeyCode.Joystick1Button12) || Input.GetKeyDown(KeyCode.Joystick1Button13) ||  Input.GetKeyDown(KeyCode.Joystick1Button14) ||
			Input.GetKeyDown(KeyCode.Joystick1Button15) || Input.GetKeyDown(KeyCode.Joystick1Button16) ||  Input.GetKeyDown(KeyCode.Joystick1Button17) ||
			Input.GetKeyDown(KeyCode.Joystick1Button18) || Input.GetKeyDown(KeyCode.Joystick1Button19)
		);

		//Put it all together!
		if(movedAxis || touchedPad) { usingController = true; }
		if(touchedKeys || movedMouse){ usingController = false; }
	}
}

