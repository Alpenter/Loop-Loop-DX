using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;

public class LowerDistortionOvertimeScript : MonoBehaviour {

    public float speed = 1f;
    Material mat = null;
    float startbump;

    private void Awake() {
        mat = GetComponent<MeshRenderer>().material;    
        startbump = mat.GetFloat("_BumpAmt");
    }

    void Update() {
        startbump -= Time.deltaTime*speed;
        if(startbump <= 0) { startbump = 0; }
        mat.SetFloat("_BumpAmt", startbump);
    }
}
