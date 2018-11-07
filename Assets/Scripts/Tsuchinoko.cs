using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsuchinoko : Cryptid {

    public float speed;
    public float rotateSpeed;

    // Use this for initialization
    void Start () {
        baseScore = 75;
        cryptidType = "Tsuchinoko";
    }
	
	// Update is called once per frame
	void Update () {
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
}
