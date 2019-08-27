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

        Move(speed, rotateSpeed);

        if (Random.Range(0.0f, 100.0f) < chanceUpDown)
        {
            ToggleRiseLower();
        }

        
	}

    //tsuchinoko switches between upright and lurking
    void ToggleRiseLower()
    {
        animator.SetBool("Upright", !animator.GetBool("Upright"));
    }
}
