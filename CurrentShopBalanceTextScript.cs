using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentShopBalanceTextScript : MonoBehaviour {

	void Update(){
		if(Game.coins >= 0){
			GetComponent<TextMesh>().text = "Balance: $" + Game.coins.ToString();
		} else {
			GetComponent<TextMesh>().text = "Balance: -$" + (Game.coins*-1).ToString();
		}
	}
}
