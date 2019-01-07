using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnoNightcrawler : Cryptid {

    public float speed;
    public float rotateSpeed;
    Vector3 targetPos;
    public GameObject zone;
    float timeChasing;

    // public float frequency;
    // public float shift;
    // public float forwardShift;
    Animator animator;

    // Use this for initialization
    void Start () {
        baseScore = 100;
        cryptidType = "Fresno Nightcrawler";
        animator = this.GetComponent<Animator>();


        //inital target position is directly in front, 100 units away
        targetPos = transform.position + transform.forward.normalized * 100;
        timeChasing = 0;
	}

    // Update is called once per frame
    void Update()
    {
        //lock rotation: for top heavy cryptids prone to falling over
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        Move();
        timeChasing += Time.deltaTime;
    }

    void Move()
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
        //if ((transform.position - targetPos).magnitude < 3 || timeChasing > 30)
        //{
            
        //    timeChasing = 0;
        //}
       
    }

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
}
