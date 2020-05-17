using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSFXVolumeScript : MonoBehaviour {
    
    void Awake() { GetComponent<AudioSource>().volume = Game.soundVolume; }
    void Start() { GetComponent<AudioSource>().volume = Game.soundVolume; }
	void Update() { GetComponent<AudioSource>().volume = Game.soundVolume; }
}
