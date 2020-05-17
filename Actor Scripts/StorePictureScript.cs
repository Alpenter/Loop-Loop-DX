using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePictureScript : MonoBehaviour {
	
	GameObject happy, sad;
	
	void Awake(){
		happy = transform.Find("Vektor Happy").gameObject;
		sad = transform.Find("Vektor Sad").gameObject;
	}
	
	void Update () {
        if (Game.beatWhite && !Game.savedVektor) {
            happy.SetActive(false);
            sad.SetActive(false);
        } else {
            if(Game.coins >= 0){
			    happy.SetActive(true);
			    sad.SetActive(false);
		    } else {
			    happy.SetActive(false);
			    sad.SetActive(true);
		    }
        }
	}
}
