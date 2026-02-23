using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithScreenSize : MonoBehaviour
{

    public Dictionary<float, float> ratioToScale = new Dictionary<float, float>()
    {
        {16f/9f,  1f },
        {16f/10f,  1f },
        {5f/4f,  .9f},
        {4f/3f, .95f},
        {21f/9f, .8f },
        {64f/27f, .8f },
        {1f, .8f }
    };

    // Start is called before the first frame update
    void OnEnable()
    {
        float aspectRatio = ((float)Screen.width) / ((float)Screen.height);
        if (ratioToScale.ContainsKey(aspectRatio))
        {
            this.transform.localScale = new Vector3(ratioToScale[aspectRatio], ratioToScale[aspectRatio]);
        }
    }

   
}
