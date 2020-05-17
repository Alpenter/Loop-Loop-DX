using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeBackgroundScript : MonoBehaviour {
	
	readonly float shrinkSpeed = 0.1f;

    // Update is called once per frame
    void Update() {
        //Manage position.
		Vector3 pos = transform.position;
		pos.y -= Time.deltaTime;
		transform.position = pos;
		
		//Manage scale.
		Vector3 scale = transform.localScale;
		scale.x -= Time.deltaTime*shrinkSpeed;
		scale.y = 0.1f;
		scale.z = scale.x;
		transform.localScale = scale;
		
		//Destroy me if I'm too smol.
		if(scale.x < 0){
			Destroy(gameObject);
		}
    }
}
