using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBasedOnControllerScript : MonoBehaviour {
    
	public enum ControlType {
		Keyboard,
		Gamepad,
	}
	public ControlType enableIf = ControlType.Keyboard;


	void Update(){
		if(enableIf == ControlType.Keyboard){
			GetComponent<Renderer>().enabled = !Game.usingController;
		}

		if(enableIf == ControlType.Gamepad){
			GetComponent<Renderer>().enabled = Game.usingController;
		}
	}
}
