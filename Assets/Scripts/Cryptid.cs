using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cryptid : MonoBehaviour {

    //thins all cryptids need:
    //base score for photoraphin
    public int baseScore;
    //multiple hitboxes to check visibility
    //variable to keep track of current animation, or at least whether or not its special
    //a name for scorin pruposes(?)
    public string cryptidType;

    //wandering variables
    protected Vector3 targetPos;
    protected float timeChasing;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //standard method to move forward some amount and to turn some amount
    public void Move(float forwardSpeed, float rotateSpeed)
    {
        //move forward
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);

        //turn right
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }

    //move randomly in 2d space
    public void Wander(float distance, float minDistance, float runSpeed, float rotateSpeed)
    {
        //choose a random target position within range and move towards it
        //https://answers.unity.com/questions/23010/ai-wandering-script.html

        //change target position once within a certain range or after chasing it for a period of time
        if (targetPos == Vector3.zero || (transform.position - targetPos).magnitude < minDistance || timeChasing > 14)
        {
            targetPos = transform.position + transform.forward * (distance / 2.0f) + Random.insideUnitSphere * distance;
            targetPos.y = transform.position.y;
            timeChasing = 0;
        }

        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);

    }

    //move in the opposite direction of a given target
    public void Flee(Transform fleeFromTarget, float forwardSpeed, float rotateSpeed)
    {
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (transform.position - fleeFromTarget.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        Move(forwardSpeed, 0);
    }
}
