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
    private const string animatorTriggerFlap = "flapOnce";
    private const string animatorBoolGrabbing = "Grabbing";

    public enum MoveState { Resting, Takeoff, Ascending, Descending, Landing, Targeting, Skedaddling, None }
    public MoveState currentMoveState;
    private MoveState nextMoveState = MoveState.None;
    private bool justChangedStates = false;

    public CryptidPathPoint outOfBoundsPoint; //mothman flies away after getting a jackalope
    private Jackalope jackalopeTarget;
    public float minDistFromJackalope;
    public float jackalopeGrabRange;
    public float landingRangeForRestPoint;
    public float maximumAltitude;
    private float distanceToNextTarget;
    public float targetHeightOffset; //aim for a little above the resting point so he can ease down

    public Transform JackalopeAttachTarget;
    public CryptidTrigger JackalopeZone;

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

        TakeOffChance = new RandomChanceInterval(30, .2f);
        FlapChance = new RandomChanceInterval(5, .15f);

        pathIndex = -1;
    }

    // Update is called once per frame
    protected override void Update()
    {
        baseScore = 200;
        cryptidType = Constants.Mothman;
        base.Update();

        //ignore bonk freeze if we get bonked while flying
        if (currentMoveState == MoveState.Ascending || currentMoveState == MoveState.Descending || currentMoveState == MoveState.Skedaddling)
        {
            lockMovementSuper = false;
        }
        
        if (lockMovementSuper) { return;}

        switch (currentMoveState)
        {
            case MoveState.Resting:
                //if we just triggered a bonk, give animations a frame to initialize before we try to do anything else
                if (justChangedStates)
                {
                    justChangedStates = false;
                    break;
                }

                //just hang out until its time to move to the next point
                if (TakeOffChance.UpdateTimerAndCheckSuccess() || nextMoveState == MoveState.Takeoff)
                {
                    TakeOff();
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
                    animator.SetBool(animatorBoolFlap, false);
                    currentMoveState = MoveState.Descending;
                }

                break;
            case MoveState.Descending:

                target = PathPoints[pathIndex].transform.position;
                RotateToward(target, rotateSpeed);
                Move(forwardSpeed);

                //keep going up if we're somehow not there yet
                if (this.transform.position.y < target.y + targetHeightOffset)
                {
                    Ascend(upSpeed, maximumAltitude);

                    //we should be flapping if we still need to go up
                    if ((target.y + targetHeightOffset) - this.transform.position.y > targetHeightOffset * 2)
                    {
                        animator.SetBool(animatorBoolFlap, true);
                    }
                }
                else 
                {
                    Descend(upSpeed, target.y + targetHeightOffset);
                    animator.SetBool(animatorBoolFlap, false);
                }

                //we could get pretty high up, so only look at x and z for proximity to landing point
                xzTarget = new Vector3(target.x, target.y, target.z);
                xzTarget.y = this.transform.position.y;
                if ((this.transform.position - xzTarget).magnitude < landingRangeForRestPoint)
                {
                    Debug.Log("Mothman approaching landing point. " +
                        "Pathpoint index " + pathIndex +
                        " pathpoint location " + PathPoints[pathIndex].transform.position);
                    animator.SetBool(animatorBoolFlap, false);
                    animator.SetBool(animatorBoolFlying, false);
                    currentMoveState = MoveState.Landing;
                    break;
                }

                //chance to flap wings
                if (FlapChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetTrigger(animatorTriggerFlap);
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
                //Vector3 targetDir = PathPoints[pathIndex].transform.position + PathPoints[pathIndex].transform.forward;
                //RotateToward(targetDir, rotateSpeed);
                RotateToMatchDirection(PathPoints[pathIndex].transform.forward, rotateSpeed);
                Descend(upSpeed, target.y);
                SlideToward(forwardSpeed / 2f, target);

                //do a little math to see if we're within a few degrees of where we want to be
                //Vector3 targetForward = (transform.position - targetPos);
                float cos = Vector3.Dot(PathPoints[pathIndex].transform.forward, transform.forward);

                //arrive at rest within a certain range
                float distance = (this.transform.position - target).magnitude;
                Debug.Log("Mothman landing. dot product of forwards: " + cos);
                if (cos >= .95f && distance < pathPointMinDist)
                {
                    Land();
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

    private void TakeOff()
    {
        //attempt to unoccupy current perch, if applicable
        if (pathIndex >= 0 && pathIndex < PathPoints.Length)
        {
            CryptidPathPoint currentPoint = PathPoints[pathIndex];
            if (currentPoint is MothmanPathPoint)
            {
                MothmanPathPoint currentMothmanPoint = currentPoint as MothmanPathPoint;
                if (currentMothmanPoint.Perch != null)
                {
                    currentMothmanPoint.Perch.perchOccupied = false;
                    currentMothmanPoint.Perch.myMothman = null;
                }
            }
        }
        pathIndex++;

        animator.SetBool(animatorBoolResting, false);
        currentMoveState = MoveState.Takeoff;
        timer = 0;
        UpdateDistanceToTarget();
        nextMoveState = MoveState.None;
    }

    private void Land()
    {
        //attempt to occupy current perch, if applicable
        if (pathIndex >= 0 && pathIndex < PathPoints.Length)
        {
            CryptidPathPoint currentPoint = PathPoints[pathIndex];
            if (currentPoint is MothmanPathPoint)
            {
                MothmanPathPoint currentMothmanPoint = currentPoint as MothmanPathPoint;
                if (currentMothmanPoint.Perch != null)
                {
                    currentMothmanPoint.Perch.perchOccupied = true;
                    currentMothmanPoint.Perch.myMothman = this;
                }
            }
        }

        animator.SetBool(animatorBoolResting, true);
        currentMoveState = MoveState.Resting;
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

    public override void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
        //base.GetBonked(leftImpact, bonked);
        switch (currentMoveState)
        {
            case MoveState.Resting:
                if (leftImpact) { animator.SetTrigger(animatorTiggerBonkLeft);}
                else { animator.SetTrigger(animatorTriggerBonkRight);}
                //would like this to trigger takeoff as well
                nextMoveState = MoveState.Takeoff;
                justChangedStates = true;
                break;

            case MoveState.Takeoff:
            case MoveState.Landing:
            case MoveState.Targeting:
                //hover looks for forward and back rather than left/right, so we have to calculate impact direction again
                //line from cryptid to carrot
                Vector3 bonkDistance = this.gameObject.transform.position - bonked.gameObject.transform.position;

                //if the line from the cryptid to the carrot is in the same direction as the cryptid's forward vector,
                //then the carrot is in front of the cryptid
                bool frontImpact = false;
                if (Vector3.Dot(this.transform.forward, bonkDistance) < 0)
                {
                    frontImpact = true;
                }
                if (frontImpact) { animator.SetTrigger(animatorTiggerBonkFront);}
                else { animator.SetTrigger(animatorTriggerBonkBack);}
                break;

            case MoveState.Ascending:
            case MoveState.Descending:
                if (leftImpact) { animator.SetTrigger(animatorTiggerBonkLeft); }
                else { animator.SetTrigger(animatorTriggerBonkRight); }
                break;

            case MoveState.Skedaddling:
                //todo
                break;
        }

        AchievementManager.UpdateBonkStats(this);
    }

    //called if the object mothman is standing on gets hit with a carrot
    public void PerchBonked()
    {
        TakeOff();
    }

    public void AquireTarget(Jackalope jackalopeAquired)
    {
        //if we somehow already have a target, prioritize the closer one
        if (jackalopeTarget != null)
        {
            float newJackalopeDistance = (this.transform.position - jackalopeAquired.transform.position).magnitude;
            float oldJackalopeDistance = (this.transform.position - jackalopeTarget.transform.position).magnitude;

            if (newJackalopeDistance < oldJackalopeDistance) { jackalopeTarget = jackalopeAquired; }
        }
        else
        {
            jackalopeTarget = jackalopeAquired;
        }
        
    }

    public void LoseTarget(Jackalope jackalopeLost)
    {
        //if this is somehow a different jackalope than our target, dont worry about it
        if (jackalopeLost != jackalopeTarget) { return; }
    }
}
