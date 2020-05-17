using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

	public float rotationSpeed;
	//float storedSpeed;
	public bool rotateX;
	public bool rotateY;
	public bool rotateZ;


	void Awake ()
	{
		//storedSpeed = rotationSpeed;
	}
	
	
	void FixedUpdate()
	{
		if(Time.deltaTime != 0){
			if (rotateX == true)
			{
				transform.Rotate(Vector3.left * rotationSpeed, Space.World);
			}
			
			
			if (rotateY == true)
			{
				transform.Rotate(Vector3.up * rotationSpeed, Space.World);
			}
			
			
			if (rotateZ == true)
			{
				transform.Rotate(Vector3.forward * rotationSpeed, Space.World);
			}
		}
	}
}
