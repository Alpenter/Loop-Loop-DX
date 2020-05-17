using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAmmoPickupScript : MonoBehaviour {
	
	float StartPointX = 0f;
	float StartPointY = 0f;
	float ControlPointX	= 20f;
	float ControlPointY	= 50f;
	float EndPointX = 0f;
	float EndPointY = 0f;
	float CurveX = 0f;
	float CurveY = 0f;
	float BezierTime = 0f;
	
	bool got = false;
	
    GameObject puppet = null;
    GameObject particleObj = null;

    AudioClip snd;
	
    

	void Awake(){
		puppet = transform.Find("puppet").gameObject;
		puppet.GetComponent<MeshRenderer>().material.color = Game.bulletColor;

        particleObj = transform.Find("particles").gameObject;
        ParticleSystem.MainModule psMain = particleObj.GetComponent<ParticleSystem>().main;
        psMain.startColor = Game.bulletColor;
	}
	
	void Start(){
        if (!Game.ownsLasers) { Destroy(gameObject); }
		Vector3 pos = transform.position;
		StartPointX = pos.x;
		StartPointY = pos.y;
		Vector3 dif = pos - Game.playerObj.transform.position;
		ControlPointX = dif.x;
		ControlPointY = dif.y;
		if(Game.playerObj.transform.position.y >= 0){ ControlPointY = ControlPointY - 34f; }
		else { ControlPointY = ControlPointY + 34f; }
		snd = Resources.Load("SFX/Laser Drop Pickup") as AudioClip;
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
				if(Game.currentLasers < Game.maxLasers){ Game.currentLasers++; }
				float size = 10f;
				PlaySound.NoLoop(snd);
				iTween.ScaleTo(puppet, new Vector3(size, size/2f, 1f), 1f);
				iTween.ColorTo(puppet, new Color(puppet.GetComponent<MeshRenderer>().material.color.r,puppet.GetComponent<MeshRenderer>().material.color.g, puppet.GetComponent<MeshRenderer>().material.color.b, 0f), 1f);
				particleObj.GetComponent<ParticleSystem>().Stop();
                got = true;
			}
		} else {
			Destroy(gameObject, 1f);
		}
	}
}
