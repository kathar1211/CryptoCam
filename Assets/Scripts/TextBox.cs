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

	// Use this for initialization
	void Start () {
        txt = this.transform.GetChild(0).GetComponent<Text>();
        talking = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //allow other classes to display dialogue
    public void DisplayText(string text)
    {
        //cancel previous dialoge
        if (talking)
        {
            StopCoroutine("displayText");
        }

        StartCoroutine("displayText",text);
    }

    public void Skip()
    {
        //stop the dialogue scrolling animation and just display the text
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
}
