using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    enum MenuState { Main, Talking, CryptidNomicon, LevelSelect, Items, Gallery, Options, Grading, GradingDone };
    MenuState currentState;

    List<Photograph> gradeablePhotos = new List<Photograph>();
    int gradingIndex = 0;
    public Image gradingThumbnail;

	// Use this for initialization
	void Start () {

        //make sure cursor settings are right
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        MoveSelector(textButtons[0]);
        currentState = MenuState.Main;
        cryptidNomicon.SetActive(false);


        options.SetActive(false);

        textBox.SetActive(false);

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
                gradingThumbnail.gameObject.SetActive(true);
                currentState = MenuState.Grading;

                //intro
                string[] dialogue = { "Oh, welcome back! Shall we take a look at those photos? I'm excited to see what you've got." };
                TedMoods[] sprites = { TedMoods.Default};
                string[] emptyScoreTxt = { "" };
                textBox.GetComponent<TextBox>().ClearTextQueue();
                textBox.GetComponent<TextBox>().FeedText(dialogue, sprites, emptyScoreTxt);
                textBox.GetComponent<TextBox>().DisplayText();
            }
        }
        
    }

    // Update is called once per frame
    void Update() {

        switch (currentState) {

            case MenuState.Main:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentButton += textButtons.Length;
                    currentButton--;
                    currentButton %= textButtons.Length;
                    MoveSelector(textButtons[currentButton]);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentButton++;
                    currentButton %= textButtons.Length;
                    MoveSelector(textButtons[currentButton]);
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    textButtons[currentButton].GetComponent<Button>().onClick.Invoke();
                }
                break;

            case MenuState.CryptidNomicon:

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
                    if (!textBox.activeInHierarchy && gradingIndex < gradeablePhotos.Count)
                    {
                        //if textbox is not active it means its finished with the current photo 
                        //set it active again and queue up the next photo
                        textBox.GetComponent<TextBox>().ClearTextQueue();
                        textBox.SetActive(true);
                        GradePhoto(gradeablePhotos[gradingIndex]);
                        gradingIndex++;
                    }
                }
                if (!textBox.activeInHierarchy && gradingIndex >= gradeablePhotos.Count)
                {
                    //outro
                    textBox.GetComponent<TextBox>().FeedText(new List<string>{"Well done, thank you as always for sharing your photos with me." }, new List<TedMoods>{TedMoods.Default });
                    textBox.GetComponent<TextBox>().DisplayText();
                    currentState = MenuState.GradingDone;
                    gradingThumbnail.gameObject.SetActive(false);
                    //send over the photos to the cyrptidnomicon
                    cryptidNomicon.GetComponent<CryptidNomicon>().RecievePhotos(gradeablePhotos);
                }
                break;
            case MenuState.GradingDone:
                if (!textBox.activeInHierarchy && gradingIndex >= gradeablePhotos.Count)
                {
                    currentState = MenuState.Main;
                    gradingThumbnail.gameObject.SetActive(false);
                }
                break;
        }
    }

    

    //buttons have achild object drop shadow that should appear when hovering
    public void ToggleImageButtonHover(GameObject button)
    {
        
        button.transform.GetChild(0).gameObject.SetActive(!button.transform.GetChild(0).gameObject.activeSelf);
    }

    public void MoveSelector(GameObject button)
    {
        Vector2 pos = textMarker.GetComponent<RectTransform>().anchoredPosition;
        textMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, button.GetComponent<RectTransform>().anchoredPosition.y);

        //currentButton = textButtons.
        //todo: update the selected button
        currentButton = System.Array.IndexOf(textButtons, button);
    }

    //methods for clicking on various things
    #region buttonMethods

    public void Talk()
    {
        currentState = MenuState.Talking;
        textBox.SetActive(true);
        ted.GetComponent<TedConvos>().Talk();
    }

    public void Embark()
    {
        //todo: level selector
        //for now just load gameplay scene
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadSceneAsync("demolvl", LoadSceneMode.Single);
        
        
    }

    public void CryptidNomicon()
    {
        cryptidNomicon.SetActive(true);
        currentState = MenuState.CryptidNomicon;
    }

    public void Items()
    {
        currentState = MenuState.Talking;
        textBox.SetActive(true);
        string txt = "You can't look at the items yet.";
        textBox.GetComponent<TextBox>().FeedText(txt);
        textBox.GetComponent<TextBox>().FeedTed(TedMoods.SquintHandUp);
        textBox.GetComponent<TextBox>().DisplayText();
    }

    public void Gallery()
    {
        currentState = MenuState.Talking;
        textBox.SetActive(true);
        string txt = "You can't look at the gallery yet.";
        textBox.GetComponent<TextBox>().FeedText(txt);
        textBox.GetComponent<TextBox>().FeedTed(TedMoods.Uncertain);
        textBox.GetComponent<TextBox>().DisplayText();
    }

    public void Options()
    {
        options.SetActive(true);
        currentState = MenuState.Options;
    }

    public void Exit()
    {
        SceneManager.LoadScene("Title");
    }

    public void Return(GameObject submenu)
    {
        //return back to main menu
        submenu.SetActive(false);
        currentState = MenuState.Main;
    }

    //iterate through the different attributes of the photo and queue up dialogue and sprites for ted that go with them
    void GradePhoto(Photograph photo)
    {
        gradingThumbnail.sprite = Sprite.Create(photo.pic, new Rect(0f, 0f, photo.pic.width, photo.pic.height), new Vector2(.5f, .5f));
        List<string> dialogue = new List<string>();
        List<TedMoods> sprites = new List<TedMoods>();
        List<string> scoreUpdates = new List<string>();
        int score = 0;

        //subject: oh, it's X! that's worth Y points
        if (photo.subjectName == "No one" || photo.finalScore <= 0)
        {
            //if theres no cryptid in the photo don't bother with the rest of the grading
            dialogue.Add("Oh, it's, uh... what is this supposed to be?");
            sprites.Add(TedMoods.LeanForward);
            //scoreUpdates.Add("Score: ");
            dialogue.Add("...Sorry, that's worth 0 points.");
            sprites.Add(TedMoods.SquintHandUp);
            scoreUpdates.Add("Final Score: 0");
            textBox.GetComponent<TextBox>().FeedText(dialogue, sprites);
            textBox.GetComponent<TextBox>().DisplayText();
            return;
        }
        else
        {
            dialogue.Add("Oh, it's " + photo.subjectName + "!");
            sprites.Add(TedMoods.Default);
            //scoreUpdates.Add("Score: ");
            //scoreUpdates.Add("Score: " + score);
            dialogue.Add("That's worth " + photo.baseScore + " points.");
            sprites.Add(TedMoods.Pleased);
            score += photo.baseScore;
            scoreUpdates.Add("Score: " + score);
        }


        //facinForward: hmm... it isn't facing the right way. i'm afraid that's -10 points
        if (photo.facinForward == false)
        {
            dialogue.Add("Oh, but it's facing away from the camera. I'm afraid that's minus " + (photo.baseScore-10) + " points.");
            sprites.Add(TedMoods.Disappointed);
            score = 10;
            scoreUpdates.Add("Score: " + score);
        }

        //visibility: a value between 0 and 1 representing percent of visibility. score is multiplied by this
        if (photo.visibility >= 1)
        {
            dialogue.Add("And you can see it perfectly clearly! 100% visibility, well done.");
            sprites.Add(TedMoods.Happy);
        }
        else if (photo.visibility >= .5)
        {
            dialogue.Add("You can see most of it clearly, but overall it's only about " + (int)(photo.visibility * 100) + "% visible.");
            sprites.Add(TedMoods.LookDownHandUp);
        }
        else
        {
            dialogue.Add("You can't see it very clearly at all. Only about " + (int)(photo.visibility * 100) + "% of it isn't blocked by obstacles in the shot.");
            sprites.Add(TedMoods.Uncertain);
        }
        score *= (int)photo.visibility;
        scoreUpdates.Add("Score: " + score);

        //distance from center
        //distancefromcenter should be a float value between 0 and ~.7
        //finalScore += (int)(200 * (.7f - pic.distanceFromCenter));
        if (photo.distanceFromCenter >= .3f)
        {
            dialogue.Add("The cryptid is nowhere near the center of the shot. Try to frame it better next time. " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points.");
            //dialogue.Add("The distance from center is " + photo.distanceFromCenter);
            sprites.Add(TedMoods.LeanForward);
        } 
        else if (photo.distanceFromCenter >= .1f)
        {
            dialogue.Add("It's a little off center, but not too bad. That's plus " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points.");
            //dialogue.Add("The distance from center is " + photo.distanceFromCenter);
            sprites.Add(TedMoods.Default);
        }
        else
        {
            dialogue.Add("Very nice! It's perfectly centered in the shot. That's plus " + (int)(200 * (.7f - photo.distanceFromCenter)) + " points!");
            //dialogue.Add("The distance from center is " + photo.distanceFromCenter);
            sprites.Add(TedMoods.Pleased);
        }
        score += (int)(200 * (.7f - photo.distanceFromCenter));
        scoreUpdates.Add("Score: " + score);

        //distance from camera
        //distance from camera is in world units, with 0 on top of camera
        //not sure what ideal distance is, for now well just say the closer the better
        //finalScore += (int)Mathf.Ceil(5000 * (1 / pic.distanceFromCamera));
        //this number might need to be adjusted but we'll say less that 100 units is close, less than 200 is middle distance, more is far away
        if (photo.distanceFromCamera < 100)
        {
            dialogue.Add("Oho! This is a pretty close up shot! Well done, that's plus " + (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera)) + " points.");
            sprites.Add(TedMoods.Happy);
        }
        else if (photo.distanceFromCamera < 200)
        {
            dialogue.Add("It's a pretty good distance away. That's plus " + (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera)) + " points.");
            sprites.Add(TedMoods.Satisfied);
        }
        else
        {
            dialogue.Add("Hmm... it's pretty far away. I know it's asking a lot, but see if you can get closer next time. " + (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera)) + " points.");
            sprites.Add(TedMoods.SquintHandUp);
        }
        score += (int)Mathf.Ceil(5000 * (1 / photo.distanceFromCamera));
        scoreUpdates.Add("Score: " + score);

        //multiple cryptids in shot
        if (photo.subjectCount > 1)
        {
            dialogue.Add("Oh! And there's more than one cryptid in the shot! Excellent work, that's worth double.");
            sprites.Add(TedMoods.Surprised);
            score *= 2;
            scoreUpdates.Add("Score: " + score);
        }

        //final score
        dialogue.Add("Let's see, overall I give this photo... " + photo.finalScore + " points.");
        sprites.Add(TedMoods.Satisfied);
        scoreUpdates.Add("Final Score: " + score);
        textBox.GetComponent<TextBox>().ClearTextQueue();
        textBox.GetComponent<TextBox>().FeedText(dialogue, sprites, scoreUpdates);
        textBox.GetComponent<TextBox>().DisplayText();
    }

    #endregion
}

