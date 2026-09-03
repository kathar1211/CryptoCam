using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//detects when a jackalope has gotten in range of mothman
public class MothmanTriggerZone : MonoBehaviour
{
    public Mothman mothman;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<Jackalope>(out Jackalope foundJackalope))
        {
            mothman.AquireTarget(foundJackalope);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent<Jackalope>(out Jackalope foundJackalope))
        {
            mothman.LoseTarget(foundJackalope);
        }
    }
}
