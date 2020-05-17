using UnityEngine;

public class ShieldedLaserShooterScript : MonoBehaviour {

    GameObject shield, laserHolder, hitEffect, hitEffectWoobly, laserPuppet, laserFadedPuppet;
    
    float laserScale;
    readonly float fallBackScale = 300f;

    readonly LayerMask wallLayer = 1024;

    private void Awake() {
        shield = transform.Find("shield").gameObject;
        laserHolder = transform.Find("laser holder").gameObject;
        hitEffect = transform.Find("hit effect").gameObject;
        hitEffectWoobly = transform.Find("hit effect wobbly").gameObject;
        laserPuppet = laserHolder.transform.Find("laser puppet").gameObject;
        laserFadedPuppet = laserHolder.transform.Find("laser faded puppet").gameObject;

        shield.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        hitEffectWoobly.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void Start() {
        //Set up the faint scaling of the shield.
        iTween.ScaleTo(shield, iTween.Hash(
            "x", 2f, "y", 2f,
            "time", 2f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong
        ));

        //Set up the faint scaling of the hit effect woobly.
        iTween.ScaleTo(hitEffectWoobly, iTween.Hash(
            "x", 0.65f, "y", 0.65f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong
        ));

        //Set up the faint scaling of the faded laser puppet.
        iTween.ScaleTo(laserFadedPuppet, iTween.Hash(
            "y", 3f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong
        ));
    }
    
    private void Update(){
        //Set the color of the shield.
        shield.GetComponent<MeshRenderer>().material.color = new Color (Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.2f);

        //Searching for walls in front of me!
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, fallBackScale, wallLayer);
        if (hit) { //If a wall was found.
            laserScale = hit.distance + 0.5f;
        } else {
            laserScale = fallBackScale + 0.5f;
        }
        
        //Scale the laser out.
        laserHolder.transform.localScale = new Vector3(1f, laserScale, 1f);

        //Manage the hit effect and the woobly.
        if (hit) {
            hitEffect.transform.position = new Vector3(hit.point.x, hit.point.y, -4f);
            hitEffectWoobly.transform.position = hitEffect.transform.position;
            //Manage the scrolling of the laser puppets when a wall isn't found.
            laserPuppet.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hit.distance, 1f);
            laserFadedPuppet.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hit.distance, 1f);
        } else {
            //Manage the scrolling of the laser puppets when a wall isn't found.
            laserPuppet.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(fallBackScale, 1f);
            laserFadedPuppet.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(fallBackScale, 1f);
        }
        hitEffect.GetComponent<Renderer>().material.color = Game.bulletColor;
        hitEffectWoobly.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 0.3f);
        hitEffectWoobly.GetComponent<MeshRenderer>().enabled = hit;

        //Handle the laser puppet coloring.
        laserPuppet.GetComponent<MeshRenderer>().material.color = Game.bulletColor;
        laserFadedPuppet.GetComponent<MeshRenderer>().material.color = new Color(Game.bulletColor.r, Game.bulletColor.g, Game.bulletColor.b, 0.3f);
    }
}
