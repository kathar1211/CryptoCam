using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour {

    float Waterheight;
    bool activateFloat = false;
    public float floatSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (activateFloat && this.transform.position.y < Waterheight)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + (floatSpeed * Time.deltaTime), this.transform.position.z);
        }
	}

    //when we hit the water make note of the height and stop gravity
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            Waterheight = other.transform.position.y;

            Rigidbody rb = this.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
            }

            activateFloat = true;
        }
    }

    /*
    //back to normal once leaving water 
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water" && this.transform.position.y > Waterheight)
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }

            activateFloat = false;
        }
    }*/
}
