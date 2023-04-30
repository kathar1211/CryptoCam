using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nessie : Cryptid {

    public float speed;
    public float rotateSpeed;

    float timeElapsed;
    public float timeUntilBreach; //in seconds

    public float surfacePos = -4;
    public float belowPos = -11;

    enum MoveState { underWaterSwim, aboveWaterSwim, breach, look};
    MoveState currentState;
    bool lookedOnce = true;

    ParticleSystem ripples;

    AudioSource AudioSource;

    public AudioClip SwimSound; //looping sfx while nessie swims above water
    public AudioClip BreachSound; //single instance sfx when nessie breaches
    public AudioClip SubmergeSound; //sfx when nessie dips below the surface

    // Use this for initialization
    void Start () {
        StartUp();
        cryptidType = Constants.Nessie;
        currentState = MoveState.underWaterSwim;
        timeElapsed = 0;
        ripples = GetComponentInChildren<ParticleSystem>();
        AudioSource = this.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
        if (lockMovementSuper) { return; }

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
                else if (RandomChance(.05f))
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
                if (RandomChance(.1f) || (lookedOnce && RandomChance(.4f)))
                {
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", false);
                    animator.SetBool("Dive", true);
                    currentState = MoveState.underWaterSwim;
                    lookedOnce = false; //reset value when nessie descends
                }
                else if(RandomChance(.4f))
                {
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", true);
                    animator.SetBool("Dive", false);
                    lookedOnce = true; //curb back on consecutive looking around animations by increasing the chance of descending after one look per surface appearance
                      
                    currentState = MoveState.look;
                    
                }
                
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

    public override bool IsVisible()
    {
        if (currentState == MoveState.underWaterSwim && (transform.position.y < surfacePos) && animator.GetCurrentAnimatorStateInfo(0).IsName("underwater swim"))
        {
            return false;
        }
        return base.IsVisible();
    }
}
