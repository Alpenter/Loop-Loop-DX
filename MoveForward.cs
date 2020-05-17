using UnityEngine;

public class MoveForward : MonoBehaviour {

    public float maxSpeed;
    public bool usesPhysics = false;

	public bool goHorizontal = true;
	public bool goVertical = false;

	public bool hasDelay	= false;
	public float delay		= 0f;
	float waitTime			= 0f;

	void Awake(){if(hasDelay){waitTime = delay;}}

    void Update()
    {
		if(waitTime <= 0){
	        if (usesPhysics == false)
	        {
				if (goHorizontal == true){
	            	Vector3 pos = transform.position;

	            	Vector3 velocity = new Vector3(maxSpeed * Time.deltaTime, 0, 0);

	           		pos += transform.rotation * velocity;

	            	transform.position = pos;
				}
				if (goVertical  == true){
					Vector3 pos = transform.position;
					
					Vector3 velocity = new Vector3(0, maxSpeed * Time.deltaTime, 0);
					
					pos += transform.rotation * velocity;
					
					transform.position = pos;
				}
			}
		}
		if(waitTime > 0){ waitTime -= Time.deltaTime;}
    }


    void FixedUpdate()
    {
		if(waitTime <= 0){
	        if (usesPhysics == true)
	        {
				if(goHorizontal == true){
	            	Vector3 pos = transform.position;

	            	Vector3 velocity = new Vector3(maxSpeed * Time.deltaTime, 0, 0);

	            	pos += transform.rotation * velocity;

	            	transform.position = pos;
				}

				if (goVertical  == true){
					Vector3 pos = transform.position;
					
					Vector3 velocity = new Vector3(0, maxSpeed * Time.deltaTime, 0);
					
					pos += transform.rotation * velocity;
					
					transform.position = pos;
				}
	        }
	    }
	}
}
