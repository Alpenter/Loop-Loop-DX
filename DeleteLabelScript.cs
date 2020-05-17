using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLabelScript : MonoBehaviour {
	
	string t = "";

	void Update(){
		if(Game.manager.GetComponent<MenuManagerScript>().dType == MenuManagerScript.DeleteType.All){
			t = "You are about to delete ALL save data!";
		} else {
			t = "You are about to delete all" + '\n' + "'Adventure Mode' progress!";
		}
		gameObject.GetComponent<TextMesh>().text = t;
	}
}
