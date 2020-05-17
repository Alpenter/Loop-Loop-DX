using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour {

	public enum ButtonType {
		Adventure,
		BulletHell,
		Legacy,
		LegacyBulletHell,
		ShipShop,
		Statistics,
		Options,
		PurchaseSoundtrack,
		PurchaseSupporterPack,
		Quit,
		BackStats,
		BackSettings,
		BackShop,
		BackDeleteSave,
		DeleteAdventureModeSave,
		DeleteAllSave,
	}
	public ButtonType butt;

	MenuManagerScript menuMan = null;
	
	AudioClip overSnd = null;
	bool unlocked = false;
    
	//Determined if button is unlocked and clickable in Start.
	void Start(){
        GameObject menuManObj = Game.manager;
		menuMan = menuManObj.GetComponent<MenuManagerScript>();
		overSnd = Resources.Load("SFX/menu select") as AudioClip;
        
		switch(butt){
			case ButtonType.Adventure:
				unlocked = true;
				break;
			case ButtonType.BulletHell:
				unlocked = Game.unlockedBulletHell;
                if (!unlocked) { menuMan.lockedMains.Add(1); }
				break;
			case ButtonType.Legacy:
				unlocked = Game.unlockedLegacy;
                if (!unlocked) { menuMan.lockedMains.Add(2); }
				break;
			case ButtonType.LegacyBulletHell:
				unlocked = Game.unlockedLegacyBulletHell;
                if (!unlocked) { menuMan.lockedMains.Add(3); }
				break;
			case ButtonType.ShipShop:
				unlocked = Game.unlockedShop;
                if (!unlocked) { menuMan.lockedMains.Add(4); }
				break;
			case ButtonType.Statistics:
				unlocked = true;
				break;
			case ButtonType.Options:
				unlocked = true;
				break;
			case ButtonType.PurchaseSoundtrack:
				unlocked = true;
				break;
			case ButtonType.PurchaseSupporterPack:
				unlocked = true;
				break;
			case ButtonType.Quit:
				unlocked = true;
				break;
			case ButtonType.BackStats:
				unlocked = true;
				break;
			case ButtonType.BackSettings:
				unlocked = true;
				break;
			case ButtonType.BackShop:
				unlocked = true;
				break;
			case ButtonType.BackDeleteSave:
				unlocked = true;
				break;
			case ButtonType.DeleteAdventureModeSave:
				unlocked = true;
				break;
			case ButtonType.DeleteAllSave:
				unlocked = true;
				break;
		}
	}
	
	void OnMouseEnter(){
		if(unlocked && !Game.usingController){ PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); }
	}

	void OnMouseOver(){ 
		if(unlocked && !Game.usingController){
			switch(butt){
				case ButtonType.Adventure:
					menuMan.selectorMouseIndex = 0;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.BulletHell:
					menuMan.selectorMouseIndex = 1;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.Legacy:
					menuMan.selectorMouseIndex = 2;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.LegacyBulletHell:
					menuMan.selectorMouseIndex = 3;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.ShipShop:
					menuMan.selectorMouseIndex = 4;
					menuMan.shopIndex = 0;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.Statistics:
					menuMan.selectorMouseIndex = 5;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.Options:
					menuMan.selectorMouseIndex = 6;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.PurchaseSoundtrack:
					menuMan.selectorMouseIndex = 7;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.PurchaseSupporterPack:
					menuMan.selectorMouseIndex = 8;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.Quit:
					menuMan.selectorMouseIndex = 9;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.BackStats:
					menuMan.selectorMouseIndex = 10;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.BackSettings:
					menuMan.selectorMouseIndex = 11;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.BackShop:
					menuMan.selectorMouseIndex = 12;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.BackDeleteSave:
					menuMan.selectorMouseIndex = 13;
					menuMan.atRightSettingsMenu = false;
					break;
				case ButtonType.DeleteAdventureModeSave:
					menuMan.selectorMouseIndex = 14;
					menuMan.atRightSettingsMenu = true;
					break;
				case ButtonType.DeleteAllSave:
					menuMan.selectorMouseIndex = 15;
					menuMan.atRightSettingsMenu = true;
					break;			
				}
			menuMan.onThing = true;
		}
	}
	
	void OnMouseExit(){
		menuMan.onThing = false;
	}

	void Update(){
		if(!unlocked && gameObject.GetComponent<TextMesh>() != null){
			gameObject.GetComponent<TextMesh>().text = "? ? ?";
		}
	}
}
