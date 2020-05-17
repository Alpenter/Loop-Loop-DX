using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParticleColor : MonoBehaviour {
	
	float r,g,b;
	Color particleColor;
	
	// Use this for initialization
	void Awake () {
		ParticleSystem ps = GetComponent<ParticleSystem>();
		var main = ps.main;
		r = 1f - Camera.main.backgroundColor.r;
		g = 1f - Camera.main.backgroundColor.g;
		b = 1f - Camera.main.backgroundColor.b;
		particleColor = new Color(r, g, b, 1f);
		main.startColor =  particleColor;
	}
}
