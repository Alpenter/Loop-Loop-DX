using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertColor : MonoBehaviour {

	float r,g,b;
	public bool dontUpdate = false;

	void Awake(){
		if(!dontUpdate){
			r = 1f - Game.backColor.r;
			g = 1f - Game.backColor.g;
			b = 1f - Game.backColor.b;
			GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
		}
	}

	void Start(){
		r = 1f - Game.backColor.r;
		g = 1f - Game.backColor.g;
		b = 1f - Game.backColor.b;
		GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
	}
	
	void Update(){
		if(!dontUpdate){
			r = 1f - Game.backColor.r;
			g = 1f - Game.backColor.g;
			b = 1f - Game.backColor.b;
			GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
		}
	}
}
