using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseLaserShooterScript : MonoBehaviour {

    int hp = 10;

    GameObject breakEffect = null;

    List<AudioClip> hitSounds = new List<AudioClip>();
    List<AudioClip> killSounds = new List<AudioClip>();

    Color targetColor;
    readonly float hitTime = 0.1f;
    float hitCounter = 0f;

    void Start() {
        //Load break effect.
        breakEffect = Resources.Load("Break Effects/Cheese Laser Break Effect") as GameObject;

        //Load hurt sounds.
        for (int i = 0; i < 10; i++) {
            AudioClip a = Resources.Load("SFX/Monster Hit " + i.ToString()) as AudioClip;
            hitSounds.Add(a);
        }

        //Load kill sound.
        for (int i = 0; i < 3; i++)
        {
            AudioClip a = Resources.Load("SFX/Enemy Kill " + i.ToString()) as AudioClip;
            killSounds.Add(a);
        }
    }

    void Update() {
        //Updating hurtcolor of the puppet.
        if (hitCounter > 0) { hitCounter -= Time.deltaTime; targetColor = Game.bulletColor; }
        else { targetColor = Game.frontColor; }
        float smoothness = 5f;
        GetComponent<MeshRenderer>().material.color = Color.Lerp(GetComponent<MeshRenderer>().material.color, targetColor, smoothness * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D bullet) {
        if (bullet.gameObject.tag == "Bullet") {
            if (hp > 1) {
                AudioClip a = hitSounds[Random.Range(0, hitSounds.Count)];
                PlaySound.NoLoop(a);
                bullet.gameObject.GetComponent<BulletScript>().PoofMe();
                hp--;
                hitCounter = hitTime;
            } else { //Killing the laser.
                //Creating the break explosion.
                GameObject breakInst = (GameObject)Instantiate(breakEffect, transform.position, Quaternion.identity);
                breakInst.transform.parent = Game.bulletHolder.transform;
                Destroy(breakInst, 5f);

                //Playing the break sound.
                AudioClip a = killSounds[Random.Range(0, killSounds.Count)];
                PlaySound.NoLoop(a);

                Destroy(gameObject);
            }
        }

        if(bullet.gameObject.tag == "Player Laser") {
            //Creating the break explosion.
            GameObject breakInst = (GameObject)Instantiate(breakEffect, transform.position, Quaternion.identity);
            breakInst.transform.parent = Game.bulletHolder.transform;
            Destroy(breakInst, 5f);

            //Playing the break sound.
            AudioClip a = killSounds[Random.Range(0, killSounds.Count)];
            PlaySound.NoLoop(a);

            Destroy(gameObject);
        }
    }
}