using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollThroughRainbowColorsScript : MonoBehaviour {
	
	float r = 1f;
	float g = 0f;
	float b = 0f;
	float a = 0f;
	public float slowdown = 0.25f;
	
	public enum SubColorMode{
		subR,
		subG,
		subB,
	}
	SubColorMode sMode = SubColorMode.subR;
	
	void Awake(){
		//Store alpha to keep.
		if(GetComponent<MeshRenderer>() != null){
			a = GetComponent<MeshRenderer>().material.color.a;
		} else if(GetComponent<SpriteRenderer>() != null){
			a = GetComponent<SpriteRenderer>().material.color.a;
		}
	}

	void Update () {
		
		//Set color limits.
		if(b > 1f){ b = 1f; }
		if(r > 1f){ r = 1f; }
		if(g > 1f){ g = 1f; }
		if(b < 0f){ b = 0f; }
		if(r < 0f){ r = 0f; }
		if(g < 0f){ g = 0f; }
		
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
		
		//Apply colors.
		if(GetComponent<MeshRenderer>() != null){
			GetComponent<MeshRenderer>().material.color = new Color(r, g, b, a);
		}
		if(GetComponent<SpriteRenderer>() != null){
			GetComponent<SpriteRenderer>().material.color = new Color(r, g, b, a);			
		}

	}
}
