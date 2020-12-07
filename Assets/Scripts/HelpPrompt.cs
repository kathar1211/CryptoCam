using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class HelpPrompt : MonoBehaviour {


    //text to help people with controls
    [SerializeField]
    Text helpPrompt;
    //time in seconds of no input before showing the help prompt
    public int helpDelay;
    //fade in by incrementing opcaity by this amount
    public float opacityIncrement;
    //hide the help prompt again after this amount of time in seconds
    public int helpDuration;

    //keep track of whether player needs help with photos
    bool needsPhotoHelp = true;
    //keeps track of whether player needs help with objects
    bool needsObjectHelp = true;

    //true when fading in/out the prompt
    bool showingPrompt = false;
    bool hidingPrompt = false;

    float gameTimer = 0;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //if all these things are false our job is done here
        if (!needsPhotoHelp && !needsObjectHelp && !showingPrompt && !hidingPrompt) { return; }

        gameTimer += Time.deltaTime;

        //if player uses a function they don't need help
        if (CrossPlatformInputManager.GetButtonDown(Constants.TakePicture))
        {
            needsPhotoHelp = false;
        }
        if (CrossPlatformInputManager.GetButtonDown(Constants.ThrowObject))
        {
            needsObjectHelp = false;
        }

        //start the process of showing the prompt when allotted time has passed
        if (gameTimer >= helpDelay && !showingPrompt && !hidingPrompt)
        {
            //prioritize - show picture prompt first
            if (needsPhotoHelp)
            {
                string display = Constants.PhotoHelpPrompt1.Replace(Constants.ParameterSTR, CustomController.GetButtonInput(Constants.ReadyCamera))
                    + Constants.PhotoHelpPrompt2.Replace(Constants.ParameterSTR, CustomController.GetButtonInput(Constants.TakePicture));
                helpPrompt.text = display;
                needsPhotoHelp = false;
            }
            else if (needsObjectHelp)
            {
                helpPrompt.text = Constants.ObjectHelpPrompt.Replace(Constants.ParameterSTR, CustomController.GetButtonInput(Constants.ThrowObject));
                needsObjectHelp = false;
            }
            else
            {
                //safeguard - if no help is needed set text to blank
                helpPrompt.text = "";
            }
            showingPrompt = true;
            gameTimer = 0;
        }

        //gradually increase opacity to fade in text
        if (showingPrompt)
        {
            helpPrompt.color = new Color(helpPrompt.color.r, helpPrompt.color.g, helpPrompt.color.b, helpPrompt.color.a + opacityIncrement);
            if (gameTimer >= helpDuration)
            {
                showingPrompt = false;
                hidingPrompt = true;
            }
        }

        //gradually decrease opacity to fade out text
        if (hidingPrompt)
        {
            helpPrompt.color = new Color(helpPrompt.color.r, helpPrompt.color.g, helpPrompt.color.b, helpPrompt.color.a - opacityIncrement);
            if (helpPrompt.color.a <= 0)
            {
                showingPrompt = false;
                hidingPrompt = false;
                gameTimer = 0;
            }
        }

    }
}
