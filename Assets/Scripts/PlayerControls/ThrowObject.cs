using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    TextMeshProUGUI objText;

    [SerializeField]
    Image throwMeter;



    //forward force applied to the thrown object
    public int throwForceMin;
    public int throwForceMax;
    public int forceIncreaseRate;
    private float throwForce;
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
        throwForce = throwForceMin;
        photographer = Photography.Instance;
        objText.text = (objectLimit - currentObjects).ToString();
        throwMeter.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        //if enough time has passed and the object limit is not exceeded, create object and throw when button is pressed
		if (InputManager.Instance.GetButton(Constants.ThrowObject) && timer >= coolDownTime && currentObjects < objectLimit)
        {
            throwMeter.gameObject.SetActive(true);
            throwForce += forceIncreaseRate * Time.deltaTime;
            throwForce = Mathf.Min(throwForce, throwForceMax);

            throwMeter.fillAmount = (throwForce - throwForceMin) / ((throwForceMax * 1f) - (throwForceMin *1f));
        }
        if (InputManager.Instance.GetButtonUp(Constants.ThrowObject) && timer >= coolDownTime && currentObjects < objectLimit)
        {
            throwMeter.gameObject.SetActive(false);
            ThrowCarrot();
            throwMeter.fillAmount = 0;
            throwForce = throwForceMin;
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
