using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class BulletHellManagerScript : MonoBehaviour {
	/*
	GameObject playerPrefab, background, timeObj, bard, bulletHolder, pauseMenu, mainCamera;
	
	[HideInInspector] public GameObject swipey;
	public GameObject[] bulletBatches;
	public GameObject singleBullet;
	

	
	
	

	// Update is called once per frame
	void Update () {
		
		
		
		
		//Set position of background.
		Vector3 backPos = new Vector3(Game.playerObj.transform.position.x/15f, 0, Game.playerObj.transform.position.y/15f);
		background.transform.position = Vector3.Lerp(background.transform.position, backPos, 1f*Time.deltaTime);
		
		//Increase time alive.
		if(!died){ timeAlive += Time.deltaTime; }
		TimeSpan timeSpan = TimeSpan.FromSeconds(timeAlive);
		string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		timeObj.GetComponent<TextMesh>().text = timeText;
		Game.maxTimeAlive = timeAlive;
		if(Game.maxTimeAlive > Game.highMaxTimeAlive){
			Game.highMaxTimeAlive = Game.maxTimeAlive;
		}
		
		//Set music volume.
		bard.GetComponent<AudioSource>().volume = Game.musicVolume;
		
		//Getting volume levels of bard.
		bard.GetComponent<AudioSource>().GetOutputData(samples, 0);
		float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160)
        {
            dbValue = -160;
        }
		
		//Set bloom to the volume of the music.
		mainCam.gameObject.GetComponent<BloomOptimized>().intensity = (rmsValue*intensity)/Game.lightValue;
		
		//Set light effects activity.
		backCam.gameObject.GetComponent<SunShafts>().enabled = Game.lightEffects;
		mainCam.gameObject.GetComponent<BloomOptimized>().enabled = Game.lightEffects;
		
		//Spawning bulletbatches.
		if(bulletCounter > 0){
			bulletCounter -= Time.deltaTime;
		} else {
			GameObject o = Instantiate(bulletBatches[UnityEngine.Random.Range(0, bulletBatches.Length)], Vector3.zero, Quaternion.identity) as GameObject;
			o.transform.parent = Game.bulletHolder.transform;
			bulletCounter = bulletSpawnTime;
		}
		
		//Spawning single bullets.
		if(singleBulletCounter > 0){
			singleBulletCounter -= Time.deltaTime;
		} else {
			GameObject o = Instantiate(singleBullet, new Vector3(UnityEngine.Random.Range(-36f, 36f), UnityEngine.Random.Range(-21f, 21f), 0), Quaternion.identity) as GameObject;
			o.transform.parent = Game.bulletHolder.transform;
			NewBulletHellBulletScript b = o.GetComponent<NewBulletHellBulletScript>();
			Vector2 v = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
			b.velocity = v.normalized;
			int loop = UnityEngine.Random.Range(0, 2);
			if(loop == 0){ b.canLoop = false; } else { b.canLoop = true; }
			singleBulletCounter = singleBulletSpawnTime;
		}
		
		//Bullet spawn time gets faster and faster.
		if(timeAlive < 30f){
			singleBulletSpawnTime = 3f;
			bulletSpawnTime = 5.5f;
		} else if(timeAlive > 30f && timeAlive < 60f){
			singleBulletSpawnTime = 2f;
			bulletSpawnTime = 5f;
		} else if(timeAlive > 60f && timeAlive < 90f){
			singleBulletSpawnTime = 1f;
			bulletSpawnTime = 4.5f;
		} else if(timeAlive > 90f && timeAlive <120f){
			singleBulletSpawnTime = 0.5f;
			bulletSpawnTime = 4f;
		} else if(timeAlive > 120f && timeAlive < 150f){
			singleBulletSpawnTime = 0.25f;
			bulletSpawnTime = 5f;
		} else if(timeAlive > 150f){
			singleBulletSpawnTime = 0.2f;
			bulletSpawnTime = 4f;
		}
		
		//Only show cursor when paused.
		Cursor.visible = paused;
	
		//Load le scene.
		if(startedSwipe){
			sceneLoadTime -= Time.deltaTime;
			bard.GetComponent<AudioSource>().pitch -= Time.deltaTime/2;
			if(bard.GetComponent<AudioSource>().pitch < 0){
				bard.GetComponent<AudioSource>().pitch = 0;
			}
			if(sceneLoadTime <= 0){
				SceneManager.LoadScene(sceneToLoad);
			}
		}
		
		//Setting position of loop indicators.
		yDown.transform.position = new Vector3(Game.playerObj.transform.position.x, -19f, 0f);
		yUp.transform.position = new Vector3(Game.playerObj.transform.position.x, 19f, 0f);
		xLeft.transform.position = new Vector3(-34.7f, Game.playerObj.transform.position.y, 0f);
		xRight.transform.position = new Vector3(34.7f, Game.playerObj.transform.position.y, 0f);
		
		//Setting Color of loop indicators.
		float xd = Vector3.Distance(new Vector3(Game.playerObj.transform.position.x, 0, 0), Vector3.zero);
		float yd = Vector3.Distance(new Vector3(0, Game.playerObj.transform.position.y, 0), Vector3.zero);
		//Debug.Log("XD: " + xd.ToString() + " YD: " + yd.ToString());
		if(xd > 20){ xTargetColor = Color.white; } else { xTargetColor = new Color(1f, 1f, 1f, 0f); }
		if(yd > 10){ yTargetColor = Color.white; } else { yTargetColor = new Color(1f, 1f, 1f, 0f); }
		xCol = Color.Lerp(xCol, xTargetColor, 5f * Time.deltaTime);
		yCol = Color.Lerp(yCol, yTargetColor, 5f * Time.deltaTime);
		
		//Apply color of loop indicators.
		xLeft.GetComponent<MeshRenderer>().material.color = xCol;
		xRight.GetComponent<MeshRenderer>().material.color = xCol;
		yUp.GetComponent<MeshRenderer>().material.color = yCol;
		yDown.GetComponent<MeshRenderer>().material.color = yCol;
		
		//Pause button!
		if(Input.GetButtonDown("Pause") && !died){
			paused = !paused;
		}
		if(died){ paused = false; }
		
		//When paused...
		if(paused && !goingBackToMenu){
			Time.timeScale = 0f;
			targetCamPos = pauseCamPos;
			targetCoverPosition = pauseCoverPosition;
		} else { //When not paused...
			if(!goingBackToMenu){ Time.timeScale = 1f; } else { Time.timeScale = 0f; }
			targetCamPos = unPauseCamPos;
			targetCoverPosition = unPauseCoverPosition;
		}
		
		//Move camera and color the cover based on paused or not.
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 20f * Time.unscaledDeltaTime);
		pauseMenu.transform.position = Vector3.Lerp(pauseMenu.transform.position, targetCoverPosition, 20f * Time.unscaledDeltaTime);
		
	} */
}