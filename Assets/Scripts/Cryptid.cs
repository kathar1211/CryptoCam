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

    //quick access
    protected Rigidbody rb;

    // Use this for initialization- needs to be called manually from base class's "Start" function
    protected void StartUp () {
        rb = this.gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //standard method to move forward some amount and to turn some amount
    public void Move(float forwardSpeed, float rotateSpeed)
    {
        //move forward
       // if (rb == null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
        }
        //else
        {
           // rb.AddForce(Vector3.forward * Time.deltaTime * forwardSpeed);
        }

        //turn right
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }

    //move forward and up
    public void Leap(float leapSpeed, float leapHeight)
    {
        transform.Translate(Vector3.forward * Time.deltaTime * leapSpeed);
        transform.Translate(Vector3.up * Time.deltaTime * leapSpeed);
    }

    //move randomly in 2d space
    public void Wander(float distance, float minDistance, float runSpeed, float rotateSpeed)
    {
        //choose a random target position within range and move towards it
        //https://answers.unity.com/questions/23010/ai-wandering-script.html
        timeChasing += Time.deltaTime;

        //change target position once within a certain range or after chasing it for a period of time
        if (targetPos == Vector3.zero || (transform.position - targetPos).magnitude < minDistance || timeChasing > 14)
        {
            targetPos = transform.position + transform.forward * (distance / 2.0f) + Random.insideUnitSphere * distance;
            targetPos.y = transform.position.y;
            timeChasing = 0;
        }

        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);

    }

    //move in the opposite direction of a given target
    public void Flee(Transform fleeFromTarget, float forwardSpeed, float rotateSpeed)
    {
        if (fleeFromTarget == null) { return; }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (transform.position - fleeFromTarget.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        Move(forwardSpeed, 0);
    }

    //move in the direction of a given target (transform)
    public void MoveToward(Transform target, float forwardSpeed, float rotateSpeed)
    {
        MoveToward(target.position, forwardSpeed, rotateSpeed);
    }

    //move in the direction of a given target (vector3)
    public void MoveToward(Vector3 target, float forwardSpeed, float rotateSpeed)
    {
        if (target == null) { return; }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (target - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        Move(forwardSpeed, 0);
    }

    //move away from a given obstacle
    public virtual void AvoidCollision(Collider other, float avoidSpeed)
    {
        if (other == null) { return; }
        Vector3 objectToCryptid = other.transform.position - transform.position;
        Vector3 awayFromObject = other.transform.forward * objectToCryptid.magnitude;
        awayFromObject.y = 0;
        Vector3 turnAway = Vector3.RotateTowards(transform.forward, awayFromObject, Mathf.PI / 2, 0);
        /*Vector3 ninetyDegreeTurn = Quaternion.LookRotation(objectToCryptid) * new Vector3(0, 1, 0);
        ninetyDegreeTurn.x = ninetyDegreeTurn.z = 0;*/
        transform.rotation = Quaternion.LookRotation(turnAway);
        targetPos = Vector3.zero;
       
    }

    //method to deal with player entering certain trigger zones; implementation varies by cryptid
    public virtual void AvoidPlayer(Collider other)
    {
        return;
    }
}
