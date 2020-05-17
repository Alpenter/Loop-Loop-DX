using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCurrentScript : MonoBehaviour {

    public enum CurrentDirection { Left, Right, Up, Down }
    public CurrentDirection direction = CurrentDirection.Left;
    
    readonly float baseStrength = 1.5f;
    
    Vector2 forceToApply = Vector2.zero;

    ScrollMatOffsetScript scrollScript = null;

    Quaternion rotation = Quaternion.identity;

    private void Start() {
        scrollScript = gameObject.AddComponent<ScrollMatOffsetScript>();
        scrollScript.isNormalMap = true;
        scrollScript.slowdown = 1f;
        scrollScript.howToScroll = ScrollMatOffsetScript.scrollType.scrollX;

        switch (direction) {
            case CurrentDirection.Left:
                scrollScript.reverseDirection = false;
                rotation = Quaternion.identity;
                forceToApply = new Vector2(-baseStrength, 0f);
                Game.currentDirection = Game.CurrentDirection.Left;
                break;
            case CurrentDirection.Right:
                scrollScript.reverseDirection = true;
                rotation = Quaternion.identity;
                forceToApply = new Vector2(baseStrength, 0f);
                Game.currentDirection = Game.CurrentDirection.Right;
                break;
            case CurrentDirection.Up:
                scrollScript.reverseDirection = true;
                rotation = Quaternion.Euler(0f, 0f, 90f);
                forceToApply = new Vector2(0f, baseStrength);
                Game.currentDirection = Game.CurrentDirection.Up;
                break;
            case CurrentDirection.Down:
                scrollScript.reverseDirection = false;
                rotation = Quaternion.Euler(0f, 0f, 90f);
                forceToApply = new Vector2(0f, -baseStrength);
                Game.currentDirection = Game.CurrentDirection.Down;
                break;
        }

        transform.localRotation = rotation;
    }

    
    private void FixedUpdate() {
		if(Game.playerObj != null){
			Game.playerObj.GetComponent<Rigidbody2D>().velocity += forceToApply;
		}
    }
}
