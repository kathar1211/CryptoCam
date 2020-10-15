using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraBehavior : MonoBehaviour {

    //how long do we show the view from this camera (in seconds)
    public float ActiveDuration;
    private float timer = 0;
    private bool active = false;
    //how fast does the camera move
    public float MovementSpeed;
    //what direction does the camera move
    public Vector3 MovementDirection;
    //true if camera should move relative to its own orientation rather than the world orientation
    public bool RelativeMovement;
    //allow camera to slow towards the end of its movements
    public float SlowDownIncrement;
    //Vector3 startPos;
    //Vector3 endPos;

	// Use this for initialization
	void Start () {
        MovementDirection = MovementDirection.normalized;
        /*startPos = this.transform.position;
        if (RelativeMovement)
        {
            Vector3 rightMove = this.transform.right * MovementDirection.x * MovementSpeed * ActiveDuration;
            Vector3 forwardMove = this.transform.forward * MovementDirection.z * MovementSpeed * ActiveDuration;
            Vector3 upMove = this.transform.up * MovementDirection.y * MovementSpeed * ActiveDuration;
            endPos = startPos + rightMove + forwardMove + upMove;
        }
        else
        {
            endPos = startPos + (MovementDirection * MovementSpeed * ActiveDuration);
        }*/
	}
	
	// Update is called once per frame
	void Update () {
		if (active)
        {
            Move();
            timer += Time.deltaTime;
            if (timer > ActiveDuration)
            {
                DeactivateCamera();
            }
        }
	}

    public void ActivateCamera()
    {
        active = true;
        this.gameObject.SetActive(true);
    }

    public void DeactivateCamera()
    {
        active = false;
        this.gameObject.SetActive(false);
    }

    void Move()
    {
        if (MovementSpeed > 0) MovementSpeed -= SlowDownIncrement * Time.deltaTime;

        if (RelativeMovement)
        {
            this.transform.Translate(MovementDirection * (MovementSpeed * Time.deltaTime), Space.Self);
        }
        else
        {
            this.transform.Translate(MovementDirection * (MovementSpeed * Time.deltaTime), Space.World);
        }
        //transform.position = Vector3.Slerp(startPos, endPos, timer / ActiveDuration);
    }
}
