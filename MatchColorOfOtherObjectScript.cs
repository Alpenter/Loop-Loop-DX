using UnityEngine;

public class MatchColorOfOtherObjectScript : MonoBehaviour {
    public GameObject objectToMatch = null;
    private void Update() {
        GetComponent<MeshRenderer>().material.color = objectToMatch.GetComponent<MeshRenderer>().material.color;
    }
}
