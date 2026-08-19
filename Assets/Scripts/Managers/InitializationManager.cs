using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class InitializationManager : MonoBehaviour
{
    public TrackErrorsPrompt ErrorsPrompt;
    public SteamFailedPrompt SteamPrompt;
    public GameObject LoadingIcon;
    private bool isSceneLoading = false;
    public bool useSteam;

    [SerializeField] SentryOptionConfiguration SentryConfig;
    [SerializeField] SteamManager SteamManager;

    private bool PauseInitialization = false;

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
    private async void Init()
    {
        //check if we've asked for permission around error tracking
        if (!PlayerPrefs.HasKey(Constants.ErrorTrackingConsent))
        {
            ErrorsPrompt.AnimateOnscreen();
            PauseInitialization = true;
        }
        else
        {
            bool errorTrackingAllowed = PlayerPrefs.GetInt(Constants.ErrorTrackingConsent) == 1;
            if (errorTrackingAllowed) { SentryConfig.EnableSentry(); Debug.Log("initialization: enabling sentry"); }
            else { SentryConfig.DisableSentry(); Debug.Log("initialization: disabling sentry"); }
        }

        await UniTask.WaitUntil(() => PauseInitialization == false);

        //initialize steam
        if (useSteam)
        {
            SteamManager.Init();
            //steam failed to initialize
            if (!SteamManager.Initialized)
            {
                //ask the user if they want to relaunch steam or proceed without
                Debug.Log("steam api not initialized");
                SteamPrompt.AnimateOnscreen();
                PauseInitialization = true;
            }
        }

        await UniTask.WaitUntil(() => PauseInitialization == false);

        //if there's nothing else we want to do first, we can proceed to the title screen
        ProceedToTitle();
    }

    public void ProceedToTitle()
    {
        LoadingIcon.SetActive(true);
        isSceneLoading = true;
        SceneManager.LoadSceneAsync("Title");
    }

    public void ContinueWithInitialization()
    {
        PauseInitialization = false;
    }
}
