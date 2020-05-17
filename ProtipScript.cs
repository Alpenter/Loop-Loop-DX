using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtipScript : MonoBehaviour {

    void Start() {
    	if(Game.cameFromLegacy){
    		GetComponent<TextMesh>().text = "Tip: Alas, this game mode is... tipless...";
    	} else {
	    	string[] texts = { 
	    		"Tip: Staying still for 7 seconds will result in death!",
	    		"Tip: This is the only game mode other than 'Adventure Mode' where you can earn money!",
	    		"Tip: The middle is usually a good place to be! Emphasis on 'usually'.",
	    		"Tip: Bullets with a white circle in the middle will loop around the screen once.",
	    		"Tip: Pressing 'Space' not only destroys the bullets... it destroys the money bags!",
	    		"Fun Fact: The developer's high score is 3:51.",
	    		"Fun Fact: This game mode was made first!",
	    	};
	    	int i = Random.Range(0, texts.Length);
	    	GetComponent<TextMesh>().text = texts[i];
    	}	
    }
}
