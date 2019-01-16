using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour {

    //animate the logo on the title screen, moon fade in etc
    public GameObject moon;
    public GameObject[] stars;
    public GameObject leftCloud;
    public GameObject rightCloud;

    //store the place where the clouds should end up
    float leftCloudPos;
    float rightCloudPos;

    //how far do the clouds move
    public int cloudDist;

    //how long does it take moon and clouds to fade in (in seconds)
    public float moonDelay;

    //how long after the moon fades in does each star appear (in seconds)
    public float starDelay;

    //how long until we start the animations
    public float initialDelay;

    //keep track of when to turn on each star
    float starTimer;
    int starIndex;

	// Use this for initialization
	void Start () {
        leftCloudPos = leftCloud.transform.position.x;
        rightCloudPos = rightCloud.transform.position.x;

        //hide all things
        moon.GetComponent<CanvasGroup>().alpha = 0;
        leftCloud.GetComponent<CanvasGroup>().alpha = 0;
        rightCloud.GetComponent<CanvasGroup>().alpha = 0;
        foreach (GameObject star in stars)
        {
            //star.GetComponent<CanvasGroup>().alpha = 0;
            star.SetActive(false);
        }

        //move clouds
        leftCloud.transform.Translate(-cloudDist, 0, 0);
        rightCloud.transform.Translate(cloudDist, 0, 0);

        starTimer = 0;
        starIndex = 0;

    }
	
	// Update is called once per frame
	void Update () {

        //option to skip animation stuff by pressing enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            moon.GetComponent<CanvasGroup>().alpha = 1;
            leftCloud.GetComponent<CanvasGroup>().alpha = 1;
            rightCloud.GetComponent<CanvasGroup>().alpha = 1;

            leftCloud.transform.Translate(leftCloudPos - leftCloud.transform.position.x, 0, 0);
            rightCloud.transform.Translate(rightCloudPos - rightCloud.transform.position.x, 0, 0);

            foreach (GameObject star in stars)
            {
                //star.GetComponent<CanvasGroup>().alpha = 1;
                star.SetActive(true);
            }
        }

        //dont start animation right away
        if (Time.time < initialDelay)
        {
            return;
        }

        //fade in moon and clouds first
        if (moon.GetComponent<CanvasGroup>().alpha != 1)
        {
            //fade in
            moon.GetComponent<CanvasGroup>().alpha += Time.deltaTime / moonDelay;
            leftCloud.GetComponent<CanvasGroup>().alpha += Time.deltaTime / moonDelay;
            rightCloud.GetComponent<CanvasGroup>().alpha += Time.deltaTime / moonDelay;

            //move the clouds
            leftCloud.transform.Translate((Time.deltaTime / moonDelay) * cloudDist, 0, 0);
            rightCloud.transform.Translate(-(Time.deltaTime / moonDelay) * cloudDist, 0, 0);
        }

        else if (starIndex < stars.Length)
        {
            //after moon has faded in do stars

            starTimer += Time.deltaTime;

            if (starTimer >= starDelay)
            {
                //stars[starIndex].GetComponent<CanvasGroup>().alpha = 1;
                stars[starIndex].SetActive(true);
                starIndex++;
                starTimer = 0;
            }
        }

        



    }
}
