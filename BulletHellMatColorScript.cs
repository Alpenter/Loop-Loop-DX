using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellMatColorScript : MonoBehaviour {

	void Update () {
		if(Game.manager != null){
			GetComponent<MeshRenderer>().material.color = Game.backColor;
		}
	}
}
