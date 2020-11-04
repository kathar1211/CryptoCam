using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsuchinoko : Cryptid {

    public float speed;
    public float rotateSpeed;
    public float fleespeed;
    Animator animator;
    //percent chance that tsuchinoko will switch between being upright or not any given frame
    public float chanceUpDown;
    enum MoveState { Circling, Seeking, Fleeing};
    MoveState currentMovestate = MoveState.Circling;

    //used when moving to/away from a particular thing
    Transform target;

    //used when moving to a particular spot
    [SerializeField]
    Transform secondLocation;

    //distance at which tsuchinoko no longer flees from something
    public float maxDistance;
    //distance at which tsuchinoko no longer seeks something
    public float minDistance;
    //distance at which tsuchinoko sees obstacles
    public float seeAhead;

    // Use this for initialization
    void Start () {
        baseScore = 75;
        cryptidType = "Tsuchinoko";
        animator = this.GetComponent<Animator>();
        animator.SetFloat("Speed", 1);
        target = secondLocation;
        StartUp();
    }
	
	// Update is called once per frame
	void Update () {

        //lock rotation: tsuchinoko is top heavy and has some trouble staying upright
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        switch (currentMovestate)
        {
            //default state: tsuchinoko slithers in a little circle
            case MoveState.Circling:
                Move(speed, rotateSpeed);
                //AvoidObstacles(seeAhead, rotateSpeed);
                //tsuchinoko only goes upright in his relaxed state
                if (Random.Range(0.0f, 100.0f) < chanceUpDown)
                {
                    ToggleRiseLower();
                }
                break;
            //tsuchinoko moves towards something until he is within a certain range of it
            case MoveState.Seeking:
                
                //if (!AvoidObstacles(seeAhead, rotateSpeed))
                {
                    MoveToward(target, fleespeed, rotateSpeed);
                }
                Move(fleespeed);
                if ((secondLocation.position - transform.position).magnitude < minDistance)
                {
                    currentMovestate = MoveState.Circling;
                    animator.SetFloat("Speed", 1);
                }
                break;
            //tsuchinoko moves away from something until he is outside a certain range of it
            case MoveState.Fleeing:
                
                //if (!AvoidObstacles(seeAhead, rotateSpeed))
                {
                    Flee(target, fleespeed, rotateSpeed);
                }
                Move(fleespeed);
                if ((target.position - transform.position).magnitude > maxDistance)
                {
                    currentMovestate = MoveState.Circling;
                    animator.SetFloat("Speed", 1);
                }
                break;

        }



        
	}

    //tsuchinoko switches between upright and lurking
    void ToggleRiseLower()
    {
        bool isUpright = animator.GetBool("Upright");
        animator.SetBool("Upright", !isUpright);
    }

    //set upright state
    void SetUpright(bool state)
    {
        animator.SetBool("Upright", state);
    }

    public override void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if (other.tag == "Player")
        {
            target = other.gameObject.transform;
            currentMovestate = MoveState.Fleeing;
            SetUpright(false);
            animator.SetFloat("Speed", fleespeed/speed);
        }

        base.OnTriggerEnter(other);
    }

    //triggers tsuchinokos decision to move to new location
    public override void AvoidPlayer(Collider other)
    {
        target = secondLocation;
        currentMovestate = MoveState.Seeking;
        SetUpright(false);
        animator.SetFloat("Speed", fleespeed / speed);
    }

    //tsuchinokos special pose is when he sits up
    public override bool SpecialPose()
    {
        bool isUpright = animator.GetBool("Upright");
        if (isUpright) { return true; }

        return base.SpecialPose();
    }
}
