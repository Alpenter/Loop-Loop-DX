using UnityEngine;

public class MatchBulletColorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(GetComponent<MeshRenderer>() != null){
			GetComponent<MeshRenderer>().material.color = Game.bulletColor;
		} else if (GetComponent<TextMesh>() != null){
			GetComponent<TextMesh>().color = Game.bulletColor;
		} else if (GetComponent<LineRenderer>() != null) {
			GetComponent<LineRenderer>().material.color = Game.bulletColor;
		} else if(GetComponent<ParticleSystem>() != null) {
            var ps = GetComponent<ParticleSystem>().main;
            ps.startColor = Game.bulletColor;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<MeshRenderer>() != null){
			GetComponent<MeshRenderer>().material.color = Game.bulletColor;
		} else if (GetComponent<TextMesh>() != null){
			GetComponent<TextMesh>().color = Game.bulletColor;
		} else if (GetComponent<LineRenderer>() != null) {
			GetComponent<LineRenderer>().material.color = Game.bulletColor;
		} else if(GetComponent<ParticleSystem>() != null) {
            var ps = GetComponent<ParticleSystem>().main;
            ps.startColor = Game.bulletColor;
        }
	}
}
