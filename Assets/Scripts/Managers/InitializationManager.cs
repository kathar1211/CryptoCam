using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationManager : MonoBehaviour
{
    public TrackErrorsPrompt ErrorsPrompt;
    public GameObject LoadingIcon;
    private bool isSceneLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //anything we want to do before launching the title screen happens here
    private void Init()
    {
        //check if we've asked for permission around error tracking
        if (!PlayerPrefs.HasKey(Constants.ErrorTrackingConsent))
        {
            ErrorsPrompt.AnimateOnscreen();
            return;
        }
        else
        {
            bool errorTrackingAllowed = PlayerPrefs.GetInt(Constants.ErrorTrackingConsent) == 1;
            if (errorTrackingAllowed) { SentryUtils.EnableSentry(); }
            else { SentryUtils.DisableSentry(); }
        }

        //if there's nothing else we want to do first, we can proceed to the title screen
        ProceedToTitle();
    }

    public void ProceedToTitle()
    {
        LoadingIcon.SetActive(true);
        isSceneLoading = true;
        SceneManager.LoadSceneAsync("Title");
    }
}
