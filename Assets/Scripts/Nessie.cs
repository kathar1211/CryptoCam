using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nessie : Cryptid {

    public float speed;
    public float rotateSpeed;

    Animator animator;
    float timeElapsed;
    public float timeUntilBreach; //in seconds

    public float surfacePos = -4;
    public float belowPos = -11;

    enum MoveState { underWaterSwim, aboveWaterSwim, breach, look};
    MoveState currentState;

    ParticleSystem ripples;

    // Use this for initialization
    void Start () {
        cryptidType = "Loch Ness Monster";
        baseScore = 500;
        currentState = MoveState.underWaterSwim;
        timeElapsed = 0;
        animator = GetComponent<Animator>();
        ripples = GetComponentInChildren<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime;

        switch (currentState)
        {
            case MoveState.underWaterSwim:
                MoveinCircle(speed,rotateSpeed);
                if (transform.position.y > belowPos) //the point at which nessies goes deepest
                {
                    
                    transform.Translate(Vector3.down * Time.deltaTime * speed/2); //move down until breach
                    ripples.Stop();
                }
                else if (RandomChance(.1f))
                {
                    //animator.SetBool("Breach", true);
                    //animator.SetBool("Look", false);
                    //animator.SetBool("Dive", false);
                    currentState = MoveState.breach;
                }

                break;
            case MoveState.breach:
                if (transform.position.y < surfacePos) //the point at which nessies body peeks out of the water
                {
                    MoveinCircle(speed, rotateSpeed);
                    transform.Translate(Vector3.up * Time.deltaTime * speed); //move up until breach

                    animator.SetBool("Breach", true);
                    animator.SetBool("Look", false);
                    animator.SetBool("Dive", false);
                }
                else
                {
                    
                    currentState = MoveState.aboveWaterSwim;
                    ripples.Play();
                }
                break;
            case MoveState.aboveWaterSwim:
                MoveinCircle(speed / 2, rotateSpeed / 2); //move half as fast above water
                if (RandomChance(.4f))
                {
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", true);
                    animator.SetBool("Dive", false);
                 
                      
                    currentState = MoveState.look;
                    
                }
                /*else if (RandomChance(.1f))
                {
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", false);
                    animator.SetBool("Dive", true);
                    currentState = MoveState.underWaterSwim;
                }*/
                break;
            case MoveState.look:
                //move wile looking around
                MoveinCircle(speed / 3, rotateSpeed / 3);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("above water swim"))
                {
                    currentState = MoveState.aboveWaterSwim;
                    animator.SetBool("Look", false);
                    animator.SetBool("MirrorLook", false);
                }
                break;
        }
	}

    void MoveinCircle(float forwardSpeed, float sideSpeed)
    {
        //move forward
        //transform.Translate(Vector3.forward * (Mathf.Cos(frequency*Time.time + shift)+forwardShift) * speed);
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);

        //turn riht
        transform.Rotate(Vector3.up * Time.deltaTime * sideSpeed);
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    bool RandomChance(float percentChanceofSuccess)
    {
        //returns true or false, based on parameter for success
        //percent chance of success should be a float between 0 and 100 
        if (percentChanceofSuccess >= 100)
        {
            return true;
        }

        if (percentChanceofSuccess <= 0)
        {
            return false;
        }

        float r = Random.Range(0.0f, 100.0f);
        if ( r > percentChanceofSuccess)
        {
            return false;
        }

        return true;
    }
}
