using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cryptid : MonoBehaviour {

    //thins all cryptids need:
    //base score for photoraphin
    [HideInInspector]
    public int baseScore;

    //a name for scorin pruposes(?)
    [HideInInspector]
    public string cryptidType;

    //wandering variables
    protected Vector3 targetPos;
    protected float timeChasing;

    //quick access
    protected Rigidbody rb;

    [HideInInspector]
    new public Renderer renderer;

    protected Animator animator;

    //allows base class to override child class and stop movement
    protected bool lockMovementSuper = false;

    //disappear when touched
    AudioSource disappearSFX;
    [SerializeField] GameObject particles;

    // Use this for initialization- needs to be called manually from base class's "Start" function
    protected void StartUp () {
        rb = this.gameObject.GetComponent<Rigidbody>();
        renderer = this.gameObject.GetComponentInChildren<Renderer>();
        animator = GetComponent<Animator>();
        GameObject fleeOBJ = GameObject.Find("FleeSFX");
        if (fleeOBJ != null) {
            disappearSFX = fleeOBJ.GetComponent<AudioSource>();
        }
	}
	
	// Update is called once per frame
    protected virtual void Update () {
        //don't move while getting bonked
		if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("bonk"))
        {
            lockMovementSuper = true;
        }
        else { lockMovementSuper = false; }
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

        //make y not a factor so cryptids dont rotate down to get away from player
        Vector3 fleeFromTargetPos = fleeFromTarget.position;
        fleeFromTargetPos.y = transform.position.y;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, (transform.position - fleeFromTargetPos), rotateSpeed * Time.deltaTime, 0);
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
        transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        //Move(forwardSpeed, 0);
        //update: handle forward movement separate from deciding direction with move() in child script
    }

    public void MoveTowardXZOnly(Vector3 target, float forwardSpeed, float rotateSpeed)
    {
        target.y = transform.position.y;
        MoveToward(target, forwardSpeed, rotateSpeed);
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

    //option to override visibility calculations
    public virtual bool IsVisible()
    {
        return true;
    }

    //should be called after doing all other movement calculations- check for object in front of cryptid and rotate if something is found
    public bool AvoidObstacles(float aheadDistance, float rotateSpeed, bool checkHeadLevel = false)
    {
        //check for obstacles at the level of the cryptids head instead of the ground-
        //useful for cyrptids like frogman who might not fit under the foliage of trees
        Vector3 position = transform.position;
        if (checkHeadLevel)
        {
            float height = renderer.bounds.size.y / 2;
            position = new Vector3(position.x, position.y + height, position.z);
        }

        Ray ray = new Ray(position, transform.forward);
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
        //float cryptidWidth = renderer.bounds.size.x / 2.0f;

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
        if (Physics.Raycast(new Ray(position, forwardLeft), out leftHit, aheadDistance) && leftHit.collider.tag != "lvl")
        {
            obstacleAhead = true;
            turnRight = true;
        }
        //check 45 degrees to the right
        if (Physics.Raycast(new Ray(position, forwardRight), out rightHit, aheadDistance) && rightHit.collider.tag != "lvl")
        {
            obstacleAhead = true;
            turnLeft = true;
        }

        if (obstacleAhead)
        {
           // Debug.DrawRay(position, (transform.forward * aheadDistance), Color.magenta);
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

                //Debug.DrawRay(position, forwardLeft * aheadDistance, Color.green);
                //Debug.DrawRay(position, forwardRight * aheadDistance, Color.red);
            }
            //turn according to what obstacles we found
            else if (turnRight)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardRight, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);

                //Debug.DrawRay(transform.position, forwardLeft * aheadDistance, Color.green);
            }
            else if (turnLeft)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardLeft, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);

                //Debug.DrawRay(position, forwardRight * aheadDistance, Color.red);
            }
            //default to turn right 
            else
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, forwardRight, rotateSpeed * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }

        //debug
        Debug.DrawRay(position, (transform.forward * aheadDistance), Color.magenta);
        //Debug.DrawRay(leftForward.origin, leftForward.direction * aheadDistance, Color.green);
        //Debug.DrawRay(rightForward.origin, rightForward.direction * aheadDistance, Color.red);
        Debug.DrawRay(position, forwardLeft * aheadDistance, Color.green);
        Debug.DrawRay(position, forwardRight * aheadDistance, Color.red);

        return obstacleAhead;
    }

    //cryptids disappear when touched by player
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DestroyZone")
        {
            if (particles != null && disappearSFX != null)
            {
                GameObject newParticles = GameObject.Instantiate(particles, this.transform.position, particles.transform.rotation);
                newParticles.transform.localScale = this.transform.localScale * 2;
                newParticles.transform.Translate(0, 1, 0);//move it up a lil
                disappearSFX.enabled = true;
                disappearSFX.Play();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BonkableObject bonked = collision.gameObject.GetComponent<BonkableObject>();
        if (bonked != null && bonked.CanDoBonk())
        {
            //determine which direction the bonk is coming from

            //line from cryptid to carrot
            Vector3 bonkDistance = this.gameObject.transform.position - collision.gameObject.transform.position;

            //if the line from the cryptid to the carrot is in the same direction as the cryptid's right vector,
            //then the carrot is on the cryptid's right
            bool leftImpact = true;
            if (Vector3.Dot(this.transform.right, bonkDistance) < 0)
            {
                leftImpact = false;
            }
            GetBonked(leftImpact);
        }
    }

    public virtual void GetBonked(bool leftImpact)
    {

        if (leftImpact && animator.HasState(0, Animator.StringToHash("bonk_left")))
        {
            animator.Play("bonk_left");
        }
        else if (!leftImpact && animator.HasState(0, Animator.StringToHash("bonk_right")))
        {
            animator.Play("bonk_right");
        }
    }

    //used for behaviors that have a random chance of changing
    //if checking in update make sure to use VERY low chance values (ie less than 1)
    //there must be a better way to do this
    protected bool RandomChance(float percentChanceofSuccess)
    {
        //returns true or false, based on parameter for success
        //percent chance of success should be a float between 0 and 100 
        if (percentChanceofSuccess >= 100)
        {
            return true;
        }

        if (percentChanceofSuccess <= 0)
        {
            return false;
        }

        float r = Random.Range(0.0f, 100.0f);
        if (r > percentChanceofSuccess)
        {
            return false;
        }

        return true;
    }

}
