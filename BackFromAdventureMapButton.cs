using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFromAdventureMapButton : MonoBehaviour {

    public GameObject manager;
    GameObject select = null;

    Color selectedColor, unselectedColor;
    Color targetColor = Color.clear;
    Vector3 targetScale, selectScale, unselectScale;

    bool selected = false;

    MenuManagerScript m = null;

    AudioClip overSnd = null;

    void Awake(){
        overSnd = Resources.Load("SFX/menu select") as AudioClip;
        select = transform.Find("select").gameObject;
        selectedColor = Color.white;
        unselectedColor = new Color(1f, 1f, 1f, 0f);
        select.GetComponent<MeshRenderer>().material.color = targetColor;
        selectScale = new Vector3(select.transform.localScale.x, 1f, 1f);
        unselectScale = new Vector3(select.transform.localScale.x, 0f, 1f);
        targetScale = new Vector3(select.transform.localScale.x, 0f, 1f);
        select.transform.localScale = new Vector3(select.transform.localScale.x, 0f, 1f);
    }

    void Start(){
        m = manager.GetComponent<MenuManagerScript>();
    }

    void Update(){
        if (selected) { targetColor = selectedColor; targetScale = selectScale; }
        else { targetColor = unselectedColor; targetScale = unselectScale; }
        select.GetComponent<MeshRenderer>().material.color = Color.Lerp(select.GetComponent<MeshRenderer>().material.color, targetColor, 25f * Time.deltaTime);
        select.transform.localScale = Vector3.Lerp(select.transform.localScale, targetScale, 20f * Time.deltaTime);
    }

    void OnMouseEnter() { PlaySound.NoLoopRandomPitch(overSnd, 1f, 1.5f); }
    void OnMouseOver() { selected = true; }
    void OnMouseExit() { selected = false; }

    void OnMouseDown(){
        if (selected){
            m.MenuAction(16);
        }
    }
}