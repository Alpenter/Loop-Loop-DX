using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBombScript : MonoBehaviour {

	GameObject puppet = null;
	GameObject ignite = null;
	GameObject firstObj = null;
    GameObject coinObj = null;

    [HideInInspector]
	public VoidBossScript v;
	
	bool activated = false;
	bool startedHurting = false;
	[HideInInspector] public bool first = false;

    AudioClip lit = null;
    AudioClip boom = null;


    LayerMask bulletLayer = 256;

	void Awake(){
		puppet = transform.Find("puppet").gameObject;
		puppet.GetComponent<AnimationScript>().enabled = false;
		ignite = transform.Find("ignite").gameObject;
		firstObj = transform.Find("first").gameObject;
        coinObj = Resources.Load("Coin") as GameObject;

        lit = Resources.Load("SFX/Void Bomb Ignite Sound") as AudioClip;
        boom = Resources.Load("SFX/Void Bomb Explosion") as AudioClip;

        //Debug.Log("bl;"+bulletLayer.value.ToString());
	}

    private void Start() {
        firstObj.SetActive(first);
        if (first) {
            firstObj.transform.parent = null;
        }
    }

    void Update(){
		if(gameObject.transform.position == Vector3.zero && activated && !startedHurting){
			v.HurtMe();
			PlaySound.NoLoop(boom);
			startedHurting = true;
		}

        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.5f);

        if(first && firstObj != null) {
            firstObj.transform.rotation = Quaternion.identity;
            firstObj.transform.position = new Vector3(pos.x, pos.y + 0.5f, -1f);
            firstObj.transform.localScale = gameObject.transform.localScale;
        }

        bool hit = Physics2D.OverlapCircle(pos, 2f, bulletLayer);
        if(hit && !activated) {
            int coinsToSpawn = 10;
            for(int i = 0; i < coinsToSpawn; i++){
                Quaternion coinRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                Instantiate(coinObj, transform.position, coinRotation);
            }
            Destroy(firstObj);
            ignite.GetComponent<ParticleSystem>().Play();
		    AudioSource litSound = gameObject.AddComponent<AudioSource>();
            litSound.clip = lit;
            litSound.volume = Game.soundVolume;
            litSound.loop = false;
            litSound.spatialBlend = 0f;
            litSound.Play();
		    puppet.GetComponent<AnimationScript>().enabled = true;
		    activated = true;
        }
	}
}
