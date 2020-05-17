using UnityEngine;
using System.Collections;

public class ScrollMatOffsetScript : MonoBehaviour {

	public enum scrollType{
		scrollX,
		scrollY,
		scrollBoth,
	}

	public scrollType howToScroll;

	Material mat;

	Vector2 mOffsetVec;

	public float slowdown = 3f;
	float offsetX = 0f;
	float offsetY = 0f;
	
	public bool reverseDirection = false;
	public bool isNormalMap = false;

	// Use this for initialization
	void Start () {
		if(GetComponent<MeshRenderer>() != null){
			mOffsetVec = GetComponent<MeshRenderer>().material.mainTextureOffset;
			offsetX = mOffsetVec.x;
			offsetY = mOffsetVec.y;
			mat = GetComponent<MeshRenderer> ().material;
		} else if(GetComponent<LineRenderer>() != null){
			mOffsetVec = GetComponent<LineRenderer>().material.mainTextureOffset;
			offsetX = mOffsetVec.x;
			offsetY = mOffsetVec.y;
			mat = GetComponent<LineRenderer> ().material;	
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (howToScroll == scrollType.scrollX || howToScroll == scrollType.scrollBoth) {
			if(!reverseDirection){
				offsetX += Time.deltaTime / slowdown;
			} else {
				offsetX -= Time.deltaTime / slowdown;
			}
			if (offsetX >= 3 || offsetX <= -3) {
				offsetX = 0;
			}
		}
		
		if (howToScroll == scrollType.scrollY || howToScroll == scrollType.scrollBoth) {
			if(!reverseDirection){
				offsetY += Time.deltaTime / slowdown;
			} else {
				offsetY -= Time.deltaTime / slowdown;
			}
			if (offsetY >= 3 || offsetY <= -3) {
				offsetY = 0;
			}
		}
		

		if (howToScroll == scrollType.scrollX) {
			mOffsetVec = new Vector2 (offsetX, mOffsetVec.y);
		}

		if (howToScroll == scrollType.scrollY) {
			mOffsetVec = new Vector2 (mOffsetVec.x, offsetY);
		}

		if (howToScroll == scrollType.scrollBoth) {
			mOffsetVec = new Vector2 (offsetX, offsetY);
		}

		if (isNormalMap) {
			mat.SetTextureOffset("_BumpMap", mOffsetVec);
		} else {
			mat.mainTextureOffset = mOffsetVec;
		}
	}
}
