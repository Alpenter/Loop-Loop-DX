using UnityEngine;
using System.Collections;

public class SetMatColor : MonoBehaviour {

	public Color matColor;

	void Awake()
	{
		GetComponent<Renderer> ().material.color = matColor;
	}
}
