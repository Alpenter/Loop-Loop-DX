using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSixBackgroundSpinControllerScript : MonoBehaviour {
    
    readonly float spinVal = 0.25f;
    float currentSpinVal = 0f;

    RotateScript sphereRS, glitterRS;

    private void Awake() {
        GameObject sphere = transform.Find("Crystal Sphere").gameObject;
        GameObject glitter = transform.Find("Glitter").gameObject;
        
        sphereRS = sphere.GetComponent<RotateScript>();
        glitterRS = glitter.GetComponent<RotateScript>();

        //establish parameter hash:
        float t = 25f;
        Hashtable ht = iTween.Hash("from", -spinVal, "to", spinVal, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.pingPong, "time", t, "onupdate", "ChangeSpinValue");

        //make iTween call:
        iTween.ValueTo(gameObject,ht);
    }

    //since our ValueTo() iscalculating floats the "onupdate" callback will expect a float as well:
    void ChangeSpinValue(float newValue){
        //apply the value of newValue:
        currentSpinVal = newValue;
    }

    private void Update() {
        sphereRS.rotationSpeed = currentSpinVal;
        glitterRS.rotationSpeed = -currentSpinVal;
    }
}