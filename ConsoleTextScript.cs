using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConsoleTextScript : MonoBehaviour {

    string playerTypedText = "";
    int charsThisLine = 0;
    readonly int maxCharPerLine = 56;
    public string consoleText = "";
    string ender = "_";
    bool showEnder = false;
    float enderFlashCounter = 0f;
    readonly float enderFlashTime = 0.333f;
    bool h, u, n, t, e, r, two;
    bool revealRecipe = false;
    public GameObject recipeObj = null;

    void Update() {

        //Key which key the player presses and set it too keystroke to be added to the string.
        foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Game.usingController) {
                if (Input.GetKeyDown(kcode) && (kcode == KeyCode.Joystick1Button0 || kcode == KeyCode.Joystick1Button1 || kcode == KeyCode.Joystick1Button2 || kcode == KeyCode.Joystick1Button3)) {
                    switch (kcode) {
                        case KeyCode.Joystick1Button0:
                            playerTypedText += "A";
                            break;
                        case KeyCode.Joystick1Button1:
                            playerTypedText += "B";
                            break;
                        case KeyCode.Joystick1Button2:
                            playerTypedText += "X";
                            break;
                        case KeyCode.Joystick1Button3:
                            playerTypedText += "Y";
                            break;
                    }
                }
            } else { 
                if (Input.GetKeyDown(kcode) && kcode != KeyCode.Mouse0 && kcode != KeyCode.Mouse1 && kcode != KeyCode.Mouse2 && kcode != KeyCode.Mouse3 && kcode != KeyCode.Mouse4 && kcode != KeyCode.Mouse5) { 
                    //Debug.Log("KeyCode down: " + kcode);
                    if(kcode == KeyCode.Space) {
                        playerTypedText += " ";
                    } else { 
                        playerTypedText += kcode.ToString()[0];
                    }
                    HunterTwoCheck(kcode);
                    charsThisLine++;
                }
            }
        }
        
        //Adding keys to player typed text, if it goes over max characters a line, add a new line.
        if(charsThisLine >= maxCharPerLine) {
            playerTypedText += '\n';
            charsThisLine = 0;
        }

        //Flash the ender... the underscore thing.
        enderFlashCounter -= Time.deltaTime;
        if(enderFlashCounter <= 0) {
            showEnder = !showEnder;
            enderFlashCounter = enderFlashTime;
        }
        if (showEnder) { ender = "_"; }
        else { ender = ""; }

        //This is only visible when recipe has not been revealed.
        GetComponent<MeshRenderer>().enabled = (!revealRecipe);
        recipeObj.SetActive(revealRecipe);

        //Displaying the text.
        consoleText = "/>" + playerTypedText + ender;
        GetComponent<TextMesh>().text = consoleText;
    }


    private void HunterTwoCheck(KeyCode k) {
        
        if (h) {
            if (u) {
                if (n) {
                    if (t) {
                        if (e) {
                            if (r) {
                                if (!two) { 
                                    if(k == KeyCode.Alpha2 || k == KeyCode.Keypad2) {
                                        Game.CheckAchievements(46);
                                        revealRecipe = true;
                                        two = true;
                                    } else {
                                        h = false;
                                        u = false;
                                        n = false;
                                        t = false;
                                        e = false;
                                        r = false;
                                        two = false;
                                    }
                                }
                            } else {
                                if(k == KeyCode.R) {
                                    r = true;
                                } else {
                                    h = false;
                                    u = false;
                                    n = false;
                                    t = false;
                                    e = false;
                                    r = false;
                                }
                            }
                        } else {
                            if(k == KeyCode.E) {
                                e = true;
                            } else {
                                h = false;
                                u = false;
                                n = false;
                                t = false;
                                e = false;
                            }
                        }
                    } else {
                        if(k == KeyCode.T) {
                            t = true;
                        } else {
                            h = false;
                            u = false;
                            n = false;
                            t = false;
                        }
                    }
                } else { 
                    if(k == KeyCode.N) {
                        n = true;
                    } else {
                        h = false;
                        u = false;
                        n = false;
                    }
                }
            } else {
                if(k == KeyCode.U) {
                    u = true;
                } else {
                    u = false;
                    h = false;
                }
            }
        } else { 
            if(k == KeyCode.H) {
                h = true;
            }
        }
    }
}

