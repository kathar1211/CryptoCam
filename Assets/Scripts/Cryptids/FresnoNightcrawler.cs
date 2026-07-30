using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnoNightcrawler : Cryptid {

    public float speed;
    public float rotateSpeed;
    public GameObject zone;

    public float minDistance = 3;
    public float maxTime = 45;

    // public float frequency;
    // public float shift;
    // public float forwardShift;
    

    //animation  parameters 
    const string Recede = "Recede";
    const string Speed = "Speed";
    const string Idle = "Idle";
    const string Lookdown = "Lookdown";
    const string Still = "still";

    private Transform fleeFromTarget;
    private Transform walkTowardTarget;
    public float fleeSpeed;
    public float maxDistance;
    public float seeObstacles;
    public float walkTowardMinDistance;

    public enum MoveState { Walk, Flee, Dance, Nothing, Wander, WalkToward, Look};
    public MoveState currentState = MoveState.Walk;

    //stuff around triggering the dance behavior
    private bool WatchingForDance = false;
    public CryptidTrigger DanceZone;
    private int BounceThreshold = 5; //how many times does the player have to bounce to trigger
    private float BounceTimeLimit = 30; //time in seconds they have to hit the threshold
    private float DancingDuration = 45; //time in seconds fresnos will keep dancing after threshold hit
    private float bounceTimer; //tracking current time againt limit
    private float danceTimer; //tracking current time against duration

    RandomChanceInterval DoneLookingChance;

    [SerializeField]
    Texture2D shutdownTxt;

    // Use this for initialization
    void Start () {
        StartUp();
        baseScore = 100;
        cryptidType = Constants.Fresno;

        //inital target position is directly in front, 100 units away
        if (currentState == MoveState.Walk)
        {
            SetNavMeshChaseTarget(PathPoints[0].transform);
        }

        DoneLookingChance = new RandomChanceInterval(1, .25f);

        if (DanceZone != null)
        {
            DanceZone.TriggerEnterAction += () => { WatchingForDance = true;};
            DanceZone.TriggerExitAction += () => { WatchingForDance = false;};
        }
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (lockMovementSuper) { return; }

        //lock rotation: for top heavy cryptids prone to falling over
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        switch (currentState)
        {
            case MoveState.Walk:
                //dont move during stationary parts of animation
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(Still))
                {
                    // MoveToward(targetPos, rotateSpeed);
                    KillNavMeshMovement();
                    break;
                }
                MoveToward(PathPoints[pathIndex].transform);
                CheckPath();
                if (pathIndex >= PathPoints.Length) { pathIndex = 0; }
                break;
            case MoveState.Wander:
                Wander(maxDistance, minDistance);
                break;
            case MoveState.Flee:
                break;
            case MoveState.Nothing:
                //do nothing
                break;
            case MoveState.WalkToward:
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(Still))
                {
                    // MoveToward(targetPos, rotateSpeed);
                    KillNavMeshMovement();
                    break;
                }
                MoveToward(walkTowardTarget);

                //stop and look once we're in range
                Vector3 actualWalktowardTarget = new Vector3 (walkTowardTarget.position.x, this.transform.position.y, walkTowardTarget.position.z);
                if ((actualWalktowardTarget - this.transform.position).magnitude <= walkTowardMinDistance)
                {
                    //rotate in the direction of the point
                    RotateToward(walkTowardTarget.position, rotateSpeed);

                    //do a little math to see if we're within a few degrees of where we want to be
                    Vector3 targetForward = (transform.position - walkTowardTarget.position);
                    float cos = Vector3.Dot(targetForward.normalized, transform.forward);
                    if (Mathf.Abs(cos) >= .9f)
                    {
                        KillNavMeshMovement();
                        currentState = MoveState.Look;
                        animator.SetBool(Idle, true);
                        animator.SetBool(Lookdown, true);
                    }
                }
                break;
            case MoveState.Look:
                if (DoneLookingChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetBool(Lookdown, false);
                    animator.SetBool(Idle, false);
                    currentState = MoveState.Walk;
                }
                break;
        }
        
    }

   /* void Move()
    {
        //dont move during stationary parts of animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("still"))
        {
            return;
        }

        //turn if needed
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);

        //move forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        //if they reach their target position (this is not supposed to happen) give them a new one on the other side of the zone to get them back on track
        //addition of time tracker lets them change target if they get stuck
        if ((transform.position - targetPos).magnitude < 3 || timeChasing > 45)
        {
            targetPos = zone.transform.position - (transform.position - zone.transform.position);
            timeChasing = 0;
        }
       
    }*/

    //keep fresno nightcrawlers in their designated zone
    private void OnTriggerExit(Collider other)
    {
       
    }

    public override void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if ((other.tag == Constants.PlayerTag && !other.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsCrouching)
            || other.tag == Constants.AvoidTag)
        {
            /* fleeFromTarget = other.gameObject.transform;
             currentState = MoveState.Flee;
             targetPos = Vector3.zero;
             animator.SetFloat(Speed, 2);*/

            //rather than flee, activate defense mechanism and become unrecognizeable
            KillNavMeshMovement();
            nav.enabled = false;
            rb.useGravity = false;
            animator.SetTrigger(Recede);
            currentState = MoveState.Nothing;
            if (shutdownTxt !=null){
                renderer.material.SetTexture("_MainTex", shutdownTxt);
            }
            
        }
        else if (other.tag == Constants.CryptidContainerTag)
        {
            //fresnos turn around and choose new point to walk to directly behind them
            //targetPos = this.transform.position + (this.transform.forward * -15);
            targetPos = zone.transform.position; //move back to the center

            timeChasing = 0;
        }
        else if (other.tag == Constants.CarrotTag)
        {
            //we don't need to chase after the new carrot if we've already got our eyes on one
            //we should also ignore carrots if we're busy running away
            if (currentState == MoveState.Walk)
            {
                currentState = MoveState.WalkToward;
                walkTowardTarget = other.transform;
                SetNavMeshChaseTarget(walkTowardTarget);
            }

        }


        base.OnTriggerEnter(other);
    }

    //restore gravity when recede animation is finished
    public void Fall()
    {
        rb.useGravity = true;
    }

    //cryptid does not count when in defensive mode
    public override bool IsVisible()
    {
        if (currentState == MoveState.Nothing)
        {
            return false;
        }

        return base.IsVisible();
    }

    public override bool SpecialPose()
    {
       //todo

        return base.SpecialPose();
    }
}
