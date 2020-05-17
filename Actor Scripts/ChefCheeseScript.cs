using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefCheeseScript : MonoBehaviour {

    bool dead = false;
    bool playedLaserSnd = false;
    bool playedGunTween = false;
    bool playedBreakSound = false;
    bool madeCoins = false;
    bool playedSecondTween = false;

    GameObject fBlocker, laserHolder, secondGuns, gunsToScale, coin, deathParticles, boom, smile;
    GameObject shootIndicatorBatch = null;
    List<GameObject> lasers = new List<GameObject>();
    List<GameObject> shootIndicators = new List<GameObject>();
    public GameObject bullet = null;
    public GameObject[] points;
    public GameObject[] laserGroup;
    public GameObject glassBreakPoof = null;
    public GameObject[] puppets;

    int targetPosIndex = 0;
    int health = 75;
    readonly int moneys = 75;

    readonly float speed = 5f;
    readonly float phase1BulletTime = 4f, phase2BulletTime = 0.5f, hitTime = 0.1f;
    float phase1BulletCounter = 5f;
    float shieldBreakWaitTime = 2f;
    float phase2BulletCounter = 1f;
    float hitCounter;
    float deathCounter = 3f;

    Color targetColor;

    Vector2 vel;

    AudioClip laserSnd, shieldBreak, boomSnd, fireShoot;

    public enum Phase {
        ComingIn,
        PhaseOne,
        Transition,
        PhaseTwo,
        Death,
    }
    Phase p = Phase.ComingIn;

    // Use this for initialization
    void Start() {
        //Find needed stuff.
        fBlocker = transform.Find("Form 1 Blocker").gameObject;
        secondGuns = transform.Find("Second Form Guns").gameObject;
        gunsToScale = secondGuns.transform.Find("To Scale").gameObject;
        deathParticles = transform.Find("explo particles").gameObject;
        boom = transform.Find("death explo").gameObject;
        smile = transform.Find("smile").gameObject;
        
        //Populate indicator list.
        shootIndicatorBatch = secondGuns.transform.Find("Shoot Indicator Batch").gameObject;
        foreach(Transform child in shootIndicatorBatch.transform) {
            shootIndicators.Add(child.gameObject);
        }
        
        //Find lasers.
        laserHolder = transform.Find("Laser Shooters").gameObject;
        foreach (Transform child in laserHolder.transform) {
            lasers.Add(child.gameObject);
        }

        //Load coins.
        coin = Resources.Load("Coin") as GameObject;

        //Load sounds.
        laserSnd = Resources.Load("SFX/Laser") as AudioClip;
        shieldBreak = Resources.Load("SFX/Glass Break") as AudioClip;
        boomSnd = Resources.Load("SFX/Spike Explosion") as AudioClip;
        fireShoot = Resources.Load("SFX/Fire Shoot") as AudioClip;

        //Disable collider until form 2.
        GetComponent<Collider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update() {

        //Set when the smile face is active.
        smile.SetActive(Game.smileyMode);

        //Set position and parents of the object.
        gameObject.transform.parent = Game.enemyHolder.transform;

        //Manage health bar.
        float divider = 0.013333f;
        Game.manager.GetComponent<GameManager>().BossHealthBar(health, divider);
        if (health < 0) { health = 0; } //You can't go negative health!

        //Lerp that color when hurt!
        //Flash me when I'm hit.
        if (hitCounter > 0f){
            hitCounter -= Time.deltaTime;
            targetColor = Game.bulletColor;
        } else {
            targetColor = Game.frontColor;
        }
        float smoothness = 5f;
        for (int i = 0; i < puppets.Length; i++) { puppets[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(puppets[i].GetComponent<MeshRenderer>().material.color, targetColor, smoothness * Time.deltaTime); }


        //Hold boss until not in Splash Screen.
        if (Game.manager.GetComponent<GameManager>().splashCounter <= 0 && !dead){
            //Setup velocity.
            vel = GetComponent<Rigidbody2D>().velocity;

            //Coming in phase.
            if (p == Phase.ComingIn) {

                //Keep the bullet shot indicators hidden.
                for (int i = 0; i < shootIndicators.Count; i++) {
                    shootIndicators[i].transform.localScale = Vector3.zero;
                    shootIndicators[i].GetComponent<MeshRenderer>().material.color = Color.clear;
                }

                transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                if (transform.position == Vector3.zero) {
                    //Enable lasers and go to phase1.
                    p = Phase.PhaseOne;
                }
                //PHASE 1
            }
            else if (p == Phase.PhaseOne) {
                //Play laser sound.
                if (!playedLaserSnd) {
                    PlaySound.NoLoop(laserSnd);
                    playedLaserSnd = true;
                }

                //Keep lasers active, if not dead.
                for (int i = 0; i < 4; i++) {
                    if (laserGroup[i] != null) {
                        laserGroup[i].SetActive(true);
                    }
                }

                //Move around to 9 designated points.
                float xa = 24f;
                float ya = 8.5f;
                Vector3[] targetPos = {
                    Vector3.zero, new Vector3(xa, 0, 0), new Vector3(-xa, 0, 0), new Vector3(0, ya, 0), new Vector3(0, -ya, 0),
                    new Vector3(xa, ya, 0), new Vector3(-xa, ya, 0), new Vector3(xa, -ya, 0), new Vector3(-xa, -ya, 0),
                };
                if (transform.position == targetPos[targetPosIndex]) {
                    targetPosIndex = Random.Range(0, 8);
                }
                transform.position = Vector3.MoveTowards(transform.position, targetPos[targetPosIndex], speed * Time.deltaTime);

                //Shooting cheese bullets.
                phase1BulletCounter -= Time.deltaTime;//HERE
                if (phase1BulletCounter <= 0) {
                    for (int i = 0; i < 4; i++) {
                        Instantiate(bullet, new Vector3(points[i].transform.position.x, points[i].transform.position.y, 75), points[i].transform.rotation * Quaternion.Euler(0, 0, -90));
                    }
                    PlaySound.NoLoop(fireShoot);
                    phase1BulletCounter = phase1BulletTime;
                }
                
                //Scale the cheese bullet shoot indicators.
                float f = (phase1BulletCounter/5f);
                Color c = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 1-f);
                if(f > 1f) { f = 1f; }
                if(f < 0f) { f = 0f; }
                for (int i = 0; i < shootIndicators.Count; i++) {
                    if(i < 4) {
                        shootIndicators[i].transform.localScale = new Vector3(5f, f, 1f);
                        shootIndicators[i].GetComponent<MeshRenderer>().material.color = c;
                    } else {
                        shootIndicators[i].transform.localScale = Vector3.zero;
                        shootIndicators[i].GetComponent<MeshRenderer>().material.color = Color.clear;
                    }
                }

                //Checking if all lasers are dead.
                for (int i = 0; i < lasers.Count; i++) {
                    if (lasers[i] == null) { lasers.Remove(lasers[i]); }
                }
                //Debug.Log(lasers.Count.ToString());
                if (lasers.Count < 1) {
                    p = Phase.Transition;
                }
                //Transition to phase 2!!
            } else if (p == Phase.Transition) {

                //Keep the bullet shot indicators hidden.
                for (int i = 0; i < shootIndicators.Count; i++) {
                    shootIndicators[i].transform.localScale = Vector3.zero;
                    shootIndicators[i].GetComponent<MeshRenderer>().material.color = Color.clear;
                }

                //Move to the middle.
                transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);

                //When he gets to the center...
                if (transform.position == Vector3.zero)
                {
                    //Scale out the new guns!
                    if (!playedGunTween)
                    {
                        iTween.ScaleTo(gunsToScale, iTween.Hash("x", 1f, "y", 1f, "z", 1f, "time", 2f, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.none));
                        secondGuns.GetComponent<RotateScript>().enabled = true;
                        playedGunTween = true;
                    }

                    //Subtract counter.
                    shieldBreakWaitTime -= Time.deltaTime;

                    if (shieldBreakWaitTime <= 1f && !playedBreakSound)
                    {
                        Destroy(fBlocker);
                        Instantiate(glassBreakPoof, new Vector3(0, 0, 40), Quaternion.identity);
                        PlaySound.NoLoop(shieldBreak);
                        playedBreakSound = true;
                    }

                    if (shieldBreakWaitTime <= 0)
                    {
                        p = Phase.PhaseTwo;
                    }
                } //Phase 2!!!!
            }
            else if (p == Phase.PhaseTwo)
            {
                //Enable that collider.
                GetComponent<Collider2D>().enabled = true;

                phase2BulletCounter -= Time.deltaTime;
                if (phase2BulletCounter <= 0) {
                    for (int i = 0; i < 8; i++) {
                        GameObject o = (GameObject)Instantiate(bullet, new Vector3(points[i].transform.position.x, points[i].transform.position.y, 75), points[i].transform.rotation * Quaternion.Euler(0, 0, -90));
                        o.GetComponent<CheeseBulletScript>().turnTime = -1f;
                    }
                    PlaySound.NoLoopRandomPitch(fireShoot, 0.5f, 1.5f);
                    phase2BulletCounter = phase2BulletTime;
                }

                //Scaling the shot indicators for phase 2.
                float f = phase2BulletCounter;
                Color c = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 1-f);
                if(f > 1f) { f = 1f; }
                if(f < 0f) { f = 0f; }
                for (int i = 0; i < shootIndicators.Count; i++) {
                    shootIndicators[i].transform.localScale = new Vector3(5f, f, 1f);
                    shootIndicators[i].GetComponent<MeshRenderer>().material.color = c;
                }
            }

            //Reapply velocity.
            GetComponent<Rigidbody2D>().velocity = vel;
        } else if (dead) {
            GetComponent<Collider2D>().enabled = false;
            if (deathCounter > 0) { deathCounter -= Time.deltaTime; }

            if (deathCounter <= 0 && !playedSecondTween) {
                iTween.MoveTo(gameObject, iTween.Hash(
                    "x", 0f, "y", -33f, "z", 0f,
                    "time", 1f,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.none
                ));
                iTween.ScaleTo(boom, iTween.Hash(
                    "x", 50f, "y", 50f, "z", 1f,
                    "time", 1f,
                    "delay", 1f,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.none
                ));
                iTween.ColorTo(boom, iTween.Hash(
                    "a", 0f,
                    "time", 0.75f,
                    "delay", 1.25f,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.none
                ));
                Game.manager.GetComponent<GameManager>().victory = true;
                PlaySound.NoLoop(boomSnd);
                playedSecondTween = true;
            }   
        }
    }

    void OnTriggerEnter2D(Collider2D playerBullet){
        if (playerBullet.gameObject.tag == "Bullet" ){
            playerBullet.gameObject.GetComponent<BulletScript>().PoofMe();
            if (health >= 1){
                hitCounter = hitTime;
                PlaySound.Damage();
                health--;
            } else {
                if (!madeCoins){
                    for (int i = 0; i < moneys; i++){
                        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                        Instantiate(coin, transform.position, rot);
                    }
                    madeCoins = true;
                }
                deathParticles.GetComponent<ParticleSystem>().Play();
                p = Phase.Death;
                iTween.ScaleTo(secondGuns, iTween.Hash("x", 0f, "y", 0f, "z", 1f, "time", 2f, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.none));
                dead = true;
            }
        }

        if(playerBullet.gameObject.tag == "Player Laser" && p == Phase.PhaseTwo) {
            health = 0;
            if (!madeCoins){
                for (int i = 0; i < moneys; i++){
                    Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    Instantiate(coin, transform.position, rot);
                }
                madeCoins = true;
            }
            deathParticles.GetComponent<ParticleSystem>().Play();
            p = Phase.Death;
            iTween.ScaleTo(secondGuns, iTween.Hash("x", 0f, "y", 0f, "z", 1f, "time", 2f, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.none));
            dead = true;
        }
    }
}