using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserScript : MonoBehaviour {
    
    float lifeTime = 0f;
    GameObject exploPuppet = null;
    [HideInInspector] public GameObject midPoint = null;

    private void Awake() {
        GetComponent<BoxCollider2D>().enabled = false;
        exploPuppet = transform.Find("explo puppet").gameObject;
        exploPuppet.GetComponent<MeshRenderer>().material.color = Game.bulletColor;
        midPoint = transform.Find("mid point").gameObject;
    }

    private void Update() {
        lifeTime += Time.deltaTime;
        GetComponent<BoxCollider2D>().enabled = (lifeTime >= 0.4f && lifeTime < 1f);
        if(lifeTime > 1.3f) {
            Destroy(gameObject);
        }
    }
}
