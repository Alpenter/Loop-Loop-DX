using UnityEngine;
using System.Collections;

public class SetRandomOffset : MonoBehaviour {

	public Vector2 textureScale;

	public Vector2[] offsetPool;


	void Awake(){
		GetComponent<MeshRenderer> ().material.mainTextureScale = textureScale;
		GetComponent<MeshRenderer> ().material.mainTextureOffset = offsetPool[Random.Range(0, offsetPool.Length)];

	}
}
