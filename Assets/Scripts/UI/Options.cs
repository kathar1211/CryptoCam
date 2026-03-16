using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using DG.Tweening;
using TMPro;
using Sentry.Unity;

public class Options : MonoBehaviour {

    //buttons screen 1
    public UIControlWithHighlight CloseOptionsScreen1;
    public UIControlWithHighlight ReadyCameraControl;
    public UIControlWithHighlight TakePictureControl;
    public UIControlWithHighlight ThrowObjectControl;
    public UIControlWithHighlight PauseControl;
    public UIControlWithHighlight CrouchControl;
    public UIControlWithHighlight RunControl;    
    public UIControlWithHighlight RestoreDefaultsControl;

    //buttons screen 2
    public UIControlWithHighlight CloseOptionsScreen2;
    public SliderWithLabel TextSpeedSlider;
    public ToggleWithHighlight FullScreenToggle;
    public SliderWithLabel BGMSlider;
    public SliderWithLabel SFXSlider;
    public ToggleWithHighlight ErrorTrackingToggle;
    public UIControlWithHighlight RestoreDefaultsSettings;

    //buttons on all screens
    public UIControlWithHighlight More;

    //screen handling
    bool isScrolling = false;
    public float scrollSpeed;
    float moreButtonPos;
    public enum ScreenState { Controls, Settings};
    public ScreenState currentScreen = ScreenState.Controls;
    public GameObject ControlScreenHolder;
    public GameObject SettingsScreenHolder;
    private float offScreenPos;

    //display the current text speed
    
    private float speed;
    public float maxSpeed;
    public float minSpeed;
    public float increment;

    //audio
    public Text bgmVol;
    public Text sfxVol;
    AudioManager audioManager;

    //hold buttons in an array that represents the order theyre in on screen
    private UIControlWithHighlight[] controlbuttonArray;
    private UIControlWithHighlight[] settingsButtonArray;
    private UIControlWithHighlight selectedButton;
    private int selectedButtonIndex;

    //submenu to tell users to register an input
    public GameObject KeyPressSubMenu;

    //true when waiting for user to register a key
    bool waitingForKeyPress = false;
    //store new input here
    ButtonOrAxis newKey;
    //store event here
    Event keyEvent;

    //gameManager
    private GameManager gameManager;

    //SFX
    [SerializeField] AudioSource NormalButtonSFX;
    [SerializeField] AudioSource MoreButtonSFX;

    //Sentry options
    [SerializeField] SentryOptionConfiguration SentryConfig;

	// Use this for initialization
	void Start () {
        controlbuttonArray = new UIControlWithHighlight[] { ReadyCameraControl, TakePictureControl,
            ThrowObjectControl, PauseControl, RunControl, CrouchControl, RestoreDefaultsControl, CloseOptionsScreen1 };

        settingsButtonArray = new UIControlWithHighlight[] { TextSpeedSlider,
            BGMSlider, SFXSlider ,FullScreenToggle, ErrorTrackingToggle, RestoreDefaultsSettings, CloseOptionsScreen2, };

        //assuming we're starting with controls screen on
        offScreenPos = SettingsScreenHolder.transform.localPosition.x;

        gameManager = GameManager.Instance;

    }

    private void OnEnable()
    {
        //if controls are saved in playerprefs load them
        CustomController.LoadAllKeys();
        LoadTextSpeed();
        LoadErrorTracking();
        audioManager = FindObjectOfType<AudioManager>();

        UpdateButtonText();
        UpdateSettingsText();
    }

