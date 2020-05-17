using UnityEngine;

public class SelfDestructOnStartScript : MonoBehaviour {
	public float lifeTime;
	void Start (){
		Destroy (gameObject, lifeTime);
	}
}