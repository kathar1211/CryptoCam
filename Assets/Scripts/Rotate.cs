using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    //quick script to rotate objects for display purposes

    public float speed = 1;
    public bool isCamera;
    public GameObject target;
    public float yOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isCamera)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * speed);
        }
        if (isCamera)
        {
            //offset
            transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y + yOffset, target.transform.position.z));
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
	}
}