    // Update is called once per frame
    void Update () {
        if (isScrolling)
        {
            return;// dont handle input or do anything else while scrolling
        }

        if (waitingForKeyPress)
        {
            //gamepad buttons dont have an event like keypresses, we have to check them all ourselves
            //https://discussions.unity.com/t/find-out-if-any-button-on-any-gamepad-has-been-pressed-and-which-one/65089
            foreach (KeyCode button in CustomController.AllGamepadButtons)
            {
                if (Input.GetKeyDown(button))
                {
                    newKey = new ButtonOrAxis(button);
                    waitingForKeyPress = false;
                    if (GameManager.Instance != null) { GameManager.Instance.DontAllowPause = false; }
                }
            }

            //allowing axis input from gamepad as well
            foreach (string axis in CustomController.AllGamepadAxesAsButtons)
            {
                if (CrossPlatformInputManager.GetButtonOrAxisDown(axis))
                {
                    newKey = new ButtonOrAxis(axis);
                    waitingForKeyPress = false;
                    if (GameManager.Instance != null) { GameManager.Instance.DontAllowPause = false; }
                }
            }
        }
        else //only navigate options screen if we're not waiting for input to be assigned
        {
            //allow using bumpers to navigate screens
            if (currentScreen == ScreenState.Controls)
            {
                if (CrossPlatformInputManager.GetAxis(Constants.RTAxis) != 0 || CrossPlatformInputManager.GetAxis(Constants.RTAxisMac) != 0)
                {
                    ShowMore();
                }
            }
            else if (currentScreen == ScreenState.Settings)
            {
                if (CrossPlatformInputManager.GetAxis(Constants.LTAxis) != 0 || CrossPlatformInputManager.GetAxis(Constants.LTAxisMac) != 0)
                {
                    ShowMore();
                }
            }

            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Vertical))
            {
                ChangeSelectButton(CrossPlatformInputManager.GetAxis(Constants.Vertical));
                Debug.Log("axis value: " + CrossPlatformInputManager.GetAxis(Constants.Vertical));
            }

            //adjust sliders if a slider is selected
            float dir = CrossPlatformInputManager.GetAxis(Constants.Horizontal);
            if (dir != 0 && selectedButton != null)
            {
                Slider sliderSelect = selectedButton.GetComponent<Slider>();
                if (sliderSelect != null)
                {

                    //increment should be a function of the min/max of the slider
                    float increment = (sliderSelect.maxValue - sliderSelect.minValue) / 100f;
                    increment = Mathf.Abs(increment);

                    //positive value is right, negative value is left
                    if (dir < 0) { increment *= -1; }

                    AdjustSliderValue(sliderSelect, increment);
                }

                else
                {

                    switch (currentScreen)
                    {
                        case ScreenState.Controls:
                            if (dir > 0) { MoveSelector(More); }
                            else { MoveSelector(controlbuttonArray[selectedButtonIndex]); }
                            break;
                        case ScreenState.Settings:
                            if (dir < 0) { MoveSelector(More); }
                            else { MoveSelector(settingsButtonArray[selectedButtonIndex]); }
                            break;
                    }

                }
            }

            if (InputManager.Instance.GetButtonDown(Constants.Submit)) //ignore clicks to avoid invoking buttons twice
            {
                if (selectedButton != null)
                {
                    Button buttonSelect = selectedButton.GetComponent<Button>();
                    if (buttonSelect != null)
                    {
                        buttonSelect.onClick.Invoke();
                    }
                    //this could be a toggle rather than a button
                    else
                    {
                        Toggle toggleSelect = selectedButton.GetComponent<Toggle>();
                        if (toggleSelect != null)
                        {
                            toggleSelect.isOn = !toggleSelect.isOn;
                        }
                    }
                    //could also be a slider but those dont do anything if you select
                }

                CustomController.UsingController = true;
            }

