using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class GradeManager : MonoBehaviour {

    [SerializeField]
    SelectableImage[] thumbnails;
    [SerializeField]
    TextBox textbox; //this is the textbox that prompts to select photos
    [SerializeField]
    Image bigThumbnail;
    SelectableImage selectedImage;

    [SerializeField]
    int columns; //need to know what the grid layout is to navigate thumbnails via controller
    UIControlWithHighlight highlightedUIcontrol;
    int activeThumbnailCount; //how many of these thumbnail objects are actually showing images

    public UIControlWithHighlight YesButton;
    public UIControlWithHighlight NoButton;
    public UIControlWithHighlight AutoButton;
    public UIControlWithHighlight DoneButton;

    GameObject gameManager;

    [SerializeField]
    GameObject ConfirmScreen;
    [SerializeField]
    TextMeshProUGUI ConfirmText;

    enum GradeState { allThumbs, bigThumb, doneConfirm};
    GradeState currentState;

    //data structure for cryptid thumbnails with an image and a star highlight
    struct CryptidIcon
    {
        public GameObject icon;
        public GameObject highlight;
    }
    //initialize cryptid icons in editor
    public GameObject[] icons;
    //keep track of our icons by dictionary
    Dictionary<string, CryptidIcon> cryptidIcons = new Dictionary<string, CryptidIcon>();

    //photos selected for grading, sorted by subject name
    //only one of each cryptid type is allowed
    public Dictionary<string, Photograph> finalSelection = new Dictionary<string, Photograph>();

    //store photograph information that corresponds with each thumbnail
    Dictionary<SelectableImage, Photograph> allPhotos = new Dictionary<SelectableImage, Photograph>();

    //sound effects
    [SerializeField]
    AudioSource ClickSFX;
    [SerializeField]
    AudioSource ConfirmSFX;
    [SerializeField]
    AudioSource CancelSFX;

    //cryptid icons for selected photos
    [SerializeField]
    Sprite jackalopeHead;
    [SerializeField]
    Sprite nessieHead;
    [SerializeField]
    Sprite fresnoHead;
    [SerializeField]
    Sprite frogmanHead;
    [SerializeField]
    Sprite tsuchinokoHead;
    [SerializeField]
    Sprite flatwoodsHead;
    [SerializeField]
    Sprite bigfootHead;
    [SerializeField]
    Sprite mothmanHead;

    // Use this for initialization
    void Start () {

        //find the game manager, which has all the data we need from the previous level stored
        gameManager = GameObject.Find("GameManager");

        /*YesButton = GameObject.Find("YesButton");
        NoButton = GameObject.Find("NoButton");
        DoneButton = GameObject.Find("Done");
        AutoButton = GameObject.Find("Auto");*/

        YesButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);

        

        //display the pictures from the game manager
        ShowThumbnails(Photography.Instance.GetPhotographs());

        textbox.CloseOnTextComplete = false;
        textbox.FeedText(Constants.ShowTed);
        textbox.DisplayText();

        bigThumbnail.gameObject.SetActive(false);

        CreateIconDictionary();

        currentState = GradeState.allThumbs;
	}

    // Update is called once per frame
    void Update()
    {

        if (currentState != GradeState.doneConfirm) //this state is handled with a controller navigable confirmation window
        {
            HandleControllerNavigation();

            //allow user to actually select things with controller
            if (CrossPlatformInputManager.GetButtonDown(Constants.Submit))
            {
                if (highlightedUIcontrol != null)
                {
                    Button buttonSelect = highlightedUIcontrol.GetComponent<Button>();
                    if (buttonSelect != null)
                    {
                        buttonSelect.onClick.Invoke();
                    }
                }
            }
        }
       
    }

    //yes/no buttons show up when user needs to enter input, disappear after
    void ToggleInputButtons(bool showYesNo)
    {
        YesButton.gameObject.SetActive(showYesNo);
        NoButton.gameObject.SetActive(showYesNo);

        DoneButton.gameObject.SetActive(!showYesNo);
        AutoButton.gameObject.SetActive(!showYesNo);
    }

    //bring up image on click
    public void Enlarge(SelectableImage src)
    {
        if (!bigThumbnail.IsActive() && currentState == GradeState.allThumbs)
        {
            //play sfx if applicable
            if (ClickSFX != null) { ClickSFX.Play(); }

            selectedImage = src;
            bigThumbnail.sprite = src.sprite;
            bigThumbnail.gameObject.SetActive(true);
            textbox.FeedText(Constants.ConfirmSelectPhoto);
            textbox.DisplayText();
            ToggleInputButtons(true);
            currentState = GradeState.bigThumb;
        }
    }

    //selected photo is added to dictionary, return to view of all photos 
    public void YesButtonClick()
    {
        if (currentState == GradeState.bigThumb)
        {
            //play sfx if applicable
            if (ConfirmSFX != null) { ConfirmSFX.Play(); }

            //indicate selection
            selectedImage.SelectedIndicator.SetActive(true);

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

            //reflect selection in icons
            if (cryptidIcons.ContainsKey(picToAdd.subjectName)) { cryptidIcons[picToAdd.subjectName].highlight.SetActive(true); }

            //hide buttons and return to photo view
            Delarge();
        }
    }

    //for when an entry in the dictionary of final selected photos needs to be replaced
    void UpdatePhoto(Photograph pic)
    {
        SelectableImage deselect = null;
        Photograph toRemove = finalSelection[pic.subjectName];
        //looping through dictionary to find image is a hit to performance, 
        //but preferable to hit to memory from an inverse dictionary of hd photos
        foreach (KeyValuePair<SelectableImage, Photograph> pair in allPhotos)
        {
            if (pair.Value.Equals(toRemove))
            {
                deselect = pair.Key;
                break;
            }
        }

        if (deselect != null)
        {
            deselect.SelectedIndicator.SetActive(false);
        }
        finalSelection[pic.subjectName] = pic;
    }

    //photo dismissed
    public void NoButtonClick()
    {
        //play sfx if applicable
        if (CancelSFX != null) { CancelSFX.Play(); }

        //if photo has been added remove it
        selectedImage.SelectedIndicator.gameObject.SetActive(false);
        Photograph picToRemove = allPhotos[selectedImage];
        if (finalSelection.ContainsKey(picToRemove.subjectName) && finalSelection[picToRemove.subjectName].Equals(picToRemove)){
            finalSelection.Remove(picToRemove.subjectName);
            if (cryptidIcons.ContainsKey(picToRemove.subjectName)) { cryptidIcons[picToRemove.subjectName].highlight.SetActive(false); }
        }
        
        Delarge();
    }

    public void Delarge()
    {
        bigThumbnail.gameObject.SetActive(false);
        textbox.FeedText(Constants.ShowTed);
        textbox.DisplayText();
        currentState = GradeState.allThumbs;
        ToggleInputButtons(false);
    }

    //method to show all saved pics at the end of the level
    public void ShowThumbnails(Photograph[] pics)
    {
        for (int i = 0; i < thumbnails.Length; i++)
        {
            if (i >= pics.Length) { thumbnails[i].gameObject.SetActive(false); continue; }
            if (pics[i].pic == null) { thumbnails[i].gameObject.SetActive(false); continue; }
            activeThumbnailCount++;

            //displayIm.sprite = Sprite.Create(pic, new Rect(0.0f, 0.0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
            thumbnails[i].sprite = Sprite.Create(pics[i].pic, new Rect(0f, 0f, pics[i].pic.width, pics[i].pic.height), new Vector2(.5f, .5f));
            allPhotos.Add(thumbnails[i], pics[i]);

            //set icon to appropriate cryptid
            GameObject selector = thumbnails[i].SelectedIndicator;
            Image icon = selector.transform.Find("icon").GetComponent<Image>();
            switch (pics[i].subjectName)
            {
                case Constants.Jackalope:
                    icon.sprite = jackalopeHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Fresno:
                    icon.sprite = fresnoHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Frogman:
                    icon.sprite = frogmanHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Nessie:
                    icon.sprite = nessieHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Tsuchinoko:
                    icon.sprite = tsuchinokoHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Flatwoods:
                    icon.sprite = flatwoodsHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Bigfoot:
                    icon.sprite = bigfootHead;
                    icon.SetNativeSize();
                    break;
                case Constants.Mothman:
                    icon.sprite = mothmanHead;
                    icon.SetNativeSize();
                    break;
                default:
                    icon.sprite = null;
                    icon.gameObject.SetActive(false);
                    break;

            }

            //hide the symbol that represents a selected photo bc none of them are selected yet
            selector.SetActive(false);
        }
    }

    //automatically select the highest scoring photos for grading
    public void AutoSelect()
    {

        //play sfx if applicable
        if (ClickSFX != null) { ClickSFX.Play(); }


        foreach (KeyValuePair<SelectableImage, Photograph> pair in allPhotos)
        {
            SelectableImage img = pair.Key;
            Photograph photo = pair.Value;

            if (finalSelection.ContainsKey(photo.subjectName))
            {
                Photograph compareTo = finalSelection[photo.subjectName];

                if(photo.finalScore > compareTo.finalScore)
                {
                    UpdatePhoto(photo);
                    img.SelectedIndicator.SetActive(true);
                }
            }
            else if (photo.finalScore > 0)
            {
                finalSelection.Add(photo.subjectName, photo);
                img.SelectedIndicator.gameObject.SetActive(true);
            }

            //reflect selection in icons
            if (cryptidIcons.ContainsKey(photo.subjectName)) { cryptidIcons[photo.subjectName].highlight.SetActive(true); }
        }
    }

    //prompt the user to confirm that they want to exit the selection screen
    public void DoneButtonClick()
    {
        if (currentState == GradeState.allThumbs)
        {
            //play sfx if applicable
            if (ClickSFX != null) { ClickSFX.Play(); }

            ConfirmScreen.SetActive(true);
            ConfirmText.text = Constants.ProceedPhotos.Replace(Constants.ParameterSTR, finalSelection.Count.ToString());
            AutoButton.gameObject.SetActive(false);
            DoneButton.gameObject.SetActive(false);
            currentState = GradeState.doneConfirm;
        }
        
    }

    //if user confirms they want to proceed to grading
    public void ConfirmDone()
    {
        //play sfx if applicable
        if (ConfirmSFX != null) { ConfirmSFX.Play(); }

        List<Photograph> photos = new List<Photograph>();
        photos.AddRange(finalSelection.Values);
        GameManager.Instance.ReturnToLab(photos);
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
        AutoButton.gameObject.SetActive(true);
        DoneButton.gameObject.SetActive(true);
        currentState = GradeState.allThumbs;
    }

    //convert game objects assigned in editor to cryptid icon data structures and set them up for use
    void CreateIconDictionary()
    {
        foreach (GameObject go in icons)
        {
            CryptidIcon cryptoIcon = new CryptidIcon();
            cryptoIcon.icon = go.transform.GetChild(1).gameObject;
            cryptoIcon.highlight = go.transform.GetChild(0).gameObject;

            //unhide the icon if a picture of the cryptid exists in the photos
            foreach(Photograph photo in Photography.Instance.GetPhotographs())
            {
                if (photo.subjectName == go.name)
                {
                    cryptoIcon.icon.GetComponent<Image>().color = Color.white;
                    break;
                }
            }

            cryptidIcons.Add(go.name, cryptoIcon);
        }
    }

    public void HandleControllerNavigation()
    {
        HandleHorizontalControllerNavigation();
        HandleVerticalControllerNavigation();
    }

    private void HandleHorizontalControllerNavigation()
    {
        //handle controller input to select ui elements

        if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Horizontal))
        {
            float horizontalDir = CrossPlatformInputManager.GetAxis(Constants.Horizontal);
            switch (currentState)
            {
                case GradeState.allThumbs:
                    //navigate among thumbnails
                    if (highlightedUIcontrol == AutoButton || highlightedUIcontrol == DoneButton) { break; }

                    int thumbnailIndex = Array.IndexOf(thumbnails, highlightedUIcontrol);
                    if (thumbnailIndex == -1) { MoveHighlight(thumbnails[0]); break; }

                    //positive value is right, negative value is left
                    if (horizontalDir < 0)
                    {
                        if (thumbnailIndex != 0) { MoveHighlight(thumbnails[thumbnailIndex - 1]); }
                    }
                    else
                    {
                        if (thumbnailIndex != thumbnails.Length - 1 && thumbnails[thumbnailIndex +1].gameObject.activeInHierarchy) { MoveHighlight(thumbnails[thumbnailIndex + 1]); }
                    }

                    break;

                case GradeState.bigThumb:
                    //navigate between the yes/no buttons
                    if (highlightedUIcontrol != YesButton) { MoveHighlight(YesButton); }
                    else { MoveHighlight(NoButton); }
                    break;
            }
        }
    }

    private void HandleVerticalControllerNavigation()
    {
        
        if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Vertical))
        {
            float verticalDir = CrossPlatformInputManager.GetAxis(Constants.Vertical);
            switch (currentState)
            {
                case GradeState.allThumbs:

                    //positive value is up (?), negative value is down (?)
                    if (verticalDir < 0)
                    {
                        if (highlightedUIcontrol == AutoButton) { MoveHighlight(DoneButton); break; }
                        if (highlightedUIcontrol == DoneButton) { break; }
                    }
                    else
                    {
                        if (highlightedUIcontrol == AutoButton) { MoveHighlight(thumbnails[activeThumbnailCount-1]); break; }
                        if (highlightedUIcontrol == DoneButton) { MoveHighlight(AutoButton); break; }
                    }


                    //navigate among thumbnails
                    if (highlightedUIcontrol == AutoButton || highlightedUIcontrol == DoneButton) { break; }

                    int thumbnailIndex = Array.IndexOf(thumbnails, highlightedUIcontrol);
                    if (thumbnailIndex == -1) { MoveHighlight(thumbnails[0]); break; }


                    //positive value is up, negative value is down
                    if (verticalDir < 0)
                    {
                        if (thumbnailIndex < thumbnails.Length - columns && thumbnails[thumbnailIndex + columns].gameObject.activeInHierarchy) { MoveHighlight(thumbnails[thumbnailIndex + columns]); }
                        else { MoveHighlight(AutoButton); }
                    }
                    else
                    {
                        if (thumbnailIndex >= columns) { MoveHighlight(thumbnails[thumbnailIndex - columns]); }
                    }

                    break;
            }
        }
    }

    public void MoveHighlight(UIControlWithHighlight button)
    {
        UIControlWithHighlight prevSelectedButton = highlightedUIcontrol;
        highlightedUIcontrol = button;

        //set hover to match dimensions of selected button
        if (prevSelectedButton != null) { prevSelectedButton.HideHighlight(); }
        if (highlightedUIcontrol != null) { highlightedUIcontrol.ShowHighlight(); }
    }
}
