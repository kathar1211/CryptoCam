using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovelandFrogman : Cryptid {

    Animator animator;
    ParticleSystem ripples;

    //keep track of frogmans move state, serializable bc his default state isnt set in stone yet
    enum MoveState { swim, walk, edgeLeap, sit}
    [SerializeField] MoveState currentState;

    //amount position needs to be adjusted after a leap
    [SerializeField]
    Vector3 leapOffset;

    public float walkSpeed;
    public float swimSpeed;

    bool needToAdjustPosition; //used to adjust transform information to match up with visuals after an edge leap

    //for demo video purposes only: in game frogman will respond to triggers not specific locations
    Vector3 LeapPosition = new Vector3(-294.82f, .4f, -14.49f);

	// Use this for initialization
	void Start () {
        cryptidType = "Loveland Frogman";
        baseScore = 300;
        animator = GetComponent<Animator>();
        ripples = GetComponentInChildren<ParticleSystem>();

        //convert offset to be relative to forgmans direction
        Vector3 upMove = new Vector3(transform.up.x * leapOffset.y, transform.up.y * leapOffset.y, transform.up.z * leapOffset.y);
        Vector3 forwardMove = new Vector3(transform.forward.x * leapOffset.z, transform.forward.y * leapOffset.z, transform.forward.z * leapOffset.z);
        leapOffset = upMove + forwardMove;
    }
	
	// Update is called once per frame
	void Update () {

        //TEMP: animator determines states rather than vice versa
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ledgeClimb"))
        {
            currentState = MoveState.edgeLeap;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("sit"))
        {
            currentState = MoveState.sit;
            //turn gravity back on after exiting water
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("creep"))
        {
            currentState = MoveState.walk;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("swim"))
        {
            currentState = MoveState.swim;
        }


        switch (currentState)
        {
            //movement states
            case MoveState.swim:
                transform.Translate(Vector3.forward * Time.deltaTime * swimSpeed);
                //no gravity while swimming
                this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                break;
            case MoveState.walk:
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);

                break;
        }

        //check when to start leapin
        if ((transform.position - LeapPosition).magnitude <= .2f)
        {
           
        }

        //apply position offset
        if (needToAdjustPosition)
        {
            transform.position += leapOffset;
            needToAdjustPosition = false;
        }
        

    }

    //event for when frog leap animation is finished
    public void EndFrogLeap()
    {
        
        animator.Play("sit", 0);
        needToAdjustPosition = true;
    

    }

    private void OnTriggerEnter(Collider other)
    {
        //frogman approaches shore
        if (other.tag == "Shore")
        {
            currentState = MoveState.edgeLeap;
            animator.SetBool("climb", true);
        }
    }
}
