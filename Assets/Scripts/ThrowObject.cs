using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class ThrowObject : MonoBehaviour {

    [SerializeField]
    GameObject carrot;

    [SerializeField]
    GameManager gameManager;
    Photography photographer;

    [SerializeField]
    Text objText;

    ///forward force applied to the thrown object
    public float throwForce;
    //time in seconds between when objects can be thrown
    public float coolDownTime;
    private float timer;
    //maximum amount of objects that can be thrown
    public int objectLimit;
    private int currentObjects;
    //how far in front of player to spawn object
    public float spawnDistance;

    //sound effect
    [SerializeField]
    AudioSource throwSFX;

	// Use this for initialization
	void Start () {
        
        timer = coolDownTime;
        currentObjects = 0;
        photographer = Photography.Instance;
        objText.text = (objectLimit - currentObjects).ToString();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        //dont throw items if camera is poised for taking pictures
        if (photographer != null && photographer.CameraReady)
        {
            return;
        }
        //if enough time has passed and the object limit is not exceeded, create object and throw when button is pressed
		if (CustomController.GetButtonDown(Constants.ThrowObject) && timer >= coolDownTime && currentObjects < objectLimit)
        {
            ThrowCarrot();
        }
	}

    void ThrowCarrot()
    {
        //create carrot and throw
        GameObject existingCarrot = Instantiate(carrot, this.transform.position + (this.transform.forward * spawnDistance), carrot.transform.rotation);
        Vector3 forwardForce = this.transform.forward * throwForce;
        existingCarrot.GetComponent<Rigidbody>().AddForce(forwardForce, ForceMode.Impulse);
        //give it a lil spin
        existingCarrot.GetComponent<Rigidbody>().AddTorque(forwardForce, ForceMode.Impulse);
        //update conditions around throwing
        timer = 0;
        currentObjects++;
        objText.text = (objectLimit - currentObjects).ToString();
        //play sfx if exists
        if (throwSFX != null) { throwSFX.Play(); }
    }
}
