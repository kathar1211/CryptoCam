using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ShowLevelIntro : MonoBehaviour
{

    [SerializeField]
    IntroCameraBehavior[] introCameras; //cut between these cameras
    [SerializeField]
    Camera mainCamera; //for when the intro is done

    //turn intro on and off in editor
    [SerializeField]
    bool skipIntro;

    //keep track of progress
    bool introFinished = false;
    int activeCameraIndex = 0;
    float timer = 0;

    //hide canvas until intro is over so its appearance will indicate when gameplay has begun
    public GameObject canvas;

    //play appropriate music
    [SerializeField]
    AudioSource BGMIntro;
    [SerializeField]
    AudioSource BGMMain;

    // Use this for initialization
    void Start()
    {
        if (!skipIntro)
        {
            StartIntro();
            canvas.SetActive(false);
        }
        else
        {
            FinishIntro();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //skip intro on just about any input
        if (Input.anyKeyDown && !introFinished)
        {
            FinishIntro();
        }

        else if (!introFinished)
        {
            UpdateIntro();
        }
    }

    //jump back to main camera and start game
    void FinishIntro()
    {
        if (BGMIntro != null) { BGMIntro.Stop(); }
        if (BGMMain != null) { BGMMain.Play(); }
        foreach (IntroCameraBehavior cam in introCameras)
        {
            cam.DeactivateCamera();
        }
        mainCamera.gameObject.SetActive(true);
        introFinished = true;
        canvas.SetActive(true);
    }

    //iterate through cameras, setting them active for their active duration
    void UpdateIntro()
    {
        if (introCameras == null || activeCameraIndex >= introCameras.Length)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= introCameras[activeCameraIndex].ActiveDuration)
        {
            introCameras[activeCameraIndex].DeactivateCamera();
            activeCameraIndex++;
            if (activeCameraIndex >= introCameras.Length)
            {
                FinishIntro(); return;
            }
            timer = 0;
            introCameras[activeCameraIndex].ActivateCamera();
        }
    }

    //set the first camera active
    void StartIntro()
    {
        if (BGMIntro != null) { BGMIntro.Play(); }
        timer = 0;
        activeCameraIndex = 0;
        introCameras[activeCameraIndex].ActivateCamera();
    }
}
