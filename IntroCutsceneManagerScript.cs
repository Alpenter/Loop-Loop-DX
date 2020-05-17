using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneManagerScript : MonoBehaviour {
	
	float sceneDuration = 68f;
	float skipCounter, skipTime = 2f;
	float holdingTime = 1f;
	float skipLoadTime = 1.1f;
	
	bool showSkip = false;
	bool triggeredLoading = false;
	bool faded = false;
	[HideInInspector] public bool gotGun = false;
	
	Vector3 targetPos = Vector3.zero;
	GameObject skip, bar, fade;
	
	void Awake(){
		skip = transform.Find("Skip Notice").gameObject; 
		bar = skip.transform.Find("bar").gameObject;
		fade = transform.Find("skip fade").gameObject;
	}
	
	void Update () {
        Game.ControllerCheck();
        
        Cursor.visible = false;

        GetComponent<AudioSource>().volume = Game.musicVolume;

		if(!triggeredLoading){
			//When scene is over, go to the first level.
			sceneDuration -= Time.deltaTime;
			if(sceneDuration <= 0f){
				SceneManager.LoadScene(7);
			}
			
			//push any button, because you are too impatient for the cutscene and wanna move on!
			if(Input.anyKey){
				skipCounter = skipTime;
				showSkip = true;
			}
			
			
			if(showSkip){
				skipCounter -= Time.deltaTime;
				targetPos = new Vector3(10.5f, -5f, 13f);
				if(skipCounter <= 0){ showSkip = false; }
			} else {
				targetPos = new Vector3(18.5f, -5, 13f);
			}
			skip.transform.localPosition = Vector3.Lerp(skip.transform.localPosition, targetPos, 2f*Time.deltaTime);
			
			//Holding space for one second skips the cutscene.
			if(Input.GetButton("Wipe")){
				holdingTime -= Time.deltaTime;
			} else {
				holdingTime = 1f;
			}
			
			//triggered lmao.
			if(holdingTime <= 0f){ triggeredLoading = true; }
			
			//Scale the bar to show how long you should hold space for.
			if(holdingTime < 0f){ holdingTime = 0f; }
			bar.transform.localScale = new Vector3(holdingTime*7f, 0.1f, 1f);
		} else {
			//Debug.Log("Herro?");
			if(!faded){
				iTween.FadeTo(fade, iTween.Hash(
					"a", 1f,
					"time", 1f,
					"easetype", iTween.EaseType.linear,
					"looptype", iTween.LoopType.none
				));
			faded = true;
			}
			skipLoadTime -= Time.deltaTime;
			if(skipLoadTime <= 0f){ SceneManager.LoadScene(7); }
		}
	}
}
