using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionNumberScript : MonoBehaviour {
	void Update(){ GetComponent<TextMesh>().text = Game.versionNumber; }
}