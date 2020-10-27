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

    public Renderer renderer;

    // Use this for initialization- needs to be called manually from base class's "Start" function
    protected void StartUp () {
        rb = this.gameObject.GetComponent<Rigidbody>();
        renderer = this.gameObject.GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //standard method to move forward some amount and to turn some amount
    public void Move(float forwardSpeed, float rotateSpeed = 0)
    {
        //move forward
       // if (rb == null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
            //transform.position = Vector3.MoveTowards(transform.position, transform.forward * 1000, forwardSpeed * Time.deltaTime);
        }
        //else
        {
           // rb.AddForce(Vector3.forward * Time.deltaTime * forwardSpeed);
        }

        //turn right
        if (rotateSpeed != 0)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
    }

    //move forward and up
    public void Leap(float leapSpeed, float leapHeight)
    {
        transform.Translate(Vector3.forward * Time.deltaTime * leapSpeed);
        transform.Translate(Vector3.up * Time.deltaTime * leapSpeed);
    }

    //move randomly in 2d space
    public void Wander(float distance, float minDistance, float runSpeed, float rotateSpeed, float changeTime = 14)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed); //rotate speed must be positive
        //choose a random target position within range and move towards it
        //https://answers.unity.com/questions/23010/ai-wandering-script.html
        timeChasing += Time.deltaTime;

        //change target position once within a certain range or after chasing it for a period of time
        if (targetPos == Vector3.zero || (transform.position - targetPos).magnitude < minDistance || timeChasing > changeTime)
        {
            targetPos = transform.position + transform.forward * (distance / 2.0f) + Random.insideUnitSphere * distance;
            targetPos.y = transform.position.y;
            timeChasing = 0;
        }

        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);

        Debug.DrawLine(transform.position, targetPos, Color.cyan);

        //transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);
        //update: handle forward movement separate from deciding direction with move() in child script

    }

    //move in the opposite direction of a given target
    public void Flee(Transform fleeFromTarget, float forwardSpeed, float rotateSpeed)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed);
        if (fleeFromTarget == null) { return; }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (transform.position - fleeFromTarget.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        //Move(forwardSpeed, 0);
        //update: handle forward movement separate from deciding direction with move() in child script
    }

    //move in the direction of a given target (transform)
    public void MoveToward(Transform target, float forwardSpeed, float rotateSpeed)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed);
        MoveToward(target.position, forwardSpeed, rotateSpeed);
    }

    //move in the direction of a given target (vector3)
    public void MoveToward(Vector3 target, float forwardSpeed, float rotateSpeed)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed);
        //vector3.zero is used in place of a null value
        if (target == Vector3.zero) { return; }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (target - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        //Move(forwardSpeed, 0);
        //update: handle forward movement separate from deciding direction with move() in child script
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

    //will need to be handled at a lower level - should return true when specific cryptids are doing an interesting animation
    public virtual bool SpecialPose()
    {
        return false;
    }

    //should be called after doing all other movement calculations- check for object in front of cryptid and rotate if something is found
    public bool AvoidObstacles(float aheadDistance, float rotateSpeed)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        RaycastHit leftHit;
        RaycastHit rightHit;

        //double rotate speed so that obstacle avoidance overpowers other rotations
        rotateSpeed = Mathf.Abs( rotateSpeed);

        //return a bool representing whether avoidance needs to happen
        bool obstacleAhead = false;

        //check 45 degrees to left and right to determine which way to turn
        //not considered clear of obstacles unless the cone is clear
        //45 degrees = pi/4 radians
        Vector3 forwardLeft = Vector3.RotateTowards(transform.forward, transform.right * -1, Mathf.PI / 4, 0);
        Vector3 forwardRight = Vector3.RotateTowards(transform.forward, transform.right, Mathf.PI / 4, 0);

        //if (renderer == null) { renderer = this.gameObject.GetComponentInChildren<Renderer>(); }
        float cryptidWidth = renderer.bounds.size.x / 2.0f;

        //Ray rightForward = new Ray(transform.position + (transform.right * cryptidWidth), transform.forward);
        //Ray leftForward = new Ray(transform.position - (transform.right * cryptidWidth), transform.forward);

        //determine which way to turn
        bool turnRight = false;
        bool turnLeft = false;

        //check for obstacle directly in front
        if (Physics.Raycast(ray, out hit, aheadDistance) && hit.collider.tag != "lvl")
        {
            obstacleAhead = true;
        }
        //check 45 degrees to the left
        if (Physics.Raycast(new Ray(transform.position, forwardLeft), out leftHit, aheadDistance) && leftHit.collider.tag != "lvl")
        {
            obstacleAhead = true;
            turnRight = true;
        }
        //check 45 degrees to the right
        if (Physics.Raycast(new Ray(transform.position, forwardRight), out rightHit, aheadDistance) && rightHit.collider.tag != "lvl")
        {
            obstacleAhead = true;
            turnLeft = true;
        }

        if (obstacleAhead)
        {
            Debug.DrawRay(transform.position, (transform.forward * aheadDistance), Color.magenta);
            //if there's an obstacle on both sides, determine which way to go based on what's closer
            if (turnRight && turnLeft)
            {
                float rightDist = (transform.position - rightHit.transform.position).magnitude;
                float leftDist = (transform.position - leftHit.transform.position).magnitude;
                if (rightDist > leftDist)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardLeft, rotateSpeed * Time.deltaTime, 0);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                else
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardRight, rotateSpeed * Time.deltaTime, 0);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }

                Debug.DrawRay(transform.position, forwardLeft * aheadDistance, Color.green);
                Debug.DrawRay(transform.position, forwardRight * aheadDistance, Color.red);
            }
            //turn according to what obstacles we found
            else if (turnRight)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardRight, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);

                Debug.DrawRay(transform.position, forwardLeft * aheadDistance, Color.green);
            }
            else if (turnLeft)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardLeft, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);

                Debug.DrawRay(transform.position, forwardRight * aheadDistance, Color.red);
            }
            //default to turn right 
            else
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardRight, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }

        //debug
        //Debug.DrawRay(transform.position, (transform.forward * aheadDistance), Color.magenta);
        //Debug.DrawRay(leftForward.origin, leftForward.direction * aheadDistance, Color.green);
        //Debug.DrawRay(rightForward.origin, rightForward.direction * aheadDistance, Color.red);
        //Debug.DrawRay(transform.position, forwardLeft * aheadDistance, Color.green);
        //Debug.DrawRay(transform.position, forwardRight * aheadDistance, Color.red);

        return obstacleAhead;
    }

}
