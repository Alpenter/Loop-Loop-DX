using UnityEngine;

public class BulletScript : MonoBehaviour {
	
	int timesLooped = 0;
	float maxSpeed = 60f;
	bool triggered = false;
	GameObject puppet = null;
	GameObject suck, suckInst, poof;
	public bool newBullet = false;
	
	void Awake(){ puppet = transform.Find("puppet").gameObject; }
	
	void Start(){ 
		suck = Resources.Load("Suck Effect") as GameObject;
		if(!newBullet){
			maxSpeed = 60f;
			poof = Resources.Load("Bullet Poof") as GameObject;
		} else {
			maxSpeed = 100f;
			poof = Resources.Load("New Bullet Poof") as GameObject;
		}
	}

    void FixedUpdate() {
        Vector3 pos = transform.position;

        //Other side of screen.
        if (Game.atWardenLevel) {
            if(pos.x < -37){ Destroy(gameObject); }
	        if(pos.x > 37){  Destroy(gameObject); }
	        if(pos.y > 22f){ Destroy(gameObject); }
	        if(pos.y < -22f){Destroy(gameObject); }
        } else {
            if(pos.x < -37){ pos.x = 36.5f;  timesLooped++; }
	        if(pos.x > 37){ pos.x = -36.5f; timesLooped++; }
	        if(pos.y > 22f){ pos.y = -21.5f; timesLooped++; }
	        if(pos.y < -22f){ pos.y = 21.5f; timesLooped++; }	
        }
		
		Vector3 velocity = new Vector3(0, maxSpeed * Time.deltaTime, 0);			
		pos += transform.rotation * velocity;
		
		transform.position = pos;
		
		if(timesLooped > 2){ 
			Destroy(gameObject); 
		}
    }

	
	
	void OnTriggerEnter2D(Collider2D col){
        //Player shooting themself.
		if(col.gameObject.tag == "Player" && !triggered && timesLooped > 0 && Game.canShootSelf && !Game.inDisruptor){
			suckInst = Instantiate(suck, new Vector3(transform.position.x, transform.position.y, -22), Quaternion.identity) as GameObject;
			Destroy(suckInst, 0.6f);
			Game.manager.GetComponent<GameManager>().ChangeLayer();
			triggered = true;
		}

        //Hitting something with the clink sound effect.
        if(col.gameObject.GetComponent<ClinkWhenHitByBulletScript>() != null){
            col.gameObject.GetComponent<ClinkWhenHitByBulletScript>().Clink();
        }

        //Bullet hitting a wall.
        PoofMe();
	}

    //Bullet's poof effect.
    public void PoofMe(){
		GameObject poofInst = Instantiate(poof, new Vector3(transform.position.x, transform.position.y, transform.position.z + 4), Quaternion.identity) as GameObject;
		poofInst.transform.parent = gameObject.transform.parent.transform;
		ParticleSystem ps = poofInst.GetComponent<ParticleSystem>();
		var main = ps.main;
		main.startColor = puppet.GetComponent<MeshRenderer>().material.color;
		Destroy(poofInst, 1f);
		Destroy(gameObject);
		triggered = true;
	}
}
