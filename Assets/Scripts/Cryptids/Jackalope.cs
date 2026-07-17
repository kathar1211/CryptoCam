using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Jackalope : Cryptid {

    //wandering properties
    public float distance;
    public float runSpeed;
    public float rotateSpeed;
    public float minDistance;

    //flee properties
    Transform fleeFromTarget;
    public float maxDistance;
    public float fleeSpeed;

    //move toward/eat properties
    Transform moveToTarget;
    public float timeToEat;
    private float eatingTimer;

    //obstacle avoidance distance
    public float seeAhead;

    float animationDuration;
    float timeElapsed;

    //animator parameters
    string StandUp = "StandUp";
    string Run = "Run";
    string Sniff = "Sniff";
    string Speed = "Speed";
    string Awake = "Awake";
    string Eat = "Eat";
    public bool startAwake;

    public enum MoveState { run, stand, scratch, flee, sleep, eat, wake, runtoward, caught};
    public MoveState currentState;
    MoveState nextState;

    //jackalope has texture switches
    [SerializeField] Texture2D awakeTexture;
    [SerializeField] Texture2D asleepTexture;

    // Use this for initialization
    void Start () {
        StartUp();
        targetPos = Vector3.zero;
        currentState = MoveState.sleep;
        nextState = MoveState.run;
        cryptidType = Constants.Jackalope;
        baseScore = 25;

        if (startAwake)
        {
            currentState = MoveState.run;
            renderer.material.SetTexture("_MainTex", awakeTexture);
            animator.Play("run", 0);
        }
        else
        {
            KillNavMeshMovement();
        }
    }
	
	// Update is called once per frame
	protected override void Update () {

        base.Update();
        if (lockMovementSuper) { return; }

        timeElapsed += Time.deltaTime;
        timeChasing += Time.deltaTime;

        //jackalope moves differently based on the different move states
        switch (currentState)
        {
            //wander with random chance to switch states
            case MoveState.run:
                //if (!AvoidObstacles(rotateSpeed))
                {
                    Wander(distance, minDistance);
                }
               // Move(runSpeed);
                if (Random.Range(0.0f,100.0f) > 99.9f)
                {
                    currentState = MoveState.scratch;
                    animationDuration = Random.Range(2.0f, 6.0f);
                    timeElapsed = 0;
                    //animator.Play("scratch n sniff");
                    animator.SetBool(Sniff, true);
                    animator.SetBool(Run, false);
                    KillNavMeshMovement();
                }
                else if (Random.Range(0.0f, 100.0f) > 99.9f)
                {
                    currentState = MoveState.stand;
                    animationDuration = Random.Range(1.0f, 4.9f);
                    timeElapsed = 0;
                    //animator.Play("stand n sniff");
                    animator.SetBool(StandUp, true);
                    animator.SetBool(Run, false);
                    KillNavMeshMovement();
                }
                break;

            //stand still and do animation
            case MoveState.scratch:
            case MoveState.stand:
                //return to wandering after random amount of time
                if (timeElapsed >= animationDuration)
                {
                    currentState = MoveState.run;
                    animator.SetBool(StandUp, false);
                    animator.SetBool(Run, true);
                    animator.SetBool(Sniff, false);
                    //animator.Play("run");
                    if (nav.enabled) { nav.isStopped = false; }
                }

                break;
            //move away from the player
            case MoveState.flee:
                if (!AvoidObstacles(rotateSpeed))
                {
                    DirectFlee(fleeFromTarget, rotateSpeed + 1);
                }
                Move(fleeSpeed);
                //stop fleeing once jackalope reaches a certain distance from player
                if ((fleeFromTarget.position - transform.position).magnitude > maxDistance)
                {
                    currentState = MoveState.run;
                    animator.SetFloat(Speed, 1);
                    nav.enabled = true;
                }
                break;

            //dont move in these states
            case MoveState.caught:
            case MoveState.sleep:
               //do nothing
                break;
            case MoveState.wake:
                //check for run animation to start moving again
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
                {
                    currentState = nextState;
                    nextState = MoveState.run;
                }
                break;
            //move toward a specific object
            case MoveState.runtoward:
                MoveToward(moveToTarget, runSpeed, rotateSpeed);
                //switch to eating within a certain range
                if ((moveToTarget.position - this.transform.position).magnitude <= minDistance)
                {
                    KillNavMeshMovement();
                    currentState = MoveState.eat;
                    animator.SetBool(Eat, true);
                    eatingTimer = 0;
                }
                break;
            case MoveState.eat:
                eatingTimer += Time.deltaTime;
                if (eatingTimer >= timeToEat)
                {
                    StopEating();
                    Destroy(moveToTarget.gameObject);
                }

                //stop eating if the carrot gets too far away
                if ((moveToTarget.position - this.transform.position).magnitude > minDistance * 2)
                {

                    StopEating();
                }

                break;
        }
	}

    private void StopEating()
    {
        if (nav.enabled)
        {
            nav.isStopped = false;
            nav.destination = targetPos;
        }
        Debug.Log("updated nav destination on " + this.name);
        currentState = MoveState.run;
        animator.SetBool(Eat, false);
    }

    public override void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if (other.tag == Constants.PlayerTag)
        {
            if (!other.gameObject.GetComponent<FirstPersonController>().IsCrouching)
            {
                PrepareToFlee(other.gameObject.transform);
            }
        }
        else if (other.tag == Constants.AvoidTag)
        {
            PrepareToFlee(other.gameObject.transform);
        }
        else if (other.tag == Constants.CarrotTag)
        {
            //we should also ignore carrots if we're busy running away
            if (currentState != MoveState.flee)
            {
                if (currentState == MoveState.sleep) { WakeUp(); nextState = MoveState.runtoward; }
                else if (currentState == MoveState.wake) { nextState = MoveState.runtoward; }
                else { currentState = MoveState.runtoward; }
                animator.SetBool(StandUp, false);
                animator.SetBool(Run, true);
                animator.SetBool(Sniff, false);
                animator.SetBool(Eat, false);
                moveToTarget = other.transform;
                SetNavMeshChaseTarget(moveToTarget);
            }

        }

        base.OnTriggerEnter(other);
    }

    private void PrepareToFlee(Transform target)
    {
        fleeFromTarget = target;
        if (currentState == MoveState.sleep) { WakeUp(); nextState = MoveState.flee; }
        else if (currentState == MoveState.wake) { nextState = MoveState.flee; }
        else { currentState = MoveState.flee; }
        animator.SetBool(StandUp, false);
        animator.SetBool(Run, true);
        animator.SetBool(Sniff, false);
        animator.SetBool(Eat, false);
        targetPos = Vector3.zero;
        animator.SetFloat(Speed, 2);
        KillNavMeshMovement();
        nav.enabled = false;
    }

    //transition from sleep state
    private void WakeUp()
    {
        animator.SetBool(Awake, true);
        renderer.material.SetTexture("_MainTex", awakeTexture);
        currentState = MoveState.wake;
    }

    //jackalope special poses include sleeping, eating, scratching, and standing
    public override bool SpecialPose()
    {
        if (currentState == MoveState.sleep || currentState == MoveState.eat || currentState == MoveState.scratch || currentState == MoveState.stand)
        {
            return true;
        }

        return base.SpecialPose();
    }

    public override void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
        if (currentState == MoveState.stand)
        {
            animator.Play("bonk_stand");
        }
        else
        {
            base.GetBonked(leftImpact);
        }
        
    }

    public void GetCaught()
    {
        nav.enabled = false;
        currentState = MoveState.caught;
        animator.SetBool(StandUp, false);
        animator.SetBool(Run, true);
        animator.SetBool(Sniff, false);
    }

}
