using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open the door to the cabin when player comes in range
/// </summary>
public class CabinOpen : MonoBehaviour {

    Animator animator;
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}
