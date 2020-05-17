using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float lifeTime;

	void Awake (){
		Destroy (gameObject, lifeTime);
	}
}