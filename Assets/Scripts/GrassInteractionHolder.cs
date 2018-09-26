using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInteractionHolder : MonoBehaviour {

    public GameObject[] cryptids;
    Vector4[] positions = new Vector4[100];

    // Use this for initialization
    void Start () {
        //cryptids = GameObject.FindGameObjectsWithTag("Cryptid");

    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < cryptids.Length; i++)
        {
            positions[i] = cryptids[i].transform.position;
        }
        Shader.SetGlobalFloat("_PositionArray", cryptids.Length);
        Shader.SetGlobalVectorArray("_Positions", positions);
    }
}
