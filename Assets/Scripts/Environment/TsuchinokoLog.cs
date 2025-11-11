using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//log with a tsuchinoko in it. certain interactions with the log trigger tsuchinoko reactions
public class TsuchinokoLog : MonoBehaviour
{

    public Tsuchinoko myTsuchinoko;
    public Transform leftBonkSecondaryLocation;
    public Transform rightBonkSecondaryLocation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private void OnTriggerEnter(Collider other)
    {
        //thought: should the log also respond to the player's avoid zone? or just let tsuchinoko handle it?
    }

    public void OnCollisionEnter(Collision collision)
    {
        BonkableObject bonked = collision.gameObject.GetComponent<BonkableObject>();
        if (bonked != null && bonked.CanDoBonk())
        {
            //determine which direction the bonk is coming from

            //line from log to carrot
            Vector3 bonkDistance = this.gameObject.transform.position - collision.gameObject.transform.position;

            //if the line from the log to the carrot is in the same direction as the log's right vector,
            //then the carrot is on the log's right
            bool leftImpact = true;
            if (Vector3.Dot(this.transform.right, bonkDistance) < 0)
            {
                leftImpact = false;
            }

            //calculate where to position bonk vfx and spawn
            Vector3 impactPosition = collision.GetContact(0).point;
            Vector3 impactDirection = bonked.transform.position - impactPosition;
            bonked.SpawnBonkVFX(impactPosition, impactDirection.normalized);

            //wake tsuchinoko and send him to a secondary location based on which side of the log was bonked
            if (leftImpact) { myTsuchinoko.WakeTsuchinoko(Tsuchinoko.MoveState.Seeking, leftBonkSecondaryLocation); }
            else { myTsuchinoko.WakeTsuchinoko(Tsuchinoko.MoveState.Seeking, rightBonkSecondaryLocation); }
        }
    }
}
