using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script used to account for differences in my non standardized cryptid rigs
public class HeadBone : MonoBehaviour {

    //joints have different default orientations - set this in editor to determine which vector corresponds to the actual cryptid's forward vector
    public enum HeadForward { Forward, Back, Up, Down, Left, Right};
    [SerializeField]
    HeadForward HeadDirection;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //access the correct vector
    public Vector3 GetForward()
    {
        switch (HeadDirection)
        {
            case HeadForward.Up:
                return transform.up;
            case HeadForward.Down:
                return transform.up * -1;
            case HeadForward.Left:
                return transform.right * -1;
            case HeadForward.Right:
                return transform.right;
            case HeadForward.Back:
                return transform.forward * -1;
            case HeadForward.Forward:
                return transform.forward;
            default:
                return transform.forward;
        }
    }
}
