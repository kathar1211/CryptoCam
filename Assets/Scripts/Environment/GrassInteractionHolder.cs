using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInteractionHolder : MonoBehaviour {

    //GameObject[] cryptids;
    Vector4[] positions = new Vector4[100];

    //float minDistance = 10;
    //[SerializeField]
    //GameObject[] nearestCryptid;

    // Use this for initialization
    void Start () {
       // cryptids = GameObject.FindGameObjectsWithTag("Cryptid");

    }
	
	// Update is called once per frame
	void Update () {

/*
        for (int i = 0; i < nearestCryptid.Length; i++)
        {
            positions[i] = nearestCryptid[i].transform.position;
        }
        Shader.SetGlobalFloat("_PositionArray", nearestCryptid.Length);
        Shader.SetGlobalVectorArray("_Positions", positions);
        */
        
    }
}
