using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cryptid : MonoBehaviour {

    //thins all cryptids need:
    //base score for photoraphin
    public int baseScore;
    //multiple hitboxes to check visibility
    //variable to keep track of current animation, or at least whether or not its special
    //a name for scorin pruposes(?)
    public string cryptidType;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //standard method to move forward some amount and to turn some amount
    public void Move(float forwardSpeed, float rotateSpeed)
    {
        //move forward
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);

        //turn right
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }
}
