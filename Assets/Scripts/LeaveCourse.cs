using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//prompt user to end the course when they reach the end
public class LeaveCourse : MonoBehaviour {

    GameManager gm;
	// Use this for initialization
	void Start () {
        gm = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerEnter(Collider other)
    {
        if (gm != null)
        gm.EndPrompt(Constants.LeaveCoursePrompt);
    }


}
