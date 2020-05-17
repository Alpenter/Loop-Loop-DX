using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour {
	void Start(){
   		DontDestroyOnLoad(gameObject);
	}
}
