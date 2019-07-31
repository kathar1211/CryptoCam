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

    //for cryptidnomicon
    public GameObject cryptidNomicon;
    GameObject page;
    public Sprite[] pages;
    int currentPage;

    //for options
    public GameObject options;

    enum MenuState { Main, Talking, CryptidNomicon, LevelSelect, Items, Gallery, Options, Grading };
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

        page = cryptidNomicon.transform.GetChild(0).gameObject;
        cryptidNomicon.SetActive(false);
        currentPage = 0;

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
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    TurnPage(false);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    TurnPage(true);
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
                    if (!textBox.activeInHierarchy && gradingIndex < gradeablePhotos.Count)
                    {
                        //if textbox is not active it means its finished with the current photo 
                        //set it active again and queue up the next photo
                        textBox.SetActive(true);
                        GradePhoto(gradeablePhotos[gradingIndex]);
                        gradingIndex++;
                    }
                }
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

    //turn pages of the cryptidnomicon. true for forward false for back
    public void TurnPage(bool forward)
    {
        if (forward && currentPage < pages.Length)
        {
            currentPage++;
        }
        else if (!forward && currentPage > 0)
        {
            currentPage--;
        }
        page.GetComponent<Image>().sprite = pages[currentPage];
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
        SceneManager.LoadSceneAsync("TerrainTest", LoadSceneMode.Single);
        
        
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

        //subject: oh, it's X! that's worth Y points
        //oh it's, uh... what is this supposed to be?
        dialogue.Add( "Oh, it's " + photo.subjectName + "!");
        sprites.Add(TedMoods.Default);
        dialogue.Add("That's worth " + photo.baseScore + " points.");
        sprites.Add(TedMoods.Pleased);
        textBox.GetComponent<TextBox>().FeedText(dialogue, sprites);

        //facinForward: hmm... it isn't facing the right way. i'm afraid that's -10 points

        //visibility: 

        //distance from center

        //distance from camera

        //multiple cryptids in shot

        //final score
        textBox.GetComponent<TextBox>().DisplayText();
    }

    #endregion
}

