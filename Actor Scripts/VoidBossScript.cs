using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class VoidBossScript : MonoBehaviour {

	public GameObject[] chunks;
	public GameObject bomb;
	public GameObject bombExplo;
	public GameObject[] spikeChunks;
	public GameObject peeBit;
    public GameObject textBox;

    GameObject suckParticles = null;
	GameObject spikeGroup = null;	
	GameObject chunkInst = null;
	GameObject face0 = null;
	GameObject face1 = null;
	GameObject hurtFace = null;
	GameObject bombInst = null;
	GameObject hurtZone = null;
    GameObject deadFace = null;
    GameObject textBatch = null;
    List<GameObject> leftSpikes = new List<GameObject>();
    List<GameObject> rightSpikes = new List<GameObject>();
    List<GameObject> upSpikes = new List<GameObject>();
    List<GameObject> downSpikes = new List<GameObject>();

	int chosenSide = 0;
	int chosenChunk = 0;
	int chosenRot = 0;
	int chunksSpawned = 0;
	int health = 5;
	
    float targetSuckForce = 1f;
	float suckForce = 0f;
	readonly float chunkSpawnTime = 2f;
	float chunkSpawnCounter = 0f;
	readonly float chunklifeTime = 7f;
	readonly float showHurtTime = 0.75f;
    readonly float peeTime = 0.2f;
    float peeCounter = 0f;
	float showHurtCounter = 0f;
	float victoryTime = 10f;

	bool hurt = false;
	bool dying = false;
	bool tweenedSpikes = false;
    bool spawnedFirstBomb = false;

    Vector2 pp = Vector2.zero; //Player position for face manager.
	Vector2 chunkSpawnPlace = Vector2.zero;
	Vector3 chunkRotVec = Vector3.zero;
	Vector3 pos = Vector3.zero;
    Vector3 facePos = Vector3.zero;

	Quaternion chunkRot = Quaternion.identity;

	private void Awake(){
		face0 = gameObject.transform.Find("face").gameObject;
		face1 = gameObject.transform.Find("face open").gameObject;
		hurtFace = gameObject.transform.Find("hurt face").gameObject;
		spikeGroup = gameObject.transform.Find("Spike Group").gameObject;
		hurtZone = gameObject.transform.Find("hurtzone").gameObject;
		suckParticles = gameObject.transform.Find("suck particles").gameObject;
        textBatch = gameObject.transform.Find("Character Text Batch").gameObject;

        spikeGroup.transform.parent = null;

		spikeGroup.SetActive(false);
		face0.SetActive(true);
		face1.SetActive(false);
		hurtFace.SetActive(false);
        textBatch.SetActive(false);

        targetSuckForce = 1f;
	}

    private void Start() {
        //Find all spike holders.
        GameObject leftSpikeHolder = spikeGroup.transform.Find("Spike Group Left").gameObject;
        GameObject rightSpikeHolder = spikeGroup.transform.Find("Spike Group Right").gameObject;
        GameObject upSpikeHolder = spikeGroup.transform.Find("Spike Group Top").gameObject;
        GameObject downSpikeHolder = spikeGroup.transform.Find("Spike Group Bottom").gameObject;

        //Add all the spikes to their appropriate lists.
        foreach(Transform child in leftSpikeHolder.transform) { leftSpikes.Add(child.gameObject); }
        foreach(Transform child in rightSpikeHolder.transform) { rightSpikes.Add(child.gameObject); }
        foreach(Transform child in upSpikeHolder.transform) { upSpikes.Add(child.gameObject); }
        foreach(Transform child in downSpikeHolder.transform) { downSpikes.Add(child.gameObject); }
    }

    // Update is called once per frame
    void Update () {
        //Hide boss offscreen, when the splash counter is below 2, put him back on screen. This is so he "surprise appears on screen".
        pos = transform.position;
        if(Game.manager.GetComponent<GameManager>().splashCounter > 2) {
            pos = new Vector3(100, 100, 100);
        } else {
            pos = new Vector3(0, 0, 20);
        }
        transform.position = pos;

        
        //Manage faces.
		FaceManager();
		
        //Doing the usual boss stuff when the player is present and the game isn't showing the splashcreen anymore.
		if(Game.playerObj != null && !Game.manager.GetComponent<GameManager>().showingBossSplashscreen){
 		    
            //Manage the boss telling you that your bullets suck.
            ManageTextBox();

            //Managing health bar.
            ManageHealthBar();

		    //Manage spawn of chunks.
		    SpawnChunks();
		
		    //Manage hurt counter.
		    ManageHurtCounter();
		
		    //Kill me when dying!
		    if(dying){ KillMe(); } else { hurtZone.SetActive(true); }
		    
		    //When to reveal the spikes...
		    spikeGroup.SetActive(health < 5);
            if (!tweenedSpikes) { TweenAllSpikes(); }

		    //When to suck harder.
		    if(health < 3 && !dying){ targetSuckForce = 1.5f; }

            //When to manage pee.
            if(health == 1) { PeeHandler(); }
	    } else { 
            hurtZone.SetActive(false); 
        }
    }

    private void FixedUpdate() {
        if(Game.playerObj != null && !Game.manager.GetComponent<GameManager>().showingBossSplashscreen){
            //Manage suck...
            ManageSuck();
        }
    }

    //The boss tells you your bullets suck if you try to shoot.
    private void ManageTextBox() {
        //Only do the enabling logic if the text box is present.
        if(textBox != null) { 
            if(Game.usingController && Game.playerObj.GetComponent<NewShipScript>().looking) {
                textBox.SetActive(true);
            } else if(Input.GetButton("Shoot") && !Game.usingController){
                textBox.SetActive(true);
            }
        }
    }

    private void ManageSuck(){
        if (!Game.manager.GetComponent<GameManager>().paused) { 
            //Slowly raising the suckforce to where it's supposed to be. (So that the player doesn't need to instantly react when loading into the boss.)
            float slowdown = 5f;
            if(suckForce < targetSuckForce) { suckForce += Time.fixedDeltaTime/slowdown; }
            if(suckForce > targetSuckForce) { suckForce = targetSuckForce; }

		    Vector2 playerPos = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
		    Vector2 centerDistance = new Vector2(transform.position.x, transform.position.y);
		    //Debug.Log(d.ToString());
		    Vector2 forceToApply = playerPos - centerDistance;
		    forceToApply.Normalize();
		    forceToApply = new Vector2(-forceToApply.x*suckForce, -forceToApply.y*suckForce);
		    //Debug.Log(forceToApply.ToString());
		    Game.playerObj.GetComponent<Rigidbody2D>().velocity += forceToApply;
        }
	}
	
	private void SpawnChunks(){
		chunkSpawnCounter -= Time.deltaTime;
		if(chunkSpawnCounter <= 0 && !dying){
			
			//Spawn a bomb every 6 chunks.
			if(chunksSpawned >= 6){
				bombInst = Instantiate(bomb, chunkSpawnPlace, Quaternion.identity) as GameObject;
                bombInst.GetComponent<VoidBombScript>().v = this;
                if (!spawnedFirstBomb) {
                    bombInst.GetComponent<VoidBombScript>().first = true;
                    spawnedFirstBomb = true;
                } else {
                    bombInst.GetComponent<VoidBombScript>().first = false;
                }
				//Move Bomb.
				iTween.MoveTo(bombInst, iTween.Hash(
					"x", 0,
					"y", 0,
					"z", 0,
					"time", 10f,
					"easeType", iTween.EaseType.linear
				));
				//Scale Bomb.
				iTween.ScaleTo(bombInst, iTween.Hash(
					"x", 0,
					"y", 0,
					"z", 0,
					"time", 0.5f,
					"easeType", iTween.EaseType.linear,
					"delay", 9.5f
				));
				Destroy(bombInst, 10.5f);
				chunksSpawned = 0;
			}
			
			//0 == left. 1 == right. 2 == up. 3 == down.
			chosenSide = Random.Range(0, 4);
			chosenChunk = Random.Range(0, chunks.Length);
			chosenRot = Random.Range(0, 4);
			
			//Setting spawn place for chunks.
			if(chosenSide == 0){ chunkSpawnPlace = new Vector2(-60, Random.Range(-40f, 40f)); }
			if(chosenSide == 1){ chunkSpawnPlace = new Vector2(60, Random.Range(-40f, 40f)); }
			if(chosenSide == 2){ chunkSpawnPlace = new Vector2(Random.Range(-60, 60), 40); }
			if(chosenSide == 3){ chunkSpawnPlace = new Vector2(Random.Range(-60, 60), -40); }
			
			//Setting the chunk rotation.
			if(chosenRot == 0){chunkRotVec = Vector3.zero;}
			if(chosenRot == 1){chunkRotVec = new Vector3(0, 0, 180);}
			if(chosenRot == 2){chunkRotVec = new Vector3(0, 0, 90);}
			if(chosenRot == 3){chunkRotVec = new Vector3(0, 0, -90);}
			chunkRot.eulerAngles = chunkRotVec;
			
			//Spawn chunk.
			if(health > 3){
				chunkInst = Instantiate(chunks[chosenChunk], chunkSpawnPlace, chunkRot) as GameObject;
			} else {
				chunkInst = Instantiate(spikeChunks[chosenChunk], chunkSpawnPlace, chunkRot) as GameObject;
			}
			//Move Chunk.
			iTween.MoveTo(chunkInst, iTween.Hash(
				"x", 0,
				"y", -0.2f,
				"z", 0,
				"time", chunklifeTime,
				"easeType", iTween.EaseType.linear
			));
			
			//Scale Chunk.
			iTween.ScaleTo(chunkInst, iTween.Hash(
				"x", 0,
				"y", 0,
				"z", 0,
				"time", 0.5f,
				"easeType", iTween.EaseType.linear,
				"delay", chunklifeTime - 0.5f
			));
			
			
			//Destroy chunk.
			Destroy(chunkInst, chunklifeTime);
			
			//Add to chunks spawned.
			chunksSpawned++;
			
			//Reset clock.
			chunkSpawnCounter = chunkSpawnTime;
		}
	}
	
	private void FaceManager(){
        
        float widthTrigger = 12;
        float heightTrigger = 7;

        if(Game.playerObj != null) { 
            pp = new Vector2(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y);
        } else {
            pp = Vector2.zero;
        }
        
		if(!hurt && !dying){
			if(pp.x < widthTrigger && pp.x > -widthTrigger && pp.y < heightTrigger && pp.y > -heightTrigger){
				face0.SetActive(false);
				face1.SetActive(true);
				hurtFace.SetActive(false);
                float shakeIntensity = 0.15f;
                facePos.x = Random.Range(-shakeIntensity, shakeIntensity);
                facePos.y = Random.Range(-shakeIntensity, shakeIntensity);
				face1.transform.position = new Vector3(facePos.x, facePos.y+2, 1);
			} else {	
                face0.SetActive(true);
                face1.SetActive(false);
				hurtFace.SetActive(false);
			}
		} else {
			face0.SetActive(false);
			face1.SetActive(false);
			hurtFace.SetActive(true);
		}
	}
	
	//I've been struck!
	public void HurtMe(){
		Instantiate(bombExplo, transform.position, Quaternion.identity);
		showHurtCounter = showHurtTime;
		if(health > 1){
			health--;
		} else {
            suckParticles.GetComponent<ParticleSystem>().Stop(); //Stop the black particles from emitting.
            health = 0; //Drop health to 0 because he's dead.
			KillMe(); //Do the rest of the kill stuff.
		}
	}
	
	//Managing the hurt counter.
	private void ManageHurtCounter(){
		if(showHurtCounter >= 0){
			showHurtCounter -= Time.deltaTime;
			hurt = true;
		} else {
			hurt = false;
		}
	}
	
    private void TweenAllSpikes() {
        float delayMultiplier = 0.1f;
        for(int a = 0; a < leftSpikes.Count; a++) {
            float targetX = -33f;
            float delayPositionOffset = 18;
            iTween.MoveTo(leftSpikes[a], iTween.Hash(
                "x", targetX,
                "time", 1f,
                "delay", delayMultiplier*(leftSpikes[a].transform.localPosition.y + delayPositionOffset),
                "easetype", iTween.EaseType.easeInOutSine,
                "looptype", iTween.LoopType.none
            ));
        }

        for (int b = 0; b < rightSpikes.Count; b++) {
            float targetX = 33f;
            float delayPositionOffset = 18;
            iTween.MoveTo(rightSpikes[b], iTween.Hash(
                "x", targetX,
                "time", 1f,
                "delay", delayMultiplier*(rightSpikes[b].transform.localPosition.y + delayPositionOffset),
                "easetype", iTween.EaseType.easeInOutSine,
                "looptype", iTween.LoopType.none
            ));
        }

        for(int c = 0; c < upSpikes.Count; c++) {
            float targetY = 18f;
            float delayPositionOffset = 33f;
            iTween.MoveTo(upSpikes[c], iTween.Hash(
                "y", targetY,
                "time", 1f,
                "delay", delayMultiplier*(upSpikes[c].transform.localPosition.x + delayPositionOffset),
                "easetype", iTween.EaseType.easeInOutSine,
                "looptype", iTween.LoopType.none
            ));
        }

        for(int d = 0; d < downSpikes.Count; d++) {
            float targetY = -18f;
            float delayPositionOffset = 33f;
            iTween.MoveTo(downSpikes[d], iTween.Hash(
                "y", targetY,
                "time", 1f,
                "delay", delayMultiplier*(downSpikes[d].transform.localPosition.x + delayPositionOffset),
                "easetype", iTween.EaseType.easeInOutSine,
                "looptype", iTween.LoopType.none
            ));
        }

        tweenedSpikes = true;
    }
	
	void KillMe(){
        //Debug.Log("Dying!");
        if (!dying) { 
		    hurtZone.SetActive(false);
		    targetSuckForce = 0f;
            //Scale boss to 0.
		    iTween.ScaleTo(gameObject, iTween.Hash(
			    "x", 0,
			    "y", 0,
			    "z", 0,
			    "time", victoryTime,
			    "easeType", iTween.EaseType.linear
		    ));
            //Scale spikes off of screen.
            iTween.ScaleTo(spikeGroup, iTween.Hash(
                "x", 2,
			    "y", 2,
			    "z", 2,
			    "time", victoryTime,
			    "easeType", iTween.EaseType.linear
            ));
            dying = true;
        } else {
            victoryTime -= Time.deltaTime;
            if(victoryTime <= 0) {
                if(deadFace == null) {
                    GameObject faceLoaded = Resources.Load("Void Dead Face") as GameObject;
                    deadFace = Instantiate(faceLoaded, Vector3.zero, Quaternion.identity);
                }
                Game.manager.GetComponent<GameManager>().victory = true;
            }
        }
	}

    private void ManageHealthBar() {
        //Manage health bar.
        float divider = 0.2f;
        Game.manager.GetComponent<GameManager>().BossHealthBar(health, divider);
        if (health < 0) { health = 0; } //You can't go negative health!
    }

    private void PeeHandler() {
        if(textBatch != null) {
            textBatch.SetActive(true);
        }
        peeCounter -= Time.deltaTime;
        if(peeCounter <= 0) {
            if(Game.playerObj != null) { 
                Instantiate(peeBit, new Vector3(Game.playerObj.transform.position.x, Game.playerObj.transform.position.y, 1f), Quaternion.identity);
            }
            peeCounter = peeTime;
        }
    }
}