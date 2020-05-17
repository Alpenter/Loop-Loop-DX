using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBombScript : MonoBehaviour {

    int health = 5;

    float hitCounter = 0f;
    private readonly float hitTime = 0.1f; 

    bool activated = false;
	bool startedHurting = false;

    GameObject puppet = null;
	GameObject ignite = null;
    GameObject crystal = null;
    GameObject crystalCrack = null;
    GameObject crystalBreak = null;

    [HideInInspector]
	public WardenScript warden = null;

    Color hurtColor = Color.white;
    Color notHurtColor = new Color(1f, 1f, 1f, 0f);
    Color targetColor = Color.clear;

    AudioClip breakSnd = null;
    AudioClip litSnd = null;
    AudioClip boomSnd = null;

    private void Awake() {
        puppet = transform.Find("puppet").gameObject;
		puppet.GetComponent<AnimationScript>().enabled = false;
		ignite = transform.Find("ignite").gameObject;
        crystalCrack = transform.Find("crack puppet").gameObject;
        crystal = transform.Find("crystal rotate controller").gameObject;
        crystalBreak = Resources.Load("Crystal Bomb Explosion Particles") as GameObject;

        breakSnd = Resources.Load("SFX/Glass Break") as AudioClip;
        litSnd = Resources.Load("SFX/Void Bomb Ignite Sound") as AudioClip;
        boomSnd = Resources.Load("SFX/Void Bomb Explosion") as AudioClip;
    }
    
    void Update() {
        if(gameObject.transform.position == Vector3.zero && activated && !startedHurting){
			warden.BlueHurtMe();
			PlaySound.NoLoop(boomSnd);
			startedHurting = true;
		}

        if(hitCounter > 0) {
            hitCounter -= Time.deltaTime;
            targetColor = hurtColor;
        } else {
            targetColor = notHurtColor;
        }
        crystalCrack.GetComponent<MeshRenderer>().material.color = Color.Lerp(crystalCrack.GetComponent<MeshRenderer>().material.color, targetColor, 5f*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D bullet) {
        if(bullet.gameObject.tag == "Bullet") {
            if(health > 1) {
                health--;
                PlaySound.Damage();
                hitCounter = hitTime;
            } else {
                PlaySound.NoLoop(breakSnd);
                Instantiate(crystalBreak, transform.position, Quaternion.identity);
                crystalCrack.SetActive(false);
                crystal.SetActive(false);
                GetComponent<PolygonCollider2D>().enabled = false;
                ignite.GetComponent<ParticleSystem>().Play();
		        AudioSource litSound = gameObject.AddComponent<AudioSource>();
                litSound.clip = litSnd;
                litSound.volume = Game.soundVolume;
                litSound.loop = false;
                litSound.spatialBlend = 0f;
                litSound.Play();
		        puppet.GetComponent<AnimationScript>().enabled = true;
		        activated = true;
            }
        }
    }
}
