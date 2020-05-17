using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleCrystalScript : MonoBehaviour {
    //Crystal size.
    public enum Size { Big, Medium, Small }
    public Size mySize = Size.Big;

    //GameObjects.
    GameObject nextPhaseObj = null; //The new "sub-enemy" we spawn when plum dies.
    GameObject crack = null;
    GameObject explo = null;

    //Ints.
    int health = 1;
    int nextFormSpeed = 2;
    readonly int zLock = 1;

    float hitCounter = 0f;
	private readonly float hitTime = 0.1f;
    
    Vector3 pos = Vector2.zero;

    Color targetColor = Color.clear;

    //Audioclips.
    AudioClip breakSnd = null;

    private void Awake() {
        //Find and load objects.
        crack = transform.Find("crack").gameObject;
        crack.GetComponent<MeshRenderer>().material.color = Color.clear;
        switch (mySize) {
            case Size.Big:
                health = 25;
                nextFormSpeed = 15;
                nextPhaseObj = Resources.Load("Warden Purple Stuff/Purple Crystal Medium") as GameObject;
                explo = Resources.Load("Warden Purple Stuff/Big Purple Crystal Explosion Particles") as GameObject;
                break;
            case Size.Medium:
                health = 15;
                nextFormSpeed = 19;
                nextPhaseObj = Resources.Load("Warden Purple Stuff/Purple Crystal Small") as GameObject;
                explo = Resources.Load("Warden Purple Stuff/Medium Purple Crystal Explosion Particles") as GameObject;
                break;
            case Size.Small:
                health = 8;
                nextFormSpeed = 0;
                nextPhaseObj = null;
                explo = Resources.Load("Warden Purple Stuff/Small Purple Crystal Explosion Particles") as GameObject;
                break;
        }
        breakSnd = Resources.Load("SFX/Glass Break") as AudioClip;
    }

    //On every frame...
    private void Update() {
        //Hit coloring.
        if(hitCounter <= 0) {
            targetColor = Color.clear;
        } else {
            hitCounter -= Time.deltaTime;
            targetColor = Color.black;
        }
        crack.GetComponent<MeshRenderer>().material.color = Color.Lerp(crack.GetComponent<MeshRenderer>().material.color, targetColor, 5f*Time.deltaTime);

        //Moving and looping around the screen.
        pos = transform.position;
        float baseLoopXLimit = 35.4f;
        float baseLoopYLimit = 20.4f;
        float sizeAddition = (crack.transform.localScale.x/2f) + 0.5f;
        float xLimit = baseLoopXLimit + sizeAddition;
        float yLimit = baseLoopYLimit + sizeAddition;

        //Looping around the screen.
		if(pos.x < -xLimit){ pos.x = xLimit - 0.5f; }
		if(pos.x > xLimit){ pos.x = -xLimit + 0.5f; }
		if(pos.y > yLimit){ pos.y = -yLimit + 0.5f; }
		if(pos.y < -yLimit){ pos.y = yLimit - 0.5f; }
			
		//Locking the z position.
		pos.z = zLock;
			
		//Updating the position.
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D bullet) {
        if(bullet.tag == "Bullet") {
            if(health > 1){
				PlaySound.Damage();
				bullet.gameObject.GetComponent<BulletScript>().PoofMe();
				health--;
				hitCounter = hitTime;
			} else { //What do to when out of health...
                Instantiate(explo, transform.position, Quaternion.identity); 
                PlaySound.NoLoop(breakSnd); //Play the death sound.
                if(mySize != Size.Small) { 
                    Vector2 dif = new Vector2(bullet.gameObject.transform.position.x, bullet.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
				    Vector2 bPoint = dif.normalized;
                    GameObject plum1 = Instantiate(nextPhaseObj, transform.position, Quaternion.identity) as GameObject;
				    GameObject plum2 = Instantiate(nextPhaseObj, transform.position, Quaternion.identity) as GameObject;
                    plum1.transform.parent = Game.wardenPurpleCrystalHolder.transform;
                    plum2.transform.parent = Game.wardenPurpleCrystalHolder.transform;
				    Vector2 sqForce = new Vector2(bPoint.y*nextFormSpeed, bPoint.x*nextFormSpeed);
				    plum1.GetComponent<Rigidbody2D>().velocity = sqForce;
				    plum2.GetComponent<Rigidbody2D>().velocity = -sqForce;
                    Destroy(gameObject);
                } else { //If it is the final plum, don't make plums and just make the final plopplet.
                    Destroy(gameObject);
                }
            }
        }
    }
}