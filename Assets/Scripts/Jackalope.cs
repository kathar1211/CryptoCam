using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackalope : Cryptid {

    //wandering properties
    public float distance;
    public float runSpeed;
    public float rotateSpeed;
    float minDistance;

    //flee properties
    Transform fleeFromTarget;
    public float maxDistance;
    public float fleeSpeed;

    Animator animator;
    float animationDuration;
    float timeElapsed;


    enum MoveState { run, stand, scratch, flee};
    MoveState currentState;

    // Use this for initialization
    void Start () {
        StartUp();
        targetPos = transform.position + Random.insideUnitSphere * distance;
        targetPos.y = transform.position.y;
        currentState = MoveState.run;
        animator = GetComponent<Animator>();
        cryptidType = "Jackalope";
        baseScore = 50;
        minDistance = 3;
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
                Wander(maxDistance, minDistance, runSpeed, rotateSpeed);
                //Wander(distance, minDistance, runSpeed, rotateSpeed);
                if (Random.Range(0.0f,100.0f) > 99.9f)
                {
                    currentState = MoveState.scratch;
                    animationDuration = Random.Range(2.0f, 6.0f);
                    timeElapsed = 0;
                    //animator.Play("scratch n sniff");
                    animator.SetBool("Sniff", true);
                    animator.SetBool("Run", false);
                }
                else if (Random.Range(0.0f, 100.0f) > 99.9f)
                {
                    currentState = MoveState.stand;
                    animationDuration = Random.Range(1.0f, 4.9f);
                    timeElapsed = 0;
                    //animator.Play("stand n sniff");
                    animator.SetBool("StandUp", true);
                    animator.SetBool("Run", false);
                }
                break;

            //stand still and do animation
            case MoveState.scratch:
            case MoveState.stand:
                //return to wandering after random amount of time
                if (timeElapsed >= animationDuration)
                {
                    currentState = MoveState.run;
                    animator.SetBool("StandUp", false);
                    animator.SetBool("Run", true);
                    animator.SetBool("Sniff", false);
                    //animator.Play("run");
                }

                break;
            //move away from the player
            case MoveState.flee:
                Flee(fleeFromTarget, fleeSpeed, rotateSpeed + 1);
                //stop fleeing once jackalope reaches a certain distance from player
                if ((fleeFromTarget.position - transform.position).magnitude > maxDistance)
                {
                    currentState = MoveState.run;
                    animator.SetFloat("Speed", 1);
                }
                break;

        }
	}

    private void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if (other.tag == "Player")
        {
            fleeFromTarget = other.gameObject.transform;
            currentState = MoveState.flee;
            animator.SetBool("StandUp", false);
            animator.SetBool("Run", true);
            animator.SetBool("Sniff", false);
            targetPos = Vector3.zero;
            animator.SetFloat("Speed", 2);
        }
    }


}
