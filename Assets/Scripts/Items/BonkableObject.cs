using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a component for items that can bonk cryptids when thrown at them
public class BonkableObject : MonoBehaviour
{
    //can this currently cause a bonk
    public bool Active;

    //how fast does this have to be going to cause a bonk
    [SerializeField]
    float VelocityThreshold;

    [SerializeField]
    GameObject bonkVFX; //prefab for vfx that spawn in when a bonk occurrs

    private float bonkCooldownTime = .5f; //time in seconds between when we trigger the bonk effects
    private float bonkTimer = .5f;

    // Start is called before the first frame update
    void Start()
    {
        Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active) { bonkTimer += Time.deltaTime; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //become deactivated once you hit the ground
        if (collision.gameObject.tag == Constants.TerrainTag) { Active = false; }

        //todo: we should slowly fade out and then destroy this object once its inactive
    }

    public bool CanDoBonk()
    {
        if (bonkTimer < bonkCooldownTime) { return false; }
        if (!Active) { return false; }
        if (Mathf.Abs(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude) < VelocityThreshold) { return false; }

        return true;
    }


    public void SpawnBonkVFX(Vector3 spawnLocation, Vector3 vfxDirection)
    {
        //spawn vfx at the spawn position
        GameObject spawnedVFX = GameObject.Instantiate(bonkVFX, spawnLocation, Quaternion.identity);

        //rotate vfx so the up vector is pointing in the direction specified
        spawnedVFX.transform.up = vfxDirection;

        //reset the bonk timer
        bonkTimer = 0;
    }
}
