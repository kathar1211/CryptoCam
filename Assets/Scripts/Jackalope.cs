using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackalope : Cryptid {

    //wanderin properties
    public float distance;
    public float runSpeed;
    public float rotateSpeed;
    Vector3 targetPos;
    Animator animator;
    float animationDuration;
    float timeElapsed;

    enum MoveState { run, stand, scratch};
    MoveState currentState;

    // Use this for initialization
    void Start () {
        targetPos = transform.position + Random.insideUnitSphere * distance;
        targetPos.y = transform.position.y;
        currentState = MoveState.run;
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        switch (currentState)
        {
            case MoveState.run:
                Wander();
                //random chance of switchin states
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

            case MoveState.scratch:
            case MoveState.stand:
                //return to runnin after random amount of time
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= animationDuration)
                {
                    currentState = MoveState.run;
                    animator.SetBool("StandUp", false);
                    animator.SetBool("Run", true);
                    animator.SetBool("Sniff", false);
                    //animator.Play("run");
                }

                break;

        }
	}

    //move randomly in 2d space
    void Wander()
    {
        //choose a random taret position within rane and move towards it
        //https://answers.unity.com/questions/23010/ai-wandering-script.html

        

        if((transform.position - targetPos).magnitude < 3)
        {
            targetPos = transform.position + transform.forward*(distance/2.0f) +  Random.insideUnitSphere * distance;
            targetPos.y = transform.position.y;
            //transform.LookAt(targetPos);
        }

        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);
        Debug.Log(targetPos);
    }
}
