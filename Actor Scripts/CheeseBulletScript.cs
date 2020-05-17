using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseBulletScript : MonoBehaviour {

    [HideInInspector] public float turnTime = 2f;
    bool offScreen = false;
    bool plannedDestroy = false;

	void Update () {
        if(turnTime > 0f) { turnTime -= Time.deltaTime*1.2f; }
        GetComponent<LookAtScript>().turnSpeed = turnTime;

        Vector3 pos = transform.position;
        offScreen = (pos.x < -38 || pos.x > 38 || pos.y > 23f || pos.y < -23f);
        GetComponent<Collider2D>().enabled = (!offScreen);
        
        if (offScreen && !plannedDestroy) {
            GetComponent<MoveForward>().enabled = false;
            Destroy(gameObject, 2f);
            plannedDestroy = true;
        }
    }
}
