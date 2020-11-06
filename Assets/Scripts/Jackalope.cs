using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    Animator animator;
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

    enum MoveState { run, stand, scratch, flee, sleep, eat, wake, runtoward};
    MoveState currentState;
    MoveState nextState;

    //jackalope has texture switches
    [SerializeField] Texture2D awakeTexture;
    [SerializeField] Texture2D asleepTexture;

    // Use this for initialization
    void Start () {
        StartUp();
        targetPos = transform.position + Random.insideUnitSphere * distance;
        targetPos.y = transform.position.y;
        currentState = MoveState.sleep;
        nextState = MoveState.run;
        animator = GetComponent<Animator>();
        cryptidType = Constants.Jackalope;
        baseScore = 50;

        if (startAwake)
        {
            currentState = MoveState.run;
            this.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", awakeTexture);
            animator.Play("run", 0);
        }
    }
	
	// Update is called once per frame
	void Update () {

        timeElapsed += Time.deltaTime;
        timeChasing += Time.deltaTime;

        //jackalope moves differently based on the different move states
        switch (currentState)
        {
            //wander with random chance to switch states
            case MoveState.run:
                if (!AvoidObstacles(seeAhead, rotateSpeed))
                {
                    Wander(maxDistance, minDistance, runSpeed, rotateSpeed);
                }
                Move(runSpeed);
                if (Random.Range(0.0f,100.0f) > 99.9f)
                {
                    currentState = MoveState.scratch;
                    animationDuration = Random.Range(2.0f, 6.0f);
                    timeElapsed = 0;
                    //animator.Play("scratch n sniff");
                    animator.SetBool(Sniff, true);
                    animator.SetBool(Run, false);
                }
                else if (Random.Range(0.0f, 100.0f) > 99.9f)
                {
                    currentState = MoveState.stand;
                    animationDuration = Random.Range(1.0f, 4.9f);
                    timeElapsed = 0;
                    //animator.Play("stand n sniff");
                    animator.SetBool(StandUp, true);
                    animator.SetBool(Run, false);
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
                }

                break;
            //move away from the player
            case MoveState.flee:              
                if (!AvoidObstacles(seeAhead, rotateSpeed))
                {
                    Flee(fleeFromTarget, fleeSpeed, rotateSpeed + 1);
                }
                Move(fleeSpeed);
                //stop fleeing once jackalope reaches a certain distance from player
                if ((fleeFromTarget.position - transform.position).magnitude > maxDistance)
                {
                    currentState = MoveState.run;
                    animator.SetFloat(Speed, 1);
                }
                break;

            //dont move in these states
            case MoveState.sleep:
                //temp: sleep for set amount of time then awake
                if (Time.time > 6)
                {

                }
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
                Move(runSpeed);
                //switch to eating within a certain range
                if ((moveToTarget.position - this.transform.position).magnitude <= minDistance)
                {
                    currentState = MoveState.eat;
                    animator.SetBool(Eat, true);
                    eatingTimer = 0;
                }
                break;
            case MoveState.eat:
                eatingTimer += Time.deltaTime;
                if (eatingTimer >= timeToEat)
                {
                    currentState = MoveState.run;
                    animator.SetBool(Eat, false);
                    Destroy(moveToTarget.gameObject);
                }
                break;
        }
	}

    public override void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if (other.tag == "Player")
        {
            fleeFromTarget = other.gameObject.transform;
            if (currentState == MoveState.sleep) { WakeUp(); nextState = MoveState.flee; }
            else { currentState = MoveState.flee; }
            animator.SetBool(StandUp, false);
            animator.SetBool(Run, true);
            animator.SetBool(Sniff, false);
            animator.SetBool(Eat, false);
            targetPos = Vector3.zero;
            animator.SetFloat(Speed, 2);
        }
        else if (other.tag == "Carrot")
        {
            if (currentState == MoveState.sleep) { WakeUp(); nextState = MoveState.runtoward; }
            else { currentState = MoveState.runtoward; }
            animator.SetBool(StandUp, false);
            animator.SetBool(Run, true);
            animator.SetBool(Sniff, false);
            animator.SetBool(Eat, false);
            moveToTarget = other.transform;
        }

        base.OnTriggerEnter(other);
    }

    //transition from sleep state
    private void WakeUp()
    {
        animator.SetBool(Awake, true);
        this.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", awakeTexture);
        currentState = MoveState.wake;
    }

    //jackalope special poses include sleeping, eating, scratching, and standing
    public override bool SpecialPose()
    {
        if (currentState == MoveState.sleep || currentState == MoveState.eat || currentState == MoveState.scratch || currentState == MoveState.stand)
        {
            return true;
        }

        return false;
    }


}
