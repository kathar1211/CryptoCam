using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraWithPostProcessing : MonoBehaviour
{
    public PostProcessLayer PostProcess;

    public void EnablePostProcessing()
    {
        PostProcess.enabled = true;
    }

    public void DisablePostProcessing()
    {
        PostProcess.enabled = false;
    }
}
