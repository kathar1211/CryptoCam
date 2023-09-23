using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://pastebin.com/ZSvX08fu
public class SetRippleShaderEffects : MonoBehaviour
{

    [SerializeField]
    RenderTexture rt;
    [SerializeField]
    Transform target;

    // Start is called before the first frame update
    void Awake()
    {
        Shader.SetGlobalTexture("_GlobalEffectRT", rt);
        Shader.SetGlobalFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);

    }

    private void Update()
    {
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
       // transform.rotation = target.rotation;
        Shader.SetGlobalVector("_Position", new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        Shader.SetGlobalFloat("_RippleYPos", target.transform.position.y);
        // Debug.Log(target.transform.position.y);
    }
}