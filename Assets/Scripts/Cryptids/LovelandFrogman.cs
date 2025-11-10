using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LovelandFrogman : Cryptid {

    ParticleSystem ripples;

    //keep track of frogmans move state, serializable bc his default state isnt set in stone yet
    enum MoveState { swim, walk, edgeLeap, sit, flee, stand, floating}
    [SerializeField] MoveState currentState;

    //amount position needs to be adjusted after a leap
    [SerializeField]
    Vector3 leapOffset;


    public float walkSpeed;
    public float swimSpeed;
    public float leapSpeed;
    public float leapHeight;
    public float rotateSpeed;
    public float changeTargetTime; //how often should direction change during wander behavior

    //properties for fleeing
    public float fleeSpeed;
    Transform fleeFromTarget;
    public float safeZone; //distance at which we no longer flee

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
    public float swimBaseOffset = -2.75f; //used to scooch navmesh base so frog sits in the water instead of on top of it

	// Use this for initialization
	void Start () {
        StartUp();
        cryptidType = Constants.Frogman;
        baseScore = 35;
        ripples = GetComponentInChildren<ParticleSystem>();

        //set up animator for starting state
        if (currentState == MoveState.walk)
        {
            animator.SetBool("creep", true);
            animator.SetBool("climb", false);
            animator.SetBool("swim", false);
            nav.baseOffset = 0;
        }
        else if (currentState == MoveState.sit)
        {
            animator.SetBool("creep", false);
            animator.SetBool("climb", false);
            animator.SetBool("swim", false);
            animator.Play("sit");
            timeToSit = Random.Range(sitTimeMin, sitTimeMax);
            nav.baseOffset = 0;
        }
        else if (currentState == MoveState.swim)
        {
            swimHeight = transform.position.y;
            nav.baseOffset = swimBaseOffset;
        }
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if (lockMovementSuper) { return; }

        

        switch (currentState)
        {
            //movement states
            case MoveState.swim:

                Wander(targetMaxDistance, targetMinDistance);

                if (swimHeight != -1 && transform.position.y != swimHeight)
                {
                    transform.Translate(Vector3.up * (swimHeight - transform.position.y) * swimSpeed * Time.deltaTime);
                }
                break;
            case MoveState.walk:

                Wander(targetMaxDistance, targetMinDistance);

                break;
            case MoveState.stand:
            case MoveState.sit:
                timer += Time.deltaTime;
                if (timer > timeToSit)
                {
                    timer = 0;
                    animator.SetBool("creep", true);
                    currentState = MoveState.walk;
                    nav.speed = walkSpeed;
                    nav.baseOffset = 0;
                }
                break;
            case MoveState.floating:
                timer += Time.deltaTime;
                if (timer > timeToSit)
                {
                    timer = 0;
                    animator.SetBool("swim", true);
                    currentState = MoveState.swim;
                    nav.speed = swimSpeed;
                    nav.baseOffset = swimBaseOffset;
                }
                break;
            case MoveState.edgeLeap:
                //Leap(leapSpeed, 0);
                break;
            case MoveState.flee:

                DirectFlee(fleeFromTarget, fleeSpeed);
                if ((fleeFromTarget.position - transform.position).magnitude > safeZone)
                {
                    currentState = MoveState.walk;
                    nav.speed = walkSpeed;
                    nav.baseOffset = 0;
                }
                break;
        }

    }

    //event for when frog leap animation is finished
    public void EndFrogLeap()
    {
        animator.SetBool("creep", false);
        animator.SetBool("climb", false);
        animator.SetBool("swim", false);
        currentState = MoveState.sit;
        AdjustPosition(true);
        timeToSit = Random.Range(sitTimeMin, sitTimeMax);
    }

    //event for when we're about to push the frog off the ledge
    public void StartFrogFall()
    {
        rb.useGravity = false;
        AdjustPosition(false);
        timeToSit = Random.Range(sitTimeMin, sitTimeMax);
        currentState = MoveState.floating;
        animator.SetBool("swim", false);

        
    }

    //set this midway through frogmans leap so its a little less jarring when it turns back on
    public void EnableGravity()
    {
        rb.useGravity = true;
    }

    //used when we need to snap frogman to a new position before/after doing animations that move him
    private void AdjustPosition(bool preleap)
    {
        //convert offset to be relative to frogmans direction
        Vector3 upMove = new Vector3(transform.up.x * leapOffset.y, transform.up.y * leapOffset.y, transform.up.z * leapOffset.y);
        Vector3 forwardMove = new Vector3(transform.forward.x * leapOffset.z, transform.forward.y * leapOffset.z, transform.forward.z * leapOffset.z);
        Vector3 totalMove = (upMove * transform.localScale.y) + (forwardMove * transform.localScale.z);

        if (preleap) {
            transform.position += totalMove; 
        }
        else {
            transform.position -= totalMove;
        }
    }

    public override void OnTriggerEnter(Collider other)
    { 
        
        //dont try to handle collisions mid leap
        if (animator.GetBool("climb")) { return; }

        //frogman leaves shore, returns to water
        if (other.tag == Constants.WaterTag && currentState != MoveState.swim && currentState != MoveState.sit)//somethings happening here
        {
            currentState = MoveState.swim;
            nav.speed = swimSpeed;
            nav.baseOffset = swimBaseOffset;
            animator.SetBool("swim", true);
            animator.SetBool("creep", false);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //no gravity while swimming
            rb.useGravity = false;
        }
        //frogman approaches shore
        else if (other.tag == Constants.ShoreTag && currentState == MoveState.swim)
        {
           // transform.Translate(preleapOffset);
            currentState = MoveState.edgeLeap;
            animator.SetBool("climb", true);
            KillNavMeshMovement();
            nav.enabled = false;
            //add extra "oomph" to the leap
            rb.AddForce(Vector3.up * leapHeight);
            rb.AddForce(Vector3.forward * leapSpeed);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            

            //debug
            GameObject.Instantiate(new GameObject(), this.transform);
        }


        //flee from player on land
        if (other.tag == Constants.PlayerTag && currentState == MoveState.walk)
        {
            if (!other.gameObject.GetComponent<FirstPersonController>().IsCrouching)
            {
                currentState = MoveState.flee;
                fleeFromTarget = other.gameObject.transform;
                KillNavMeshMovement();
            }
        }
        else if (other.tag == Constants.AvoidTag)
        {
            currentState = MoveState.flee;
            fleeFromTarget = other.gameObject.transform;
            KillNavMeshMovement();
        }

        base.OnTriggerEnter(other);
    }

    //leaping is frogmans special pose
    public override bool SpecialPose()
    {
        if (animator.GetBool("climb")){ return true; }
        else if (currentState == MoveState.sit) { return true; }
        return base.SpecialPose();
    }

    public override void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
       switch (currentState)
        {
            case MoveState.sit:
                //sit looks for forward and back rather than left/right, so we have to calculate impact direction again
                //line from cryptid to carrot
                Vector3 bonkDistance = this.gameObject.transform.position - bonked.gameObject.transform.position;

                //if the line from the cryptid to the carrot is in the same direction as the cryptid's forward vector,
                //then the carrot is in front of the cryptid
                bool frontImpact = false;
                if (Vector3.Dot(this.transform.forward, bonkDistance) < 0)
                {
                    frontImpact = true;
                }

                if (frontImpact) { animator.Play("frogman_sit_bonk_forward"); }
                else { animator.Play("sit_bonk_back"); }

                break;
            case MoveState.walk:
                if (leftImpact)
                {
                    animator.Play("stand_bonk_left");
                }
                else
                {
                    animator.Play("stand_bonk_right");
                }
                break;
            case MoveState.swim:
                if (leftImpact)
                {
                    animator.Play("swim_bonk_left");
                }
                else
                {
                    animator.Play("swim_bonk_right");
                }
                break;
        }
    }
}
