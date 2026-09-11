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
    private Rigidbody rb;
    bool checkedLanding = false;

    float prevFrameVelocity = -1;

    // Start is called before the first frame update
    void Start()
    {
        Active = true;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active) { bonkTimer += Time.deltaTime; }

        //check what this landed on so we can grant the lilypad achievement if applicable
        if (this.rb.velocity.magnitude == 0 && prevFrameVelocity == 0 && !checkedLanding)
        {
            if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 20)){
                if (hit.collider.tag == "lilypad")
                {
                    AchievementManager.GrantAchievement(AchievementManager.STEAM_ACHIEVEMENTS.LAND_CARROT_ON_LILYPAD);
                }
                checkedLanding = true;
                Debug.Log("carrot landed on " + hit.collider.gameObject.name);
            }
            
        }
        prevFrameVelocity = rb.velocity.magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //become deactivated once you hit the ground
        if (collision.gameObject.tag == Constants.TerrainTag) { Active = false; }
        else if (collision.gameObject.tag == Constants.WaterTag) { Active = false; }

        //todo: we should slowly fade out and then destroy this object once its inactive
    }

    public bool CanDoBonk()
    {
        if (bonkTimer < bonkCooldownTime) { return false; }
        if (!Active) { return false; }
        if (Mathf.Abs(rb.velocity.magnitude) < VelocityThreshold) { return false; }

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
