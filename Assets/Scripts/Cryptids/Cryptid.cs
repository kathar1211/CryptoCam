using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] GameObject particles;

    //used for determining if cryptid is visible/centered in shot
    public Transform CenterOfMass;

    //keep track of obstacles within our path
    private List<Collider> obstacles;
    public float obstacleAvoidanceInterval = 1; //time in seconds between doing checks for new obstacles
    private float timeOfLastObstacleCheck = -1;
    private Collider currentObstacle;

    //for cryptids that move along a path. make sure these are in order
    public CryptidPathPoint[] PathPoints;
    protected int pathIndex = 0;
    public float pathPointMinDist = 5;

    protected NavMeshAgent nav;
    private float movementTimer;
    private float wanderRepositionInterval = 10;
    private float fleeRepositionInterval = 1.5f;
    private float chaseRepositionInterval = .5f;

    // Use this for initialization- needs to be called manually from base class's "Start" function
    protected void StartUp () {
        rb = this.gameObject.GetComponent<Rigidbody>();
        nav = this.gameObject.GetComponent<NavMeshAgent>();
        renderer = this.gameObject.GetComponentInChildren<Renderer>();
        animator = GetComponent<Animator>();
        obstacles = new List<Collider>();
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
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);

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
    public void Wander(float distance, float minDistance, float runSpeed, float rotateSpeed, bool avoidObstacles = false)
    {
        Wander(distance, minDistance);

        //override navmesh movement to keep cryptids from sliding weird
        MoveManuallyAlongNavMeshPath(runSpeed, rotateSpeed, avoidObstacles);

    }

    public void Wander(float distance, float minDistance)
    {
        movementTimer += Time.deltaTime;
        UnKillNavMeshMovement();

        //change target position once within a certain range or after chasing it for a period of time
        if (targetPos == Vector3.zero || (transform.position - nav.destination).magnitude < minDistance || movementTimer > wanderRepositionInterval)
        {
            //docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
            //get a random position on the navmesh by sampling a few times at a small radius
            for (int i = 0; i < 10; i++)
            {
                targetPos = transform.position + transform.forward * (distance / 2.0f) + Random.insideUnitSphere * distance;
                targetPos.y = transform.position.y - (nav.baseOffset * transform.localScale.y);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPos, out hit, 2, nav.areaMask))
                {
                    movementTimer = 0;
                    nav.destination = hit.position;
                    break;
                }
            }

        }

        // ForceNavMeshToMoveForward();
        Debug.DrawLine(transform.position, targetPos, Color.cyan);
    }

    public void Flee(Transform fleeFromTarget, float minDistance)
    {
        movementTimer += Time.deltaTime;
        UnKillNavMeshMovement();

        if (movementTimer > fleeRepositionInterval || (transform.position - nav.destination).magnitude < minDistance)
        {
            SetNavmeshFleeTarget(fleeFromTarget);
        }

        // ForceNavMeshToMoveForward();
        Debug.DrawLine(transform.position, nav.destination, Color.red);
    }

    //move in the opposite direction of a given target. uses navmesh agent
    public void Flee(Transform fleeFromTarget, float minDistance, float fleeSpeed, float rotateSpeed)
    {
        Flee(fleeFromTarget, minDistance);

        //override navmesh movement to keep cryptids from sliding weird
         MoveManuallyAlongNavMeshPath(fleeSpeed, rotateSpeed, false);

    }

    //move in the opposite direction of a given target. does not use navmesh
    public void DirectFlee(Transform fleeFromTarget, float rotateSpeed)
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

    //set the navmesh target to a point in the opposite direction of the thing to flee from
    protected void SetNavmeshFleeTarget(Transform fleeFromTarget, string navMeshAreaName = null)
    {
        if (fleeFromTarget == null) { return; }

        Vector3 fleeFromTargetPos = fleeFromTarget.position;
        fleeFromTargetPos.y = transform.position.y + nav.baseOffset;
        Vector3 oppositeDirection = transform.position - fleeFromTargetPos;
        oppositeDirection.Normalize();

        int navMeshArea = NavMesh.AllAreas;
        if (navMeshAreaName != null)
        {
            navMeshArea = NavMesh.GetAreaFromName(navMeshAreaName);
        }

        //docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
        //get a random position on the navmesh by sampling a few times at a small radius
        for (int i = 0; i < 10; i++)
        {
            Vector3 positionAwayFromTarget = this.transform.position + (oppositeDirection * Random.Range(25, 100)); //lets make sure this works before getting overly concerned about values
            Debug.DrawLine(transform.position, positionAwayFromTarget, Color.gray);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(positionAwayFromTarget, out hit, 2, navMeshArea))
            {
                movementTimer = 0;
                nav.destination = hit.position;
                break;
            }
        }
    }

    protected void SetNavMeshChaseTarget(Transform chaseTarget)
    {
        movementTimer = 0;
        nav.destination = chaseTarget.position;
    }

    //move in the direction of a given target (transform). utilizes navmesh for pathing but movement is manual
    public void MoveToward(Transform target, float speed, float rotateSpeed, bool avoidObstacles = false)
    {
        MoveToward(target);

        Debug.DrawLine(transform.position, nav.destination, Color.cyan);
        MoveManuallyAlongNavMeshPath(speed, rotateSpeed, avoidObstacles);
    }

    public void MoveToward(Transform target)
    {
        movementTimer += Time.deltaTime;
        UnKillNavMeshMovement();

        //same as flee, periodically update to make sure our target location is still aligned with the target object
        if (movementTimer > chaseRepositionInterval && nav.destination != target.position)
        {
            SetNavMeshChaseTarget(target);
        }
    }

    //rotate in the direction of a given target (vector3). does not utilize navmesh
    public void RotateToward(Vector3 target, float rotateSpeed)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed);
        //vector3.zero is used in place of a null value
        if (target == Vector3.zero) { return; }
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawLine(target, transform.position, Color.green);
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (target - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        //Move(forwardSpeed, 0);
        //update: handle forward movement separate from deciding direction with move() in child script
        Debug.DrawRay(transform.position, newDir, Color.blue);
    }

    public void RotateAway(Vector3 target, float rotateSpeed)
    {
        rotateSpeed = Mathf.Abs(rotateSpeed);
        //vector3.zero is used in place of a null value
        if (target == Vector3.zero) { return; }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (transform.position - target), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        //Move(forwardSpeed, 0);
        //update: handle forward movement separate from deciding direction with move() in child script
        Debug.DrawRay(transform.position, newDir, Color.blue);
    }

    //move along path once we've gotten within distance of a path point
    public void CheckPath()
    {
        //todo:what happens when we run out of points
        if (pathIndex >= PathPoints.Length) { return; }

        CryptidPathPoint currentPoint = PathPoints[pathIndex];
        if ((transform.position - currentPoint.transform.position).magnitude < pathPointMinDist)
        {
            pathIndex++;
            if (currentPoint.DoActionAtPoint) { DoActionAtPathPoint(currentPoint); }
        }
    }

    protected virtual void DoActionAtPathPoint(CryptidPathPoint triggerPoint)
    {
        //specifics should be handled in child class
    }

    //method to deal with player entering certain trigger zones; implementation varies by cryptid
    //todo: delete. not being used as far as i can tell
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
    public bool AvoidObstacles(float rotateSpeed)
    {
        //if there are no obstacles in our path we can peace out
        if (obstacles == null || obstacles.Count == 0)
        {
            currentObstacle = null;
            return false;
        }

        if (currentObstacle == null || Time.time - timeOfLastObstacleCheck > obstacleAvoidanceInterval || !obstacles.Contains(currentObstacle))
        {
            //prioritize the obstacle thats most directly in front of us
            float cos = 1;
            Collider obstacleToAvoid = null;
            //make local copy of obstacles before iterating; original might be modified during loop
            List<Collider> obstaclesCopy = new List<Collider>(obstacles);
            foreach (Collider other in obstaclesCopy)
            {
                //it's possible obstacles have been removed/destroyed
                if (other == null) { obstacles.Remove(other); continue; }

                Vector3 dist = transform.position - other.transform.position;
                float newCos = Vector3.Dot(this.transform.right, dist.normalized);
                if (Mathf.Abs(newCos) < Mathf.Abs(cos))
                {
                    cos = newCos;
                    obstacleToAvoid = other;
                }
            }

            currentObstacle = obstacleToAvoid;
            timeOfLastObstacleCheck = Time.time;
        }
        if (currentObstacle == null) { return false; } //not sure how this would happen but i guess it can

        float angle = Vector3.Dot(this.transform.right, (transform.position - currentObstacle.transform.position).normalized);
        Debug.DrawRay(this.transform.position, currentObstacle.transform.position - this.transform.position, Color.red);

        //the amount that it rotates is a function of how far to the right/left the object is
        float avoidRotateSpeed = (1-Mathf.Abs(angle)) * (rotateSpeed /1f);
        //float avoidRotateSpeed =  (rotateSpeed / 10f);

        //this obstacle is on the right side of us, so turn to the left
        if (angle < 0 || Mathf.Abs(angle) <.3f)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.right * -1, avoidRotateSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        //this obstacle is on the left side of us, so turn to the right
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.right, avoidRotateSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        //if the obstacle is more than 45degrees on the periphery, obstacle avoidance doesnt need to override other movement
        //if (Mathf.Abs(angle) > .5) { return false; }

        return true;
    }

    //cryptids disappear when touched by player
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DestroyZone")
        { 
            //we're instantiating the particles and the sfx bc both these references are expected to be to prefabs
            if (particles != null)
            {
                GameObject newParticles = GameObject.Instantiate(particles, this.transform.position, particles.transform.rotation);
                newParticles.SetActive(true);
                //newParticles.transform.localScale = this.transform.localScale * 2;
                //newParticles.transform.Translate(0, 1, 0);//move it up a lil
            }

            Destroy(this.gameObject);
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
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
            GetBonked(leftImpact, bonked);

            //calculate where to position bonk vfx and spawn
            Vector3 impactPosition = collision.GetContact(0).point;
            Vector3 impactDirection = bonked.transform.position - impactPosition;
            bonked.SpawnBonkVFX(impactPosition, impactDirection.normalized);
        }
    }

    public virtual void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
        //cancel out the force applied from the impact of the carrot. i dont want it actually knocking anyone over
        rb.velocity = Vector3.zero;
        KillNavMeshMovement();
        nav.enabled = false;

        if (leftImpact && animator.HasState(0, Animator.StringToHash("bonk_left")))
        {
            animator.Play("bonk_left");
        }
        else if (!leftImpact && animator.HasState(0, Animator.StringToHash("bonk_right")))
        {
            animator.Play("bonk_right");
        }
    }

    public void AddObstacleToList(Collider obstacle)
    {
        if (obstacles != null && !obstacles.Contains(obstacle)){
            obstacles.Add(obstacle);
        }
    }

    public void RemoveObstacleFromList(Collider obstacle)
    {
        if (obstacles != null && obstacles.Contains(obstacle)){
            obstacles.Remove(obstacle);
        }
    }

    protected void KillNavMeshMovement()
    {
        if (nav.enabled)
        {
            nav.velocity = Vector3.zero;
            nav.isStopped = true;
        }
    }

    protected void UnKillNavMeshMovement()
    {
        if (!nav.enabled) { nav.enabled = true; }
        if (nav.isStopped) { nav.isStopped = false; }
        nav.updatePosition = true;
        nav.updateRotation = true;
    }

    //navmesh handles rotation and movement independently
    //but i want crytpids to always move in the direction they're facing
    protected void MoveManuallyAlongNavMeshPath(float moveSpeed, float rotateSpeed, bool avoidObstacles)
    {
        nav.updatePosition = false;
        nav.updateRotation = false;

        //option to do additional obstacle avoidance in the event the navmesh and cryptid get separated (common)
        if (!avoidObstacles || !AvoidObstacles(rotateSpeed))
        {
            RotateToward(nav.nextPosition, rotateSpeed);
        }
        Move(moveSpeed);
    }

}
