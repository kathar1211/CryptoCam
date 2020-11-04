using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//alternative to custominputemanager because it doesn't let me programatically edit the keys
public class CustomController : MonoBehaviour {

    //map names to inputs here
    static Dictionary<string, KeyCode> buttons = new Dictionary<string, KeyCode>();

    //store default inputs
    static Dictionary<string, KeyCode> defaultButtons = new Dictionary<string, KeyCode>
    {
        {Constants.ReadyCamera, KeyCode.Mouse1 },
        {Constants.TakePicture, KeyCode.Mouse0 },
        {Constants.ThrowObject, KeyCode.Q },
        {Constants.Pause, KeyCode.Escape }
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

    //true if a saved button is not being pressed (false if button does not exist)
    public static bool GetButtonUp(string buttonName)
    {
        if (buttons.ContainsKey(buttonName))
        {
            return Input.GetKeyUp(buttons[buttonName]);
        }
        return false;
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

}
