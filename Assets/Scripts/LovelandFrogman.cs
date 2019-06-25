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

    bool needToAdjustPosition; //used to adjust transform information to match up with visuals after an edge leap

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
        }

        if (needToAdjustPosition)
        {
            this.transform.position += leapOffset;
            needToAdjustPosition = false;
        }

	}

    //event for when frog leap animation is finished
    public void EndFrogLeap()
    {
        
        animator.Play("sit", 0);
        needToAdjustPosition = true;

    }
}
