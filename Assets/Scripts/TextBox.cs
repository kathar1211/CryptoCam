﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class TextBox : MonoBehaviour {

    Text txt; //where to put text
    public Text scoretxt; //in cases where score is displayed and updated with text
    public float textDelay; //text scroll speed
    int textIndex;

    public Ted ted; //update ted sprites with text as needed
    Queue<TedMoods> allTeds; //queue ted sprites along with text lines
    Queue<string> allScores;

    bool talking; //keep track of whether text is being displayed

    string currentText;
    Queue<string> allText;

    [SerializeField]
    AudioSource textSFX;

    [SerializeField]
    Animator ButtonHolder;
    [SerializeField]
    Button LeftOptionButton;
    [SerializeField]
    Button RightOptionButton;

    //decide whether or not text box disappears when finished scrolling through text
    public bool CloseOnTextComplete { get; set; }

    //if something should happen when this line is displayed, store it here
    private Dictionary<string, Action> linesThatDoThings;

    //if we need to stop dialogue from progressing until something happens, use this
    private bool progressLocked = false;

	// Use this for initialization
	void Awake () {
        txt = this.transform.GetChild(0).GetComponent<Text>();
        CloseOnTextComplete = true; //default setting
        talking = false;

        allText = new Queue<string>();
        allTeds = new Queue<TedMoods>();
        allScores = new Queue<string>();
        linesThatDoThings = new Dictionary<string, Action>();
        //this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
        //scroll through text if there is text in the queue
        if (allText!= null && allText.Count != 0 && !progressLocked)
        {
            if (CrossPlatformInputManager.GetButtonDown(Constants.Submit))
            {
                Continue();
            }
        }
        else if (!progressLocked)
        {
            //if theres no text in the queue we're done talking or on the last line
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                
                if (talking)
                {
                    Skip();
                }
                else
                {
                    if (scoretxt != null) { scoretxt.text = ""; }
                    if (CloseOnTextComplete) { this.gameObject.SetActive(false); }
                }
            }
        }

	}

    //skip or move on to the next line, called when input to advance is detected
    void Continue()
    {
        if (talking)
        {
            Skip();
        }
        else
        {
            if (allTeds.Count != 0 && ted != null)
            {
                ted.SetTedSprite(allTeds.Dequeue());
            }

            if (allScores.Count != 0 && scoretxt != null)
            {
                scoretxt.text = allScores.Dequeue();
            }
            else
            {
                scoretxt.text = "";
            }
            DisplayText(allText.Dequeue());

        }
    }

    //allow other classes to display dialogue
    public void DisplayText()
    {
        this.gameObject.SetActive(true);
        if (allText.Count != 0)
        {
            DisplayText(allText.Dequeue());
        }
        if (allTeds.Count != 0)
        {
            ted.SetTedSprite(allTeds.Dequeue());
        }
    }

    void DisplayText(string text)
    {
        //cancel previous dialoge
        if (talking)
        {
            StopCoroutine("displayText");
        }

        StartCoroutine("displayText",text);
    }

    void DisplayTextWithTed(string text, TedMoods tedMood)
    {
        if (ted != null && !talking)
        {
            ted.SetTedSprite(tedMood);
        }
        displayText(text);
    }


    //stop the dialogue scrolling animation and just display the text
    public void Skip()
    {
        if (talking)
        {
            StopCoroutine("displayText");
            txt.text = currentText;
            talking = false;
        }
    }

    //show new dialogue
    IEnumerator displayText(string text)
    {
        //start by wiping out preexisting text
        txt.text = "";
        currentText = text;
        textIndex = 0;
        talking = true;

        //get current text speed value by multiplying the delay by the saved modifier
        float modifiedDelay = textDelay / LoadTextSpeed();

        //if there's an action associated with this text, start it now
        if (linesThatDoThings.ContainsKey(text))
        {
            linesThatDoThings[text].Invoke();

            //remove it from our dictionary now that it's no longer needed
            linesThatDoThings.Remove(text);
        }

        while (textIndex < text.Length)
        {
            txt.text += text[textIndex];
            textIndex++;
            //todo: play sound effect for text scrolling
            if (textSFX != null ) { textSFX.Play(); }
            yield return new WaitForSeconds (modifiedDelay);
        }

        talking = false;
       
    }

    //each new line of text should be its own instance in the array
    public string[] ParseLongText(string longtext)
    {
        string[] final = longtext.Split('\n');
        return final;
    }

    //send text to a queue to be displayed line by line
    public void FeedText(string[] text)
    {
        //takes an array of strings and puts them in the queue
        foreach(string line in text)
        {
            FeedText(line);
        }
        
    }

    //a list is fine too
    public void FeedText(List<string> text)
    {
        foreach (string line in text)
        {
            FeedText(line);
        }
    }

    //overload method that also take a ted sprite to go with each line
    public void FeedText(string[] text, TedMoods[] sprites, string[] scores = null)
    {
        foreach (string line in text)
        {
            FeedText(line);
        }
        foreach (TedMoods ted in sprites)
        {
            FeedTed(ted);
        }
        if (scores!= null)
        {
            foreach (string score in scores)
            {
                FeedScore(score);
            }
        }
    }

    //overload method that also take a ted sprite to go with each line
    public void FeedText(List<string> text, List<TedMoods> sprites, List<string> scores = null)
    {
        foreach (string line in text)
        {
            FeedText(line);
        }
        foreach (TedMoods ted in sprites)
        {
            FeedTed(ted);
        }
        if (scores != null)
        {
            foreach (string score in scores)
            {
                FeedScore(score);
            }
        }
    }

    //allow adding single lines of text
    public void FeedText(string line)
    {
        allText.Enqueue(line); 
    }

    //allow adding single lines with a single sprite
    public void FeedText(string line, TedMoods sprite)
    {
        allText.Enqueue(line);
        allTeds.Enqueue(sprite);
    }

    //add a single line of text with an accompanying action
    public void FeedText(string line, Action lineAction)
    {
        linesThatDoThings.Add(line, lineAction);
        FeedText(line);
    }

    //add sprites to ted queue
    public void FeedTed(TedMoods ted)
    {
        allTeds.Enqueue(ted);
    }
    //queue score indicators if necessary
    public void FeedScore(string score)
    {
        allScores.Enqueue(score);
    }
        
    //method to cancel text thats been sent
    public void ClearTextQueue()
    {
        allText.Clear();
        allTeds.Clear();
        allScores.Clear();
        if (talking) { StopCoroutine("displayText"); }
        talking = false;
    }

    //grab current text delay modifier
    float LoadTextSpeed()
    {
        if (PlayerPrefs.HasKey(Constants.TextSpeed))
        {
            return PlayerPrefs.GetFloat(Constants.TextSpeed);
        }

        //if nothing is saved return 1 (no modifier)
        return 1;
    }

    public void ButtonsIn()
    {
        if (ButtonHolder != null)
        {
            ButtonHolder.gameObject.SetActive(true);
            ButtonHolder.Play("ButtonsIn");

            //lock progress while we wait for input
            progressLocked = true;
        }
    }

    public void ButtonsOut()
    {
        if (ButtonHolder != null)
        {
            ButtonHolder.Play("ButtonsOut");
        }
        progressLocked = false;
    }

    public void SetLeftButton(string buttonText, Action buttonAction, bool shouldAdvanceDialogue = true)
    {
        if (LeftOptionButton != null)
        {
            LeftOptionButton.onClick.RemoveAllListeners();
            LeftOptionButton.onClick.AddListener(() => buttonAction.Invoke());
            if (shouldAdvanceDialogue) { LeftOptionButton.onClick.AddListener(() => Continue()); }
            Text buttonLabel = LeftOptionButton.GetComponentInChildren<Text>();
            if (buttonLabel != null) { buttonLabel.text = buttonText; }
        }
    }

    public void SetRightButton(string buttonText, Action buttonAction, bool shouldAdvanceDialogue = true)
    {
        if (RightOptionButton != null)
        {
            RightOptionButton.onClick.RemoveAllListeners();
            RightOptionButton.onClick.AddListener(() => buttonAction.Invoke());
            if (shouldAdvanceDialogue) { RightOptionButton.onClick.AddListener(() => Continue()); }
            Text buttonLabel = RightOptionButton.GetComponentInChildren<Text>();
            if (buttonLabel != null) { buttonLabel.text = buttonText; }
        }
    }
}
