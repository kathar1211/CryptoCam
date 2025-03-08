using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin2d : MonoBehaviour {

    public int rotateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, rotateSpeed, 0);
	}
}
