using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class DeathScreenManager : MonoBehaviour {
    GameObject swipe;
    int lvl;
    bool loading = false;
    float loadTime = 1f;

	void Awake () {
        Time.timeScale = 1f;
        BloomOptimized blum = Camera.main.gameObject.GetComponent<BloomOptimized>();
        blum.enabled = Game.lightEffects;
        Camera.main.backgroundColor = Game.frontColor;
        if (Game.coins > 0) { Game.coins = (Game.coins / 2); }
        swipe = transform.Find("Swipey").gameObject;
        iTween.MoveTo(swipe, iTween.Hash(
            "x", -23f,
            "y", 0f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.none
        ));
        Game.SaveGame();
    }
	
	public void BeginLoad(int level){
        iTween.MoveTo(swipe, iTween.Hash(
            "x", 0f,
            "y", 0f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.none
        ));
        lvl = level;
        loading = true;
    }

    void Update(){

        //Checking if you are using a controller.
        Game.ControllerCheck();

        if (Game.usingController && !loading) {
            if (Input.GetButtonDown("Cancel")) {
                BeginLoad(1);
            }
            if (Input.GetButtonDown("Submit")) {
                BeginLoad(Game.levelDiedOn);
            }
        }

        if (loading){
            loadTime -= Time.deltaTime;
            if (loadTime <= 0){
                SceneManager.LoadScene(lvl);
            }
        }
    }
}
