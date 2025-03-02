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

    private const string SitBool = "Sit";
    private const string WalkBool = "Walk";
    private const string TurnTrigger = "TurnPose";
    private const string HeadScratchDoneTrigger = "DoneHeadScratch";

    //keep track of  move state,
    enum MoveState { wander, sit, idle, befuddle }
    [SerializeField] MoveState currentState;

    //wandering properties
    public float distance;
    public float runSpeed;
    public float rotateSpeed;
    public float minDistance;

    //avoid obstacles properties
    public float seeAhead;

    // Start is called before the first frame update
    void Start()
    {
        StartUp();
    }

    // Update is called once per frame
    protected override void Update()
    {
       // base.Update();
    }

    public override void GetBonked(bool leftImpact)
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
        }

        
    }
}
