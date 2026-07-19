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

    public enum MoveState { Resting, Hovering, Flying, Targeting }
    public MoveState currentMoveState;

    private Jackalope jackalopeTarget;
    public float minDistFromJackalope;
    public float jackalopeGrabRange;

    public Transform JackalopeAttachTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (currentMoveState)
        {
            case MoveState.Resting:
                break;
            case MoveState.Hovering:
                break;
            case MoveState.Flying:
                break;
            case MoveState.Targeting:
                break;
        }
    }
}
