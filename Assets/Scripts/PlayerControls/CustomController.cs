using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

//alternative to custominputemanager because it doesn't let me programatically edit the keys
public class CustomController : MonoBehaviour {

    //map names to inputs here
    static Dictionary<string, ButtonControl> buttons = new Dictionary<string, ButtonControl>();

    //store default inputs
    static Dictionary<string, ButtonControl> defaultButtons = new Dictionary<string, ButtonControl>
    {
        {Constants.ReadyCamera, Mouse.current.rightButton},
        {Constants.TakePicture,  Mouse.current.leftButton},
        {Constants.ThrowObject, Keyboard.current.qKey },
        {Constants.Pause, Keyboard.current.escapeKey },
        {Constants.CrouchButton, Keyboard.current.leftCtrlKey },
        {Constants.RunButton, Keyboard.current.leftShiftKey }
    };

    static Dictionary<string, ButtonControl> defaultGamepadButtons = new Dictionary<string, ButtonControl>
    {
        {Constants.ReadyCamera, Gamepad.current.rightShoulder},
        {Constants.TakePicture, Gamepad.current.leftShoulder },
        {Constants.ThrowObject, Gamepad.current.xButton },
        {Constants.Pause, Gamepad.current.startButton },
        {Constants.CrouchButton, Gamepad.current.bButton },
        {Constants.RunButton, Gamepad.current.leftStickButton }
    };

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //true if a saved button was pressed down this frame (false if button does not exist)
    public static bool GetButtonDown(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return buttons[buttonName].wasPressedThisFrame;
        }
        return false;
    }

    //true if a saved button is being pressed (false if button does not exist)
    public static bool GetButton(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return buttons[buttonName].isPressed;
        }
        return false;
    }

    //true if a saved button was released this frame (false if button does not exist)
    public static bool GetButtonUp(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return buttons[buttonName].wasReleasedThisFrame;
        }
        return false;
    }

    //write current values to playerprefs
    public static void SaveAllKeys()
    {
        foreach (KeyValuePair<string,ButtonControl> button in buttons)
        {
            PlayerPrefs.SetInt(button.Key, (int)button.Value);
        }
    }

    //load values from playerprefs and update dictionary
    public static void LoadAllKeys()
    {
        //restore defaults first so if any buttons are skipped in playerprefs we still have values for them
        RestoreDefaults();
        //expected button names are stored as keys in the default button dictionary
        foreach (KeyValuePair<string,KeyCode> button in defaultButtons)
        {
            if (PlayerPrefs.HasKey(button.Key))
            {
                buttons[button.Key] = (KeyCode)PlayerPrefs.GetInt(button.Key);
            }
        }
    }

    //restore default control settings
    public static void RestoreDefaults()
    {
        buttons = new Dictionary<string, ButtonControl>(defaultButtons);
    }

    //used for displaying what the current input settings are
    public static string GetButtonInput(string name)
    {
        if (buttons.ContainsKey(name))
        {
            //convert mouse0 mouse1 etc to names humans understand better
            switch (buttons[name])
            {
                case KeyCode.Mouse0:
                    return "Left Click";
                case KeyCode.Mouse1:
                    return "Right Click";
                default:
                    return buttons[name].ToString();
            }
        }
        return null;
    }

    //update button mapping for a given key
    public static void SetButton(string name, KeyCode key)
    {
        if (buttons.ContainsKey(name))
        {
            buttons[name] = key;
        }
        else //this shouldn't happen- added controls won't be loaded later
        {
            buttons.Add(name, key);
        }
    }

    public bool IsControllerConnected()
    {
        string[] controllerNames = Input.GetJoystickNames();
        if (controllerNames.Length == 0) { return false; }
        return true;
    }

}
