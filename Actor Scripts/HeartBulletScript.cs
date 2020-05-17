using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBulletScript : MonoBehaviour {

    int frameIndex = 0;
    GameObject puppet = null;
    bool closeEnough = false, setNewForce = false;
    float speed = 12f;
	float sizeToBe = 4f;
	
    // Start is called before the first frame update
    void Awake() {
        puppet = transform.Find("puppet").gameObject;
    }

    void Start() {
        iTween.ScaleTo(puppet, iTween.Hash(
            "x", sizeToBe, "y", sizeToBe, "z", 1,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype",iTween.LoopType.none,
            "time", 0.75f
        ));    
    }

    // Update is called once per frame
    void Update() {
        //Animting.
        Vector2[] framePool = { Vector2.zero, new Vector2(0.5f, 0f) };
        if (closeEnough) { frameIndex = 1; } else { frameIndex = 0; }
        puppet.GetComponent<MeshRenderer>().material.mainTextureOffset = framePool[frameIndex];

        //Tracking position.
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        if (Game.playerObj != null) {
            //Tracking when close enough to player.
            Vector2 playerPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
            float d = Vector2.Distance(playerPos, pos);
            if (d < 10) { closeEnough = true; }

            //AI manged here.
            Vector2 vel = GetComponent<Rigidbody2D>().velocity;

            //AI when not close enough.
            if (!closeEnough)
            {
                Vector2 dif = playerPos - pos;
                vel = dif.normalized;
                vel = vel * speed;
            }
            else
            { //AI when close enough.
                if (!setNewForce)
                {
                    speed = 33f;
                    Vector2 dif = playerPos - pos;
                    vel = dif.normalized;
                    vel = vel * speed;
                    setNewForce = true;
                }
            }
            GetComponent<Rigidbody2D>().velocity = vel;
        }

        //Destroying the object.
        float xLim = 37f;
        float yLim = 22f;
        if(pos.x > xLim || pos.x < -xLim || pos.y > yLim || pos.y < -yLim) { Destroy(gameObject); }
    }
}
