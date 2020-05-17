using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButton : MonoBehaviour {

	public enum Butt {
		Resume,
		BackToMenu,
	}
	public Butt b;

	GameObject line = null;
    GameObject gManObj = null;

    GameManager gMan = null;

	AudioClip snd = null;

	bool selected = false;
	bool pressed = false;

	void Awake(){
		line = transform.Find("line").gameObject;
		snd = Resources.Load("SFX/Button Click") as AudioClip;
	}

    private void Start() {
		gManObj = Game.manager;
		gMan = gManObj.GetComponent<GameManager>();
    }

    void OnMouseOver(){ if(!Game.usingController) { selected = true; } }
	void OnMouseExit(){ selected = false; }

	void OnMouseDown(){
        if (!gMan.goingBackToMenu) { //No button is interactible in any way when the going back to menu transition is playing.
		    if(!pressed){
                switch (b) {
                    case Butt.Resume:
                        gMan.PauseMenuAction(0);
                        break;
                    case Butt.BackToMenu:
                        gMan.PauseMenuAction(1);
                        break;
                }
                pressed = true;
		    }
		    PlaySound.NoLoop(snd);
        }
	}
	void OnMouseUp(){ pressed = false; }

	void Update () {
        //When to set the line to be active.
		line.SetActive(selected);

        if (Game.usingController) { selected = false; }
	}
}
