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

    //transform of whatever we're avoiding, if we're avoiding something
    private Transform avoidTarget;
    public float fleeDistance; //how far from the target do we need to get before we stop fleeing

    //animation params
    private const string LookAroundTrigger = "LookAround";
    private const string PoseBool = "Pose";
    private const string MoveBool = "Move";

    // Start is called before the first frame update
    void Start()
    {
        baseScore = 175;
        cryptidType = Constants.Flatwoods;
        StartUp();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (lockMovementSuper) { return; }

        if (Photography.Instance.CameraReady
            && currentState != MoveState.flee && currentState != MoveState.turnAway 
            && currentState != MoveState.turnToward && currentState != MoveState.pose)
        {
            avoidTarget = Photography.Instance.gameObject.transform;
            animator.SetBool(MoveBool, false);
            currentState = MoveState.turnAway;
            poseTimer = 0;
        }

        switch (currentState)
        {
            case MoveState.wander:
                if (!AvoidObstacles(seeAhead, rotateSpeed, true))
                {
                    Wander(distance, minDistance, runSpeed, rotateSpeed);
                }
                else
                {
                    //still increment time spent chasing this wander point while avoiding obstacles to avoid getting stuck
                    timeChasing += Time.deltaTime;
                }
                Move(runSpeed);

                //random chance to hover ominously
                if (RandomChance(.3f)) {
                    animator.SetBool(MoveBool, false);
                    currentState = MoveState.hover;
                }

                break;
            case MoveState.turnAway:

                //fleeing with a movespeed of 0 is the same as just rotating away
                Flee(avoidTarget, 0, rotateSpeed);

                //if the camera gets put away we can return to wandering
                if (!Photography.Instance.CameraReady)
                {
                    animator.SetBool(MoveBool, true);
                    currentState = MoveState.wander;
                }

                //after a certain amount of time we can turn around and do a pose
                poseTimer += Time.deltaTime;
                if (poseTimer >= timeUntilPose)
                {
                    poseTimer = 0;
                    currentState = MoveState.turnToward;
                }

                break;
            case MoveState.turnToward:
                //movetoward with a movespeed of 0 is the same as rotating toward
                MoveToward(avoidTarget, 0, rotateSpeed);

                //once we're within an error margin of facing the player, start the pose timer
                Vector3 flatwoodToPlayerDirection = avoidTarget.transform.position - transform.position;
                flatwoodToPlayerDirection = flatwoodToPlayerDirection.normalized;

                if ((flatwoodToPlayerDirection - transform.forward).magnitude < 1)
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
                }

                break;
            case MoveState.flee:
                if (!AvoidObstacles(seeAhead, rotateSpeed, true))
                { 
                    Flee(avoidTarget, runSpeed, rotateSpeed);
                }
                Move(runSpeed);

                //stop fleeing once we get far enough away
                if ((avoidTarget.position - transform.position).magnitude > fleeDistance)
                {
                    poseTimer = 0;
                    animator.SetBool(MoveBool, false);
                    currentState = MoveState.turnAway;
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
                if (RandomChance(.1f))
                {
                    animator.SetTrigger(LookAroundTrigger);
                }

                //chance to return to wandering
                else if (RandomChance(.5f))
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
        }


        base.OnTriggerEnter(other);
    }
}
