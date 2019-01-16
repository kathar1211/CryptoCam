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

    //for cryptidnomicon
    public GameObject cryptidNomicon;
    GameObject page;
    public Sprite[] pages;
    int currentPage;

    //for options
    public GameObject options;

    enum MenuState { Main, Talking, CryptidNomicon, LevelSelect, Items, Gallery, Option };
    MenuState currentState;

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
                //textbox will reach conclusion and handle input on its own, just check if its donw
                if (!textBox.gameObject.activeInHierarchy)
                {
                    currentState = MenuState.Main;
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
        string[] txt = { "This is just some test dialogue.", "I'll say more interesting things in the final game." };
        textBox.GetComponent<TextBox>().FeedText(txt);
        textBox.GetComponent<TextBox>().DisplayText(); //display text should be calling when finished feeding lines
    }

    public void Embark()
    {
        //todo: level selector
        //for now just load gameplay scene
        SceneManager.LoadScene("TerrainTest");
        
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
        textBox.GetComponent<TextBox>().DisplayText();
    }

    public void Gallery()
    {
        currentState = MenuState.Talking;
        textBox.SetActive(true);
        string txt = "You can't look at the gallery yet.";
        textBox.GetComponent<TextBox>().FeedText(txt);
        textBox.GetComponent<TextBox>().DisplayText();
    }

    public void Options()
    {
        options.SetActive(true);
        currentState = MenuState.Option;
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



    #endregion
}

