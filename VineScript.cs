using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineScript : MonoBehaviour {
    
    public enum CurveDirection { Up, Down, }
    CurveDirection curveDir = CurveDirection.Up;

    List <LineRenderer> vines = new List<LineRenderer>();

    float curve = 0.4f;
    readonly float minCurve = 0.1f;
    readonly float maxCurve = 0.4f;

    int vineCount = 2;
    readonly float startYPos = 1.6f;
    readonly float zPos = -2.5f;
    
  
    private void Awake() {
        //Which way the curve should start going.
        int startCurveDir = Random.Range(0, 1);
        if(startCurveDir == 0) {
            curve = Random.Range(minCurve, minCurve + 0.15f);
            curveDir = CurveDirection.Up;
        } else {
            curve = Random.Range(minCurve + 0.2f, maxCurve);
            curveDir = CurveDirection.Down;
        }

        //How many vines the detail block will use.
        int minVines = 1;
        int maxVines = 3;
        vineCount = Random.Range(minVines, maxVines);
        
        //Find and add the vines to the list.
        for(int i = 0; i < maxVines; i++) {
            GameObject obj = transform.Find("vine " + i.ToString()).gameObject;
            if (i <= vineCount) {
                LineRenderer line = obj.GetComponent<LineRenderer>();
                vines.Add(line);
                line.useWorldSpace = false;
                line.numCapVertices = 3;
                line.startWidth = 0.2f;
                line.positionCount = Random.Range(3, 7);
                Vector3 pos = new Vector3(Random.Range(-1.5f, 1.5f), 0f, zPos);
                obj.transform.localPosition = pos;
            } else {
                LineRenderer line = obj.GetComponent<LineRenderer>();
                line.enabled = false;
                obj.SetActive(false);
            }
        }
    }
    

    private void Update() {
        float yMult = 0.333f;
        float windMult = 0.2f;

        switch (curveDir) {
            case CurveDirection.Down:
                curve -= Time.deltaTime/2;
                if(curve <= minCurve) {
                    curveDir = CurveDirection.Up;
                }
                break;
            case CurveDirection.Up:
                curve += Time.deltaTime/2;
                if(curve >= maxCurve) {
                    curveDir = CurveDirection.Down;
                }
                break;
        }

        for (int i = 0; i < vineCount + 1; i++) { //Setting the position for the line points.
            int vinePositionCount = vines[i].positionCount;
            
            for (int j = 0; j < vinePositionCount; j++) {
                switch (Game.currentDirection) {
                    case Game.CurrentDirection.Down:
                        vines[i].SetPosition(j, new Vector3(0, startYPos - ((yMult*j)*2), 0));
                        break;
                    case Game.CurrentDirection.Up:
                        vines[i].SetPosition(j, new Vector3(0, startYPos - ((yMult*j)*2), 0));
                        break;
                    case Game.CurrentDirection.Left:
                        vines[i].SetPosition(j, new Vector3(-(windMult*j*(j*curve)), startYPos - (yMult*j), 0));
                        break;
                    case Game.CurrentDirection.Right:
                        vines[i].SetPosition(j, new Vector3(windMult*j*(j*curve), startYPos - (yMult*j), 0));
                        break;
                }   
            }
        }
    }
}