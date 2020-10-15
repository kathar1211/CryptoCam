using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradeManager : MonoBehaviour {

    [SerializeField]
    Image[] thumbnails;
    [SerializeField]
    TextBox textbox; //this is the textbox that prompts to select photos
    [SerializeField]
    Image bigThumbnail;
    Image selectedImage;

    GameObject YesButton;
    GameObject NoButton;
    GameObject AutoButton;
    GameObject DoneButton;

    GameObject gameManager;

    [SerializeField]
    GameObject ConfirmScreen;

    enum GradeState { allThumbs, bigThumb, tedGrading};
    GradeState currentState;

    //photos selected for grading, sorted by subject name
    //only one of each cryptid type is allowed
    public Dictionary<string, Photograph> finalSelection = new Dictionary<string, Photograph>();

    //store photograph information that corresponds with each thumbnail
    Dictionary<Image, Photograph> allPhotos = new Dictionary<Image, Photograph>();

    //sound effects
    [SerializeField]
    AudioSource ClickSFX;
    [SerializeField]
    AudioSource ConfirmSFX;
    [SerializeField]
    AudioSource CancelSFX;

    // Use this for initialization
    void Start () {

        //find the game manager, which has all the data we need from the previous level stored
        gameManager = GameObject.Find("GameManager");

        YesButton = GameObject.Find("YesButton");
        NoButton = GameObject.Find("NoButton");
        DoneButton = GameObject.Find("Done");
        AutoButton = GameObject.Find("Auto");

        YesButton.SetActive(false);
        NoButton.SetActive(false);

        //hide the symbol that represents a selected photo bc none of them are selected yet
        foreach(Image img in thumbnails)
        {
            img.gameObject.transform.Find("Selected").gameObject.SetActive(false);
        }

        //display the pictures from the game manager
        ShowThumbnails(gameManager.GetComponent<Photography>().GetPhotographs());

        textbox.CloseOnTextComplete = false;
        textbox.FeedText(Constants.ShowTed);
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

        DoneButton.SetActive(!DoneButton.activeSelf);
        AutoButton.SetActive(!AutoButton.activeSelf);
    }

    //bring up image on click
    public void Enlarge(Image src)
    {
        //play sfx if applicable
        if (ClickSFX != null) { ClickSFX.Play(); }

        selectedImage = src;
        bigThumbnail.sprite = src.sprite;
        bigThumbnail.gameObject.SetActive(true);
        textbox.FeedText(Constants.ConfirmSelectPhoto);
        textbox.DisplayText();
        ToggleInputButtons();
    }

    //selected photo is added to dictionary, return to view of all photos 
    public void YesButtonClick()
    {
        //play sfx if applicable
        if (ConfirmSFX !=null){ ConfirmSFX.Play(); }

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
            if (!picToAdd.Equals(finalSelection[picToAdd.subjectName]))
            {
                UpdatePhoto(picToAdd);
            }
        }

        //hide buttons and return to photo view
        ToggleInputButtons();
        Delarge();
    }

    //for when an entry in the dictionary of final selected photos needs to be replaced
    void UpdatePhoto(Photograph pic)
    {
        Image deselect = null;
        Photograph toRemove = finalSelection[pic.subjectName];
        //looping through dictionary to find image is a hit to performance, 
        //but preferable to hit to memory from an inverse dictionary of hd photos
        foreach (KeyValuePair<Image, Photograph> pair in allPhotos)
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
        finalSelection[pic.subjectName] = pic;
    }

    //photo dismissed
    public void NoButtonClick()
    {
        //play sfx if applicable
        if (CancelSFX != null) { CancelSFX.Play(); }

        //if photo has been added remove it
        selectedImage.gameObject.transform.Find("Selected").gameObject.SetActive(false);
        Photograph picToRemove = allPhotos[selectedImage];
        if (finalSelection.ContainsKey(picToRemove.subjectName) && finalSelection[picToRemove.subjectName].Equals(picToRemove)){
            finalSelection.Remove(picToRemove.subjectName);
        }

        ToggleInputButtons();
        Delarge();
    }

    public void Delarge()
    {
        bigThumbnail.gameObject.SetActive(false);
        textbox.FeedText(Constants.ShowTed);
        textbox.DisplayText();
    }

    //method to show all saved pics at the end of the level
    public void ShowThumbnails(Photograph[] pics)
    {
        for (int i = 0; i < thumbnails.Length; i++)
        {
            if (i >= pics.Length) { thumbnails[i].gameObject.SetActive(false); continue; }
            if (pics[i].pic == null) { thumbnails[i].gameObject.SetActive(false); continue; }

            //displayIm.sprite = Sprite.Create(pic, new Rect(0.0f, 0.0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
            thumbnails[i].sprite = Sprite.Create(pics[i].pic, new Rect(0f, 0f, pics[i].pic.width, pics[i].pic.height), new Vector2(.5f, .5f));
            allPhotos.Add(thumbnails[i], pics[i]);
        }
    }

    //automatically select the highest scoring photos for grading
    public void AutoSelect()
    {

        //play sfx if applicable
        if (ClickSFX != null) { ClickSFX.Play(); }


        foreach (KeyValuePair<Image, Photograph> pair in allPhotos)
        {
            Image img = pair.Key;
            Photograph photo = pair.Value;

            if (finalSelection.ContainsKey(photo.subjectName))
            {
                Photograph compareTo = finalSelection[photo.subjectName];

                if(photo.finalScore > compareTo.finalScore)
                {
                    UpdatePhoto(photo);
                    img.gameObject.transform.Find("Selected").gameObject.SetActive(true);
                }
            }
            else
            {
                finalSelection.Add(photo.subjectName, photo);
                img.gameObject.transform.Find("Selected").gameObject.SetActive(true);
            }
        }
    }

    //prompt the user to confirm that they want to exit the selection screen
    public void DoneButtonClick()
    {
        //play sfx if applicable
        if (ClickSFX != null) { ClickSFX.Play(); }

        ConfirmScreen.SetActive(true);
        ConfirmScreen.transform.Find("Text").GetComponent<Text>().text = Constants.ProceedPhotos.Replace(Constants.ParameterSTR, finalSelection.Count.ToString());
        AutoButton.SetActive(false);
        DoneButton.SetActive(false);
    }

    //if user confirms they want to proceed to grading
    public void ConfirmDone()
    {
        //play sfx if applicable
        if (ConfirmSFX != null) { ConfirmSFX.Play(); }

        List<Photograph> photos = new List<Photograph>();
        photos.AddRange(finalSelection.Values);
        gameManager.GetComponent<GameManager>().ReturnToLab(photos);
        //currentState = GradeState.tedGrading;
       // Selectioncanvas.enabled = false;
       // GradingCanvas.enabled = true;
    }

    //if user decided to return to selection
    public void DeconfirmDone()
    {
        //play sfx if applicable
        if (CancelSFX != null) { CancelSFX.Play(); }

        ConfirmScreen.SetActive(false);
        AutoButton.SetActive(true);
        DoneButton.SetActive(true);
    }
}
