using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletHellDeathMenuScript : MonoBehaviour {

	float holdTime = 1f, counter = 0f;
	float loadTime = 0f;
	
	int lvl = 0;
	int mult = 1;
	
	bool holding = false;
	bool loading = false;
	bool startedSwipe = false;
	
    string inputTracker = "";

	GameObject fillSphere, swiper, line;
	GameObject surviveTime, highestScore, currentScore;
	
	TimeSpan timeSpan, highTimeSpan;
	
	string sTime, high, current;
	
	void Awake(){ 
		fillSphere = transform.Find("fill sphere").gameObject; 
		swiper = transform.Find("swipe").gameObject;
		line = transform.Find("line").gameObject;
		
		//Find text objs.
		surviveTime = transform.Find("survive time").gameObject;
		highestScore = transform.Find("highest score").gameObject;
		currentScore = transform.Find("current score").gameObject;

		//The reason for this code is because, if we used the back color from legacy bullet hell mode, the text would be too dark too see.
		if(Game.cameFromLegacy){ Game.backColor = Game.frontColor; }
	}

	//I'm just using Start to check for achievements.
	void Start(){

		//Checking for regular achievements.
		if(!Game.cameFromLegacy){
			if(Game.maxTimeAlive >= 30){ Game.CheckAchievements(26); }
			if(Game.maxTimeAlive >= 60){ Game.CheckAchievements(27); }
			if(Game.maxTimeAlive >= 120){ Game.CheckAchievements(28); }
			if(Game.maxTimeAlive >= 180){ Game.CheckAchievements(29); }
			if(Game.maxTimeAlive >= 210){ Game.CheckAchievements(30); }
		} else { //Check for legacy achievements.
			if(Game.maxTimeAliveLegacy >= 15f) { Game.CheckAchievements(42); }
			if(Game.maxTimeAliveLegacy >= 30f){ Game.CheckAchievements(43); }
			if(Game.maxTimeAliveLegacy >= 60f){ Game.CheckAchievements(44); }
		}

	}

	
	void Update(){
		//Show cursor.
		Cursor.visible = true;
		
        //Check if you are using a controller.
        Game.ControllerCheck();
        if (Game.usingController) {
            inputTracker = "Horizontal [Gamepad]";
        } else {
            inputTracker = "Horizontal";
        }

		//track your x input.
		float x = Input.GetAxis(inputTracker);
		
		//If holding right, retry level.
		if(x > 0.6f && !loading){ 
			if(Game.cameFromLegacy){ lvl = 3; }
			else { lvl = 4; }
			mult = 1;
			holding = true; 
		}
		
		//If holding left, return to menu.
		if(x < -0.6f && !loading){
			lvl = 1;
			mult = -1;
			holding = true; 
		}
		
		//when you aren't holding.
		if(x < 0.6 && x > -0.6f){ holding = false; }
		
		//When you are holding, up goes the counter.
		if(holding && !loading){
			counter += Time.deltaTime*1.5f;
			if(counter >= holdTime){
				loading = true;
			}
		} else if(!holding && !loading) {
			counter = 0;
		}
		
		//Manage scale of fill sphere.
		fillSphere.transform.localScale = new Vector3(counter/4f, counter/4f, 1f);
		
		//Manage line.
		LineRenderer l = line.GetComponent<LineRenderer>();
		l.SetPosition(0, new Vector3(0, -3.5f, 0f));
		l.SetPosition(1, new Vector3((counter*2f)*mult, -3.5f, 0f));
		l.material.color = Game.backColor;
		
		//Writing text.
		if(!Game.cameFromLegacy){
			timeSpan = TimeSpan.FromSeconds(Game.maxTimeAlive);
			highTimeSpan = TimeSpan.FromSeconds(Game.highMaxTimeAlive);
			sTime = string.Format("...and survived for {0:D1} minutes and {1:D2} seconds.", timeSpan.Minutes, timeSpan.Seconds);
			current = string.Format("Current Score [{0:D1}:{1:D2}]", timeSpan.Minutes, timeSpan.Seconds);
			high = string.Format("High Score [{0:D1}:{1:D2}]", highTimeSpan.Minutes, highTimeSpan.Seconds);
		} else {
			sTime = "...and survived for "+ Game.maxTimeAliveLegacy +" seconds.";
			current = "Current Score ["+ Game.maxTimeAliveLegacy +"]";
			high = "High Score ["+ Game.highMaxTimeAliveLegacy +"]";
		}
		surviveTime.GetComponent<TextMesh>().text = sTime;
		highestScore.GetComponent<TextMesh>().text = high;
		currentScore.GetComponent<TextMesh>().text = current;
		
		//Loading 
		if(loading){
			if(!startedSwipe){
				iTween.MoveTo(swiper, iTween.Hash(
					"x", 0f,
					"y", 0f,
					"time", 0.4f,
					"easetype", iTween.EaseType.easeInOutSine,
					"looptype", iTween.LoopType.none
				));
				startedSwipe = true;
			}
			loadTime += Time.deltaTime;
			if(loadTime > 0.5f){
				SceneManager.LoadScene(lvl);
			}
		}
	}
}
