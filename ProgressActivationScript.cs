using UnityEngine;

public class ProgressActivationScript : MonoBehaviour {
	
	public enum Activation {
		Enable,
		Disable,
	}
	
	public enum Mark {
		GotGun,
		BeatZoneRed1,
		BeatZoneYellow2,
		BeatZoneGreen3,
		BeatZoneBlue4,
		BeatZonePurple5,
		BeatZoneWhite6,
		BeatFinalBoss,
	}

	public Activation act;
	public Mark m;
	bool a;
	
	void Awake(){
		if(act == Activation.Enable){
			a = true;
		} else if(act == Activation.Disable){
			a = false;
		}
        gameObject.SetActive(true);
	}
	
	void Start(){
		if(m == Mark.GotGun && Game.gotGun) { gameObject.SetActive(a); }
		if(m == Mark.GotGun && !Game.gotGun) { gameObject.SetActive(!a); }

        if(m == Mark.BeatZoneRed1 && Game.beatFirstLevel) { gameObject.SetActive(a); }
        if(m == Mark.BeatZoneRed1 && !Game.beatFirstLevel) { gameObject.SetActive(!a); }

        if(m == Mark.BeatZoneYellow2 && Game.beatYellow) { gameObject.SetActive(a); }
        if(m == Mark.BeatZoneYellow2 && !Game.beatYellow) { gameObject.SetActive(!a); }
           
        if(m == Mark.BeatZoneGreen3 && Game.beatGreen) { gameObject.SetActive(a); }
        if(m == Mark.BeatZoneGreen3 && !Game.beatGreen) { gameObject.SetActive(!a); }

        if(m == Mark.BeatZoneBlue4 && Game.beatBlue) { gameObject.SetActive(a);  }
        if(m == Mark.BeatZoneBlue4 && !Game.beatBlue) { gameObject.SetActive(!a);  }

        if(m == Mark.BeatZonePurple5 && Game.beatPurple) { gameObject.SetActive(a); }
        if(m == Mark.BeatZonePurple5 && !Game.beatPurple) { gameObject.SetActive(!a); }

        if(m == Mark.BeatZoneWhite6 && Game.beatWhite) { gameObject.SetActive(a); }
        if(m == Mark.BeatZoneWhite6 && !Game.beatWhite) { gameObject.SetActive(!a); }

        if(m == Mark.BeatFinalBoss && Game.beatFinalBoss) { gameObject.SetActive(a); }
        if(m == Mark.BeatFinalBoss && !Game.beatFinalBoss) { gameObject.SetActive(!a); }
    }
}
