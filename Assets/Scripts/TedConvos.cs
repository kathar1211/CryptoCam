using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TedConvos : MonoBehaviour {

    //text file where talk blurbs are saved
    public TextAsset script;
    //text box to save lines to
    public TextBox tedBox;
    
    //a blurb represents a short bit of dialogue, and the sprites associated with it
    public struct Blurb
    {
       public List<string> dialogue;
       public List<TedMoods> sprites;
    }

    //all stored blurbs read in from file
    List<Blurb> allBlurbs = new List<Blurb>();

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
        int blurbIndex = Random.Range(0, allBlurbs.Count-1);
        tedBox.GetComponent<TextBox>().FeedText(allBlurbs[blurbIndex].dialogue, allBlurbs[blurbIndex].sprites);
        tedBox.GetComponent<TextBox>().DisplayText(); //display text should be called when finished feeding lines
    }

    //read script in from text file and save lines to blurbs
    //expected format for a blurb:
    ///STARTBLURB/
    //int representing sprite/"dialogue in quotes"
    ///ENDBLURB/
    void ReadConvos()
    {
        string path = "Assets/Resources/tedconvo.txt";
        StreamReader reader = new StreamReader(path);

        string startKey = "/STARTBLURB/";
        string endKey = "/ENDBLURB/";

        //keep track of whether the line we're reading is part of a blurb
        bool readingBlurb = false;
        //keep track of lines encountered while reading a blurb
        List<string> blurbLines = new List<string>();

        //read the first line and start looping
        string currentLine = reader.ReadLine();
        while (currentLine != null)
        {
            //if we find the start key, start paying attention and do nothing else
            if (currentLine == startKey)
            {
                readingBlurb = true;
                currentLine = reader.ReadLine();
                continue;
            }

            //if we find the stop key, stop paying attention and create a blurb from what we have saved
            if (currentLine == endKey)
            {
                readingBlurb = false;
                allBlurbs.Add(ParseBlurbFromText(blurbLines));
                blurbLines.Clear();
                currentLine = reader.ReadLine();
                continue;
            }

            //if we're reading a blurb save the line
            if (readingBlurb)
            {
                blurbLines.Add(currentLine);
            }

            //read next line to continue the loop
            currentLine = reader.ReadLine();
        }

        //close reader when finished
        reader.Close();
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
