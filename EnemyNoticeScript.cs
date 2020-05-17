using UnityEngine;

public class EnemyNoticeScript : MonoBehaviour {

    readonly float alph = 0.6f;
    readonly float speed = (1f/3f);

    GameObject puppet = null;
    
    Color startColor = Color.clear;
    Color targetColor = Color.clear;
    
    GameManager gm = null;

    private void Awake() {
        puppet = transform.Find("puppet").gameObject;
    }
    
    private void Start() {
        gm = Game.manager.GetComponent<GameManager>();
        if(gm.zTheme != GameManager.ZoneThemes.WhiteZone6) {
            startColor = Game.backColor;
        } else {
            startColor = Game.frontColor;
        }

        targetColor = new Color(startColor.r, startColor.g, startColor.b, alph);
        
        puppet.GetComponent<MeshRenderer>().material.color = startColor;

        iTween.ColorTo(puppet, iTween.Hash(
            "r", targetColor.r,
            "g", targetColor.g,
            "b", targetColor.b,
            "a", alph,
            "time", speed,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong
        ));
    }
}
