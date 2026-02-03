using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[Serializable]
public struct ButtonOrAxis
{
    public bool IsButton;
    public KeyCode Key;
    public string AxisName;
    public string PlayerFacingName;

    public ButtonOrAxis(KeyCode key)
    {
        IsButton = true;
        Key = key;
        AxisName = "";
        PlayerFacingName = CustomController.GetButtonDisplayName(key);
    }

    public ButtonOrAxis(string axis)
    {
        IsButton = false;
        Key = KeyCode.None;
        AxisName = axis;
        PlayerFacingName = CustomController.GetAxisDisplayName(axis);
    }
}

//alternative to custominputemanager because it doesn't let me programatically edit the keys
public class CustomController : MonoBehaviour {

    //https://discussions.unity.com/t/xbox-one-controller-mapping-solved/187077/7

    //map names to inputs here
    static Dictionary<string, ButtonOrAxis> buttons = new Dictionary<string, ButtonOrAxis>();

    //store default inputs
    static Dictionary<string, ButtonOrAxis> defaultButtons = new Dictionary<string, ButtonOrAxis>
    {
        {Constants.ReadyCamera, new ButtonOrAxis(KeyCode.Mouse1) },
        {Constants.TakePicture, new ButtonOrAxis(KeyCode.Mouse0) },
        {Constants.ThrowObject,new ButtonOrAxis( KeyCode.Q) },
        {Constants.Pause, new ButtonOrAxis(KeyCode.Escape) },
        {Constants.CrouchButton, new ButtonOrAxis(KeyCode.LeftControl) },
        {Constants.RunButton, new ButtonOrAxis(KeyCode.LeftShift) }
    };

    static Dictionary<string, ButtonOrAxis> defaultGamepadButtons = new Dictionary<string, ButtonOrAxis>
    {
        {Constants.ReadyCamera, new ButtonOrAxis(Constants.LTAxis) },
        {Constants.TakePicture, new ButtonOrAxis(Constants.RTAxis) },
        {Constants.ThrowObject,new ButtonOrAxis( KeyCode.Joystick1Button10) },
        {Constants.Pause, new ButtonOrAxis(KeyCode.Joystick1Button0) },
        {Constants.CrouchButton, new ButtonOrAxis(KeyCode.LeftControl) },
        {Constants.RunButton, new ButtonOrAxis(KeyCode.LeftShift) }
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

    public static ReadOnlyCollection<String> AllGamepadAxesAsButtons = new ReadOnlyCollection<string>(new List<string>
    {
        Constants.LTAxis,
        Constants.RTAxis
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
            ButtonOrAxis input = buttons[buttonName];
            if (input.IsButton) { return Input.GetKeyDown(input.Key); }
            else { return CrossPlatformInputManager.GetButtonOrAxisDown(input.AxisName); }
        }
        return false;
    }

    //true if a saved button is being pressed (false if button does not exist)
    public static bool GetButton(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            ButtonOrAxis input = buttons[buttonName];

            if (input.IsButton) { return Input.GetKey(input.Key); ; }
            else { return CrossPlatformInputManager.GetAxis(input.AxisName) != 0; }
        }
        return false;
    }

    //true if a saved button was released this frame (false if button does not exist)
    public static bool GetButtonUp(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            ButtonOrAxis input = buttons[buttonName];
            if (input.IsButton) { return Input.GetKeyUp(input.Key); }
            else { return CrossPlatformInputManager.GetButtonOrAxisUp(input.AxisName); }
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
        foreach (KeyValuePair<string,ButtonOrAxis> button in buttons)
        {
            PlayerPrefs.SetString(button.Key, JsonUtility.ToJson(button.Value));
        }
    }

    //load values from playerprefs and update dictionary
    public static void LoadAllKeys()
    {
        //restore defaults first so if any buttons are skipped in playerprefs we still have values for them
        RestoreDefaults();
        //expected button names are stored as keys in the default button dictionary
        foreach (KeyValuePair<string,ButtonOrAxis> button in defaultButtons)
        {
            if (PlayerPrefs.HasKey(button.Key))
            {
                try
                {
                    buttons[button.Key] = JsonUtility.FromJson<ButtonOrAxis>(PlayerPrefs.GetString(button.Key));
                }
                catch
                {
                    continue;
                }
            }
        }
    }

    //restore default control settings
    public static void RestoreDefaults()
    {
        buttons = new Dictionary<string, ButtonOrAxis>(defaultButtons);
    }

    //used for displaying what the current input settings are
    public static string GetButtonInput(string name)
    {
        if (buttons.ContainsKey(name))
        {
            ButtonOrAxis button = buttons[name];
            if (button.IsButton) { return GetButtonDisplayName(button.Key); }
            else { return GetAxisDisplayName(button.AxisName); }
        }
        return null;
    }

    public static string GetButtonDisplayName(KeyCode key)
    {
        switch (key)
        {
            //convert mouse0 mouse1 etc to names humans understand better
            case KeyCode.Mouse0:
                return "Left Click";
            case KeyCode.Mouse1:
                return "Right Click";

            //these mappings are different on mac   
            //https://discussions.unity.com/t/xbox-one-controller-mapping-solved/187077/7
#if UNITY_STANDALONE_OSX
                //same for joystick buttons
                 case KeyCode.JoystickButton16:
                    return "A";
                case KeyCode.JoystickButton17:
                    return "B";
                case KeyCode.JoystickButton18:
                    return "X";
                case KeyCode.JoystickButton19:
                    return "Y";
                case KeyCode.JoystickButton13:
                    return "LB";
                case KeyCode.JoystickButton14:
                    return "RB";
                case KeyCode.JoystickButton10:
                    return "Select";
                case KeyCode.JoystickButton9:
                    return "Start";
                case KeyCode.JoystickButton11:
                    return "Left Stick Button";
                case KeyCode.JoystickButton12:
                    return "Right Stick Button";
#else
            //same for joystick buttons
            case KeyCode.JoystickButton0:
                return "A";
            case KeyCode.JoystickButton1:
                return "B";
            case KeyCode.JoystickButton2:
                return "X";
            case KeyCode.JoystickButton3:
                return "Y";
            case KeyCode.JoystickButton4:
                return "LB";
            case KeyCode.JoystickButton5:
                return "RB";
            case KeyCode.JoystickButton6:
                return "Select";
            case KeyCode.JoystickButton7:
                return "Start";
            case KeyCode.JoystickButton8:
                return "Left Stick Button";
            case KeyCode.JoystickButton9:
                return "Right Stick Button";
#endif
            default:
                return key.ToString();
        }
    }

    public static string GetAxisDisplayName(string axis)
    {
        switch (axis)
        {
            case Constants.RTAxis:
                return "RT";
            case Constants.LTAxis:
                return "LT";
            default:
                return axis;
        }
    }

    //update button mapping for a given key
    public static void SetButton(string name, KeyCode key)
    {
        if (buttons.ContainsKey(name))
        {
            buttons[name] = new ButtonOrAxis(key);
        }
        else //this shouldn't happen- added controls won't be loaded later
        {
            buttons.Add(name, new ButtonOrAxis(key));
        }
    }

    public static void SetAxis(string controlName, string axisName)
    {
        if (buttons.ContainsKey(controlName))
        {
            buttons[controlName] = new ButtonOrAxis(axisName);
        }
        else //this shouldn't happen- added controls won't be loaded later
        {
            buttons.Add(controlName, new ButtonOrAxis(axisName));
        }
    }

    public bool IsControllerConnected()
    {
        string[] controllerNames = Input.GetJoystickNames();
        if (controllerNames.Length == 0) { return false; }
        return true;
    }

}
