using UnityEngine;

public class TriggerSameTweenAfterSecondsScript : MonoBehaviour {

    public string tweenName = "";
    public float timeBeforeTween = 1f;
    float counter = 0f;

    private void Awake() {
        counter = timeBeforeTween;    
    }

    private void Update() {
        counter -= Time.deltaTime;
        if(counter <= 0) {
            iTweenEvent.GetEvent(gameObject, tweenName).Play();
            counter = timeBeforeTween;
        }
    }
}
