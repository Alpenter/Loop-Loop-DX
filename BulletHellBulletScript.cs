using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellBulletScript : MonoBehaviour {

	float maxSpeed = 0;
	Vector3 pos = Vector3.zero;

	void Start(){
		maxSpeed = Random.Range(18f, 20f);
		pos = transform.position;
	}

	void Update () {
		Vector3 velocity = new Vector3(0, maxSpeed * Time.deltaTime, 0);
		pos += transform.rotation * velocity;

		//Other side of screen.
		if(pos.x < -37){ pos.x = 36.5f; }
		if(pos.x > 37){ pos.x = -36.5f; }
		if(pos.y > 22f){ pos.y = -21.5f; }
		if(pos.y < -22f){ pos.y = 21.5f; }

		transform.position = pos;
	}
}
