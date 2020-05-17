using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCutsceneScript : MonoBehaviour {
    
    int index = 0;
	int endAtLine = 0;
	int charsCurrently = 0;
    int maxCharsALine = 30;
    List<int> speakingCharacterIndex = new List<int>();

    float fadeOutTime = 7.4f;
    readonly float letterPause = 0.01f;
    
	public string[] textLines;
	string textBeingWritten = "";
	
	bool ended = false;
	bool ableToGoOn = false;
    bool playedFadeTween = false;

    TextAsset tAss = null;

    GameObject textHolder = null;
    GameObject graphicHolder = null;
    GameObject heartText = null;
    GameObject carlAndVektorText = null;
    GameObject carlAndVektorTextBox = null;
    GameObject carlPicture = null;
    GameObject vektorPicture = null;
    GameObject spookySoundObj = null;
    GameObject fadeOut = null;

    AudioSource spooky = null;
    AudioSource whiteNoise = null;

    AudioClip letterWriteSound = null;
    AudioClip advanceTextSound = null;

    TextMesh cAndVTMesh = null;
    TextMesh heartTMesh = null;
    TextMesh tMeshCurrentlyBeingUsed = null;

    public enum Speaking {
        Heart,
        Carl,
        Vektor,
    }
    public Speaking whoSpeak = Speaking.Heart;

    //On Awake, find and load all needed assets.
    private void Awake() {
        //Find the holder gameobjects.
        textHolder = transform.Find("Texts").gameObject;
        graphicHolder = transform.Find("Graphics").gameObject;

        //Find the text children.
        heartText = textHolder.transform.Find("heart text").gameObject;
        carlAndVektorText = textHolder.transform.Find("carl and vektor text").gameObject;

        //Find the graphic children.
        carlAndVektorTextBox = graphicHolder.transform.Find("carl and vektor text box").gameObject;
        carlPicture = graphicHolder.transform.Find("carl picture").gameObject;
        vektorPicture = graphicHolder.transform.Find("vektor picture").gameObject;

        //Find the spooky sound object and audio source.
        spookySoundObj = transform.Find("Deal With Heart Sound").gameObject;
        spooky = spookySoundObj.GetComponent<AudioSource>();
        whiteNoise = gameObject.GetComponent<AudioSource>();
        whiteNoise.volume = Game.soundVolume;

        //Find the fadeout gameobject.
        fadeOut = transform.Find("Fade Out").gameObject;

        //Load audio.
        letterWriteSound = Resources.Load("SFX/Letter Write Sound") as AudioClip;
        advanceTextSound = Resources.Load("SFX/AdvanceText") as AudioClip;

        //Load text meshes.
        cAndVTMesh = carlAndVektorText.GetComponent<TextMesh>();
        heartTMesh = heartText.GetComponent<TextMesh>();

        //Load text assets.
        if (Game.savedVektor) {
            tAss = Resources.Load("Good Ending Text") as TextAsset;
        } else {
            tAss = Resources.Load("Bad Ending Text") as TextAsset;
        }
    }

    private void Start() {
        //Read text from the text file.
		textLines = (tAss.text.Split('\n'));
		endAtLine = textLines.Length - 1;
		StartCoroutine(TypeText ());

        //Populate list with who speaks at what line. (0 = heart, 1 = Carl, 2 = Vektor).
        if (Game.savedVektor) {
            int[] a = { 
                0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 2, 1, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 0, 2, 2, 1, 2, 0, 2, 1, 1,
            };
            for(int i = 0; i < a.Length; i++) {
                speakingCharacterIndex.Add(a[i]);
            }
        } else {
            int[] a = {
                0, 1, 0, 1, 0, 1, 0, 0 , 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
            };
            for(int i = 0; i < a.Length; i++) {
                speakingCharacterIndex.Add(a[i]);
            }
        }
    }

    private void Update() {
        if (!ended) { //If the cutscene is not ended...

            //Managing the spooky sound effect.
            if (!Game.savedVektor) {
                if(index < 20) { spooky.volume = 0f; }
                else if(index == 20) { spooky.volume = 0.1f;}
                else if(index == 21) { spooky.volume = 0.25f;}
                else if(index == 22) { spooky.volume = 0.4f;}
                else if(index == 23) { spooky.volume = 0.6f;}
                else if(index == 24) { spooky.volume = 0.8f;}
                else if(index == 25) { spooky.volume = 1f;}
            } else {
                spooky.volume = 0f;
            }

            //Determine which text mesh is currently being used.
            int currentSpeaker = speakingCharacterIndex[index];
            switch (currentSpeaker) {
                case 0: //Heart speaking.
                    whoSpeak = Speaking.Heart;
                    maxCharsALine = 30;
                    tMeshCurrentlyBeingUsed = heartTMesh;
                    cAndVTMesh.text = "";
                    break;
                case 1: //Carl Speaking.
                    whoSpeak = Speaking.Carl;
                    maxCharsALine = 48;
                    tMeshCurrentlyBeingUsed = cAndVTMesh;
                    heartTMesh.text = "";
                    break;
                case 2: //Vektor speaking.
                    whoSpeak = Speaking.Vektor;
                    maxCharsALine = 48;
                    tMeshCurrentlyBeingUsed = cAndVTMesh;
                    heartTMesh.text = "";
                    break;
            }

            //When to set certain graphics to be active.
            heartText.SetActive(whoSpeak == Speaking.Heart);
            carlAndVektorText.SetActive(whoSpeak == Speaking.Vektor || whoSpeak == Speaking.Carl);
            carlAndVektorTextBox.SetActive(whoSpeak == Speaking.Vektor || whoSpeak == Speaking.Carl);
            carlPicture.SetActive(whoSpeak == Speaking.Carl);
            vektorPicture.SetActive(whoSpeak == Speaking.Vektor);

            if((Input.GetButtonDown("Submit") || Input.GetButtonDown("Shoot")) && index != endAtLine && ableToGoOn){
				index++;
				charsCurrently = 0;
				PlaySound.NoLoop(advanceTextSound);
				textBeingWritten = "";
				StartCoroutine(TypeText ());
				tMeshCurrentlyBeingUsed.text = "";
				ableToGoOn = false;
			}
			
			if((Input.GetButtonDown("Submit") || Input.GetButtonDown("Shoot")) && index >= endAtLine && ableToGoOn){
		        ended = true;
			}
        } else { //Load credits when ended...
            if (!Game.savedVektor) {
                SceneManager.LoadScene(16);
            } else {
                fadeOutTime -= Time.deltaTime;
                whiteNoise.volume -= Time.deltaTime/7f;
                if(whiteNoise.volume <= 0) { whiteNoise.volume = 0; }
                if (!playedFadeTween) {
                    iTweenEvent.GetEvent(fadeOut, "fadeout").Play();
                    playedFadeTween = true;
                }
                if(fadeOutTime <= 0) {
                    SceneManager.LoadScene(16);
                }
            }
        }
    }

    //Writing out the text.
    IEnumerator TypeText () {
		foreach (char letter in textLines[index].ToCharArray()) {
			yield return new WaitForSeconds (letterPause);		
			if (letter == '~'){
				ableToGoOn = true;
			} else {
				textBeingWritten += letter;
			}

			if(letter != ' '){
				PlaySound.NoLoop(letterWriteSound);
			}

			if (charsCurrently >= maxCharsALine && letter == ' '){
				textBeingWritten = textBeingWritten + "\n";
				charsCurrently = 0;
			}

			if(tMeshCurrentlyBeingUsed != null){
				//Text should equal the text being written out in the coroutine.
				tMeshCurrentlyBeingUsed.text = textBeingWritten;
			}

			charsCurrently ++;
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}
}