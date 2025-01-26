using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using DG.Tweening;

public class Options : MonoBehaviour {

    //buttons screen 1
    public GameObject ReadyCameraControl;
    public GameObject TakePictureControl;
    public GameObject ThrowObjectControl;
    public GameObject PauseControl;
    public GameObject CrouchControl;
    public GameObject RunControl;    
    public GameObject RestoreDefaultsControl;

    //buttons screen 2
    public GameObject IncreaseText;
    public GameObject DecreaseText;
    public GameObject FullScreenOn;
    public GameObject FullScreenOff;
    public GameObject IncreaseBGM;
    public GameObject DecreaseBGM;
    public GameObject IncreaseSFX;
    public GameObject DecreaseSFX;

    //buttons on all screens
    public GameObject Exit;
    public GameObject More;

    //screen handling
    bool isScrolling = false;
    public float scrollSpeed;
    float moreButtonPos;
    public enum ScreenState { Controls, Settings};
    public ScreenState currentScreen = ScreenState.Controls;

    //display the current text speed
    public Text textSpeed;
    private float speed;
    public float maxSpeed;
    public float minSpeed;
    public float increment;

    //audio
    public Text bgmVol;
    public Text sfxVol;
    AudioManager audioManager;

    //select buttons with arrow keys/controller and highlight selected
    public GameObject buttonHighlight;
    //hold buttons in an array that represents the order theyre in on screen
    private GameObject[] controlbuttonArray;
    private GameObject[] settingsButtonArray;
    private GameObject selectedButton;
    private int selectedButtonIndex;

    //submenu to tell users to register an input
    public GameObject KeyPressSubMenu;

    //true when waiting for user to register a key
    bool waitingForKeyPress = false;
    //store new input here
    KeyCode newKey;
    //store event here
    Event keyEvent;

    //gameManager
    public GameManager gameManager;

    //SFX
    [SerializeField] AudioSource NormalButtonSFX;
    [SerializeField] AudioSource MoreButtonSFX;

	// Use this for initialization
	void Start () {
        controlbuttonArray = new GameObject[] { ReadyCameraControl, TakePictureControl,
            ThrowObjectControl, PauseControl, RunControl, CrouchControl, RestoreDefaultsControl, Exit };

        settingsButtonArray = new GameObject[] { DecreaseText,
            IncreaseText, FullScreenOn, FullScreenOff, IncreaseBGM, DecreaseBGM, IncreaseSFX, DecreaseSFX, Exit };

        //if controls are saved in playerprefs load them
        CustomController.LoadAllKeys();
        LoadTextSpeed();
        audioManager = FindObjectOfType<AudioManager>();

        UpdateButtonText();
        UpdateSettingsText();


    }
	
	// Update is called once per frame
	void Update () {
        if (isScrolling)
        {
            switch (currentScreen)
            {
                case ScreenState.Controls:
                    More.transform.Translate(-1 * scrollSpeed * Time.unscaledDeltaTime, 0, 0);
                    if (More.transform.localPosition.x <= moreButtonPos * -1)
                    {
                        isScrolling = false;
                        currentScreen = ScreenState.Settings;
                        //More.GetComponent<Image>().sprite = triangleFlipped;
                        Vector3 localScale = More.transform.localScale;
                        More.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
                        //snap to position
                        More.transform.localPosition = new Vector3(moreButtonPos * -1, More.transform.localPosition.y, More.transform.localPosition.z);
                    }
                    break;
                case ScreenState.Settings:
                    More.transform.Translate(1 * scrollSpeed * Time.unscaledDeltaTime, 0, 0);
                    if (More.transform.localPosition.x >= moreButtonPos * -1)
                    {
                        isScrolling = false;
                        currentScreen = ScreenState.Controls;
                        //More.GetComponent<Image>().sprite = triangle;
                        //flip sprite
                        Vector3 localScale = More.transform.localScale;
                        More.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
                        //snap to position
                        More.transform.localPosition = new Vector3(moreButtonPos * -1, More.transform.localPosition.y, More.transform.localPosition.z);
                    }
                    break;
            }
            
            return;// dont handle input or do anything else while scrolling
        }

		if (CrossPlatformInputManager.GetButtonDown(Constants.Vertical))
        {
            ChangeSelectButton(CrossPlatformInputManager.GetAxis(Constants.Vertical));
        }

        if (CrossPlatformInputManager.GetButtonDown(Constants.Submit) && !Input.GetMouseButtonDown(0)) //ignore clicks to avoid invoking buttons twice
        {
            if (selectedButton != null) { selectedButton.GetComponent<Button>().onClick.Invoke(); }
        }
	}

