using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

    Text txt; //where to put text
    public float textDelay; //text scroll speed
    int textIndex;

    bool talking; //keep track of whether text is being displayed

    string currentText;
    Queue<string> allText;

    //decide whether or not text box disappears when finished scrolling through text
    public bool CloseOnTextComplete { get; set; }

	// Use this for initialization
	void Awake () {
        txt = this.transform.GetChild(0).GetComponent<Text>();
        CloseOnTextComplete = true; //default setting
        talking = false;

        allText = new Queue<string>();
        //this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
        //scroll through text if there is text in the queue
        if (allText!= null && allText.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                if (talking)
                {
                    Skip();
                }
                else
                {
                    DisplayText(allText.Dequeue());
                }
            }
        }
        else
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
                    if (CloseOnTextComplete) { this.gameObject.SetActive(false); }
                }
            }
        }

	}

    //allow other classes to display dialogue
    public void DisplayText()
    {
        this.gameObject.SetActive(true);
        if(allText.Count!=0)
        DisplayText(allText.Dequeue());
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

        while (textIndex < text.Length)
        {
            

            txt.text += text[textIndex];
            textIndex++;
            yield return new WaitForSeconds (textDelay);

           
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

    //allow adding single lines of text
    public void FeedText(string line)
    {
        allText.Enqueue(line);
       
    }



    //method to cancel text thats been sent
    public void ClearTextQueue()
    {
        allText.Clear();
        if (talking) { StopCoroutine("displayText"); }
        talking = false;
    }
}
