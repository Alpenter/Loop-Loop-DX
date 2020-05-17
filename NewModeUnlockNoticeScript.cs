using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewModeUnlockNoticeScript : MonoBehaviour {

	void Awake () {
		if(Game.lowestLayer > 0){ Destroy(gameObject); }
	}
}
