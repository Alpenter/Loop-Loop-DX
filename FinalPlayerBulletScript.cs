using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPlayerBulletScript : MonoBehaviour {

    readonly float speed = 40f;
    GameObject poog = null;
    WardenScript warden = null;

    private void Awake() {
        poog = Resources.Load("New Bullet Poof") as GameObject;
    }

    private void FixedUpdate() {
        Vector3 pos = transform.position;
        pos.x += Time.deltaTime*speed;
        transform.position = pos;
        if(transform.childCount <= 0 || pos.x > 38) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.transform.parent.gameObject != null) {
            GameObject wardenObj = other.gameObject.transform.parent.gameObject;
            warden = wardenObj.GetComponent<WardenScript>();
            warden.HurtFinalPhase();
            GameObject explo = Instantiate(poog, transform.position, Quaternion.identity) as GameObject;
            Destroy(explo, 1f);
            Destroy(gameObject);
        }
    }
}
