using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFrontColorWithAlphaScript : MonoBehaviour {
	
	public float alpha = 1f;
	
	void Update(){
		GetComponent<MeshRenderer>().material.color = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, alpha);
	}
}
