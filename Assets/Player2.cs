using UnityEngine;
using System.Collections;

public class Player2 : MonoBehaviour {

    //Vector3 cubesize;
    float step = 4f;
    float moveSpeed = 4f;

    enum States { idle, moving };

    States state;
    float percentMoved;

	// Use this for initialization
	void Start () {

        //cubesize = renderer.bounds.size;
        state = States.idle;

	}
    Quaternion newRotation = Quaternion.EulerAngles(0, 0, 0);
	// Update is called once per frame
	void Update () {
        //float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float minAngle = 0.0F;
        float maxAngle = 90.0F;
       // Debug.Log(transform.eulerAngles);


        if (state == States.idle) //x != 0 ||
        {
            
        }
        else if (state == States.moving)
        {
            if (percentMoved > 100f)
            {
                transform.rotation = newRotation;
                state = States.idle;
            }
            else
            {
                percentMoved += moveSpeed;// *Time.deltaTime;
                //Debug.Log(percentMoved);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, percentMoved / 100f);
            }
        }
        
	}
}
