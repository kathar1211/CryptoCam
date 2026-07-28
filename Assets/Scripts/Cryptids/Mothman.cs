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
    private const string animatorTriggerFlap = "flap";
    private const string animatorBoolGrabbing = "Grabbing";

    public enum MoveState { Resting, Takeoff, Flying, Landing, Targeting }
    public MoveState currentMoveState;

    private Jackalope jackalopeTarget;
    public float minDistFromJackalope;
    public float jackalopeGrabRange;
    public float landingRangeForRestPoint;
    public float maximumAltitude;

    public Transform JackalopeAttachTarget;

    public float upSpeed;
    public float forwardSpeed;
    public float rotateSpeed;

    RandomChanceInterval TakeOffChance;
    RandomChanceInterval FlapChance;

    // Start is called before the first frame update
    void Start()
    {
        TakeOffChance = new RandomChanceInterval(30, .2f);
        FlapChance = new RandomChanceInterval(5, .15f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (currentMoveState)
        {
            case MoveState.Resting:
                break;
            case MoveState.Takeoff:
                break;
            case MoveState.Flying:
                //fly towards the target while moving up
                Vector3 target = PathPoints[pathIndex].transform.position;
                RotateToward(target, rotateSpeed);
                Move(forwardSpeed);
                Ascend(upSpeed, maximumAltitude);
                if ((this.transform.position - target).magnitude < landingRangeForRestPoint)
                {
                    animator.SetBool(animatorBoolFlying, false);
                    currentMoveState = MoveState.Landing;
                }

                //chance to flap wings
                if (FlapChance.UpdateTimerAndCheckSuccess())
                {
                    animator.SetTrigger(animatorTriggerFlap);
                }

                break;
            case MoveState.Landing:
                //come down for landing, and orient self to face the way the path point is
                target = PathPoints[pathIndex].transform.position;
                Vector3 targetDir = PathPoints[pathIndex].transform.forward;
                RotateToward(targetDir, rotateSpeed);
                Move(forwardSpeed / 2f);
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
    }
}
