using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSoundTextMatchScript : MonoBehaviour {
	
	public enum Sound {
		SFX,
		Music,
		Resolution,
	}
	public Sound snd;
	
	void Update () {
		if(snd == Sound.SFX){
			GetComponent<TextMesh>().text = Mathf.RoundToInt(Game.soundVolume * 10f).ToString();
		} else if(snd == Sound.Music){
			GetComponent<TextMesh>().text = Mathf.RoundToInt(Game.musicVolume * 10f).ToString();
		} else if(snd == Sound.Resolution){
			GetComponent<TextMesh>().text = Screen.width.ToString() + " x " + Screen.height.ToString();	
		}
	}
}
