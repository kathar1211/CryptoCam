using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for an additional collider attached to cryptids to help them avoid obstacles
public class CryptidCollider : MonoBehaviour {

    [SerializeField]
    Cryptid baseCryptid;
    [SerializeField]
    float avoidSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //ignore collisions from the cryptid itself
        if (other.transform.root == baseCryptid.transform) { return; }
        //ignore ground collisions
        if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Water") { return; }
        //handle interactions with player
        if (other.gameObject.tag == "Player"){ baseCryptid.AvoidPlayer(other); }
        //otherwise move
        baseCryptid.AvoidCollision(other, avoidSpeed);
    }
}
