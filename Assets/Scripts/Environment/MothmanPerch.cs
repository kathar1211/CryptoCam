using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothmanPerch : MonoBehaviour
{
    public Mothman myMothman;
    public bool perchOccupied;

    public void OnCollisionEnter(Collision collision)
    {
        if (!perchOccupied || myMothman == null) { return; }

        BonkableObject bonked = collision.gameObject.GetComponent<BonkableObject>();
        if (bonked != null && bonked.CanDoBonk())
        {
            //calculate where to position bonk vfx and spawn
            Vector3 impactPosition = collision.GetContact(0).point;
            Vector3 impactDirection = bonked.transform.position - impactPosition;
            bonked.SpawnBonkVFX(impactPosition, impactDirection.normalized);

            //notify mothman
            myMothman.PerchBonked();
        }
    }
}
