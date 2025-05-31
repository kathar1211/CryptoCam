using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceForward : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main == null) { return; }

		//sprite always facin camera
		Vector3 towardsCamera = this.transform.position - Camera.main.transform.position;
		Quaternion rotation = Quaternion.LookRotation(towardsCamera, Vector3.up);
		transform.rotation = rotation;
    }
}
