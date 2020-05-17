using UnityEngine;
using System.Collections;

public class ReactToMusic : MonoBehaviour {
    
    //Vectors for starting position, scale, and rotation.
    Vector3 startingPos;
    Vector3 startingScale;
    Vector3 startingRot;

    //Variables for keeping track of music.
    int qSamples = 1024;
    float refValue = 0.1f;
    float rmsValue;
    float dbValue;
    private float[] samples;
    public float intensity = 1f;

    //Bools for how to animate with music.
    public enum ReactType {
        None,
        ScaleX,
        ScaleY,
        ScaleBothXAndY,
        PositionX,
        PositionY,
    }
    public ReactType howToReact = ReactType.None;
    

    void Awake() {
        startingPos = gameObject.transform.localPosition;
        startingScale = gameObject.transform.localScale;
    }

    void Start() {
        samples = new float[qSamples];
    }


    void Update() {
        if(Game.bardObj != null){
            GetVolume();

            switch(howToReact){
                case ReactType.ScaleX:
                    gameObject.transform.localScale = new Vector3(startingScale.x + intensity * rmsValue, startingScale.y, startingScale.z);
                    break;
                case ReactType.ScaleY:
                    gameObject.transform.localScale = new Vector3(startingScale.x, startingScale.y + intensity * rmsValue, startingScale.z);
                    break;
                case ReactType.ScaleBothXAndY:
                    gameObject.transform.localScale = new Vector3(startingScale.x + intensity * rmsValue, startingScale.y + intensity * rmsValue, startingScale.z);
                    break;
                case ReactType.PositionX:
                    gameObject.transform.localPosition = new Vector3(startingPos.x + intensity * rmsValue, startingPos.y, startingPos.z);
                    break;
                case ReactType.PositionY:
                    gameObject.transform.localPosition = new Vector3(startingPos.x, startingPos.y + intensity * rmsValue, startingPos.z);
                    break;
            }
        }
    }


    void GetVolume() {
        Game.bardObj.GetComponent<AudioSource>().GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < qSamples; i++) {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160) {
            dbValue = -160;
        }
    }
}
