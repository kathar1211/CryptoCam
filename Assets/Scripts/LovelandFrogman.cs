using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LovelandFrogman : Cryptid {

    ParticleSystem ripples;

    //keep track of frogmans move state, serializable bc his default state isnt set in stone yet
    enum MoveState { swim, walk, edgeLeap, sit, flee}
    [SerializeField] MoveState currentState;

    //amount position needs to be adjusted after a leap
    [SerializeField]
    Vector3 leapOffset;

    //amount position needs to be adjusted before a leap
    [SerializeField]
    Vector3 preleapOffset;

    public float walkSpeed;
    public float swimSpeed;
    public float leapSpeed;
    public float leapHeight;
    float rotateSpeed;
    public float maxRotateSpeed;
    public float changeTargetTime; //how often should direction change during wander behavior
    public float seeObstacles;

    //properties for fleeing
    public float fleeSpeed;
    Transform fleeFromTarget;
    public float safeZone; //distance at which we no longer flee

    bool needToAdjustPosition; //used to adjust transform information to match up with visuals after an edge leap

    [SerializeField]
    float timeToTurn; //how often to switch directions
    float timeToSit; //how long to sit after a leap
    [SerializeField]
    float sitTimeMax;
    [SerializeField]
    float sitTimeMin;
    private float timer;

    //values for wandering
    public float targetMaxDistance;
    public float targetMinDistance;

    //represents the height frog should be at to look like he's swimming apropriately
    private float swimHeight = -1;

	// Use this for initialization
	void Start () {
        StartUp();
        cryptidType = Constants.Frogman;
        ripples = GetComponentInChildren<ParticleSystem>();

        //convert offset to be relative to forgmans direction
        Vector3 upMove = new Vector3(transform.up.x * leapOffset.y, transform.up.y * leapOffset.y, transform.up.z * leapOffset.y);
        Vector3 forwardMove = new Vector3(transform.forward.x * leapOffset.z, transform.forward.y * leapOffset.z, transform.forward.z * leapOffset.z);
        leapOffset = upMove + forwardMove;
        preleapOffset = leapOffset;

        rotateSpeed = Random.Range(-maxRotateSpeed, maxRotateSpeed);

        //set up animator for starting state
        if (currentState == MoveState.walk || currentState == MoveState.sit)
        {
            EndFrogLeap();
        }
        else if (currentState == MoveState.swim)
        {
            swimHeight = transform.position.y;
        }
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if (lockMovementSuper) { return; }

        timer += Time.deltaTime;

        switch (currentState)
        {
            //movement states
            case MoveState.swim:

                //Move(swimSpeed, rotateSpeed);
                if (!AvoidObstacles(seeObstacles, rotateSpeed)){
                    Move(swimSpeed, rotateSpeed);
                }
                else
                {
                    Move(swimSpeed);
                }
                if (swimHeight != -1 && transform.position.y != swimHeight)
                {
                    transform.Translate(Vector3.up * (swimHeight - transform.position.y) * swimSpeed * Time.deltaTime);
                }
                break;
            case MoveState.walk:
                if (!AvoidObstacles(seeObstacles, rotateSpeed, true))
                {
                    Wander(targetMaxDistance, targetMinDistance, walkSpeed, rotateSpeed, changeTargetTime);
                }
                else
                {
                    targetPos = Vector3.zero;
                }
                //move forward after setting direction in other methods
                Move(walkSpeed);
                break;
            case MoveState.sit:
                if (timer > timeToSit)
                {
                    timer = 0;
                    animator.SetBool("creep", true);
                    currentState = MoveState.walk;
                }
                break;
            case MoveState.edgeLeap:
                //Leap(leapSpeed, 0);
                break;
            case MoveState.flee:
                
                if (!AvoidObstacles(seeObstacles, rotateSpeed))
                {
                    Flee(fleeFromTarget, fleeSpeed, rotateSpeed);
                }
                Move(fleeSpeed);
                if ((fleeFromTarget.position - transform.position).magnitude > safeZone)
                {
                    currentState = MoveState.walk;
                }
                break;
        }

        //apply position offset after leaping
        if (needToAdjustPosition)
        {
            transform.position += leapOffset;
            needToAdjustPosition = false;
        }

        //clear timer and set new direction
        if (timer > timeToTurn && currentState != MoveState.sit)
        {
            rotateSpeed = Random.Range(-maxRotateSpeed, maxRotateSpeed);
            timer = 0;
        }      

    }

    //event for when frog leap animation is finished
    public void EndFrogLeap()
    {
        animator.SetBool("creep", false);
        animator.SetBool("climb", false);
        animator.Play("sit", 0);
        currentState = MoveState.sit;
        needToAdjustPosition = true;
        timeToSit = Random.Range(sitTimeMin, sitTimeMax);
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnTriggerEnter(Collider other)
    { 
        
        //frogman leaves shore, returns to water
        if (other.tag == Constants.WaterTag && currentState != MoveState.swim)
        {
            currentState = MoveState.swim;
            animator.Play("swim", 0);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //no gravity while swimming
            rb.useGravity = false;
        }
        //frogman approaches shore
        else if (other.tag == Constants.ShoreTag && currentState == MoveState.swim)
        {
            transform.Translate(preleapOffset);
            rb.useGravity = true;
            currentState = MoveState.edgeLeap;
            animator.SetBool("climb", true);
            //add extra "oomph" to the leap
            rb.AddForce(Vector3.up * leapHeight);
            rb.AddForce(Vector3.forward * leapSpeed);
            rb.useGravity = true;
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
        }


        //flee from player on land
        if (other.tag == Constants.PlayerTag && currentState == MoveState.walk)
        {
            if (!other.gameObject.GetComponent<FirstPersonController>().IsCrouching)
            {
                currentState = MoveState.flee;
                fleeFromTarget = other.gameObject.transform;
            }
        }
        else if (other.tag == Constants.AvoidTag)
        {
            currentState = MoveState.flee;
            fleeFromTarget = other.gameObject.transform;
        }

        base.OnTriggerEnter(other);
    }

    public override void AvoidCollision(Collider other, float avoidSpeed)
    {
        //turning sharply away from obstacles causes problems on land; only do it in water
        if (currentState == MoveState.swim)
        {
            base.AvoidCollision(other, avoidSpeed);
        }
    }

    //sitting is frogmans special pose
    public override bool SpecialPose()
    {
        if (currentState == MoveState.sit){ return true; }
        return base.SpecialPose();
    }
}
