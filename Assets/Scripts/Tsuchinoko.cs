using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsuchinoko : Cryptid {

    public float speed;
    public float rotateSpeed;
    Animator animator;
    //percent chance that tsuchinoko will switch between being upright or not any given frame
    public float chanceUpDown;

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

        if (Random.Range(0.0f, 100.0f) < chanceUpDown)
        {
            ToggleRiseLower();
        }

        
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
    /*void Rise()
    {
        animator.SetBool("Upright", true);
    }

    //tsuchinoko assumes the downright position
    void Lower()
    {
        animator.SetBool("Upright", false);
    }*/

    void ToggleRiseLower()
    {
        animator.SetBool("Upright", !animator.GetBool("Upright"));
    }
}
