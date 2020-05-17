using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTextureScript : MonoBehaviour {

    public Texture textore;
    
	void Start () {
        GetComponent<MeshRenderer>().material.mainTexture = textore;	
	}
}
