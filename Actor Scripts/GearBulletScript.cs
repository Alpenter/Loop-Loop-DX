using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBulletScript : MonoBehaviour {

    readonly float speed = 22;
    float targetYVelocity = 0f;

    Vector2 vel = Vector2.zero;
    Vector3 pos = Vector3.zero;
    Vector3 playerPos = Vector3.zero;

    private void Update() {
        pos = transform.position;
        vel = GetComponent<Rigidbody2D>().velocity;

        if(Game.playerObj != null) {
            playerPos = Game.playerObj.transform.position;        
        }

        vel.x = -speed;
        float sameHeightSize = 0.1f;
        if(pos.y > (playerPos.y + sameHeightSize)) {
            targetYVelocity = -speed;
        } else if(pos.y < (playerPos.y - sameHeightSize)) {
            targetYVelocity = speed;
        } else if(pos.y > (playerPos.y - sameHeightSize) && pos.y < (playerPos.y + sameHeightSize)) {
            targetYVelocity = 0;
        }
        float ySmooth = 1f;
        vel.y = Mathf.Lerp(vel.y, targetYVelocity, ySmooth * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = vel;

        if(pos.x < -50f) { Destroy(gameObject); }
    }
}
