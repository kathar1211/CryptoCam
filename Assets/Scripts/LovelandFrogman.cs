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
    float rotateSpeed;
    public float maxRotateSpeed;

    bool needToAdjustPosition; //used to adjust transform information to match up with visuals after an edge leap

    [SerializeField]
    float timeToTurn; //how often to switch directions
    float timeToSit; //how long to sit after a leap
    [SerializeField]
    float sitTimeMax;
    [SerializeField]
    float sitTimeMin;
    private float timer;

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

        rotateSpeed = Random.Range(-maxRotateSpeed, maxRotateSpeed);
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        /*
        //TEMP: animator determines states rather than vice versa
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ledgeClimb"))
        {
            currentState = MoveState.edgeLeap;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("sit"))
        {
            currentState = MoveState.sit;
            
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("creep"))
        {
            currentState = MoveState.walk;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("swim"))
        {
            currentState = MoveState.swim;
        }*/


        switch (currentState)
        {
            //movement states
            case MoveState.swim:
                Move(swimSpeed, rotateSpeed);
                //no gravity while swimming
                this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                Move(swimSpeed, rotateSpeed);
                break;
            case MoveState.walk:
                Move(walkSpeed, rotateSpeed);
                //turn gravity back on after exiting water
                this.gameObject.GetComponent<Rigidbody>().useGravity = true;
                Move(walkSpeed, rotateSpeed);
                break;
            case MoveState.sit:
                if (timer > timeToSit)
                {
                    timer = 0;
                    animator.SetBool("creep", true);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //frogman leaves shore, returns to water
        if (other.tag == "Water" && currentState == MoveState.walk)
        {
            currentState = MoveState.swim;
            animator.Play("swim", 0);
        }
        //frogman approaches shore
        else if (other.tag == "Shore" && currentState == MoveState.swim)
        {
            currentState = MoveState.edgeLeap;
            animator.SetBool("climb", true);
        }
    }

}
