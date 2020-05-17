using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurposeBulletScript : MonoBehaviour {

    GameObject effectObj = null;

    readonly float effectSpawnTime = 0.05f;
    readonly float speed = -12.5f;
    float effectSpawnCounter = 0f;
    float randomXChange;

    int changeDir = 0;

    private void Awake() {
        effectObj = Resources.Load("Purpose Bullet Effect") as GameObject;
        changeDir = Random.Range(0, 2);
        randomXChange = Random.Range(-9f, 0f);
    }
    
    void Update() {
        //Position of the bullet.
        Vector3 pos = transform.position;
        
        //Destroy past offscreen on x to the left.
        if(pos.x < -37f) { Destroy(gameObject); }

        //Spawn the effect of the bullet.
        effectSpawnCounter -= Time.deltaTime;
        if (effectSpawnCounter <= 0) {
            Instantiate(effectObj, pos, Quaternion.identity);
            effectSpawnCounter = effectSpawnTime;
        }

        //Velocity of the bullet.
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;

        float xChange = 8f;
        
        if(pos.x < xChange) {
            if(pos.x < randomXChange) {
                switch (changeDir) {
                    case 0:
                        vel.y = speed;
                        break;
                    case 1:
                        vel.y = -speed;
                        break;
                }
            } else {
                switch (changeDir) {
                    case 0:
                        vel.y = -speed;
                        break;
                    case 1:
                        vel.y = speed;
                        break;
                }
            }
        } else {
            vel.x = speed;
        }

        GetComponent<Rigidbody2D>().velocity = vel;
    }
}
