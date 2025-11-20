using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class FlatwoodsMonster : Cryptid
{
    //keep track of  move state,
    enum MoveState { wander, hover, flee, turnAway, turnToward, pose }
    [SerializeField] MoveState currentState;

    //wandering properties
    public float distance;
    public float runSpeed;
    public float rotateSpeed;
    public float minDistance;

    //avoid obstacles properties
    public float seeAhead;

    //how long do we wait until doing the peace sign
    public float timeUntilPose;
    //how long do we hold the peace sign
    public float poseDuration;
    private float poseTimer;

    //cooldown after doing the peace sign- during this period we'll ignore the camera completely
    public float poseCooldown;
    private float poseCooldownTimer;

    //transform of whatever we're avoiding, if we're avoiding something
    private Transform avoidTarget;
    public float fleeDistance; //how far from the target do we need to get before we stop fleeing

    //animation params
    private const string LookAroundTrigger = "LookAround";
    private const string PoseBool = "Pose";
    private const string MoveBool = "Move";

    RandomChanceInterval HoverChance;
    RandomChanceInterval LookAroundChance;
    RandomChanceInterval WanderChance;

    // Start is called before the first frame update
    void Start()
    {
        baseScore = 175;
        cryptidType = Constants.Flatwoods;
        StartUp();

        HoverChance = new RandomChanceInterval(2, .4f);
        LookAroundChance = new RandomChanceInterval(1, .1f);
        WanderChance = new RandomChanceInterval(1, .3f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (lockMovementSuper) { return; }

        if (IsInCameraView()
            && (currentState == MoveState.wander || currentState == MoveState.hover)
            && poseCooldownTimer <= 0)
        {
            avoidTarget = Photography.Instance.gameObject.transform;
            animator.SetBool(MoveBool, false);
            currentState = MoveState.turnAway;
            poseTimer = 0;
            KillNavMeshMovement();
        }

        if (poseCooldownTimer >= 0)
        {
            poseCooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case MoveState.wander:
                //if (!AvoidObstacles(rotateSpeed))
                //{
                    Wander(distance, minDistance, runSpeed, rotateSpeed);
               // }
               // else
                //{
                    //still increment time spent chasing this wander point while avoiding obstacles to avoid getting stuck
                //    timeChasing += Time.deltaTime;
                //}
               // Move(runSpeed);

                //random chance to hover ominously
                if (HoverChance.UpdateTimerAndCheckSuccess()) {
                    animator.SetBool(MoveBool, false);
                    currentState = MoveState.hover;
                    KillNavMeshMovement();
                }

                break;
            case MoveState.turnAway:

                //rotatetoward with a negative rotate speed is the same as rotate away
                RotateAway(avoidTarget.position, rotateSpeed);

                //if the camera gets put away we can return to wandering
                if (!Photography.Instance.CameraReady)
                {
                    animator.SetBool(MoveBool, true);
                    currentState = MoveState.wander;
                    poseTimer = 0;
                }

                //after a certain amount of time in frame we can turn around and do a pose
                if (IsInCameraView())
                {
                    poseTimer += Time.deltaTime;
                }

                if (poseTimer >= timeUntilPose)
                {
                    poseTimer = 0;
                    currentState = MoveState.turnToward;

                    //rotate slightly to the right
                    //this way, next frame when we start the 180 degree rotation, Vector3.RotateTowards will know which way to go
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.right, rotateSpeed * Time.deltaTime, 0);
                    transform.rotation = Quaternion.LookRotation(newDir, transform.up);
                }

                break;
            case MoveState.turnToward:
                //movetoward with a movespeed of 0 is the same as rotating toward
                RotateToward(avoidTarget.position, rotateSpeed);

                //once we're within an error margin of facing the player, start the pose timer
                Vector3 flatwoodToPlayerDirection = avoidTarget.transform.position - transform.position;
                flatwoodToPlayerDirection = flatwoodToPlayerDirection.normalized;
                //ignore y position differences
                flatwoodToPlayerDirection.y = 0;

                if ((flatwoodToPlayerDirection - transform.forward).magnitude < .1)
                {
                    animator.SetBool(PoseBool, true);
                    currentState = MoveState.pose;
                }

                break;
            case MoveState.pose:
                //count up on the pose timer and then return to idle
                poseTimer += Time.deltaTime;
                if (poseTimer > poseDuration)
                {
                    animator.SetBool(PoseBool, false);
                    currentState = MoveState.hover;
                    poseTimer = 0;
                    poseCooldownTimer = poseCooldown;
                }

                break;
            case MoveState.flee:
               // if (!AvoidObstacles(rotateSpeed))
                //{ 
                    Flee(avoidTarget, minDistance, runSpeed, rotateSpeed);
                //}
               // Move(runSpeed);

                //stop fleeing once we get far enough away
                if ((avoidTarget.position - transform.position).magnitude > fleeDistance)
                {
                    if (poseCooldownTimer <= 0)
                    {
                        poseTimer = 0;
                        animator.SetBool(MoveBool, false);
                        currentState = MoveState.turnAway;
                    }
                    else
                    {
                        currentState = MoveState.wander;
                    }
                }
                break;
            case MoveState.hover:
                //no movement
                //don't try to do anything else unless we're in the regular idle animation and ready to transition
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    return;
                }

                //chance to look around (animation only no movestate change)
                if (LookAroundChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetTrigger(LookAroundTrigger);
                }

                //chance to return to wandering
                else if (WanderChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetBool(MoveBool, true);
                    currentState = MoveState.wander;
                }

                break;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.AvoidTag)
        {
            avoidTarget = other.transform;
            animator.SetBool(MoveBool, true);
            currentState = MoveState.flee;
            SetNavmeshFleeTarget(avoidTarget);
        }


        base.OnTriggerEnter(other);
    }

    public override bool SpecialPose()
    {
        //not enough to be in the pose state- i want the specific animation where shes holding the peace sign
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("peace_hold")) { return true; }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("peace_start")) { return true; }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("peace_end")) { return true; }

        return base.SpecialPose();
    }

    public bool IsInCameraView()
    {
        if (Photography.Instance == null) { return false; }
        if (!Photography.Instance.CameraReady) { return false; }
        if (!Photography.Instance.IsInCameraView(renderer)){ return false; }

        return true;

    }
}
