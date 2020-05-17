using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonScript : MonoBehaviour {
	
	public enum Setting {
		SFXDown,
		SFXUp,
		MusicDown,
		MusicUp,
		LightEffects,
		VSync,
		Benefits,
		Fullscreen,
		ResolutionUp,
		ResolutionDown,
		SmileyMode,
		PartyMode,
        TootMode,
        SpeechSounds,
	}
	public Setting settingType;
	GameObject X = null;
	MenuManagerScript menuMan = null;
	GameObject puppet = null;
	AudioClip snd = null;
	int numToSend = 0;
	public bool atMenu = false;

	void Awake(){
		puppet = transform.Find("puppet").gameObject;
		snd = Resources.Load("SFX/Button Click") as AudioClip;
		if(settingType == Setting.LightEffects || settingType == Setting.VSync || settingType == Setting.Benefits || settingType == Setting.Fullscreen || settingType == Setting.SmileyMode || settingType == Setting.PartyMode || settingType == Setting.TootMode || settingType == Setting.SpeechSounds){
			X = transform.Find("x").gameObject;
		}
	}

	void Start(){
		if(atMenu){
			menuMan = Game.manager.GetComponent<MenuManagerScript>();
		}
	}
	
	void OnMouseDown(){
		PlaySound.NoLoop(snd);
		if(atMenu){ 
			switch(settingType){
				case Setting.SFXDown:
					numToSend = 0;
					break;
				case Setting.SFXUp:
					numToSend = 1;
					break;
				case Setting.MusicDown:
					numToSend = 2;
					break;
				case Setting.MusicUp:
					numToSend = 3;
					break;
				case Setting.LightEffects:
					numToSend = 4;
					break;
				case Setting.VSync:
					numToSend = 5;
					break;
				case Setting.Fullscreen:
					numToSend = 6;
					break;
				case Setting.ResolutionDown:
					numToSend = 7;
					break;
				case Setting.ResolutionUp:
					numToSend = 8;
					break;
				case Setting.SmileyMode:
					numToSend = 9;
					break;
				case Setting.PartyMode:
					numToSend = 10;
					break;
                case Setting.TootMode:
                    numToSend = 11;
                    break;
                case Setting.SpeechSounds:
                    Game.CheckAchievements(33);
                	numToSend = 12;
                	break;
			}
			menuMan.SettingAction(numToSend);
		} else { 
			if(settingType == Setting.SFXDown){
				if(Game.soundVolume > 0f){ Game.soundVolume = Game.soundVolume - 0.1f; }
			} else if(settingType == Setting.SFXUp){
				if(Game.soundVolume < 1f){ Game.soundVolume = Game.soundVolume + 0.1f; }
			} else if(settingType == Setting.MusicDown){
				if(Game.musicVolume > 0f){ Game.musicVolume = Game.musicVolume - 0.1f; }
			} else if(settingType == Setting.MusicUp){
				if(Game.musicVolume < 1f){ Game.musicVolume = Game.musicVolume + 0.1f; }
			} else if(settingType == Setting.LightEffects){
				Game.lightEffects = !Game.lightEffects;
			} else if(settingType == Setting.VSync){
				Game.vSync = !Game.vSync;
			} else if(settingType == Setting.Fullscreen){
				Game.isFullscreen = !Game.isFullscreen;
			} else if(settingType == Setting.ResolutionDown){
				if(menuMan.currentRes > 0){ menuMan.currentRes--; }
				menuMan.ApplyResolution();
			} else if(settingType == Setting.ResolutionUp){
				if(menuMan.currentRes < menuMan.resLength){ menuMan.currentRes++; }
				menuMan.ApplyResolution();
			} else if(settingType == Setting.SpeechSounds){
                Game.CheckAchievements(33);
				Game.speechSounds = !Game.speechSounds;
			}
		}
		if(Time.timeScale > 0.5f) { WoobleEffect(); }
		Game.ApplySettings();
		Game.SaveSettings();
	}
	
	void Update(){
		switch(settingType){
			case Setting.LightEffects:
				X.SetActive(Game.lightEffects);
				break;
			case Setting.VSync:
				X.SetActive(Game.vSync);
				break;
			case Setting.Fullscreen:
				X.SetActive(Game.isFullscreen);
				break;
			case Setting.SmileyMode:
				X.SetActive(Game.smileyMode);
				break;
			case Setting.PartyMode:
				X.SetActive(Game.partyMode);
				break;
            case Setting.TootMode:
                X.SetActive(Game.tootMode);
                break;
            case Setting.SpeechSounds:
            	X.SetActive(Game.speechSounds);
            	break;
		}
	}
	
	private void WoobleEffect(){
		GameObject o = Instantiate(puppet, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1), puppet.transform.rotation) as GameObject;
		//o.transform.localScale = puppet.transform.localScale;
		Destroy(o.GetComponent<MatchFrontColorScript>());
		float tweenTime = 1f;
		//Scale.
		iTween.ScaleTo(o, iTween.Hash(
			"x", puppet.transform.localScale.x + 0.25f,
			"y", puppet.transform.localScale.y + 0.25f,
			"z", 1f,
			"time", tweenTime
		));
		//Color.
		iTween.ColorTo(o, iTween.Hash(
			"a", 0f,
			"time", (tweenTime/2f),
			"easetype", iTween.EaseType.easeInOutSine,
			"looptype", iTween.LoopType.none
		));		
		//Destroy.
		Destroy(o, tweenTime);
	}
}
