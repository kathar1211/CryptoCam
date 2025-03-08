using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour {

    float floatSpeed; //speed at which firefly approaches taret
     float distance;  //amount taret can move each frame
    Vector3 targetPos;      //taret the firefly floats towards
    public Sprite[] sprites; //sprites used for animation
    bool glowUp;            //keeps track if animation needs to increase or decrease glow
    int spriteIndex;        //index of the sprite currently bein used
    float frameSpeed; //how lon animation cycles throuh each frame
    float currentFrameTime; //time passed since frame was last chaned
    public bool stationary = false;

    // Use this for initialization
    void Start () {
        
        spriteIndex = Random.Range(0,sprites.Length-1);
        glowUp = true;
        currentFrameTime = 0;
        targetPos = transform.position + new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));

        //randomize values so each firefly is unique
        floatSpeed = Random.Range(.4f, 2);
        distance = 1; //this value ended up not mattering much
        frameSpeed = Random.Range(.1f, .2f);
        transform.GetChild(0).gameObject.GetComponent<Light>().range = Random.Range(1, 2.1f);
        transform.GetChild(0).gameObject.GetComponent<Light>().range  += spriteIndex * .1f;
    }
	
	// Update is called once per frame
	void Update () {

        //sprite always facin camera
        transform.forward = Camera.main.transform.forward;

        //cycle throuh sprite animation
        currentFrameTime += Time.deltaTime;
        if (currentFrameTime >= frameSpeed)
        {
            //boomeran effect: if last sprite is reached move backwards, if first sprite is reached move fowards
            if (glowUp)
            {
                spriteIndex++;
                transform.GetChild(0).gameObject.GetComponent<Light>().range+=.1f;
                if (spriteIndex >= sprites.Length - 1)
                {
                    glowUp = false;
                }
            }
            else if (!glowUp)
            {
                spriteIndex--;
                transform.GetChild(0).gameObject.GetComponent<Light>().range-=.1f;
                if (spriteIndex <= 0)
                {
                    glowUp = true;
                }
            }
            //set active sprite
            spriteIndex %= sprites.Length;
            GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
            currentFrameTime = 0;
        }

        if (!stationary) { Wander(); }
	}

    //fireflies wander in 3 dimensions/hover aimlessly
    void Wander()
    {
        //https://www.openprocessing.org/sketch/16479#

        //choose a random taret position within rane and move towards it
        targetPos += new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
        transform.position = Vector3.MoveTowards(transform.position, targetPos, floatSpeed * Time.deltaTime);

    }
}
