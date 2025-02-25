﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnoNightcrawler : Cryptid {

    public float speed;
    public float rotateSpeed;
    public GameObject zone;

    public float minDistance = 3;
    public float maxTime = 45;

    // public float frequency;
    // public float shift;
    // public float forwardShift;
    string Speed = "Speed";

    private Transform fleeFromTarget;
    public float fleeSpeed;
    public float maxDistance;
    public float seeObstacles;

    enum MoveState { Walk, Flee, Dance, Nothing};
    MoveState currentState = MoveState.Walk;

    [SerializeField]
    Texture2D shutdownTxt;

    // Use this for initialization
    void Start () {
        StartUp();
        baseScore = 100;
        cryptidType = Constants.Fresno;

        //inital target position is directly in front, 100 units away
        targetPos = transform.position + transform.forward.normalized * 100;
        timeChasing = 0;
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (lockMovementSuper) { return; }

        //lock rotation: for top heavy cryptids prone to falling over
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        switch (currentState)
        {
            case MoveState.Walk:
                //dont move during stationary parts of animation
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag("still"))
                {
                    break;
                }
                //prioritize obstacle avoidance
                if (!AvoidObstacles(seeObstacles, rotateSpeed))
                {
                    MoveToward(targetPos, speed, rotateSpeed);
                }
                Move(speed);
                timeChasing += Time.deltaTime;
                //if they reach their target position give them a new one on the other side of the zone to get them back on track
                //addition of time tracker lets them change target if they get stuck
                if ((transform.position - targetPos).magnitude < minDistance || timeChasing > maxTime)
                {
                    targetPos = zone.transform.position - (transform.position - zone.transform.position);
                    timeChasing = 0;
                }
                break;
            case MoveState.Flee:
                //dont move during stationary parts of animation
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag("still"))
                {
                    break;
                }
                if (!AvoidObstacles(seeObstacles, rotateSpeed))
                {
                    Flee(fleeFromTarget, fleeSpeed, rotateSpeed + 1);
                }
                Move(fleeSpeed);
                //stop fleeing once jackalope reaches a certain distance from player
                if ((fleeFromTarget.position - transform.position).magnitude > maxDistance)
                {
                    currentState = MoveState.Walk;
                    animator.SetFloat(Speed, 1);
                    targetPos = zone.transform.position - (transform.position - zone.transform.position);
                    timeChasing = 0;
                }
                break;
            case MoveState.Nothing:
                //do nothing
                break;
        }
        
    }

   /* void Move()
    {
        //dont move during stationary parts of animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("still"))
        {
            return;
        }

        //turn if needed
        Vector3 newDir = Vector3.RotateTowards(transform.forward, (targetPos - transform.position), rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDir);

        //move forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        //if they reach their target position (this is not supposed to happen) give them a new one on the other side of the zone to get them back on track
        //addition of time tracker lets them change target if they get stuck
        if ((transform.position - targetPos).magnitude < 3 || timeChasing > 45)
        {
            targetPos = zone.transform.position - (transform.position - zone.transform.position);
            timeChasing = 0;
        }
       
    }*/

    //keep fresno nightcrawlers in their designated zone
    private void OnTriggerExit(Collider other)
    {
        //fresnos turn around and choose new point to walk to- in the center of the zone plus or minus 15 degrees
        targetPos = zone.transform.position - (transform.position - zone.transform.position);

        //rotate
        //https://answers.unity.com/questions/46770/rotate-a-vector3-direction.html
        //targetPos = Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.up) * targetPos;
        timeChasing = 0;
    }

    public override void OnTriggerEnter(Collider other)
    {
        //flee from player if they come in range
        if ((other.tag == Constants.PlayerTag && !other.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsCrouching)
            || other.tag == Constants.AvoidTag)
        {
            /* fleeFromTarget = other.gameObject.transform;
             currentState = MoveState.Flee;
             targetPos = Vector3.zero;
             animator.SetFloat(Speed, 2);*/

            //rather than flee, activate defense mechanism and become unrecognizeable
            rb.useGravity = false;
            animator.SetTrigger("Recede");
            currentState = MoveState.Nothing;
            if (shutdownTxt !=null){
                renderer.material.SetTexture("_MainTex", shutdownTxt);
            }
        }

        base.OnTriggerEnter(other);
    }

    //restore gravity when recede animation is finished
    public void Fall()
    {
        rb.useGravity = true;
    }

    //cryptid does not count when in defensive mode
    public override bool IsVisible()
    {
        if (currentState == MoveState.Nothing)
        {
            return false;
        }

        return base.IsVisible();
    }
}
