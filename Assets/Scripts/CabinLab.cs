using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class CabinLab : MonoBehaviour {


    //two types of buttons to keep track of: text buttons and image buttons
    public GameObject[] textButtons;
    public GameObject[] imageButtons;
    public GameObject textMarker;
    int currentButton;

    public GameObject textBox; //for dialogue
    public GameObject ted;

    public GameObject cryptidNomicon;

    //for options
    public GameObject options;
    //for credits
    public GameObject credits;

    enum MenuState { Main, Talking, CryptidNomicon, LevelSelect, Items, Gallery, Options, Grading, GradingDone, Credits };
    MenuState currentState;

    List<Photograph> gradeablePhotos = new List<Photograph>();
    int gradingIndex = -1;
    public Animator gradingThumbnailHolder;
    public Image newThumbnail;
    public Image prevThumbnail;
    public Text prevScoreText;

    [SerializeField]
    AudioSource moveCursorSFX;
    [SerializeField]
    AudioSource selectSFX;
    [SerializeField]
    AudioSource doorKnockSFX;
    [SerializeField]
    AudioSource doorOpenSFX;

    [SerializeField]
    GameObject loadingAnim;

    public GameObject blackScreen;

    bool introPlaying = false;

	// Use this for initialization
	void Start () {

        Application.backgroundLoadingPriority = ThreadPriority.Low;

        //make sure cursor settings are right
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        MoveSelector(textButtons[0]);
        currentState = MenuState.Main;
        cryptidNomicon.SetActive(false);


        options.SetActive(false);

        textBox.SetActive(false);
        gradingThumbnailHolder.gameObject.SetActive(false);

        //if the gamemanager object is found and it has a non empty list of photos, jump right into grading mode
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            List<Photograph> p = gameManager.GetComponent<GameManager>().pics4grading;
            //move the images from the gamemanager script to the cabin lab script
            if (p != null && p.Count > 0)
            {
                gradeablePhotos.AddRange(p);
                gameManager.GetComponent<GameManager>().pics4grading.Clear();
                //gradingThumbnail.gameObject.SetActive(true);
                currentState = MenuState.Grading;

                //intro
                string[] dialogue = { Constants.WelcomeBack };
                TedMoods[] sprites = { TedMoods.Default};
                string[] emptyScoreTxt = { "" };
                textBox.GetComponent<TextBox>().ClearTextQueue();
                textBox.GetComponent<TextBox>().FeedText(dialogue, sprites, emptyScoreTxt);
                textBox.GetComponent<TextBox>().DisplayText();
                //currentState = MenuState.Grading;
            }
        }

        //if it's the first time starting the game, set up the intro sequence
        if ((!PlayerPrefs.HasKey(Constants.FirstPlay) || PlayerPrefs.GetInt(Constants.FirstPlay) == 0) && currentState != MenuState.Grading)
        {
            StartCoroutine(StartIntro());
            introPlaying = true;
        }
        
    }

    //intro sequence
    IEnumerator StartIntro()
    {
        if (blackScreen != null)
        {
            blackScreen.SetActive(true);
        }
        if (doorKnockSFX != null)
        {
            doorKnockSFX.Play();
            //StartCoroutine(WaitForSound(doorKnockSFX));
            yield return WaitForSound(doorKnockSFX);
        }
        if (doorOpenSFX != null)
        {
            doorOpenSFX.Play();
            //StartCoroutine(WaitForSound(doorOpenSFX));
            yield return WaitForSound(doorOpenSFX);
        }
        if (blackScreen != null)
        {
            blackScreen.SetActive(false);
        }

        currentState = MenuState.Talking;
        PlayerPrefs.SetInt(Constants.FirstPlay, 1);
        textBox.SetActive(true);
        textBox.GetComponent<TextBox>().ClearTextQueue();
        textBox.GetComponent<TextBox>().FeedText(Constants.TedIntro, Constants.IntroMoods);
        textBox.GetComponent<TextBox>().DisplayText();

        introPlaying = false;
        yield return null;
    }

    // Update is called once per frame
    void Update() {

        switch (currentState) {

            case MenuState.Main:
                //no input allowed during intro
                if (introPlaying) { return; }

                if (CrossPlatformInputManager.GetButtonDown(Constants.Vertical)){
                    float Ymov = CrossPlatformInputManager.GetAxis(Constants.Vertical);
                    //if (Input.GetKeyDown(KeyCode.UpArrow))
                    if (Ymov > 0)
                    {
                        currentButton += textButtons.Length;
                        currentButton--;
                        currentButton %= textButtons.Length;
                        MoveSelector(textButtons[currentButton]);
                    }
                    if (Ymov < 0)
                    // if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        currentButton++;
                        currentButton %= textButtons.Length;
                        MoveSelector(textButtons[currentButton]);
                    }
                }
                //don't count clicks unless user is clicking directly on something
                if (CrossPlatformInputManager.GetButtonDown(Constants.Submit))
                {
                    if (currentButton >= 0 && currentButton < textButtons.Length)
                    {
                        if (selectSFX != null) selectSFX.Play();

                        textButtons[currentButton].GetComponent<Button>().onClick.Invoke();
                        HideAllHovers();
                    }
                }
                break;

            case MenuState.CryptidNomicon:
                if (cryptidNomicon.GetComponent<CryptidNomicon>().ReadyToClose)
                {
                    Return(cryptidNomicon);
                }
                break;

            case MenuState.Talking:
                //textbox will reach conclusion and handle input on its own, just check if its done
                if (!textBox.gameObject.activeInHierarchy)
                {
                    currentState = MenuState.Main;
                }

                break;
            case MenuState.Grading:
                //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    if (!textBox.activeInHierarchy && gradingIndex < gradeablePhotos.Count - 1)
                    {
                        Debug.Log("queued grading for photo " + gradingIndex + ". increasing to " + gradingIndex + 1);
                        gradingIndex++;
                        //if textbox is not active it means its finished with the current photo 
                        //set it active again and queue up the next photo
                        textBox.GetComponent<TextBox>().ClearTextQueue();
                        textBox.SetActive(true);
                        GradePhoto(gradeablePhotos[gradingIndex]);
                    }
                }
                if (!textBox.activeInHierarchy && gradingIndex >= gradeablePhotos.Count)
                {
                    //outro
                    textBox.GetComponent<TextBox>().FeedText(Constants.DoneGrading,TedMoods.Default);
                    textBox.GetComponent<TextBox>().DisplayText();
                    currentState = MenuState.GradingDone;
                    gradingThumbnailHolder.gameObject.SetActive(false);
                    //send over the photos to the cyrptidnomicon
                    cryptidNomicon.GetComponent<CryptidNomicon>().RecievePhotos(gradeablePhotos);
                }
                break;
            case MenuState.GradingDone:
                if (!textBox.activeInHierarchy && gradingIndex >= gradeablePhotos.Count)
                {
                    currentState = MenuState.Main;
                    gradingThumbnailHolder.gameObject.SetActive(false);
                }
                break;
            case MenuState.Credits:
                //exit credits on click
                if (CrossPlatformInputManager.GetButtonDown(Constants.Submit))
                {
                    Return(credits);
                }
                break;
        }
    }

    

    //buttons have a child object drop shadow that should appear when hovering
    public void ToggleImageButtonHover(GameObject button)
    {
        if (currentState == MenuState.Main)
        {
            button.transform.GetChild(0).gameObject.SetActive(!button.transform.GetChild(0).gameObject.activeSelf);
        }
    }

    public void MoveSelector(GameObject button)
    {
        if (currentState == MenuState.Main)
        {
            Vector2 pos = textMarker.GetComponent<RectTransform>().anchoredPosition;
            textMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, button.GetComponent<RectTransform>().anchoredPosition.y);

            //currentButton = textButtons.
            //todo: update the selected button
            currentButton = System.Array.IndexOf(textButtons, button);

            if (moveCursorSFX != null) moveCursorSFX.Play();
        }
    }

    //methods for clicking on various things
    #region buttonMethods

    public void Talk()
    {
        if (currentState == MenuState.Main)
        {
            currentState = MenuState.Talking;
            textBox.SetActive(true);
            ted.GetComponent<TedConvos>().Talk();
        }
    }

    public void Embark()
    {
        if (currentState == MenuState.Main)
        {
            //todo: level selector
            //for now just load gameplay scene
            loadingAnim.SetActive(true);
            Destroy(GameObject.Find("GameManager"));
            SceneManager.LoadSceneAsync("demolvl", LoadSceneMode.Single);
        }
        
    }

    public void CryptidNomicon()
    {
        if (currentState == MenuState.Main)
        {
            cryptidNomicon.GetComponent<CryptidNomicon>().ReadyToClose = false;
            cryptidNomicon.SetActive(true);
            currentState = MenuState.CryptidNomicon;

        }
    }

    public void Items()
    {
        if (currentState == MenuState.Main)
        {
            currentState = MenuState.Talking;
            textBox.SetActive(true);
            string txt = "You can't look at the items yet.";
            textBox.GetComponent<TextBox>().FeedText(txt);
            textBox.GetComponent<TextBox>().FeedTed(TedMoods.SquintHandUp);
            textBox.GetComponent<TextBox>().DisplayText();
        }
    }

    public void Gallery()
    {
        if (currentState == MenuState.Main)
        {
            currentState = MenuState.Talking;
            textBox.SetActive(true);
            string txt = "You can't look at the gallery yet.";
            textBox.GetComponent<TextBox>().FeedText(txt);
            textBox.GetComponent<TextBox>().FeedTed(TedMoods.Uncertain);
            textBox.GetComponent<TextBox>().DisplayText();
        }
    }

    public void Options()
    {
        if (currentState == MenuState.Main)
        {
            options.SetActive(true);
            currentState = MenuState.Options;
        }
    }

    public void Credits()
    {
        if (currentState == MenuState.Main)
        {
            credits.SetActive(true);
            currentState = MenuState.Credits;
        }
    }

    public void Exit()
    {
        if (currentState == MenuState.Main)
        {
            loadingAnim.SetActive(true);
            SceneManager.LoadSceneAsync("Title");
        }
    }

    public void Return(GameObject submenu)
    {
        //return back to main menu
        submenu.SetActive(false);
        currentState = MenuState.Main;
    }


    #endregion

    //iterate through the different attributes of the photo and queue up dialogue and sprites for ted that go with them
    void GradePhoto(Photograph photo)
    {
        gradingThumbnailHolder.gameObject.SetActive(true);
        gradingThumbnailHolder.Play("PopIn");
        newThumbnail.sprite = Sprite.Create(photo.pic, new Rect(0f, 0f, photo.pic.width, photo.pic.height), new Vector2(.5f, .5f));
        List<string> dialogue = new List<string>();
        List<TedMoods> sprites = new List<TedMoods>();
        List<string> scoreUpdates = new List<string>();
        int score = 0;

        //subject: oh, it's X! that's worth Y points
        if (photo.subjectName == "No one" || photo.finalScore <= 0)
        {
            //if theres no cryptid in the photo don't bother with the rest of the grading
            dialogue.Add(Constants.NoSubject);
            sprites.Add(TedMoods.LeanForward);
            //scoreUpdates.Add(Constants.Score);
            dialogue.Add(Constants.NoPoints);
            sprites.Add(TedMoods.SquintHandUp);
            scoreUpdates.Add(Constants.FinalScore + 0);
            textBox.GetComponent<TextBox>().FeedText(dialogue, sprites);
            textBox.GetComponent<TextBox>().DisplayText();
            return;
        }
        else
        {
            //dialogue.Add("Oh, it's " + photo.subjectName + "!");
            dialogue.Add(Constants.FoundSubject.Replace(Constants.ParameterSTR, photo.subjectName));
            sprites.Add(TedMoods.Default);
            //.Add("That's worth " + photo.baseScore + " points.");
            dialogue.Add(Constants.FoundPoints.Replace(Constants.ParameterSTR, photo.baseScore.ToString()));
            sprites.Add(TedMoods.Pleased);
            score += photo.baseScore;
            scoreUpdates.Add(Constants.Score + score);
        }


        //facinForward: hmm... it isn't facing the right way. i'm afraid that's -10 points
        if (photo.facinForward == false)
        {
            //dialogue.Add("Oh, but it's facing away from the camera. I'm afraid that's minus " + (photo.baseScore-10) + " points.");
            dialogue.Add(Constants.FacingAway.Replace(Constants.ParameterSTR, (photo.baseScore - 10).ToString()));
            sprites.Add(TedMoods.Disappointed);
            score = 10;
            scoreUpdates.Add(Constants.Score + score);
        }

        //visibility: a value between 0 and 1 representing percent of visibility. score is multiplied by this
        if (photo.visibility >= 1)
        {
            //dialogue.Add("And you can see it perfectly clearly! 100% visibility, well done.");
            dialogue.Add(Constants.GoodVisibility);
            sprites.Add(TedMoods.Happy);
        }
        else if (photo.visibility >= .5)
        {
            //dialogue.Add("You can see most of it clearly, but overall it's only about " + (int)(photo.visibility * 100) + "% visible.");
            dialogue.Add(Constants.OKVisibility.Replace(Constants.ParameterSTR, ((int)(photo.visibility * 100)).ToString()));
            sprites.Add(TedMoods.LookDownHandUp);
        }
        else
        {
            //dialogue.Add("You can't see it very clearly at all. Only about " + (int)(photo.visibility * 100) + "% of it isn't blocked by obstacles in the shot.");
            dialogue.Add(Constants.PoorVisibility.Replace(Constants.ParameterSTR, ((int)(photo.visibility * 100)).ToString()));
            sprites.Add(TedMoods.Uncertain);
        }
        score = (int)(score*photo.visibility);
        scoreUpdates.Add(Constants.Score + score);

        //distance from center
        //distancefromcenter should be a float value between 0 and ~.7
        //score += (int)(200 * (.7f - pic.distanceFromCenter));
        int centerScore = (int)(200 * (.7f - photo.distanceFromCenter));
        string centerPoints = centerScore.ToString();
        if (photo.distanceFromCenter >= .3f)
        {
            //dialogue.Add("The cryptid is nowhere near the center of the shot. Try to frame it better next time. " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points.");
            dialogue.Add(Constants.PoorCenter.Replace(Constants.ParameterSTR, centerPoints));
            sprites.Add(TedMoods.LeanForward);
        } 
        else if (photo.distanceFromCenter >= .1f)
        {
            //dialogue.Add("It's a little off center, but not too bad. That's plus " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points.");
            dialogue.Add(Constants.OKCenter.Replace(Constants.ParameterSTR, centerPoints));
            sprites.Add(TedMoods.Default);
        }
        else
        {
            //dialogue.Add("Very nice! It's perfectly centered in the shot. That's plus " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points!");
            dialogue.Add(Constants.GoodCenter.Replace(Constants.ParameterSTR, centerPoints));
            sprites.Add(TedMoods.Pleased);
        }
        score += centerScore;
        scoreUpdates.Add(Constants.Score + score);

        //distance from camera
        //distance from camera is in world units, with 0 on top of camera
        //not sure what ideal distance is, for now we'll just say the closer the better
        //finalScore += (int)Mathf.Ceil(5000 * (1 / pic.distanceFromCamera));
        //this number might need to be adjusted but we'll say less that 100 units is close, less than 200 is middle distance, more is far away
        int distanceScore = (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera));
        string distancePoints = distanceScore.ToString();
        if (photo.distanceFromCamera < 100)
        {
            //dialogue.Add("Oho! This is a pretty close up shot! Well done, that's plus " +  + " points.");
            dialogue.Add(Constants.GoodDistance.Replace(Constants.ParameterSTR, distancePoints));
            sprites.Add(TedMoods.Happy);
        }
        else if (photo.distanceFromCamera < 200)
        {
            //dialogue.Add("It's a pretty good distance away. That's plus " + (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera)) + " points.");
            dialogue.Add(Constants.OKDistance.Replace(Constants.ParameterSTR, distancePoints));
            sprites.Add(TedMoods.Satisfied);
        }
        else
        {
            //dialogue.Add("Hmm... it's pretty far away. I know it's asking a lot, but see if you can get closer next time. " + (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera)) + " points.");
            dialogue.Add(Constants.PoorDistance.Replace(Constants.ParameterSTR, distancePoints));
            sprites.Add(TedMoods.SquintHandUp);
        }
        score += distanceScore;
        scoreUpdates.Add(Constants.Score + score);

        //give bonus points for cool pose if applicable
        if (photo.coolPose)
        {
            int coolPoseBonus = 200;
            dialogue.Add(Constants.SpecialPose.Replace(Constants.ParameterSTR, coolPoseBonus.ToString()));
            sprites.Add(TedMoods.LookUpHandUp);
            score += coolPoseBonus;
            scoreUpdates.Add(Constants.Score + score);
        }

        //multiple cryptids in shot
        if (photo.subjectCount > 1)
        {
            //dialogue.Add("Oh! And there's more than one cryptid in the shot! Excellent work, that's worth double.");
            dialogue.Add(Constants.DoubleCryptid);
            sprites.Add(TedMoods.Surprised);
            score *= 2;
            scoreUpdates.Add(Constants.Score + score);
        }

        //final score
        //dialogue.Add("Let's see, overall I give this photo... " + photo.finalScore + " points.");
        dialogue.Add(Constants.EndGrading.Replace(Constants.ParameterSTR, photo.finalScore.ToString()));
        sprites.Add(TedMoods.Satisfied);
        scoreUpdates.Add(Constants.FinalScore + score);

        //send off the text and sprites
        TextBox actualTextBox = textBox.GetComponent<TextBox>();
        actualTextBox.ClearTextQueue();
        actualTextBox.FeedText(dialogue, sprites, scoreUpdates);
        

        //todo: check here for an existing photo in the cryptidnomicon, prompt user if they want to overwrite it
        CryptidNomicon cryptidData = cryptidNomicon.GetComponent<CryptidNomicon>();
        if (cryptidData != null && cryptidData.HasEntry(photo.subjectName)){

            PageContent content = cryptidData.GetEntry(photo.subjectName);
            Texture2D pic = content.image;
            prevThumbnail.sprite = Sprite.Create(pic, new Rect(0f, 0f, photo.pic.width, photo.pic.height), new Vector2(.5f, .5f));
            Debug.Log("grabbing existing photo for " + content.name + " at index " + gradingIndex);

            actualTextBox.FeedText(Constants.ExistingEntry.Replace(Constants.ParameterSTR, content.photoScore.ToString()), ShowPreviousPhoto);
            actualTextBox.FeedTed(TedMoods.Surprised);

            actualTextBox.FeedText(Constants.OverwritePrompt, PromptPhotoChoice);
            actualTextBox.FeedTed(TedMoods.LookDownHandUp);

            actualTextBox.SetLeftButton(Constants.KeepPreviousText, ChoseOldPhoto);
            actualTextBox.SetRightButton(Constants.UseNewText, ChoseNewPhoto);
        }

        actualTextBox.DisplayText();
    }

    

    //https://gamedev.stackexchange.com/questions/134002/how-can-i-do-something-after-an-audio-has-finished
    //wait for audiosource to finish
    public IEnumerator WaitForSound(AudioSource audiosource)
    {
        yield return new WaitUntil(() => audiosource.isPlaying == false);
    }

    //when something in the background is selected all hover drop shadows should hide
    void HideAllHovers()
    {
        GameObject[] shadows = GameObject.FindGameObjectsWithTag(Constants.DropShadowTag);
        foreach (GameObject shadow in shadows)
        {
            shadow.SetActive(false);
        }
    }

    #region Branching Photo Choice Helpers
    //animate in the thumbnail of the previous photo if the user has a choice to make
    public void ShowPreviousPhoto()
    {
        gradingThumbnailHolder.Play("SlideOver");

        Debug.Log("grabbing existing photo at index " + gradingIndex);
        CryptidNomicon cryptidData = cryptidNomicon.GetComponent<CryptidNomicon>();
        PageContent content = cryptidData.GetEntry(gradeablePhotos[gradingIndex].subjectName);
        prevScoreText.text = Constants.Score + content.photoScore;
    }

    //show buttons that give players the choice to keep an old photo vs a new one
    public void PromptPhotoChoice()
    {
        textBox.GetComponent<TextBox>().ButtonsIn();
    }

    //if the player chooses to use the new photo
    public void ChoseNewPhoto()
    {
        gradingThumbnailHolder.Play("NewSelected");
        textBox.GetComponent<TextBox>().ButtonsOut();
        textBox.GetComponent<TextBox>().FeedText(Constants.NewPhotoSelected, TedMoods.Satisfied);

        //new photo is already in the list of photos that will be processed, so no need to do anything there
    }

    //if the player chooses to keep their old photo
    public void ChoseOldPhoto()
    {
        gradingThumbnailHolder.Play("PreviousSelected");
        textBox.GetComponent<TextBox>().ButtonsOut();
        textBox.GetComponent<TextBox>().FeedText(Constants.OldPhotoSelected, TedMoods.Satisfied);

        //just replace the new photo in our list of photos submitted with one created from the existing cryptidnomicon page
        CryptidNomicon cryptidData = cryptidNomicon.GetComponent<CryptidNomicon>();
        PageContent content = cryptidData.GetEntry(gradeablePhotos[gradingIndex].subjectName);
        gradeablePhotos[gradingIndex] = cryptidData.PageToPhoto(content);
    }
    #endregion
}

