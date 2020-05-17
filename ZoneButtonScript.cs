using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneButtonScript : MonoBehaviour {

    public enum Zone {
        Zone1,
        Zone2,
        Zone3,
        Zone4,
        Zone5,
        Zone6,
        ZoneBoss,
    }
    public Zone z;
    int lvl = 0;
    GameObject selectObj;
    [HideInInspector] public bool over = false;
    Color targetColor, currentColor;
    readonly float alph = 0.3f;
    public GameObject manager;
    MenuManagerScript m;

    void Start() {
        selectObj = transform.Find("select").gameObject;
        m = manager.GetComponent<MenuManagerScript>();
        switch (z) {
            case Zone.Zone1:
                lvl = 7;
                targetColor = new Color(1f, 0f, 0f, alph);
                break;
            case Zone.Zone2:
                lvl = 9;
                targetColor = new Color(1f, 1f, 0f, alph); 
                break;
            case Zone.Zone3:
                lvl = 10;
                targetColor = new Color(0f, 1f, 0f, alph);
                break;
            case Zone.Zone4:
                lvl = 11;
                targetColor = new Color(0f, 0f, 1f, alph);
                break;
            case Zone.Zone5:
                lvl = 12;
                targetColor = new Color(1f, 0f, 1f, alph);
                break;
            case Zone.Zone6:
                lvl = 13;
                targetColor = new Color(0f, 0f, 0f, alph);
                break;
            case Zone.ZoneBoss:
                lvl = 14;
                targetColor = new Color(1f, 1f, 1f, alph);
                break;
        }
    }

    void OnMouseOver() { if(!Game.usingController){ over = true; } }
    void OnMouseExit() { if(!Game.usingController){ over = false; } }
    
    void OnMouseDown() {
        if (over && !Game.usingController) {
            m.LoadLevel(lvl);
        }
    }

    void Update() {
        if (Game.usingController) {
            Vector2 p0 = m.adventurePadSelector.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(p0, -Vector2.up, 0.2f);
            if(hit.collider != null) { 
                if(hit.collider.gameObject == this.gameObject) {
                     over = true;
                }
            } else {
                over = false;
            }

            if(over && Input.GetButtonDown("Submit") && m.adventureDelayCounter <= 0) {
                m.LoadLevel(lvl);
            }
        }
        
        if (over) { currentColor = targetColor; }
        else { currentColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f); }
        selectObj.GetComponent<MeshRenderer>().material.color = Color.Lerp(selectObj.GetComponent<MeshRenderer>().material.color, currentColor, 20f * Time.deltaTime);
    }

}
