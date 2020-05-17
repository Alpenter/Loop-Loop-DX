using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CrystalTileSetterScript : MonoBehaviour {

	readonly LayerMask wallLayer = 1024; //The layer of the walls.

	public void SetBlocks(){
		//Find every block that is my child.
		List<GameObject> blocks = new List<GameObject>();
        foreach (Transform t in gameObject.transform) {
            blocks.Add(t.gameObject);
        }

		//This for loop does what we need to do to each detail block.
		for (int i = 0; i < blocks.Count; i++) {
			float size = 2.5f;
			
			Vector2 pos = new Vector2(blocks[i].transform.position.x, blocks[i].transform.position.y);

			bool n = Physics2D.OverlapPoint(pos + new Vector2(0f, size), wallLayer);
			bool ne = Physics2D.OverlapPoint(pos + new Vector2(size, size), wallLayer);
			bool e = Physics2D.OverlapPoint(pos + new Vector2(size, 0f), wallLayer);
			bool se = Physics2D.OverlapPoint(pos + new Vector2(size, -size), wallLayer);
			bool s = Physics2D.OverlapPoint(pos + new Vector2(0f, -size), wallLayer);
			bool sw = Physics2D.OverlapPoint(pos + new Vector2(-size, -size), wallLayer);
			bool w = Physics2D.OverlapPoint(pos + new Vector2(-size, 0f), wallLayer);
			bool nw = Physics2D.OverlapPoint(pos + new Vector2(-size, size), wallLayer);

			Vector2 vecToBe = GetVector(n, ne, e, se, s, sw, w, nw);

			if(blocks[i].GetComponent<CrystalTileScript>() != null){
				Debug.Log("Set Vector: " + vecToBe.ToString());
				blocks[i].GetComponent<CrystalTileScript>().offSetToBe = vecToBe;
			} else {
				Debug.Log("Something in me does not have a 'CrystalTileScript'!");
			}
		}
		Debug.Log("Walls found: " + blocks.Count.ToString());
	}

	private Vector2 GetVector(bool n, bool ne, bool e, bool se, bool s, bool sw, bool w, bool nw){
		Vector2 v = Vector2.zero;

		if(!n && !s && !w && !e){ //Single tile.
			v = new Vector2(0f, 0.9f);
		} else if(n && ne && e && se && s && sw && w && nw){ //Completely surrounded.
			v = new Vector2(0.1f, 0.8f);
		} else if(n && !e && !s && !w){ //U-tile.
			v = new Vector2(0.2f, 0.9f);
		} else if(!n && !e && s && !w){ //n-tile.
			v = new Vector2(0.4f, 0.9f);
		} else if(!n && e && !s && !w){ //C-tile.
			v = new Vector2(0.6f, 0.9f);
		} else if(!n && !e && !s && w){ //D-tile.
			v = new Vector2(0.8f, 0.9f);
		} else if(!n && e && !s && w){ //Thin Horizontal.
			v = new Vector2(0.3f, 0.8f);
		} else if(n && !e && s && !w){ //Thin Vertical.
			v = new Vector2(0.5f, 0.8f);
		} else if(!n && e && !se && s && !w){ //Thin top-left corner.
			v = new Vector2(0f, 0.7f);
		} else if(!n && !e && s && !sw && w){ //Thin top-right corner.
			v = new Vector2(0.2f, 0.7f); 
		} else if(n && !ne && e && !s && !w){ //Thin bottom-left corner.
			v = new Vector2(0.4f, 0.7f); 
		} else if(n && !e && !s && w && !nw){ //Thin bottom-right corner.
			v = new Vector2(0.6f, 0.7f); 
		} else if(!n && e && se && s && !w){ //Top right corner.
			v = new Vector2(0.1f, 0.6f);
		} else if(!n && !e && s && sw && w){ //Top left corner.
			v = new Vector2(0.3f, 0.6f);
		} else if(n && e && ne && !w && !s){ //Bottom right corner.
			v = new Vector2(0.5f, 0.6f);
		} else if(n && w && !s && !e && nw){ //Bottom left corner.
			v = new Vector2(0.7f, 0.6f);
		} else if(!n && w && e && s && se && sw){ //Top Wall.
			v = new Vector2(0f, 0.5f);
		} else if(n && w && !e && s && nw && sw){ //Right Wall.
			v = new Vector2(0.2f, 0.5f);
		} else if(n && !w && e && s && ne && se){ //Left Wall.
			v = new Vector2(0.4f, 0.5f);
		} else if(n && w && e && !s && ne && nw){ //Bottom Wall.
			v = new Vector2(0.6f, 0.5f);
		} else if(n && ne && e && se && s && !sw && w && nw){ //Bottom left inner corner.
			v = new Vector2(0.9f, 0.8f);
		} else if(n && ne && e && !se && s && sw && w && nw){ //Bottom right inner corner.
			v = new Vector2(0.9f, 0.6f);
		} else if(n && !ne && e && se && s && sw && w && nw){ //Top right inner corner.
			v = new Vector2(0.9f, 0.4f);
		} else if(n && ne && e && se && s && sw && w && !nw){ //Top left inner corner.
			v = new Vector2(0.8f, 0.3f);
		} else if(!n && w && e && s && !sw && !se){ //North T-Tile.
			v = new Vector2(0.1f, 0.4f);
		} else if(n && w && !e && s && !nw && !sw){ //East T-Tile.
			v = new Vector2(0.3f, 0.4f);
		} else if(!s && !ne && !nw && n && w && e){ //South T-Tile.
			v = new Vector2(0.5f, 0.4f);
		} else if(!w && !ne && !se && n && s && e){ //West T-Tile.
			v = new Vector2(0.7f, 0.4f);
		} else if(n && w && e && s && !ne && !se && !nw && !sw){ //4-inner corner.
			v = new Vector2(0f, 0.1f);
		} else if(n && w && e && s && !ne && se && !nw && !sw){ //3-inner corner filled SE.
			v = new Vector2(0.2f, 0.1f);
		} else if(n && w && e && s && !ne && !se && !nw && sw){ //3-inner corner filled SW.
			v = new Vector2(0.4f, 0.1f);
		} else if(n && w && e && s && !ne && !se && nw && !sw){ //3-inner corner filled NW.
			v = new Vector2(0.6f, 0.1f);
		} else if(n && w && e && s && ne && !se && !nw && !sw){ //3-inner corner filled NE.
			v = new Vector2(0.8f, 0.1f);
		} else if(n && w && e && s && ne && !se && !nw && sw){ //2-inner corner mirrored.
			v = new Vector2(0.1f, 0f);
		} else if(n && w && e && s && !ne && se && nw && !sw){ //2- inner corner.
			v = new Vector2(0.3f, 0f);
		} else if(n && w && e && s && !ne && !se && nw && sw){ //2-inner corner east.
			v = new Vector2(0.9f, 0.2f);
		} else if(n && w && e && s && ne && !se && nw && !sw){ //2-inner corner south.
			v = new Vector2(0.5f, 0f);
		} else if(n && w && e && s && ne && se && !nw && !sw){ //2-inner corner west.
			v = new Vector2(0.7f, 0f);
		} else if(n && w && e && s && !ne && se && !nw && sw){ //2-inner corner north.
			v = new Vector2(0.9f, 0f);
		}
		return v;
	}
}
