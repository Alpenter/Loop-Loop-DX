using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour {
	
	float StartPointX		= 0f;
	float StartPointY 		= 0f;
	float ControlPointX 	= 20f;
	float ControlPointY 	= 50f;
	float EndPointX			= 0f;
	float EndPointY 			= 0f;
	float CurveX 				= 0f;
	float CurveY 				= 0f;
	float BezierTime 		= 0f;
	
	bool got = false;
	public bool newHeart = false;
	AudioClip snd;
	
	void Awake(){
		if(newHeart){
			GameObject puppet = transform.Find("puppet").gameObject;
			puppet.GetComponent<MeshRenderer>().material.color = Game.frontColor;
		}
	}
	
	void Start(){
		Vector3 pos = transform.position;
		StartPointX = pos.x;
		StartPointY = pos.y;
		Vector3 dif = pos - Game.playerObj.transform.position;
		ControlPointX = dif.x;
		ControlPointY = dif.y;
		if(Game.playerObj.transform.position.y >= 0){ ControlPointY = ControlPointY - 34f; }
		else { ControlPointY = ControlPointY + 34f; }
		snd = Resources.Load("SFX/Health Pickup") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
		if(!got && Game.playerObj != null) {
			Vector3 playerPos = Game.playerObj.transform.position;
			EndPointX = playerPos.x;
			EndPointY = playerPos.y;
			
			BezierTime += Time.deltaTime;
			
			CurveX = (((1-BezierTime)*(1-BezierTime)) * StartPointX) + (2 * BezierTime * (1 - BezierTime) * ControlPointX) + ((BezierTime * BezierTime) * EndPointX);
			CurveY = (((1-BezierTime)*(1-BezierTime)) * StartPointY) + (2 * BezierTime * (1 - BezierTime) * ControlPointY) + ((BezierTime * BezierTime) * EndPointY);
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(CurveX, CurveY, 0), 7f *Time.deltaTime);
			
			if(BezierTime >= 1.11f){
				GameObject puppet = transform.Find("puppet").gameObject;
				if(!newHeart){
					if(Game.currentHealth < 5){ Game.currentHealth++; }
				} else {
					if(Game.newCurrentHealth < Game.newMaxHealth){ Game.newCurrentHealth++; }
				}
				float size = 10f;
				PlaySound.NoLoop(snd);
				iTween.ScaleTo(puppet, new Vector3(size, size, 1f), 1f);
				iTween.ColorTo(puppet, new Color(puppet.GetComponent<MeshRenderer>().material.color.r,puppet.GetComponent<MeshRenderer>().material.color.g, puppet.GetComponent<MeshRenderer>().material.color.b, 0f), 1f);
				got = true;
			}
		} else {
			Destroy(gameObject, 1f);
		}
	}
}
