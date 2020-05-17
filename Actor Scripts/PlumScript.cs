using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlumScript : MonoBehaviour {

    public enum Phase { One, Two, Three, Final, }
    public Phase myPhase = Phase.One;

    GameObject nextPhaseObj = null; //The new "sub-enemy" we spawn when plum dies.
    GameObject puppet = null;
    GameObject explo = null;
    GameObject coin = null;

    List<AudioClip> hitSounds = new List<AudioClip>();
    AudioClip popSound = null;
    AudioClip introSnd = null;

    int health = 50;
    int nextFormSpeed = 2;
    readonly int moneys = 20;
    readonly int zLock = 1;

    float hitCounter = 0f;
	private readonly float hitTime = 0.1f;

    bool playedIntroSound = false;

    Vector3 pos = Vector2.zero;

    Color targetColor = Color.white;

    //We will use Start to set the specific stats for each form.
    private void Start() {
        switch (myPhase) {
            case Phase.One:
                health = 66;
                nextFormSpeed = 5;
                nextPhaseObj = Resources.Load("Plum Stuff/Plum Form Two") as GameObject;
                explo = Resources.Load("Plum Stuff/Form One Pop") as GameObject;
                popSound = Resources.Load("SFX/Plum Form One Pop Sound") as AudioClip;
                introSnd = Resources.Load("SFX/Incoming Plum Charge") as AudioClip;
                //Tween to the center.
                iTween.MoveTo(gameObject, iTween.Hash("x", 0f, "y", 0f, "z", 0f, "delay", 5f, "time", 20f, "easetype", iTween.EaseType.easeOutSine, "looptype", iTween.LoopType.none));
                break;
            case Phase.Two:
                health = 50;
                nextFormSpeed = 10;
                nextPhaseObj = Resources.Load("Plum Stuff/Plum Form Three") as GameObject;
                explo = Resources.Load("Plum Stuff/Form Two Pop") as GameObject;
                popSound = Resources.Load("SFX/Plum Form Two Pop Sound") as AudioClip;
                break;
            case Phase.Three:
                health = 35;
                nextFormSpeed = 15;
                nextPhaseObj = Resources.Load("Plum Stuff/Plum Form Final") as GameObject;
                explo = Resources.Load("Plum Stuff/Form Three Pop") as GameObject;
                popSound = Resources.Load("SFX/Plum Form Three Pop Sound") as AudioClip;
                break;
            case Phase.Final:
                health = 20;
                nextFormSpeed = 0;
                nextPhaseObj = null;
                explo = Resources.Load("Plum Stuff/Final Form Pop") as GameObject;
                popSound = Resources.Load("SFX/Plum Final Form Pop") as AudioClip;
                break;
        }

        //Set the parent to be the plum batch.
        gameObject.transform.parent = Game.plumBatch.transform;

        //find the puppet.
        puppet = transform.Find("puppet").gameObject;

        //Find the coin object.
        coin = Resources.Load("Coin") as GameObject;

        //Load hit sounds.
        for(int i = 0; i < 10; i++){
        	AudioClip a = Resources.Load("SFX/Monster Hit "+i.ToString()) as AudioClip;
        	hitSounds.Add(a);
        }
    }
    
    private void Update() {
        
        //Screen looping.
        if(myPhase != Phase.One) { //The only phase that doesn't screen loop is the first one.
            pos = transform.position;

            float baseLoopXLimit = 35.4f;
            float baseLoopYLimit = 20.4f;
            float sizeAddition = puppet.transform.localScale.x/2f;
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
        } else {
             float soundDist = 40f;
             Vector2 vTwoPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
             float d = Vector2.Distance(vTwoPos, Vector2.zero);
             if(d < soundDist && !playedIntroSound){
                PlaySound.NoLoop(introSnd);
                playedIntroSound = true;
             }
        }




        //Flash me when I'm hit.
		if(hitCounter > 0f){
			hitCounter -= Time.deltaTime;
			targetColor = Game.bulletColor;
		} else {
			targetColor = new Color(0.7f, 0f, 1f);
		}

		//Lerp that color!
		float smoothness = 5f;
		puppet.GetComponent<MeshRenderer>().material.color = Color.Lerp(puppet.GetComponent<MeshRenderer>().material.color, targetColor, smoothness*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D bullet) {
        if(bullet.tag == "Bullet") {
            if(health > 1){
				AudioClip a = hitSounds[Random.Range(0, hitSounds.Count)];
				PlaySound.NoLoop(a);
				bullet.gameObject.GetComponent<BulletScript>().PoofMe();
				health--;
				hitCounter = hitTime;
			} else { //What do to when out of health...
                Instantiate(explo, transform.position, Quaternion.identity); //Spawn the death pop explosion.
                PlaySound.NoLoop(popSound); //Play the death sound.
                if(myPhase != Phase.Final) { //If it isn't the final plum, split into to plums!
                    Vector2 bPoint = new Vector2(bullet.gameObject.transform.position.x, bullet.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
				    GameObject plum1 = Instantiate(nextPhaseObj, transform.position, Quaternion.identity) as GameObject;
				    GameObject plum2 = Instantiate(nextPhaseObj, transform.position, Quaternion.identity) as GameObject;
                    plum1.transform.parent = Game.plumBatch.transform;
                    plum1.transform.parent = Game.plumBatch.transform;
				    Vector2 sqForce = new Vector2((bPoint.y*5)*nextFormSpeed/3, (bPoint.x*5)*nextFormSpeed/3);
				    plum1.GetComponent<Rigidbody2D>().AddForce(sqForce.normalized*nextFormSpeed, ForceMode2D.Impulse);
				    plum2.GetComponent<Rigidbody2D>().AddForce(-sqForce.normalized*nextFormSpeed, ForceMode2D.Impulse);
                    Destroy(gameObject);
                } else { //If it is the final plum, don't make plums and just make the final plopplet.
                    for (int i = 0; i < moneys; i++) {
                        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                        Instantiate(coin, transform.position, rot);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
