using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class SplashScreenManager : MonoBehaviour {

	float timeTillLoad = 14.1f;

	void Awake(){
		Game.steamy = true;		
	}

	void Start(){
		Game.LoadGame();
		Game.LoadSettings();
		Game.ApplySettings();

		if(Game.steamy){ 
			Game.boughtSupporterPack = SteamApps.BIsSubscribedApp((AppId_t)1194021);
		} else {
			Game.boughtSupporterPack = false;
		}
		//Debug.Log("Supporter Pack? : " + Game.boughtSupporterPack.ToString());
	}
	
	void Update(){
		timeTillLoad -= Time.deltaTime;
		if(timeTillLoad <= 0){
			//Debug.Log("Loading!");
			SceneManager.LoadScene(1);
		}
	}
}