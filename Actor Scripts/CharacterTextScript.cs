using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTextScript : MonoBehaviour {

	public enum Person{
		Carl,
		Vektor,
		Redd,
		Cheese,
		Cotton,
		Vortex,
		Plum,
		Warden,
	}
	public Person per;
	public enum Side {
		Left,
		Right,
	}
	Side s;
	
	public string text;
	string textBeingWritten = "";
	
	Texture pic;
	GameObject picture, textObj, box;
	
    AnimationScript anim = null;

    AudioClip speechNoise = null;
	AudioClip snd = null;
	
	AudioSource speechPlayer = null;

    readonly float letterPause = 0.01f;
	
    Vector2 textureScale = Vector2.zero;
	
	void Awake(){
		//Start off at correct position.
		transform.position = new Vector3(0, -24, -5);
		
		//Load letter write sound.
		snd = Resources.Load("SFX/Letter Write Sound") as AudioClip;
		
		//Find needed objects and components.
		picture = transform.Find("Picture").gameObject;
		textObj = transform.Find("New Text").gameObject;
		box = transform.Find("box").gameObject;
		anim = picture.GetComponent<AnimationScript>();
		anim.pauseBetweenFrames = 0.2f;

        //Apply settings based on who is talking.
        switch (per) {
            case Person.Carl: //Done
                pic = Resources.Load("Portraits/Commander Carl Image Sheet") as Texture;
                speechNoise = Resources.Load("SFX/Carl Speaking") as AudioClip;
			    s = Side.Left;
			    anim.framesOffset[0] = Vector2.zero;
			    anim.framesOffset[1] = new Vector2(0.75f, 0f);
			    textureScale = new Vector2(0.25f, 1f);
                break;
            case Person.Vektor: //Done
            	pic = Resources.Load("Portraits/Vektor Face Sheet") as Texture;
            	speechNoise = Resources.Load("SFX/Vektor Speak") as AudioClip;
				s = Side.Left;
				anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.5f, 0f);
				textureScale = new Vector2(0.5f, 1f);
				picture.transform.localScale = new Vector3(8f, 8f, 1f);
				break; 
			case Person.Redd: //Done
				pic = Resources.Load("Portraits/Reinforcer Redd") as Texture;
				speechNoise = Resources.Load("SFX/Redd Speaking") as AudioClip;
				s = Side.Right;
				anim.enabled = false;
				textureScale = Vector2.one;
				break;
			case Person.Cheese: //Done
				pic = Resources.Load("Portraits/Chef Cheese") as Texture;
				speechNoise = Resources.Load("SFX/Chef Cheese Talking") as AudioClip;
				s = Side.Right;
				anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.5f, 0f);
				textureScale = new Vector2(0.5f, 1f);
				break;
			case Person.Cotton: //Done
				pic = Resources.Load("Portraits/Cotton") as Texture;
				speechNoise = Resources.Load("SFX/Chef Cheese Talking") as AudioClip;
				s = Side.Right;
				anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.5f, 0f);
				textureScale = new Vector2(0.5f, 1f);
				break;
			case Person.Vortex: //Done
				pic = Resources.Load("Portraits/The Void") as Texture;
				speechNoise = Resources.Load("SFX/Void Speaking") as AudioClip;
				s = Side.Right;
				anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.5f, 0f);
				textureScale = new Vector2(0.5f, 1f);
				break;
			case Person.Plum:
				pic = Resources.Load("Portraits/Punisher Plum") as Texture;
				speechNoise = Resources.Load("SFX/Plum Speak") as AudioClip;
				s = Side.Right;
				anim.framesOffset[0] = Vector2.zero;
				anim.framesOffset[1] = new Vector2(0.5f, 0f);
				textureScale = new Vector2(0.5f, 1f);
				break;
			case Person.Warden:
				pic = Resources.Load("Portraits/The Warden") as Texture;
				speechNoise = Resources.Load("SFX/Warden Speak") as AudioClip;
				s = Side.Right;
				anim.enabled = false;
				textureScale = Vector2.one;
				break;
        }
		
        //Find the audio source that plays speech.
        if(GetComponent<AudioSource>() == null){
        	speechPlayer = gameObject.AddComponent<AudioSource>();
        } else if (GetComponent<AudioSource>() != null){
        	speechPlayer = GetComponent<AudioSource>();
        }
        speechPlayer.clip = speechNoise;
        speechPlayer.volume = Game.soundVolume;
        speechPlayer.loop = false;
        speechPlayer.pitch = Random.Range(0.8f, 1.2f);
        
		//Apply stuff.
		picture.GetComponent<MeshRenderer>().material.mainTexture = pic;
		picture.GetComponent<MeshRenderer>().material.mainTextureScale = textureScale;
		if(s == Side.Left){
			picture.transform.localPosition = new Vector3(-32, 0, 0);
			box.transform.localPosition = new Vector3(4, -2, 2);
			textObj.transform.localPosition = new Vector3(-27, -2, 0);
		}else if(s == Side.Right){
			picture.transform.localPosition = new Vector3(30.75f, 0, 0);
			box.transform.localPosition = new Vector3(-4, -2, 2);
			textObj.transform.localPosition = new Vector3(-35, -2, 0);
		}
	}
	
	void Start(){
        if(!Game.tootMode && Game.speechSounds){ speechPlayer.Play(); }

		//Set parent to bullet holder, so the text dissapears when you change level.
		transform.parent = Game.bulletHolder.transform;
		
		//Come in.
		iTween.MoveTo(gameObject, iTween.Hash(
			"x", 0f, "y", -16f, "z", -5f,
			"time", 0.5f,
			"looptype", iTween.LoopType.none,
			"easetype", iTween.EaseType.easeInOutSine
		));
		//Come out.
		iTween.MoveTo(gameObject, iTween.Hash(
			"x", 0f, "y", -25f, "z", -5f,
			"time", 1f,
			"delay", 5f,
			"looptype", iTween.LoopType.none,
			"easetype", iTween.EaseType.easeInOutSine
		));
		//Destroy after a while.
		Destroy(gameObject, 6f);
		
		//Start writing!
		StartCoroutine(TypeText ());
	}
	
	// Update is called once per frame
	void Update () {
        if (Game.manager.GetComponent<GameManager>().zTheme == GameManager.ZoneThemes.GreenZone3 || Game.brokeCore) {
            picture.GetComponent<MeshRenderer>().material.color = Game.frontColor;
        } else if (Game.manager.GetComponent<GameManager>().zTheme == GameManager.ZoneThemes.FinalBossZone7) {
            picture.GetComponent<MeshRenderer>().material.color = Color.white;
        } else {
            picture.GetComponent<MeshRenderer>().material.color = Game.backColor;
        }
        box.GetComponent<MeshRenderer>().material.color = new Color(Game.frontColor.r, Game.frontColor.g, Game.frontColor.b, 0.6666f);
	}
	
	//Writing out the text.
	IEnumerator TypeText () {
		foreach (char letter in text.ToCharArray()) {
			yield return new WaitForSeconds (letterPause);		
			
			textBeingWritten += letter;
		
			if(letter != ' '){
				if(!Game.tootMode){ PlaySound.NoLoop(snd); }
			}

			if(text != null){
				//Text should equal the text being written out in the coroutine.
				textObj.GetComponent<TextMesh>().text = textBeingWritten;
			}

			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}
}
