using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nessie : Cryptid {

    public float forwardSpeed;
    public float upDownSpeed;
    public float rotateSpeed;

    public float surfacePos = -4;
    public float belowPos = -11;

    enum MoveState { underWaterSwim, aboveWaterSwim, breach, look, waitToSubmerge};
    MoveState currentState;
    bool lookedOnce = true;

    ParticleSystem ripples;

    HeadBone head;
    bool headBonk;

    bool justChangedStates;

    // Use this for initialization
    void Start () {
        StartUp();
        baseScore = 200;
        cryptidType = Constants.Nessie;
        currentState = MoveState.underWaterSwim;
        ripples = GetComponentInChildren<ParticleSystem>();
        head = GetComponentInChildren<HeadBone>();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
        if (lockMovementSuper) { return; }

        switch (currentState)
        {
            case MoveState.underWaterSwim:
                MoveinCircle(forwardSpeed,rotateSpeed);
                if (transform.position.y > belowPos) //the point at which nessies goes deepest
                {
                    
                    transform.Translate(Vector3.down * Time.deltaTime * upDownSpeed/2); //move down until we're fully below water
                    
                }
                else if (RandomChance(.05f))
               //else if (RandomChance(1))
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
                    MoveinCircle(forwardSpeed, rotateSpeed);
                    transform.Translate(Vector3.up * Time.deltaTime * upDownSpeed); //move up until breach

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
                MoveinCircle(forwardSpeed / 2, rotateSpeed / 2); //move half as fast above water
                if (RandomChance(.1f) || (lookedOnce && RandomChance(.2f)))
                //if (RandomChance(0))
                {
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", false);
                    animator.SetBool("Dive", true);
                    currentState = MoveState.underWaterSwim;
                    ripples.Stop();
                    lookedOnce = false; //reset value when nessie descends
                }
                else if(RandomChance(.2f))
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
                MoveinCircle(forwardSpeed / 3, rotateSpeed / 3);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("above water swim"))
                {
                    currentState = MoveState.aboveWaterSwim;
                    animator.SetBool("Look", false);
                    animator.SetBool("MirrorLook", false);
                }
                break;
            case MoveState.waitToSubmerge:
                //give it a frame for animations to init
                if (justChangedStates)
                {
                    justChangedStates = false;
                    break;
                }

                //hang tight until animations have queued back into above water swim
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("above water swim"))
                {
                    //now we return to the depths
                    animator.SetBool("Breach", false);
                    animator.SetBool("Look", false);
                    animator.SetBool("MirrorLook", false);
                    animator.SetBool("Dive", true);
                    currentState = MoveState.underWaterSwim;
                    ripples.Stop();
                    lookedOnce = false; //reset value when nessie descends
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
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
    }

    public override bool IsVisible()
    {
        if (currentState == MoveState.underWaterSwim && (transform.position.y < surfacePos) && animator.GetCurrentAnimatorStateInfo(0).IsName("underwater swim"))
        {
            return false;
        }
        return base.IsVisible();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        //detect if this is a headbonk or not
        headBonk = false;
        ContactPoint contact = collision.GetContact(0);
        Collider headBone = head.GetComponent<Collider>();
        if (headBone != null)
        {
            if (contact.thisCollider == headBone || contact.otherCollider == headBone)
            {
                headBonk = true;
            }
        }

        base.OnCollisionEnter(collision);
    }

    public override void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
        //nessie can only be bonked above water
        if (currentState != MoveState.aboveWaterSwim && currentState != MoveState.look && currentState != MoveState.breach) { return; }

        //if nessie gets bonked on her head, play the bonk animation
        if (headBonk) { base.GetBonked(leftImpact, bonked); } //bonk transitions into look in animator
        //if nessie gets bonked anywhere else, skip the bonk animation. just turn to look 
        else
        {
            if (leftImpact) { animator.SetBool("Look", true); }
            else { animator.SetBool("MirrorLook", true); }
        }

        //after look animations finish go back underwater
        currentState = MoveState.waitToSubmerge;
        justChangedStates = true;

        //it's possible for a carrot to land on nessie's back and stay there; deactivate object to prevent it from triggering bonks forever
        if (bonked != null) { bonked.Active = false; }
    }

    public override bool SpecialPose()
    {
        //nessie's cheeky look is her special pose
        if (animator.GetBool("Look") || animator.GetBool("MirrorLook")){ return true; }

        return base.SpecialPose();
    }
}
