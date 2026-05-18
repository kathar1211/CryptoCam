using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptidTrigger : MonoBehaviour
{
    public System.Action TriggerEnterAction; 

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
        if (TriggerEnterAction != null && other.CompareTag(Constants.PlayerTag)) { TriggerEnterAction.Invoke(); }
    }
}
