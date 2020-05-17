using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

	public Vector2[] framesOffset;
	public float pauseBetweenFrames;
	float counter;
	int frameAmount;
	[HideInInspector]
	public int currentFrame;
	public bool doNotLoop = false;
	public bool ignoreDeltaTime = false;
	
	void Awake () 
	{
		currentFrame = 0;
		counter = pauseBetweenFrames;
		frameAmount = framesOffset.Length - 1;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!ignoreDeltaTime){
			counter -= Time.deltaTime;
		} else {
			counter -= 1;
		}
		
		if(counter <= 0)
		{
			if(currentFrame < frameAmount)
			{
				currentFrame ++;
			} else {
				if(!doNotLoop){
					currentFrame = 0;
				}
			}
			counter = pauseBetweenFrames;
		}

		GetComponent<MeshRenderer>().material.mainTextureOffset = framesOffset[currentFrame];
	}
}
