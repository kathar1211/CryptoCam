using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsuchinoko : Cryptid {

    public float speed;
    public float rotateSpeed;
    public float circlingRotateSpeed;
    public float fleespeed;
    //percent chance that tsuchinoko will switch between being upright or not any given frame
    public float chanceUpDown;
    public enum MoveState { Circling, Seeking, Fleeing, Sleeping, Awake};
    public MoveState currentMovestate = MoveState.Circling;

    private MoveState nextState; //queue a movestate for after tsuchinoko wakes up

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

    public CryptidTrigger trigger;

    // Use this for initialization
    void Start () {
        StartUp();
        baseScore = 75;
        cryptidType = Constants.Tsuchinoko;
        animator.SetFloat("Speed", 1);
        target = secondLocation;

        //if tsuchinoko starts asleep make sure to coordingate animations
        if (currentMovestate == MoveState.Sleeping)
        {
            animator.Play("sleep");
        }

        if (trigger != null)
        {
            trigger.TriggerEnterAction += TriggerMove;
        }
    }
	
	// Update is called once per frame
	protected override void Update () {

        base.Update();
        if (lockMovementSuper) { return; }

        //lock rotation: tsuchinoko is top heavy and has some trouble staying upright
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        switch (currentMovestate)
        {
            //default state: tsuchinoko slithers in a little circle
            case MoveState.Circling:
                Move(speed, circlingRotateSpeed);
                //tsuchinoko only goes upright in his relaxed state
                if (Random.Range(0.0f, 100.0f) < chanceUpDown)
                {
                    ToggleRiseLower();
                }
                break;
            //tsuchinoko moves towards something until he is within a certain range of it
            case MoveState.Seeking:
                
                if (nav.destination != target.position) { SetupNavMeshSeek(); }

                //motion is handled by navmesh agent, but check here if we get in range
                if ((target.position - transform.position).magnitude < minDistance)
                {
                    KillNavMeshMovement();
                    currentMovestate = MoveState.Circling;
                    animator.SetFloat("Speed", 1);
                }
                Debug.DrawRay(transform.position, target.position - transform.position, Color.yellow);

                break;
            //tsuchinoko moves away from something until he is outside a certain range of it
            case MoveState.Fleeing:

                Flee(target, fleespeed);
                if ((target.position - transform.position).magnitude > maxDistance)
                {
                    KillNavMeshMovement();
                    currentMovestate = MoveState.Circling;
                    animator.SetFloat("Speed", 1);
                }
                break;
            case MoveState.Sleeping:
                //do nothing
                break;
            case MoveState.Awake:
                //check for the wake up animation to end before moving again
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("downSlither"))
                {
                    currentMovestate = nextState;
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
        if (other.tag == Constants.PlayerTag)
        {
            if (!other.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsCrouching)
            {
                target = other.gameObject.transform;
                currentMovestate = MoveState.Fleeing;
                SetUpright(false);
                animator.SetFloat("Speed", fleespeed / speed);
                nav.speed = fleespeed;
                SetNavmeshFleeTarget(target);
            }
        }
        else if (other.tag == Constants.AvoidTag)
        {
            target = other.gameObject.transform;
            currentMovestate = MoveState.Fleeing;
            SetUpright(false);
            animator.SetFloat("Speed", fleespeed / speed);
            nav.speed = fleespeed;
            SetNavmeshFleeTarget(target);
        }

        base.OnTriggerEnter(other);
    }

    //triggers tsuchinokos decision to move to new location
    public void TriggerMove()
    {
        target = secondLocation;
        currentMovestate = MoveState.Seeking;
        
    }

    private void SetupNavMeshSeek()
    {
        UnKillNavMeshMovement();
        nav.destination = target.position;
        nav.speed = fleespeed;
        SetUpright(false);
        animator.SetFloat("Speed", fleespeed / speed);
        SetNavMeshChaseTarget(target);
    }

    //tsuchinokos special pose is when he sits up
    public override bool SpecialPose()
    {
        bool isUpright = animator.GetBool("Upright");
        if (isUpright) { return true; }

        return base.SpecialPose();
    }

    //tsuchinoko is only bonkable when upright
    public override void GetBonked(bool leftImpact, BonkableObject bonked = null)
    {
        bool isUpright = animator.GetBool("Upright");
        if (isUpright) { animator.Play("bonk"); }
    }

    //wake up tsuchinoko, and specify what he should do after he's awake
    public void WakeTsuchinoko(MoveState postWakeAction, Transform postWakeTarget = null)
    {
        Debug.Log("tsuchinoko awaken");
        if (currentMovestate != MoveState.Sleeping) { return; }

        currentMovestate = MoveState.Awake;
        animator.SetTrigger("Awake");
        nextState = postWakeAction;
        if (postWakeTarget != null) { 
            target = postWakeTarget; 
        }
    }
}
