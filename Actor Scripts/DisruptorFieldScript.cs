using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisruptorFieldScript : MonoBehaviour {
    
    GameObject puppetOne, puppetTwo;
    
    Color colorToBe = Color.white;

    readonly float alphaColor = 0.35f;
    float scrollSlowdown = 2f;

    private void Awake() {
        puppetOne = transform.Find("puppet one").gameObject;
        puppetTwo = transform.Find("puppet two").gameObject;
    }

    private void Start() {
        Vector3 objScale = transform.localScale;
        Vector2 scale = new Vector2(objScale.x/10, objScale.y/10);
        puppetOne.GetComponent<MeshRenderer>().material.mainTextureScale = scale;
        puppetTwo.GetComponent<MeshRenderer>().material.mainTextureScale = scale;
    }

    private void Update() {
        //What changes when the player is in the disruptor or not.
        if (Game.inDisruptor) {
            colorToBe = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, alphaColor);
            scrollSlowdown = 2f;
        } else {
            colorToBe = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, alphaColor);
            scrollSlowdown = 4f;
        }

        //Apply values to the puppets.
        float smooth = 25f;
        puppetOne.GetComponent<MeshRenderer>().material.color = Color.Lerp(puppetOne.GetComponent<MeshRenderer>().material.color, colorToBe, smooth*Time.deltaTime);
        puppetTwo.GetComponent<MeshRenderer>().material.color = Color.Lerp(puppetTwo.GetComponent<MeshRenderer>().material.color, colorToBe, smooth*Time.deltaTime);
        puppetOne.GetComponent<ScrollMatOffsetScript>().slowdown = scrollSlowdown;
        puppetTwo.GetComponent<ScrollMatOffsetScript>().slowdown = scrollSlowdown;
    }
}
