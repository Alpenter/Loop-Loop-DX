using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManagerScript : MonoBehaviour {

    AudioSource music = null;
    
    float timeSinceStarted = 0f;
    float timeSinceExit = 0f;
    readonly float fadeOutTime = 4f;

    bool startedTunes = false;
    bool exiting = false;
    bool startedFadeOut = false;
    
    GameObject fadeOut = null;
    GameObject ship = null;

    private void Awake() {
        fadeOut = transform.Find("fade out").gameObject;
        ship = transform.Find("ship").gameObject;

        music = GetComponent<AudioSource>();
        music.volume = Game.musicVolume;
    }

    private void Start() {
        Game.beatGame = true;
        if (Game.savedVektor) {
            Game.sawGoodEnding = true;
            Game.CheckAchievements(7);
        }
        Game.SaveGame();
    }

    private void Update() {
        //Show the ship fly by when you save vektor.
        ship.SetActive(Game.savedVektor);

        //Increase time since started.
        timeSinceStarted += Time.deltaTime;

        //Play music past 2 and a half seconds.
        if(timeSinceStarted > 2.5f) {
            if (!startedTunes) {
                music.Play();
                startedTunes = true;
            }    
        }

        if(timeSinceStarted > 55 && Input.anyKeyDown) {
            exiting = true;
        }

        if (exiting) {
            if (!startedFadeOut) {
                iTween.ColorTo(fadeOut, iTween.Hash(
                    "a", 1f, "time", fadeOutTime - 0.1f,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.none
                ));
                startedFadeOut = true;
            }
            timeSinceExit+=Time.deltaTime;
            if(timeSinceExit >= fadeOutTime) {
                SceneManager.LoadScene(0);
            }
        }

    }
}
