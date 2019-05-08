using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradeManager : MonoBehaviour {

    [SerializeField]
    Image[] thumbnails;
    [SerializeField]
    TextBox textbox;
    [SerializeField]
    Image bigThumbnail;
    Image selectedImage;

    GameObject YesButton;
    GameObject NoButton;

    GameObject gameManager;

    enum GradeState { allThumbs, bigThumb};
    GradeState currentState;

    //photos selected for grading, sorted by subject name
    //only one of each cryptid type is allowed
    public Dictionary<string, Photograph> finalSelection = new Dictionary<string, Photograph>();

    //store photograph information that corresponds with each thumbnail
    Dictionary<Image, Photograph> allPhotos = new Dictionary<Image, Photograph>();

	// Use this for initialization
	void Start () {

        //find the game manager, which has all the data we need from the previous level stored
        gameManager = GameObject.Find("GameManager");

        YesButton = GameObject.Find("YesButton");
        NoButton = GameObject.Find("NoButton");

        YesButton.SetActive(false);
        NoButton.SetActive(false);

        //hide the symbol that represents a selected photo bc none of them are selected yet
        foreach(Image img in thumbnails)
        {
            img.gameObject.transform.Find("Selected").gameObject.SetActive(false);
        }

        //display the pictures from the game manager
        ShowThumbnails(gameManager.GetComponent<Photography>().GetPhotographs());

        textbox.FeedText("Select pictures to show to Ted");
        textbox.DisplayText();

        bigThumbnail.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //yes/no buttons show up when user needs to enter input, disappear after
    void ToggleInputButtons()
    {
        YesButton.SetActive(!YesButton.activeSelf);
        NoButton.SetActive(!NoButton.activeSelf);
    }

    //bring up image on click
    public void Enlarge(Image src)
    {
        selectedImage = src;
        bigThumbnail.sprite = src.sprite;
        bigThumbnail.gameObject.SetActive(true);
        textbox.FeedText("Select this picture?");
        textbox.DisplayText();
        ToggleInputButtons();
    }

    //selected photo is added to dictionary, return to view of all photos 
    public void YesButtonClick()
    {
        //indicate selection
        selectedImage.gameObject.transform.Find("Selected").gameObject.SetActive(true);

        //add photo to dictionary
        Photograph picToAdd = allPhotos[selectedImage];
        if (!finalSelection.ContainsKey(picToAdd.subjectName))
        {
            finalSelection.Add(picToAdd.subjectName, picToAdd);
        }
        //if a photo of that cryptid has already been added, deselect and replace
        else
        {
            Image deselect = null;
            Photograph toRemove = finalSelection[picToAdd.subjectName];
            //looping through dictionary to find image is a hit to performance, 
            //but preferable to hit to memory from an inverse dictionary of hd photos
            foreach (KeyValuePair<Image,Photograph> pair in allPhotos)
            {
                if (pair.Value.Equals(toRemove))
                {
                    deselect = pair.Key;
                    break;
                }
            }

            if (deselect != null)
            {
                deselect.gameObject.transform.Find("Selected").gameObject.SetActive(false);
            }
            finalSelection[picToAdd.subjectName] = picToAdd;
        }

        //hide buttons and return to photo view
        ToggleInputButtons();
        Delarge();
    }

    //photo dismissed
    public void NoButtonClick()
    {
        //if photo has been added remove it
        selectedImage.gameObject.transform.Find("Selected").gameObject.SetActive(false);
        Photograph picToRemove = allPhotos[selectedImage];
        if (finalSelection.ContainsKey(picToRemove.subjectName)){
            finalSelection.Remove(picToRemove.subjectName);
        }

        ToggleInputButtons();
        Delarge();
    }

    public void Delarge()
    {
        bigThumbnail.gameObject.SetActive(false);
        textbox.FeedText("Select pictures to show to Ted");
        textbox.DisplayText();
    }

    //method to show all saved pics at the end of the level
    public void ShowThumbnails(Photograph[] pics)
    {
        for (int i = 0; i < thumbnails.Length; i++)
        {
            if (i >= pics.Length) { return; }

            //displayIm.sprite = Sprite.Create(pic, new Rect(0.0f, 0.0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
            thumbnails[i].sprite = Sprite.Create(pics[i].pic, new Rect(0f, 0f, pics[i].pic.width, pics[i].pic.height), new Vector2(.5f, .5f));
            allPhotos.Add(thumbnails[i], pics[i]);
        }
    }
}
