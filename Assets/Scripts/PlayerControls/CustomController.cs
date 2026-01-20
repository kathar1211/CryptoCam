using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

//alternative to custominputemanager because it doesn't let me programatically edit the keys
public class CustomController : MonoBehaviour {

    //https://discussions.unity.com/t/xbox-one-controller-mapping-solved/187077/7

    //map names to inputs here
    static Dictionary<string, KeyCode> buttons = new Dictionary<string, KeyCode>();

    //store default inputs
    static Dictionary<string, KeyCode> defaultButtons = new Dictionary<string, KeyCode>
    {
        {Constants.ReadyCamera, KeyCode.Mouse1 },
        {Constants.TakePicture, KeyCode.Mouse0 },
        {Constants.ThrowObject, KeyCode.Q },
        {Constants.Pause, KeyCode.Escape },
        {Constants.CrouchButton, KeyCode.LeftControl },
        {Constants.RunButton, KeyCode.LeftShift }
    };

    static Dictionary<string, KeyCode> defaultGamepadButtons = new Dictionary<string, KeyCode>
    {
        {Constants.ReadyCamera, KeyCode.Joystick1Button11 },
        {Constants.TakePicture, KeyCode.Joystick1Button14 },
        {Constants.ThrowObject, KeyCode.Joystick1Button10 },
        {Constants.Pause, KeyCode.Joystick1Button0 },
        {Constants.CrouchButton, KeyCode.LeftControl },
        {Constants.RunButton, KeyCode.LeftShift }
    };

    //need this for detecting input from a gamepad during remapping
    public static ReadOnlyCollection<KeyCode> AllGamepadButtons = new ReadOnlyCollection<KeyCode>(new List<KeyCode> 
    {
         KeyCode.JoystickButton19,
        KeyCode.JoystickButton0,
        KeyCode.JoystickButton1,
        KeyCode.JoystickButton2,
        KeyCode.JoystickButton3,
        KeyCode.JoystickButton4,
        KeyCode.JoystickButton5,
        KeyCode.JoystickButton6,
        KeyCode.JoystickButton7,
        KeyCode.JoystickButton8,
        KeyCode.JoystickButton9,
        KeyCode.JoystickButton10,
        KeyCode.JoystickButton11,
        KeyCode.JoystickButton12,
        KeyCode.JoystickButton13,
        KeyCode.JoystickButton14,
        KeyCode.JoystickButton15,
        KeyCode.JoystickButton16,
        KeyCode.JoystickButton17,
        KeyCode.JoystickButton18,
       
    });

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
            return Input.GetKeyDown(buttons[buttonName]);
        }
        return false;
    }

    //true if a saved button is being pressed (false if button does not exist)
    public static bool GetButton(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return Input.GetKey(buttons[buttonName]);
        }
        return false;
    }

    //true if a saved button was released this frame (false if button does not exist)
    public static bool GetButtonUp(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return Input.GetKeyUp(buttons[buttonName]);
        }
        return false;
    }

    public static float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    //write current values to playerprefs
    public static void SaveAllKeys()
    {
        foreach (KeyValuePair<string,KeyCode> button in buttons)
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
        buttons = new Dictionary<string, KeyCode>(defaultButtons);
    }

    //used for displaying what the current input settings are
    public static string GetButtonInput(string name)
    {
        if (buttons.ContainsKey(name))
        {
            
            switch (buttons[name])
            {
                //convert mouse0 mouse1 etc to names humans understand better
                case KeyCode.Mouse0:
                    return "Left Click";
                case KeyCode.Mouse1:
                    return "Right Click";

                //same for joystick buttons
                case KeyCode.JoystickButton0:
                    return "button 0";
                default:;
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
