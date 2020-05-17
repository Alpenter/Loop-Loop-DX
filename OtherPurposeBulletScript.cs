using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPurposeBulletScript : MonoBehaviour {

    public enum StartSide {
        Up,
        Down,
    }
    public StartSide start = StartSide.Down;

    GameObject effectObj = null;
    
    readonly float effectSpawnTime = 0.05f;
    readonly float speed = 12.5f;
    float startYVel = 0f;
    float effectSpawnCounter = 0f;
    float randomYChange;

    bool passedThreshold = false;

    private void Start() {
        passedThreshold = false;
        effectObj = Resources.Load("Purpose Bullet Effect") as GameObject;
        switch (start) {
            case StartSide.Down:
                startYVel = speed;
                randomYChange = Random.Range(-15f, 0f);
                break;
            case StartSide.Up:
                startYVel = -speed;
                randomYChange = Random.Range(15f, 0f);
                break;
        }
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

        //Determining when the bullet velocity changes.
        switch (start) {
            case StartSide.Down:
                if(pos.y >= randomYChange) { 
                    passedThreshold = true;
                }
                break;
            case StartSide.Up:
                if(pos.y <= randomYChange) {
                    passedThreshold = true;
                }
                break;
        }

        if (passedThreshold) {
            vel = new Vector2(-speed, 0);
        } else {
            vel.y = startYVel;
            vel.x = 0;
        }

        GetComponent<Rigidbody2D>().velocity = vel;
    }
}
