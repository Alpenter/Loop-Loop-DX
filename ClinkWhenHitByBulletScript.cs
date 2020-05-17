using UnityEngine;

public class ClinkWhenHitByBulletScript : MonoBehaviour {
	
	AudioClip snd;
	
	void Awake(){ snd = Resources.Load("SFX/Metal Clink") as AudioClip; }

    public void  Clink (){
        PlaySound.NoLoopRandomPitch(snd, 0.75f, 1.25f);
    }
    
}
