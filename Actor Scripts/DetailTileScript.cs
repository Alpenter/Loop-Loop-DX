using UnityEngine;

public class DetailTileScript : MonoBehaviour {

    int randomFrameIndex = 0;
    GameObject puppet = null;
    Vector2[] framePool = new Vector2[20];

    Vector3 scale = Vector3.one;
    Quaternion rot = Quaternion.identity;
    LayerMask wallLayer = 1024;

    private void Awake() { puppet = transform.Find("puppet").gameObject; }
    
    private void Start() {
        //Get type of tile.
        Vector3 pos = transform.position;
        Transform nt = transform.Find("n");
        Transform st = transform.Find("s");
        Transform wt = transform.Find("w");
        Transform et = transform.Find("e");
        bool n = Physics2D.OverlapPoint(new Vector2(nt.position.x, nt.position.y), wallLayer);
        bool s = Physics2D.OverlapPoint(new Vector2(st.position.x, st.position.y), wallLayer);
        bool w = Physics2D.OverlapPoint(new Vector2(wt.position.x, wt.position.y), wallLayer);
        bool e = Physics2D.OverlapPoint(new Vector2(et.position.x, et.position.y), wallLayer);

        //Set tile properties.
        if(n && s && w && e) {//All 4 sides.
            scale = Vector3.one;
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.2f);
            framePool[1] = new Vector2(0.2f, 0.2f);
            framePool[2] = new Vector2(0.4f, 0.2f);
            framePool[3] = new Vector2(0.6f, 0.2f);
            framePool[4] = new Vector2(0.8f, 0.2f);
            randomFrameIndex = Random.Range(0, 5);
        } else if (!n && !s && w && e) { //Two-Sided (Horizontal)
            scale = Vector3.one;
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0f);
            framePool[1] = new Vector2(0.2f, 0f);
            framePool[2] = new Vector2(0.4f, 0f);
            framePool[3] = new Vector2(0.6f, 0f);
            randomFrameIndex = Random.Range(0, 4);
        } else if(n && s && !w && !e) { //Two-sided (Vertical)
            scale = Vector3.one;
            rot = Quaternion.Euler(0f, 0f, 90f);
            framePool[0] = new Vector2(0.2f, 0f);
            framePool[1] = new Vector2(0.6f, 0f);
            randomFrameIndex = Random.Range(0, 2);
        } else if (!n && s && w && e) { //3-Sided (North Clear).
            scale = Vector3.one;
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.4f);
            framePool[1] = new Vector2(0.2f, 0.4f);
            framePool[2] = new Vector2(0.4f, 0.4f);
            framePool[3] = new Vector2(0.6f, 0.4f);
            framePool[4] = new Vector2(0.8f, 0.4f);
            randomFrameIndex = Random.Range(0, 5);
        } else if (n && !s && w && e) { //3-Sided (South Clear).
            scale = new Vector3(1f, -1f, 1f);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.4f);
            framePool[1] = new Vector2(0.2f, 0.4f);
            framePool[2] = new Vector2(0.4f, 0.4f);
            framePool[3] = new Vector2(0.6f, 0.4f);
            framePool[4] = new Vector2(0.8f, 0.4f);
            randomFrameIndex = Random.Range(0, 5);
        } else if (n && s && !w && e) { //3-Sided (West Clear).
            scale = Vector3.one;
            rot = Quaternion.Euler(0f, 0f, 90f);
            framePool[0] = new Vector2(0f, 0.4f);
            framePool[1] = new Vector2(0.2f, 0.4f);
            framePool[2] = new Vector2(0.4f, 0.4f);
            framePool[3] = new Vector2(0.6f, 0.4f);
            framePool[4] = new Vector2(0.8f, 0.4f);
            randomFrameIndex = Random.Range(0, 5);
        } else if(n && s && w && !e) { //3-Sided (East Clear).
            scale = Vector3.one;
            rot = Quaternion.Euler(0f, 0f, 270f);
            framePool[0] = new Vector2(0f, 0.4f);
            framePool[1] = new Vector2(0.2f, 0.4f);
            framePool[2] = new Vector2(0.4f, 0.4f);
            framePool[3] = new Vector2(0.6f, 0.4f);
            framePool[4] = new Vector2(0.8f, 0.4f);
            randomFrameIndex = Random.Range(0, 5);
        } else if (n && !s && !w && !e) { //1 - Sided (North).
            scale = Vector3.one;
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.5f);
            framePool[1] = new Vector2(0.1f, 0.5f);
            framePool[2] = new Vector2(0.2f, 0.5f);
            framePool[3] = new Vector2(0.3f, 0.5f);
            framePool[4] = new Vector2(0.4f, 0.5f);
            framePool[5] = new Vector2(0.5f, 0.5f);
            framePool[6] = new Vector2(0.6f, 0.5f);
            framePool[7] = new Vector2(0.7f, 0.5f);
            framePool[8] = new Vector2(0.8f, 0.5f);
            framePool[9] = new Vector2(0.9f, 0.5f);
            framePool[10] = new Vector2(0f, 0.7f);
            framePool[11] = new Vector2(0.1f, 0.7f);
            framePool[12] = new Vector2(0.2f, 0.7f);
            framePool[13] = new Vector2(0.3f, 0.7f);
            framePool[14] = new Vector2(0.4f, 0.7f);
            framePool[15] = new Vector2(0.5f, 0.7f);
            framePool[16] = new Vector2(0.6f, 0.7f);
            framePool[17] = new Vector2(0.7f, 0.7f);
            framePool[18] = new Vector2(0.8f, 0.7f);
            framePool[19] = new Vector2(0.9f, 0.7f);
            randomFrameIndex = Random.Range(0, 20);
        } else if (!n && s && !w && !e) { //1 - Sided (South).
            scale = new Vector3(1, -1, 1);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0.1f, 0.5f);
            framePool[1] = new Vector2(0.2f, 0.5f);
            framePool[2] = new Vector2(0.8f, 0.5f);
            framePool[3] = new Vector2(0.9f, 0.5f);
            framePool[4] = new Vector2(0.1f, 0.7f);
            framePool[5] = new Vector2(0.2f, 0.7f);
            framePool[6] = new Vector2(0.3f, 0.7f);
            framePool[7] = new Vector2(0.4f, 0.7f);
            framePool[8] = new Vector2(0.6f, 0.7f);
            framePool[9] = new Vector2(0.7f, 0.7f);
            randomFrameIndex = Random.Range(0, 10);
        } else if (!n && !s && w && !e) { //1 - Sided (West).
            scale = Vector3.one;
            rot = Quaternion.Euler(0f, 0f, 90f);
            framePool[0] = new Vector2(0.1f, 0.5f);
            framePool[1] = new Vector2(0.2f, 0.5f);
            framePool[2] = new Vector2(0.8f, 0.5f);
            framePool[3] = new Vector2(0.9f, 0.5f);
            framePool[4] = new Vector2(0.1f, 0.7f);
            framePool[5] = new Vector2(0.2f, 0.7f);
            framePool[6] = new Vector2(0.3f, 0.7f);
            framePool[7] = new Vector2(0.4f, 0.7f);
            framePool[8] = new Vector2(0.6f, 0.7f);
            framePool[9] = new Vector2(0.7f, 0.7f);
            randomFrameIndex = Random.Range(0, 10);
        } else if (!n && !s && !w && e) { //1 - Sided (East).
            scale = Vector3.one;
            rot = Quaternion.Euler(0f, 0f, 270f);
            framePool[0] = new Vector2(0.1f, 0.5f);
            framePool[1] = new Vector2(0.2f, 0.5f);
            framePool[2] = new Vector2(0.8f, 0.5f);
            framePool[3] = new Vector2(0.9f, 0.5f);
            framePool[4] = new Vector2(0.1f, 0.7f);
            framePool[5] = new Vector2(0.2f, 0.7f);
            framePool[6] = new Vector2(0.3f, 0.7f);
            framePool[7] = new Vector2(0.4f, 0.7f);
            framePool[8] = new Vector2(0.6f, 0.7f);
            framePool[9] = new Vector2(0.7f, 0.7f);
            randomFrameIndex = Random.Range(0, 10);
        } else if(n && w && !e && !s){ //Corner (North-West)
            scale = new Vector3(-1, 1, 1);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.9f);
            framePool[1] = new Vector2(0.2f, 0.9f);
            framePool[2] = new Vector2(0.4f, 0.9f);
            framePool[3] = new Vector2(0.6f, 0.9f);
            framePool[4] = new Vector2(0.8f, 0.9f);
            randomFrameIndex = Random.Range(0, 5);
        } else if(n && !w && e && !s) { //Corner (North-East)
            scale = new Vector3(1, 1, 1);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.9f);
            framePool[1] = new Vector2(0.2f, 0.9f);
            framePool[2] = new Vector2(0.4f, 0.9f);
            framePool[3] = new Vector2(0.6f, 0.9f);
            framePool[4] = new Vector2(0.8f, 0.9f);
            randomFrameIndex = Random.Range(0, 5);
        } else if(!n && w && !e && s) { //Corner (South-West)
            scale = new Vector3(-1, -1, 1);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.9f);
            framePool[1] = new Vector2(0.4f, 0.9f);
            framePool[2] = new Vector2(0.6f, 0.9f);
            framePool[3] = new Vector2(0.8f, 0.9f);
            randomFrameIndex = Random.Range(0, 4);
        } else if(!n && !w && e && s) { //Corner (South-East)
            scale = new Vector3(1, -1, 1);
            rot = Quaternion.identity;
            framePool[0] = new Vector2(0f, 0.9f);
            framePool[1] = new Vector2(0.4f, 0.9f);
            framePool[2] = new Vector2(0.6f, 0.9f);
            framePool[3] = new Vector2(0.8f, 0.9f);
            randomFrameIndex = Random.Range(0, 4);
        } else {
            puppet.SetActive(false);
        }

        //Applying determined settings.
        puppet.transform.localScale = scale*3.01f;
        puppet.transform.localRotation = rot;
        puppet.GetComponent<MeshRenderer>().material.mainTextureOffset = framePool[randomFrameIndex];
        puppet.GetComponent<MeshRenderer>().material.color = Game.frontColor;
    }
}
