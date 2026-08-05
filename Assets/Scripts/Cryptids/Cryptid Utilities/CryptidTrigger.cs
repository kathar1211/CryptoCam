using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptidTrigger : MonoBehaviour
{
    public System.Action TriggerEnterAction;
    public System.Action TriggerExitAction;

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

    private void OnTriggerExit(Collider other)
    {
        if (TriggerExitAction != null && other.CompareTag(Constants.PlayerTag)) { TriggerExitAction.Invoke();}
    }
}
