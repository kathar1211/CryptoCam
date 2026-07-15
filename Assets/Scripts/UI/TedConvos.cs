using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TedConvos : MonoBehaviour {

    //text file where talk blurbs are saved
    public TextAsset script;
    //text box to save lines to
    public TextBox tedBox;
    //text file with convos
    public TextAsset txtFile;
    
    //a blurb represents a short bit of dialogue, and the sprites associated with it
    public struct Blurb
    {
       public List<string> dialogue;
       public List<TedMoods> sprites;
    }

    //all stored blurbs read in from file
    List<Blurb> allBlurbs = new List<Blurb>();
    //keep track of what ted has already said to reduce the amount he repeats himself
    List<Blurb> readBlurbs = new List<Blurb>(); 

	// Use this for initialization
	void Start () {
        ReadConvos();
	}
	
	// Update is called once per frame
	void Update () {

    }

    //grab a random saved blurb for display
    public void Talk()
    {
        //if we've already said all the blurbs, restore them
        if (allBlurbs.Count == 0)
        {
            allBlurbs = new List<Blurb>(readBlurbs);
            readBlurbs.Clear();
        }

        //grab blurbs in order until player has read them all, then grab random
        int blurbIndex = 0;
        if (PlayerPrefs.HasKey(Constants.HasSeenAllTedDialogue) && (PlayerPrefs.GetInt(Constants.HasSeenAllTedDialogue) == 1))
        {
            blurbIndex = Random.Range(0, allBlurbs.Count - 1);
        }
        else
        {
            blurbIndex = PlayerPrefs.GetInt(Constants.TedTalkIndex, 0);
        }
      
        Blurb nextblurb = allBlurbs[blurbIndex];
        tedBox.GetComponent<TextBox>().FeedText(nextblurb.dialogue, nextblurb.sprites);
        tedBox.GetComponent<TextBox>().DisplayText(); //display text should be called when finished feeding lines

        //mark that ted has now said this already
        blurbIndex++;
        PlayerPrefs.SetInt(Constants.TedTalkIndex, blurbIndex);
        if (blurbIndex >= allBlurbs.Count) { PlayerPrefs.SetInt(Constants.HasSeenAllTedDialogue, 1); }
    }
    
    ///
    void ReadConvos()
    {

        string startKey = "/STARTBLURB/";
        string endKey = "/ENDBLURB/";

        //keep track of whether the line we're reading is part of a blurb
        bool readingBlurb = false;
        //keep track of lines encountered while reading a blurb
        List<string> blurbLines = new List<string>();

        string[] allLines = txtFile.text.Split('\n');

        //read the first line and start looping
        //string currentLine = reader.ReadLine();
        foreach (string currentLine in allLines)
        {
            //if we find the start key, start paying attention and do nothing else
            if (currentLine.Contains(startKey))
            {
                readingBlurb = true;
                continue;
            }

            //if we find the stop key, stop paying attention and create a blurb from what we have saved
            if (currentLine.Contains(endKey))
            {
                readingBlurb = false;
                allBlurbs.Add(ParseBlurbFromText(blurbLines));
                blurbLines.Clear();
                continue;
            }

            //if we're reading a blurb save the line
            if (readingBlurb)
            {
                blurbLines.Add(currentLine);
            }

        }
    }

    //take text file text and interpret into blurb
    Blurb ParseBlurbFromText(List<string> text)
    {       
        Blurb myBlurb = new Blurb();
        myBlurb.dialogue = new List<string>();
        myBlurb.sprites = new List<TedMoods>();

        foreach (string line in text)
        {
            //for each line, '/' is the character separating the tedmood enum value from the dialogue string
            string[] components = line.Split('/');
            //each line should have two components: a sprite and a line of dialogue
            if (components.Length >= 2)
            {
                myBlurb.dialogue.Add(components[1]);
                int sprite = 0;
                int.TryParse(components[0], out sprite);
                myBlurb.sprites.Add((TedMoods)sprite);
            }
        }

        return myBlurb;
    }
}
