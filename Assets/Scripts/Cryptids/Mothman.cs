using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothman : Cryptid
{

    //animator parameters
    private const string animatorTiggerBonkLeft = "bonk_left";
    private const string animatorTriggerBonkRight = "bonk_right";
    private const string animatorTiggerBonkFront = "bonk_front";
    private const string animatorTriggerBonkBack = "bonk_back";
    private const string animatorBoolResting = "Resting";
    private const string animatorBoolFlying = "Flying";
    private const string animatorBoolFlap = "flap";
    private const string animatorBoolGrabbing = "Grabbing";

    public enum MoveState { Resting, Takeoff, Ascending, Descending, Landing, Targeting, Skedaddling }
    public MoveState currentMoveState;

    public CryptidPathPoint outOfBoundsPoint; //mothman flies away after getting a jackalope
    private Jackalope jackalopeTarget;
    public float minDistFromJackalope;
    public float jackalopeGrabRange;
    public float landingRangeForRestPoint;
    public float maximumAltitude;
    private float distanceToNextTarget;

    public Transform JackalopeAttachTarget;

    public float upSpeed;
    public float forwardSpeed;
    public float rotateSpeed;
    public float takeOffTime;

    RandomChanceInterval TakeOffChance;
    RandomChanceInterval FlapChance;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartUp();

        TakeOffChance = new RandomChanceInterval(10, .2f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (currentMoveState)
        {
            case MoveState.Resting:
                //just hang out until its time to move to the next point
                if (TakeOffChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetBool(animatorBoolResting, false);
                    currentMoveState = MoveState.Takeoff;
                    timer = 0;
                    UpdateDistanceToTarget();
                }
                break;
            case MoveState.Takeoff:
                //move straight up for an amount of time before actually moving to next target
                Ascend(upSpeed, maximumAltitude);
                timer += Time.deltaTime;
                if (timer >= takeOffTime)
                {
                    animator.SetBool(animatorBoolFlying, true);
                    animator.SetBool(animatorBoolFlap, true);
                    currentMoveState = MoveState.Ascending;
                    timer = 0;
                }
                break;
            case MoveState.Ascending:
                //fly towards the target while moving up
                Vector3 target = PathPoints[pathIndex].transform.position;
                RotateToward(target, rotateSpeed);
                Move(forwardSpeed);
                Ascend(upSpeed, maximumAltitude);

                //start to descend once we're halfway to the target
                Vector3 xzTarget = new Vector3(target.x, target.y, target.z);
                xzTarget.y = this.transform.position.y;
                if ((this.transform.position - xzTarget).magnitude < distanceToNextTarget/2f)
                {
                    currentMoveState = MoveState.Descending;
                    animator.SetBool(animatorBoolFlap, false);
                }

                break;
            case MoveState.Descending:

                target = PathPoints[pathIndex].transform.position;
                RotateToward(target, rotateSpeed);
                Move(forwardSpeed);

                //keep going up if we're somehow not there yet
                if (this.transform.position.y < target.y)
                {
                    Ascend(upSpeed, maximumAltitude);
                }
                else
                {
                    Descend(upSpeed, target.y);
                }
               

                //we could get pretty high up, so only look at x and z for proximity to landing point
                xzTarget = new Vector3(target.x, target.y, target.z);
                xzTarget.y = this.transform.position.y;
                if ((this.transform.position - xzTarget).magnitude < landingRangeForRestPoint)
                {
                    animator.SetBool(animatorBoolFlying, false);
                    currentMoveState = MoveState.Landing;
                    break;
                }
                break;
            case MoveState.Skedaddling:
                //mothman flies to a spot the player cant reach
                RotateToward(outOfBoundsPoint.transform.position, rotateSpeed);
                Move(forwardSpeed);
                Ascend(upSpeed, maximumAltitude);

                //at a certain point he and the jackalope should probably despawn

                break;
            case MoveState.Landing:
                //come down for landing, and orient self to face the way the path point is
                target = PathPoints[pathIndex].transform.position;
                Vector3 targetDir = PathPoints[pathIndex].transform.forward;
                RotateToward(targetDir, rotateSpeed);
                Descend(upSpeed, target.y);
                SlideToward(forwardSpeed / 2f, target);

                //do a little math to see if we're within a few degrees of where we want to be
                Vector3 targetForward = (transform.position - targetPos);
                float cos = Vector3.Dot(targetForward.normalized, transform.forward);

                //arrive at rest within a certain range
                float distance = (this.transform.position - target).magnitude;
                if (Mathf.Abs(cos) >= .9f && distance < pathPointMinDist)
                {
                    animator.SetBool(animatorBoolResting, true);
                    currentMoveState = MoveState.Resting;
                }

                break;
            case MoveState.Targeting:
                break;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public void SnatchJackalope()
    {
        if (jackalopeTarget == null) { return; }
        jackalopeTarget.GetCaught();
        jackalopeTarget.transform.SetParent(JackalopeAttachTarget);
    }

    //mark how far the next resting spot is when we first takeoff, so that later we can measure progress against it
    private void UpdateDistanceToTarget()
    {
        //look at xz distance only
        Vector3 target = PathPoints[pathIndex].transform.position;
        Vector3 xzTarget = new Vector3(target.x, target.y, target.z);
        xzTarget.y = this.transform.position.y;
        distanceToNextTarget = xzTarget.magnitude;

    }
}
