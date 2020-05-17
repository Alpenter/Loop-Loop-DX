using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteSaveInstructionTextScript : MonoBehaviour {
   
    MenuManagerScript m = null;
    float timeLeft = 5f;
    GameObject fadeObj = null;
    Color colorToBe;

    private void Awake() {
        fadeObj = transform.Find("fade").gameObject;
    }

    private void Start() {
        m = Game.manager.GetComponent<MenuManagerScript>();
    }

    private void Update() {
        int numToDisplay = Mathf.RoundToInt(timeLeft);
        GetComponent<TextMesh>().text = "Hold 'X' for " + numToDisplay.ToString() + " seconds...";
        
        if (m.currentMenu == MenuManagerScript.Menu.DeleteSave) { 
            
            if (Input.GetButton("Delete Save")) {
                timeLeft -= Time.deltaTime;
                float alph = 1f - (timeLeft/5f);
                colorToBe = new Color(0f, 0f, 0f, alph);
                m.menuMusic.GetComponent<AudioSource>().pitch = 1f - alph;
            } else {
                timeLeft = 5;
                float smooth = 15f;
                colorToBe.a = Mathf.Lerp(colorToBe.a, 0f, smooth*Time.deltaTime);
                m.menuMusic.GetComponent<AudioSource>().pitch = Mathf.Lerp(m.menuMusic.GetComponent<AudioSource>().pitch, 1f, smooth*Time.deltaTime);
                timeLeft = 5f;
            }
            
            if(timeLeft <= 0) {
                if(m.dType == MenuManagerScript.DeleteType.All) {
                    Game.DeleteSave();
                } else {
                    Game.DeleteAdventureSave();
                }
                SceneManager.LoadScene(0);
            }
        } else {
            colorToBe.a = 0f;
            if(m.currentMenu == MenuManagerScript.Menu.Settings){ m.menuMusic.GetComponent<AudioSource>().pitch = 1f; }
            timeLeft = 5f;
        }
        fadeObj.GetComponent<MeshRenderer>().material.color = colorToBe;
    }
}
