using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreButtonScript : MonoBehaviour {

	//Variables.
	int rank;
	int rankLim;
	
    string whatIBought = "";

	GameObject border = null;
	GameObject box = null;

	bool over = false;
	bool canRefund = false;
	
	public enum Power{
		Health,
		Orbitals,
		ShotSpeed,
		Speed,
		ShotSplit,
		FloppyDisk,
		Refund,
	}
	public Power pow;
	MenuManagerScript menuMan = null;
	
	//Find border at first frame.
	void Awake(){ 
		border = transform.Find("border").gameObject; 
		if(pow == Power.Refund) {
			box = transform.Find("box").gameObject;
		}
	}
	
	//Find the menu manager at the start.
	void Start(){
		menuMan = Game.manager.GetComponent<MenuManagerScript>();
		if(pow == Power.ShotSplit){
			rankLim = 2;
		} else if(pow == Power.FloppyDisk){
            rankLim = 5;
        } else { rankLim = 3; }
	}
	
	//When you mouse over, track it.
	void OnMouseOver(){
        if (!Game.usingController) { 
		    if(pow == Power.Refund){
			    menuMan.shopIndex = 11;
		    } else { 
			    //Setting the shop index for every button that isn't a floppy disk.
			    if(rank < rankLim && pow != Power.FloppyDisk){
				    if(pow == Power.Health){
					    menuMan.shopIndex = 2;
				    } else if(pow == Power.ShotSpeed){
					    menuMan.shopIndex = 4;
				    } else if(pow == Power.Speed){
					    menuMan.shopIndex = 5;
				    } else if(pow == Power.ShotSplit){
					    menuMan.shopIndex = 6;
				    }
			    } else if(pow == Power.FloppyDisk) { //Setting the index for the floppy disk.
				    if(Game.ownsFloppyDisk){
					    menuMan.shopIndex = 8;
				    } else {
					    menuMan.shopIndex = 7;
				    }
			    } else { //If you are at this index, you have fully upgraded what you have moused over.
				    menuMan.shopIndex = 8;
			    }
		    }
		    over = true;
        }
	}

	//When you leave it, track it as well.
	void OnMouseExit(){ over = false; }

	//On every frame...
	void Update(){
		
		//Which type of buttons we manage.
		if(pow == Power.Refund){
			//Manage the refund button.
			ManageRefundButton();
		} else {
			//Manage every other button.
			ManageEveryOtherButton();
		}
	}
	
	//Management for the refund button.
	void ManageRefundButton(){
		//Always check if you can refund.
		canRefund = menuMan.RefundCheck();
		
		box.SetActive(canRefund);
		
		//If you are able to refund...
		if(canRefund){
			//Show border when you are over the button.
			border.SetActive(over);
			
			//When user clicks the refund button...
			if(Input.GetMouseButtonDown(0) && over && !Game.usingController){
                //Do the refund! (Add an extra 0!)
                menuMan.Refund();
			}
		} else {
			border.SetActive(false);
		}
	}
	
	void ManageEveryOtherButton(){
		//Show border when you are over the button.
		border.SetActive(over && !Game.usingController);
		
		//Track current rank of power up.
		if(pow == Power.Health){
			rank = Game.healthRank;
		} else if(pow == Power.ShotSpeed){
			rank = Game.shotSpeedRank;
		} else if(pow == Power.Speed){
			rank = Game.speedRank;
		} else if(pow == Power.ShotSplit){
			rank = Game.shotSplitRank;
		} else if(pow == Power.FloppyDisk){
			rank = 4; //Floppy disc has rank 4.
		}
		
		//When you click over a button and have money to spend...
		if(Input.GetMouseButtonDown(0) && over && Game.coins >= 0 && rank < rankLim && !Game.usingController && !(pow == Power.FloppyDisk && Game.ownsFloppyDisk)){
            switch (pow) { 
                case Power.Health:
                    whatIBought = "Health";
                    break;
                case Power.ShotSpeed:
                    whatIBought = "Shot Speed";
                    break;
                case Power.Speed:
                    whatIBought = "Speed";
                    break;
                case Power.ShotSplit:
                    whatIBought = "Shot Split";
                    break;
                case Power.FloppyDisk:
                    whatIBought = "Floppy";
                    break;
			}
	        
            //Run the function that returns what we gathered about this purchase.
            menuMan.ShopAction(rank, whatIBought);
		}	
	}
}