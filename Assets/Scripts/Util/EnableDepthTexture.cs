using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDepthTexture : MonoBehaviour {

    //goes on any camera that needs to render the water shader

	// Use this for initialization
	void Start () {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
