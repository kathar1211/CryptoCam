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

    // Start is called before the first frame update
    void Start()
    {
        Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //become deactivated once you hit the ground
        if (collision.gameObject.tag == Constants.TerrainTag) { Active = false; }
    }

    public bool CanDoBonk()
    {
        return (Active && Mathf.Abs(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude) >= VelocityThreshold);
    }
}
