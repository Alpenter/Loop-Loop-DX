using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDropScript : MonoBehaviour {
	
	void Start(){
		ParticleSystemRenderer rr = gameObject.GetComponent<ParticleSystemRenderer>();
        rr.material.color = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.333f);
	}
}
