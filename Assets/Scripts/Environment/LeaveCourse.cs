using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//prompt user to end the course when they reach the end
public class LeaveCourse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.EndPrompt(Constants.LeaveCoursePrompt);
    }


}