    void ChangeSelectButton(float input)
    {
        GameObject[] buttonArray = new GameObject[] { };
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
        buttonHighlight.SetActive(true);
        buttonHighlight.transform.position = selectedButton.transform.position;
        float width = selectedButton.GetComponent<RectTransform>().rect.width;
        float height = selectedButton.GetComponent<RectTransform>().rect.height;
        buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 5);
        buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 5);

    }

    //turn full screen on and off
    public void ToggleFullScreen(bool tf)
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        Screen.fullScreen = tf;
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

                newKey = keyEvent.keyCode; //Assigns newKey to the key user presses

                //make sure it was pressed this frame 
                if (Input.GetKeyDown(keyEvent.keyCode))
                {
                    waitingForKeyPress = false;
                }

            }

            //mouse clicks count as acceptable input
            else if (keyEvent.isMouse)
            {
                //convert mouse button number (0 left click, 1 right click) to mouse keycode (323 left click, 324 right click)
                newKey = (KeyCode)((int)KeyCode.Mouse0 + keyEvent.button);

                //make sure it was pressed this frame
                if (Input.GetMouseButtonDown(keyEvent.button)) {
                    waitingForKeyPress = false;
                }
            }

            //https://forum.unity.com/threads/how-can-i-know-that-any-button-on-gamepad-is-pressed.757322/

            //var gamepadButtonPressed = Gamepad.current.allControls.Any(x => x is ButtonControl button && x.isPressed && !x.synthetic);
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
        yield return WaitForKey();

        CustomController.SetButton(controlKey, newKey);
        CustomController.SaveAllKeys();
        UpdateButtonText();
        KeyPressSubMenu.SetActive(false);
        if (gameManager != null) { gameManager.DontAllowPause = false; }

        yield return null;
    }

    //set control buttons to display current configuration
    void UpdateButtonText()
    {
        ReadyCameraControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.ReadyCamera);
        TakePictureControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.TakePicture);
        ThrowObjectControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.ThrowObject);
        PauseControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.Pause);
        RunControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.RunButton);
        CrouchControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.CrouchButton);
    }

    void UpdateSettingsText()
    {
        textSpeed.text = speed.ToString("0.0");
        int bgm = audioManager.getBGMVolume();
        string bgmString = bgm.ToString("0");
        bgmVol.text = bgmString;
        sfxVol.text = audioManager.getSFXVolume().ToString("0");
    }

    public void RestoreDefaults()
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        CustomController.RestoreDefaults();
        UpdateButtonText();
        CustomController.SaveAllKeys();
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

    //adjust the audio settings for background music
    public void AdjustBGMVolume(bool increase)
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        if (!isScrolling)
        {
            int bgm = audioManager.getBGMVolume();
            if (increase && bgm < 10)
            {
                bgm += 1;
            }
            else if (!increase && bgm > 0)
            {
                bgm -= 1;
            }
            audioManager.UpdateBGMVolume(bgm);
            UpdateSettingsText();
        }
    }

    //adjust the audio settings for sound effects
    public void AdjustSFXVolume(bool increase)
    {
        if (NormalButtonSFX != null) { NormalButtonSFX.Play(); }
        if (!isScrolling)
        {
            int sfx = audioManager.getSFXVolume();
            if (increase && sfx < 10)
            {
                sfx += 1;
            }
            else if (!increase && sfx > 0)
            {
                sfx -= 1;
            }
            audioManager.UpdateSFXVolume(sfx);
            UpdateSettingsText();
        }
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
    }
    
    //save text speed to playerprefs
    void SaveTextSpeed()
    {
        PlayerPrefs.SetFloat(Constants.TextSpeed, speed);
    }

    //if selector is activated it still needs to reflect mouseover options
    public void MoveSelector(GameObject button)
    {
        if (!isScrolling)
        {
            selectedButton = button;
            //selectedButtonIndex = ArrayUtility.IndexOf<GameObject>(buttonArray, button);
            GameObject[] buttonArray = new GameObject[] { };
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
            selectedButtonIndex = Array.IndexOf(buttonArray, button);
            //set hover to match dimensions of selected button
            buttonHighlight.transform.position = selectedButton.transform.position;
            float width = selectedButton.GetComponent<RectTransform>().rect.width;
            float height = selectedButton.GetComponent<RectTransform>().rect.height;
            buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 5);
            buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 5);
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
    }
}
