using UnityEngine;

public class BackgroundBubblesScript : MonoBehaviour {

    public enum BubbleDirection { Up, Down, Left, Right }
    public BubbleDirection direction;
    bool playBubbles = false;
    readonly Vector3 awayPos = new Vector3(100, 100, 100);
    Vector3 properPos;
    

    private void Update() {
        Vector3 pos = transform.position;
        
        switch (direction) {
            case BubbleDirection.Up:
                playBubbles = (Game.currentDirection == Game.CurrentDirection.Up);
                properPos = new Vector3(0, -4, -3);
                break;
            case BubbleDirection.Down:
                playBubbles = (Game.currentDirection == Game.CurrentDirection.Down);
                properPos = new Vector3(0, -4, 3);
                break;
            case BubbleDirection.Left:
                playBubbles = (Game.currentDirection == Game.CurrentDirection.Left);
                properPos = new Vector3(5.5f, -4, 0);
                break;
            case BubbleDirection.Right:
                playBubbles = (Game.currentDirection == Game.CurrentDirection.Right);
                properPos = new Vector3(-5.5f, -4, 0);
                break;
        }

        if (playBubbles) { pos = properPos; }
        else { pos = awayPos; }

        transform.position = pos;
    }
}
