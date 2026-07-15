using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CameraManager : MonoBehaviour
{
    public CameraWithPostProcessing[] Cameras;
    [SerializeField]
    FirstPersonController PlayerCamera;
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

    public void OnUpdateCameraSensitivity()
    {
        if (PlayerCamera == null) { return; }
        MouseLook mouseControl = PlayerCamera.GetMouseControls();
        if (mouseControl != null) { mouseControl.LoadSensitivitySetting(); }
    }

    public void OnUpdateCameraFOV()
    {
        if (Photography.Instance != null) { Photography.Instance.RefreshFOV(); }
    }
}
