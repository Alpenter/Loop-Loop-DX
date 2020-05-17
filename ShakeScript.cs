using UnityEngine;

public class ShakeScript : MonoBehaviour {

    float intensity = 0.025f;
    Vector3 startPos = Vector3.zero;

    private void Awake() {
        startPos = transform.position;
    }

    private void Update() {
        Vector3 pos = transform.position;

        float x = Random.Range(startPos.x - intensity, startPos.x + intensity);
        float y = Random.Range(startPos.y - intensity, startPos.y + intensity);
        pos = new Vector3(x, y);

        transform.position = pos;
    }
}
