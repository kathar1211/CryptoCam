using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for external collider that's used for obstacle avoidance
public class CryptidCollider : MonoBehaviour {

    [SerializeField]
    Cryptid baseCryptid;

	// Use this for initialization
	void Start () {

        BoxCollider box = this.GetComponent<BoxCollider>();

        //get colliders that are already in range at start, as they wont fire OnTriggerEnter
        foreach (Collider col in Physics.OverlapBox(box.center, box.size / 2f, this.transform.rotation))
        {
            OnTriggerEnter(col);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!ShouldIgnoreCollision(other))
        {
            baseCryptid.AddObstacleToList(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ShouldIgnoreCollision(other))
        {
            baseCryptid.RemoveObstacleFromList(other);
        }
    }

    private bool ShouldIgnoreCollision(Collider other)
    {
        //ignore collisions from the cryptid itself
        if (other.transform.root == baseCryptid.transform) { return true; }
        //ignore ground collisions
        if (other.gameObject.tag == Constants.WaterTag || other.gameObject.tag == Constants.TerrainTag) { return true; }

        return false;
    }
}
