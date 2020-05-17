using UnityEngine;
using System.Collections;

public class MoveInShape : MonoBehaviour {

	public float moveSpeed;

	public bool useDelay;
	public float startDelayTime;
	float delay;

	public Vector3[] points;

	int currentPoint;
	int targetPoint;
	int zPos;

	void Awake()
	{
		currentPoint = 0;
		targetPoint = 1;
		if(useDelay){delay = startDelayTime;}
		else
		{delay = 0;}
	}

	void Update () 
	{
		if(delay >0){ delay -= Time.deltaTime; }
		else{
		transform.position = Vector3.MoveTowards(transform.position, points[targetPoint], moveSpeed*Time.deltaTime);

		if(transform.position == points[targetPoint])
			{
				if(targetPoint == points.Length - 1)
				{
					targetPoint = 0;
					currentPoint = 1;
				}else{
					targetPoint += 1;
					currentPoint += 1;
				}
			}
		}
	}
}