            if (InputManager.Instance.GetButtonDown(Constants.Cancel))
            {
                Close();
            }
        }

       
	}

    public void ChangeSelectButton(float input)
    {
        UIControlWithHighlight prevSelectedButton = selectedButton;

        UIControlWithHighlight[] buttonArray = new UIControlWithHighlight[] { };
        //scroll through approproate buttons depending on which screen is active
        switch (currentScreen)
        {
            case ScreenState.Controls:
                buttonArray = controlbuttonArray;
                break;
            case ScreenState.Settings:
                buttonArray = settingsButtonArray;
                break;
        }
        //start at index 0 if nothing is selected yet
        if (selectedButton == null)
        {
            selectedButtonIndex = 0;          
        }
        //moving down
        else if (input < 0){
            selectedButtonIndex++;
            selectedButtonIndex %= buttonArray.Length;
        }
        //moving up
        else if (input > 0)
        {
            selectedButtonIndex += buttonArray.Length;
            selectedButtonIndex--;
            selectedButtonIndex %= buttonArray.Length;
        }
        selectedButton = buttonArray[selectedButtonIndex];

        //update highlight
        if (prevSelectedButton != null) { prevSelectedButton.HideHighlight(); }
        if (selectedButton != null) { selectedButton.ShowHighlight(); }

    }

    //turn full screen on and off
    public void ToggleFullScreen(bool tf)
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        Screen.fullScreen = tf;
    }

    public void OnFullScreenToggleChanged()
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        Screen.fullScreen = FullScreenToggle.isOn;
    }

    //https://www.studica.com/blog/custom-input-manager-unity-tutorial
    private void OnGUI()
    {
        /*keyEvent dictates what key our user presses

         * bt using Event.current to detect the current

         * event

         */
        if (waitingForKeyPress) {
            keyEvent = Event.current;

            //Executes if a button gets pressed and

            //the user presses a key

            if (keyEvent.keyCode != KeyCode.None)
            {

                newKey = new ButtonOrAxis(keyEvent.keyCode); //Assigns newKey to the key user presses

                //make sure it was pressed this frame 
                if (Input.GetKeyDown(keyEvent.keyCode))
                {
                    waitingForKeyPress = false;
                    if (GameManager.Instance != null) { GameManager.Instance.DontAllowPause = false; }
                }

            }

            //mouse clicks count as acceptable input
            else if (keyEvent.isMouse)
            {
                //convert mouse button number (0 left click, 1 right click) to mouse keycode (323 left click, 324 right click)
                int keycode = (int)KeyCode.Mouse0 + keyEvent.button;
                try
                {
                    KeyCode mouseKey = (KeyCode)(keycode);
                    newKey = new ButtonOrAxis(mouseKey);
                }
                catch(Exception e)
                {
                    Debug.LogError("encountered error " + e.Message + "after attempting to convert input " + keyEvent.button + " to a mouse keycode");
                }
                //make sure it was pressed this frame
                if (Input.GetMouseButtonDown(keyEvent.button)) {
                    waitingForKeyPress = false;
                    if (GameManager.Instance != null) { GameManager.Instance.DontAllowPause = false; }
                }
                
            }

        }
    }

    //iterates endlessly until a key is pressed
    IEnumerator WaitForKey()
    {
        while (waitingForKeyPress)
            yield return null;
    }

    //method called by buttons- send which control is being updated and start the process of waiting for input
    public void ChangeInputButtonClicked(string controlKey)
    {
        if (!isScrolling)
        {
            if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
            if (!waitingForKeyPress)
            {
                //dont allow user to unpause by inputting the current pause key
                if (gameManager != null) { gameManager.DontAllowPause = true; }
                StartCoroutine(AssignKey(controlKey));
                KeyPressSubMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    //wait for input, then update values and return to the main options menu
    IEnumerator AssignKey(string controlKey)
    {
        waitingForKeyPress = true;
        if (GameManager.Instance != null) { GameManager.Instance.DontAllowPause = true; }
        yield return WaitForKey();

        if (newKey.IsButton)
        {
            CustomController.SetButton(controlKey, newKey.Key);
        }
        else
        {
            CustomController.SetAxis(controlKey, newKey.AxisName);
        }
        CustomController.SaveKey(controlKey);
        UpdateButtonText();
        KeyPressSubMenu.SetActive(false);
        if (gameManager != null) { gameManager.DontAllowPause = false; }

        yield return null;
    }

    //set control buttons to display current configuration
    void UpdateButtonText()
    {
        ReadyCameraControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.ReadyCamera);
        TakePictureControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.TakePicture);
        ThrowObjectControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.ThrowObject);
        PauseControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.Pause);
        RunControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.RunButton);
        CrouchControl.GetComponentInChildren<TMP_Text>().text = CustomController.GetButtonInput(Constants.CrouchButton);
    }

    void UpdateSettingsText()
    {
        TextSpeedSlider.value = speed;
        BGMSlider.value = audioManager.getBGMVolume();
        SFXSlider.value = audioManager.getSFXVolume();
        FullScreenToggle.isOn = Screen.fullScreen;
    }

    public void RestoreControlDefaults()
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        CustomController.RestoreDefaults();
        UpdateButtonText();
        CustomController.ClearAllKeys();
    }

    public void RestoreSettingDefaults()
    {
        BGMSlider.value = 0;
        SFXSlider.value = 0;
        TextSpeedSlider.value = 1;
    }

    //method to change the speed at which dialogue appears. true to increase false to decrease
    public void AdjustTextSpeed (bool increase)
    {
        if (!isScrolling)
        {
            if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
            if (increase && speed < maxSpeed)
            {
                speed += increment;
            }
            else if (!increase && speed > minSpeed)
            {
                speed -= increment;
            }
            //save value and update ui
            SaveTextSpeed();
            UpdateSettingsText();
        }
    }

    public void OnTextSpeedSliderChanged()
    {
        speed = TextSpeedSlider.value;
        SaveTextSpeed();
    }

    //adjust the audio settings for background music
    public void OnBGMSliderChanged()
    {
        float bgm = BGMSlider.value;
        audioManager.UpdateBGMVolume(bgm);
    }

    //adjust the audio settings for sound effects
    public void OnSFXSliderChanged()
    {
        float sfx = SFXSlider.value;
        audioManager.UpdateSFXVolume(sfx);
    }

   
    public void AdjustSliderValue(Slider slider, float increment)
    {
        slider.value += increment;
    }

    //load text speed from playerprefs
    void LoadTextSpeed()
    {
        if (PlayerPrefs.HasKey(Constants.TextSpeed))
        {
            speed = PlayerPrefs.GetFloat(Constants.TextSpeed);
        }
        else
        {
            speed = 1;
        }
        TextSpeedSlider.value = speed;
    }
    
    //save text speed to playerprefs
    void SaveTextSpeed()
    {
        PlayerPrefs.SetFloat(Constants.TextSpeed, speed);
    }

    //load error tracking enabled from playerprefs
    void LoadErrorTracking()
    {
        ErrorTrackingToggle.isOn = PlayerPrefs.GetInt(Constants.ErrorTrackingConsent, 0) == 1;
        Debug.Log("sentry on: " + ErrorTrackingToggle.isOn);
    }

    public void OnErrorTrackingToggleChanged()
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        int trackingEnabled = ErrorTrackingToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(Constants.ErrorTrackingConsent, trackingEnabled);

        // SentryOptions.EnableSentry(ErrorTrackingToggle.isOn);
        if (ErrorTrackingToggle.isOn)
        {
            SentryConfig.EnableSentry();
            Debug.Log("sentry on");
        }
        else
        {
            SentryConfig.DisableSentry();
            Debug.Log("sentry off");
        }

    }

    //if selector is activated it still needs to reflect mouseover options
    public void MoveSelector(UIControlWithHighlight button)
    {
        if (!isScrolling)
        {
            UIControlWithHighlight prevSelectedButton = selectedButton;
            selectedButton = button;
            //selectedButtonIndex = ArrayUtility.IndexOf<GameObject>(buttonArray, button);
            UIControlWithHighlight[] buttonArray = new UIControlWithHighlight[] { };
            //scroll through approproate buttons depending on which screen is active
            switch (currentScreen)
            {
                case ScreenState.Controls:
                    buttonArray = controlbuttonArray;
                    break;
                case ScreenState.Settings:
                    buttonArray = settingsButtonArray;
                    break;
            }

            int newButtonIndex = Array.IndexOf(buttonArray, button);
           if (newButtonIndex != -1) { selectedButtonIndex = newButtonIndex; }
            //set hover to match dimensions of selected button
            if (prevSelectedButton != null) { prevSelectedButton.HideHighlight(); }
            if (selectedButton != null) { selectedButton.ShowHighlight(); }
        }
    }

    public void ShowMore()
    {
        if (MoreButtonSFX != null) { MoreButtonSFX.Play(); }
        if (!isScrolling)
        {
            moreButtonPos = More.transform.localPosition.x;
            isScrolling = true;
            
        }

        Sequence transition = DOTween.Sequence();
        transition.SetUpdate(true);
        transition.Append(More.transform.DOLocalMoveX(moreButtonPos * -1, .5f).SetEase(Ease.OutSine));
        

        //update selected screen
        switch (currentScreen)
        {
            case ScreenState.Controls:
                currentScreen = ScreenState.Settings;
                transition.Join(SettingsScreenHolder.transform.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine));
                transition.Join(ControlScreenHolder.transform.DOLocalMoveX(offScreenPos * -1, .5f).SetEase(Ease.OutSine));
                break;
            case ScreenState.Settings:
                transition.Join(ControlScreenHolder.transform.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine));
                transition.Join(SettingsScreenHolder.transform.DOLocalMoveX(offScreenPos * 1, .5f).SetEase(Ease.OutSine));
                currentScreen = ScreenState.Controls;
                break;
        }

        transition.AppendCallback(OnShowMoreComplete);
        transition.Play();
    }

    public void OnShowMoreComplete()
    {
        isScrolling = false;

        //flip sprite
        Vector3 localScale = More.transform.localScale;
        More.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);

        //snap to position
        More.transform.localPosition = new Vector3(moreButtonPos * -1, More.transform.localPosition.y, More.transform.localPosition.z);
    }

    public void ThrowFakeError()
    {
        Debug.LogError("This error was thrown on purpose at " + DateTime.Now);
    }

    private void Close()
    {
        CloseOptionsScreen1.GetComponent<Button>().onClick.Invoke();
    }
}
