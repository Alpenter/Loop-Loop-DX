using UnityEngine;

public class MatchColorOnAwakeScript : MonoBehaviour {

    public enum ColorType {
        Front,
        Back,
        Bullet,
    }
    public ColorType cType = ColorType.Bullet;
    Color colorToBe = Color.clear;

    private void Awake() {
        switch (cType) {
            case ColorType.Front:
                colorToBe = Game.frontColor;
                break;
            case ColorType.Back:
                colorToBe = Game.backColor;
                break;
            case ColorType.Bullet:
                colorToBe = Game.bulletColor;
                break;
        }

        if(GetComponent<MeshRenderer>() != null){
			GetComponent<MeshRenderer>().material.color = colorToBe;
		} else if (GetComponent<TextMesh>() != null){
			GetComponent<TextMesh>().color = colorToBe;
		} else if (GetComponent<LineRenderer>() != null) {
			GetComponent<LineRenderer>().material.color = colorToBe;
		} else if(GetComponent<ParticleSystem>() != null) {
            var ps = GetComponent<ParticleSystem>().main;
            ps.startColor = colorToBe;
        }
    }
}
