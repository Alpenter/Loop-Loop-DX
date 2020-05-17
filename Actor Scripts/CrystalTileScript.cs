using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalTileScript : MonoBehaviour {

	public Vector2 offSetToBe = Vector2.zero;
	GameObject colorPuppet = null;
    GameObject crystalPuppet = null;
	readonly float alf = 0.12f;

	void Start(){
		GameObject outlinePuppet = transform.Find("outline puppet").gameObject;
		crystalPuppet = transform.Find("crystal puppet").gameObject;

		outlinePuppet.GetComponent<MeshRenderer>().material.mainTextureOffset = offSetToBe;
		crystalPuppet.GetComponent<MeshRenderer>().material.SetTextureOffset("_BumpMap", offSetToBe);

		colorPuppet = transform.Find("color puppet").gameObject;
	}

	void Update(){
		colorPuppet.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, alf);
        crystalPuppet.SetActive(Game.lightEffects);
	}
}
