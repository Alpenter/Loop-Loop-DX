using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBombWaterWooshEffectScript : MonoBehaviour {

    GameObject puppet = null;
    Renderer rend;


    void Awake() {
        puppet = transform.Find("puppet").gameObject;
        rend = puppet.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("FX/Glass/Stained BumpDistort");

        iTween.ScaleTo(gameObject, iTween.Hash(
            "time", 1.9f,
            "x", 75f,
            "y", 75f,
            "z", 1f,
            "easeType", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.none
        ));

        iTween.ValueTo(gameObject, iTween.Hash(
              "from", 128f,
              "to", 0f,
              "time", 1.9f,
              "onupdatetarget", gameObject,
              "easetype", iTween.EaseType.easeInOutSine,
              "onupdate", "SetBump",
              "looptype", iTween.LoopType.none
          ));
    }

    void SetBump(float bmp){
        rend.material.SetFloat("_BumpAmt", bmp);
    }
}