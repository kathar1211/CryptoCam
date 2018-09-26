using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Reverse : MonoBehaviour {

    

	// Use this for initialization
	void Start () {

        //https://answers.unity.com/questions/476810/flip-a-mesh-inside-out.html
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
