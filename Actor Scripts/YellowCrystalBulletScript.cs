using UnityEngine;

public class YellowCrystalBulletScript : MonoBehaviour {

    ParticleSystem particles = null;

    private void Awake() {
        GameObject particlesObj = transform.Find("Particle System").gameObject;
        particles = particlesObj.GetComponent<ParticleSystem>();
    }

    private void Update() {
        Vector2 wardenPos = new Vector2(Game.warden.transform.position.x, Game.warden.transform.position.y);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        float distanceFromWarden = Vector2.Distance(wardenPos, pos);
        if(distanceFromWarden <= 1f && GetComponent<MoveForward>().maxSpeed > -0.1f) {
            GetComponent<MoveForward>().maxSpeed = 0;
            particles.Stop();
            Destroy(gameObject, 0.5f);
        } else {
            GetComponent<MoveForward>().maxSpeed += Time.deltaTime*5f;
        }
    }
}
