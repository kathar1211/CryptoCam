using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CameraWithPostProcessing[] Cameras;
    public Camera PlayerCamera;
    public bool PostProcessingOn;

    // Start is called before the first frame update
    void Start()
    {
        LoadPostProcessingSetting();
        UpdatePostProcessing();
    }

    public void OnPostProcessingSettingChanged()
    {
        bool newSetting = PlayerPrefs.GetInt(Constants.PostProcessingEnabled, 1) == 1;
        if (PostProcessingOn != newSetting)
        {
            PostProcessingOn = newSetting;
            UpdatePostProcessing();
        }
    }

    private void LoadPostProcessingSetting()
    {
        PostProcessingOn = PlayerPrefs.GetInt(Constants.PostProcessingEnabled, 1) == 1;
    }

    private void UpdatePostProcessing()
    {
        foreach (CameraWithPostProcessing camera in Cameras)
        {
            if (PostProcessingOn) { camera.EnablePostProcessing(); }
            else { camera.DisablePostProcessing(); }
        }
    }
}
