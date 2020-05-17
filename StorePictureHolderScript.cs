using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePictureHolderScript : MonoBehaviour {
	
	public enum Power{
		Health,
		Orbitals,
		ShotSpeed,
		Speed,
		ShotSplit,
		FloppyDisk,
	}
	public Power pow;
	List<GameObject> pics = new List<GameObject>();
	int rank = 0;
	
	//On the first frame...
	void Start () {
		//Find the pics.
		foreach (Transform child in gameObject.transform) {
			pics.Add(child.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(pow == Power.FloppyDisk){
			pics[1].SetActive(Game.ownsFloppyDisk);
		} else {
			if(pow == Power.Health){
				rank = Game.healthRank;
			} else if(pow == Power.ShotSpeed){
				rank = Game.shotSpeedRank;
			} else if(pow == Power.Speed){
				rank = Game.speedRank;
			} else if(pow == Power.ShotSplit){
				rank = Game.shotSplitRank;
			}
			
			for(int i = 0; i < pics.Count; i++){
				if(pics[i].name == rank.ToString()) {
					pics[i].SetActive(true);
				} else {
					pics[i].SetActive(false);
				}
			}
		}
	}
}