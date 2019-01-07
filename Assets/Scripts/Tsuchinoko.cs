using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsuchinoko : Cryptid {

    public float speed;
    public float rotateSpeed;
    Animator animator;

    // Use this for initialization
    void Start () {
        baseScore = 75;
        cryptidType = "Tsuchinoko";
        animator = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        //lock rotation: tsuchinoko is top heavy and has some trouble staying upright
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        Move();

        
	}

    void Move()
    {
        //move forward
        //transform.Translate(Vector3.forward * (Mathf.Cos(frequency*Time.time + shift)+forwardShift) * speed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        //turn riht
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }

    //tsuchinoko assumes the upright position
    void Rise()
    {
        animator.SetBool("Upright", true);
    }

    //tsuchinoko assumes the downright position
    void Lower()
    {
        animator.SetBool("Upright", false);
    }
}
