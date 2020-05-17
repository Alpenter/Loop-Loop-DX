using UnityEngine;

public class XDModeScript : MonoBehaviour {

	string t = "";

	void Awake(){
		if(Game.boughtSupporterPack){
			int i = Random.Range(0, 2);
			//Debug.Log(i.ToString());
			switch(i){
				case 0:
					t = "L p-L p DX";
					break;
				case 1:
					t = "L p-L p XD";
					break;
				case 2:
					t = "L p-L p XD";
					break;
			}
		} else {
			t = "L p-L p DX";
		}

		GetComponent<TextMesh>().text = t;
	}
}
