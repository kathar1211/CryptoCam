﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    //handles button clicks and selections for title screen

    public GameObject[] buttons;
    public GameObject selector;

    int currentButton;

    //need a short 1 frame buffer for key presses;
    //keep track of whether the animation is done as well as whether it was done 1 frame ago
    bool animationDone;
    bool animationDonePevFrame;

    // Use this for initialization
    void Start () {
        currentButton = 0;
        MoveSelector(buttons[0]);
        animationDone = false;
        animationDonePevFrame = false;
	}
	
	// Update is called once per frame
	void Update () {

        animationDonePevFrame = animationDone;

        //only accept input once buttons have appeared on screen
        if (buttons[buttons.Length - 1].activeInHierarchy)
        {
            animationDone = true;
        }
        if (animationDone && animationDonePevFrame)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentButton += buttons.Length;
                currentButton--;
                currentButton %= buttons.Length;
                MoveSelector(buttons[currentButton]);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentButton++;
                currentButton %= buttons.Length;
                MoveSelector(buttons[currentButton]);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                buttons[currentButton].GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    //todo: if player selects continue, load sava data
    public void Continue()
    {
        Debug.Log("clicked continue");
    }

    //if player selects new game, load next scene
    public void NewGame()
    {
        Debug.Log("clicked newgame");
        SceneManager.LoadScene("Lab");

    }

    //bring up options menu
    public void Options()
    {
        Debug.Log("Clicked options");
    }

    //quit
    public void Exit()
    {
        Debug.Log("clicked exit");
        Application.Quit();
    }

    //indicate to player which button is currently selected
    public void MoveSelector(GameObject button)
    {
        int xmargin = 10;
        int ymargin = 5;
        Debug.Log(button.GetComponent<RectTransform>().rect.position.y);
        //selector.GetComponent<RectTransform>().position = new Vector2((this.transform.position.x-(button.GetComponent<RectTransform>().rect.width)), button.GetComponent<RectTransform>().position.y);
        //selector.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(this.transform.position.x + (button.GetComponent<RectTransform>().rect.width), button.GetComponent<RectTransform>().position.y, 0);
        selector.GetComponent<RectTransform>().anchoredPosition = new Vector2(-button.GetComponent<RectTransform>().rect.width/2 -xmargin, button.GetComponent<RectTransform>().anchoredPosition.y + ymargin);
        selector.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(button.GetComponent<RectTransform>().rect.width + xmargin*2, selector.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y);

        currentButton = System.Array.IndexOf(buttons, button);
    }
}