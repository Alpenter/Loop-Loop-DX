using UnityEngine;
using System.Collections;

public class MatchOtherObjectScript : MonoBehaviour {
		
	public GameObject matchThis;

	public bool usePhysics = false;

	public bool matchPos = false;
	public bool matchScale = false;

	public bool matchPlayerPos = false;
	
	public float lerpAmount = 1000f;


	void Update () {
		if (!usePhysics) {
			if (matchPos) {
				gameObject.transform.position = Vector3.Lerp(transform.position, matchThis.transform.position, Time.deltaTime*lerpAmount);
			}
			if (matchScale) {
				gameObject.transform.localScale = matchThis.transform.localScale;
			}
			if(matchPlayerPos && Game.playerObj != null) {
				gameObject.transform.position = Game.playerObj.transform.position;
			}
		}
	}

	void FixedUpdate() {
		if (usePhysics) {
			if (matchPos) {
				gameObject.transform.position = Vector3.Lerp(transform.position, matchThis.transform.position, Time.deltaTime*lerpAmount);
			}
			if (matchScale) {
				gameObject.transform.localScale = matchThis.transform.localScale;
			}
			if(matchPlayerPos) {
				gameObject.transform.position = Game.playerObj.transform.position;
			}
		}
	}
}
