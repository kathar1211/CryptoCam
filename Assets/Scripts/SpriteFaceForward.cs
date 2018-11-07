using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceForward : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //sprite always facin camera
        transform.forward = Camera.main.transform.forward;
    }
}
