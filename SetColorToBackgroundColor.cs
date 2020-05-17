using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorToBackgroundColor : MonoBehaviour {
	
	void Update(){
		GetComponent<Renderer>().material.color = Camera.main.backgroundColor;
	}
}
