using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigfoot : Cryptid
{

    //animation names and animator paramters
    private const string SIT_BONK_LEFT = "sit_bonk_left";
    private const string SIT_BONK_RIGHT = "sit_bonk_right";
    private const string STAND_BONK_LEFT = "bonk_left";
    private const string STAND_BONK_RIGHT = "bonk_right";
    private const string SIT_IDLE = "sit_idle";
    private const string SIT_TRANSITION_UP = "sit_transition_reverse";

    private const string SitBool = "Sit";
    private const string WalkBool = "Walk";
    private const string TurnTrigger = "TurnPose";
    private const string HeadScratchDoneTrigger = "DoneHeadScratch";

    public const float MaxSitTime = 300;
    public const float MinSitTime = -1;
    public const float MaxScratchTime = 60;
    public const float MinScratchTime = 0;

    //keep track of  move state,
    enum MoveState { wander, sit, idle, befuddle, flee, pointReached }
    [SerializeField] MoveState currentState;
    private bool lookedOnce;

    //wandering properties
    public float wanderDistance;
    public float walkSpeed;
    public float rotateSpeed;
    public float minDistance;

    //avoid obstacles properties
    public float seeAhead;

    //transform of whatever we're avoiding, if we're avoiding something
    private Transform avoidTarget;
    public float fleeDistance; //how far from the target do we need to get before we stop fleeing

    //handles time between state transitions
    private float timer = 0;

    public float timeToSit = -1; //negative one indicates infinite
    public float timeToScratch = 0;

    //random driven behaviors
    RandomChanceInterval LookPoseChance;
    RandomChanceInterval IdleChance;
    RandomChanceInterval WalkChance;
    RandomChanceInterval SitChance;

    // Start is called before the first frame update
    void Start()
    {
        baseScore = 250;
        cryptidType = Constants.Bigfoot;

        StartUp();

        //make sure initial anim settings match our initial movestate
        switch (currentState)
        {
            case MoveState.wander:
                animator.SetBool(WalkBool, true);
                animator.SetBool(SitBool, false);
                break;
            case MoveState.sit:
                animator.SetBool(SitBool, true);
                animator.SetBool(WalkBool, false);
                break;
            case MoveState.idle:
                animator.SetBool(SitBool, false);
                animator.SetBool(WalkBool, false);
                break;
        }

        LookPoseChance = new RandomChanceInterval(1, .06f);
        IdleChance = new RandomChanceInterval(3, .1f);
        WalkChance = new RandomChanceInterval(3, .2f);
        SitChance = new RandomChanceInterval(3, .2f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        // base.Update();

        switch (currentState)
        {
            case MoveState.wander:

                if (!AvoidObstacles(rotateSpeed))
                {
                    if (pathIndex < PathPoints.Length)
                    {
                        MoveToward(PathPoints[pathIndex].transform, rotateSpeed);
                    }
                    else
                    {
                        Wander(wanderDistance, minDistance, walkSpeed, rotateSpeed);
                    }
                }
                //move forward after setting direction in other methods
                Move(walkSpeed);
                CheckPath();

                //chance to do the pose
                if (LookPoseChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetTrigger(TurnTrigger);
                    LookPoseChance.SetChance(.03f); //lower the chance of subsequent poses
                    break ;
                }

                //chance to stop and idle for a bit
                if (IdleChance.UpdateTimerAndCheckSuccess())
                {
                    LookPoseChance.SetChance(.06f); //put this back to normal
                    animator.SetBool(WalkBool, false);
                    currentState = MoveState.idle;
                }

                break;
            case MoveState.sit:
                if (timer > timeToSit && timeToSit != -1)
                {
                    timer = 0;
                    animator.SetBool(SitBool, false);
                    currentState = MoveState.idle;
                }
                else if (timeToSit != -1)
                {
                    timer += Time.deltaTime;
                }
                break;
            case MoveState.idle:
                //chance to start walkin
                if (WalkChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetBool(WalkBool, true);
                    currentState = MoveState.wander;
                }

                /*
                //chance to start sittin
                if (SitChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetBool(SitBool, true);
                    currentState = MoveState.sit;

                    //how long we sittin?
                    timer = 0;
                    timeToSit = Random.Range(MinSitTime, MaxSitTime);
                }*/

                break;
            case MoveState.befuddle:
                //wait til the bonk animation is done
                if (animator.GetCurrentAnimatorStateInfo(0).IsName(STAND_BONK_LEFT) || animator.GetCurrentAnimatorStateInfo(0).IsName(STAND_BONK_RIGHT))
                {
                    break;
                }

                //rotate towards the object that bonked u. no other movement
                MoveTowardXZOnly(targetPos, rotateSpeed);

                //increment timer
                timer += Time.deltaTime;

                //after enough time return to idle
                if (timer > timeToScratch)
                {
                    timer = 0;
                    animator.SetTrigger(HeadScratchDoneTrigger);
                    currentState = MoveState.idle;
                }

                break;
            case MoveState.flee:
                if (!AvoidObstacles(rotateSpeed))
                {
                    Flee(avoidTarget, walkSpeed, rotateSpeed);
                }
                Move(walkSpeed);

                //stop fleeing once we get far enough away
                if ((avoidTarget.position - transform.position).magnitude > fleeDistance)
                {
                    currentState = MoveState.wander;
                }
                break;
            case MoveState.pointReached:

                //rotate in the direction of the point
                MoveTowardXZOnly(targetPos, rotateSpeed);

                //once we're facing the right way, sit
                animator.SetBool(SitBool, true);
                currentState = MoveState.sit;

                //how long we sittin?
                timer = 0;
                timeToSit = Random.Range(MinSitTime, MaxSitTime);

                break;

        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.AvoidTag)
        {
            //if we're sitting we need to get up first
            if (currentState == MoveState.sit)
            {
                animator.SetBool(SitBool, false);
                currentState = MoveState.idle;
                return;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName(SIT_IDLE) || animator.GetCurrentAnimatorStateInfo(0).IsName(SIT_TRANSITION_UP)){
                //wait for these anims to finish
                return;
            }

            avoidTarget = other.transform;
            currentState = MoveState.flee;
        }


        base.OnTriggerEnter(other);
    }

    public override void GetBonked(bool leftImpact, BonkableObject bonked)
    {

        if (currentState == MoveState.sit)
        {
            if (leftImpact)
            {
                animator.Play(SIT_BONK_LEFT);
            }
            else
            {
                animator.Play(SIT_BONK_RIGHT);
            }

            //get up after getting bonked
            animator.SetBool(SitBool, false);
        }
        else
        {
            if (leftImpact)
            {
                animator.Play(STAND_BONK_LEFT);
            }
            else
            {
                animator.Play(STAND_BONK_RIGHT);
            }

            //stop moving while getting bonked
            currentState = MoveState.befuddle;

            //prepare to rotate towards the object
            targetPos = bonked.transform.position;

            //reset timer in preparation for headcratch time
            timer = 0;
            timeToScratch = Random.Range(MinScratchTime, MaxScratchTime);
        }
    }

    public override bool SpecialPose()
    {
        //todo

        return base.SpecialPose();
    }

    protected override void DoActionAtPathPoint(CryptidPathPoint triggerPoint)
    {
        currentState = MoveState.pointReached;
        targetPos = this.transform.position + triggerPoint.transform.forward;
    }
}
