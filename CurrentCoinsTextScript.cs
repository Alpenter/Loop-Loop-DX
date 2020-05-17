using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCoinsTextScript : MonoBehaviour {

	void Update () {
        string s = "and you got " + Game.currentCoinsCollectedInLevel.ToString() + " coins!";
        GetComponent<TextMesh>().text = s;
	}
}
