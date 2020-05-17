using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeeBitScript : MonoBehaviour {
    
    readonly float timeToGo = 5f;
    readonly float scaleToBe = 3f;

    private void Start() {
        iTween.MoveTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", -0.5f, "time", timeToGo, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
        iTween.ScaleTo(gameObject, iTween.Hash("x", scaleToBe, "y", scaleToBe, "z", 0.1f, "time", timeToGo, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
        iTween.ColorTo(gameObject, iTween.Hash("a", 0f, "time", timeToGo, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
        Destroy(gameObject, timeToGo);
    }
}
