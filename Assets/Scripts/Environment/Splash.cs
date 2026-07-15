using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public GameObject SplashEffectPrefab;
    public GameObject SplashEffectPrefabNoTrigger;

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
        if (other.isTrigger) { return; }

        if (other.tag == "NoSplash") { return; }

        //only counts if we're entering the water from above
        if (other.transform.position.y < this.transform.position.y) { return; }

        GameObject splashToInstantiate = SplashEffectPrefab;

        if (other.tag == Constants.CryptidTag) { splashToInstantiate = SplashEffectPrefabNoTrigger; }//prevent cryptids from spawning splashes that affect them

        GameObject splash = GameObject.Instantiate(splashToInstantiate, other.transform.position, SplashEffectPrefab.transform.rotation);
        
    }
}
