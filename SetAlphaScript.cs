using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAlphaScript : MonoBehaviour {

	public float alpha = 1f;

	void Update () {
		GetComponent<MeshRenderer>().material.color = new Color(
			GetComponent<MeshRenderer>().material.color.r,
			GetComponent<MeshRenderer>().material.color.g,
			GetComponent<MeshRenderer>().material.color.b,
			alpha
		);
	}
}