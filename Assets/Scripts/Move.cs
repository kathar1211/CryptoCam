using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float speed;
    public float rotateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //move forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        //turn riht
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
	}
}
