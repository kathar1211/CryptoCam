using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;


public class Options : MonoBehaviour {

    //buttons
    public GameObject ReadyCameraControl;
    public GameObject TakePictureControl;
    public GameObject ThrowObjectControl;
    public GameObject PauseControl;
    public GameObject Exit;
    public GameObject RestoreDefaultsControl;
    public GameObject IncreaseText;
    public GameObject DecreaseText;
    public GameObject FullScreenOn;
    public GameObject FullScreenOff;

    //display the current text speed
    public Text textSpeed;
    private float speed;
    public float maxSpeed;
    public float minSpeed;
    public float increment;

    //select buttons with arrow keys/controller and highlight selected
    public GameObject buttonHighlight;
    //hold buttons in an array that represents the order theyre in on screen
    private GameObject[] buttonArray;
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

	// Use this for initialization
	void Start () {
        buttonArray = new GameObject[] { ReadyCameraControl, TakePictureControl,
            ThrowObjectControl, PauseControl, RestoreDefaultsControl, DecreaseText,
            IncreaseText, FullScreenOn, FullScreenOff, Exit };

        //if controls are saved in playerprefs load them
        CustomController.LoadAllKeys();
        LoadTextSpeed();

        UpdateButtonText();
    }
	
	// Update is called once per frame
	void Update () {
		if (CrossPlatformInputManager.GetButtonDown(Constants.Vertical))
        {
            ChangeSelectButton(CrossPlatformInputManager.GetAxis(Constants.Vertical));
        }

        if (CrossPlatformInputManager.GetButtonDown(Constants.Submit))
        {
            if (selectedButton != null) { selectedButton.GetComponent<Button>().onClick.Invoke(); }
        }
	}

    void ChangeSelectButton(float input)
    {
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
        if (!waitingForKeyPress) {
            StartCoroutine(AssignKey(controlKey));
            KeyPressSubMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
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

        yield return null;
    }

    //set control buttons to display current configuration
    void UpdateButtonText()
    {
        ReadyCameraControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.ReadyCamera);
        TakePictureControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.TakePicture);
        ThrowObjectControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.ThrowObject);
        PauseControl.GetComponentInChildren<Text>().text = CustomController.GetButtonInput(Constants.Pause);
        textSpeed.text = speed.ToString("0.0");
    }

    public void RestoreDefaults()
    {
        CustomController.RestoreDefaults();
        UpdateButtonText();
        CustomController.SaveAllKeys();
    }

    //method to change the speed at which dialogue appears. true to increase false to decrease
    public void AdjustTextSpeed (bool increase)
    {
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
        UpdateButtonText();
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
        selectedButton = button;
        //selectedButtonIndex = ArrayUtility.IndexOf<GameObject>(buttonArray, button);
        selectedButtonIndex = Array.IndexOf(buttonArray, button);
        buttonHighlight.transform.position = selectedButton.transform.position;
        float width = selectedButton.GetComponent<RectTransform>().rect.width;
        float height = selectedButton.GetComponent<RectTransform>().rect.height;
        buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 5);
        buttonHighlight.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 5);
    }
}
