using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFrontColorScript : MonoBehaviour {

	public bool matchBackInstead = false;
	public bool onlyOnAwake = false;
	
	void Awake(){
		if(onlyOnAwake){
			if(matchBackInstead){
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.backColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.backColor;
				} else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.backColor;
                }
			} else {			
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.frontColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.frontColor;
				} else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.frontColor;
                }
			}
		}
	}
	
	void Start(){
		if(!onlyOnAwake){
			if(matchBackInstead){
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.backColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.backColor;
				} else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.backColor;
                }
			} else {			
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.frontColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.frontColor;
				}  else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.frontColor;
                }
			}
		}
	}
	
	void Update () {
		if(!onlyOnAwake){
			if(matchBackInstead){
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.backColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.backColor;
				} else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.backColor;
                }
			} else {			
				if(GetComponent<MeshRenderer>() != null){
					GetComponent<MeshRenderer>().material.color = Game.frontColor;
				} else if (GetComponent<TextMesh>() != null){
					GetComponent<TextMesh>().color = Game.frontColor;
				} else if(GetComponent<ParticleSystem>() != null) {
                    var ps = GetComponent<ParticleSystem>().main;
                    ps.startColor = Game.frontColor;
                }
			}
		}
	}
}
