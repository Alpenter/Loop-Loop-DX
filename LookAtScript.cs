using UnityEngine;
using System.Collections;

public class LookAtScript : MonoBehaviour {

	public float turnSpeed;
	public GameObject lookHere;

	public bool lookAtPlayer;
    public bool destroyWhenTargetNotFound = false;
	public bool turnFasterOverTime = false;

    public float turnFasterOverTimeSpeed;

	void Update () {
		if(lookAtPlayer && lookHere == null) {
			lookHere = Game.playerObj;
			return;
		}

        if(!lookAtPlayer && lookHere == null && destroyWhenTargetNotFound) {
            Destroy(gameObject);
        }

		if(turnFasterOverTime) {
			turnSpeed += Time.deltaTime * turnFasterOverTimeSpeed;
		}

        if(lookHere != null) { 
		    Vector3 dir = transform.position - lookHere.transform.position;
		    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		    Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		    transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
        }
	}
}
