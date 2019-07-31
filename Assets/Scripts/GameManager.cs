using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //max time allowed to complete the level in seconds
    //time to walk from point a to point b with no stopping at walk spped 7 is 4minutes
    //float timeRemaining = 5 * 60 + 1;

    //pokemon snap allows 60 pictures per course
    //int picsRemaining = 15;

   // public Photograph[] picsTaken = new Photograph[15];

    //[SerializeField] Canvas canvas;
    [SerializeField] Text timeText;
    [SerializeField]
    Image endprompt;

    [SerializeField]
    Image loadingAnim;

    bool LevelOver;

    public List<Photograph> pics4grading = new List<Photograph>();

    //when scene is loaded and level begins start with some panning shots of the level before transitioning into gameplay
    public bool transitionsEnabled;

    bool paused = false;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        Object.DontDestroyOnLoad(this.gameObject);
        endprompt.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update () {

		//toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
                Time.timeScale = 1;
            }
            else if (!paused)
            {
                paused = true;
                Time.timeScale = 0;
            }
           // SceneManager.LoadScene("Lab");
        }

        //decrease timer
        //timeRemaining -= Time.deltaTime;
        //int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        //int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
       // timeText.text = "Time Remaining: " + string.Format("{0:0}:{1:00}", minutes, seconds);
	}

    //ask the user if they would like to end the course
    public void EndPrompt()
    {
        if (LevelOver) { return; }
        if (!endprompt.gameObject.activeSelf)
        {
            Time.timeScale = 0;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            endprompt.gameObject.SetActive(true);

            
        }
    }

    //overload end prompt that allows for a custom message
    public void EndPrompt(string txt)
    {
        if (LevelOver) { return; }
        if (!endprompt.gameObject.activeSelf)
        {
            Time.timeScale = 0;
            endprompt.transform.GetChild(0).GetComponent<Text>().text = txt;
            endprompt.gameObject.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //if user clicks yes and ends course
    public void EndCourse()
    {
        Time.timeScale = 1;

        //load async next scene
        LevelOver = true;
        SceneManager.LoadSceneAsync("Grading", LoadSceneMode.Single);

        //set loading icon to active
        loadingAnim.gameObject.SetActive(true);
    }

    //if user clicks no and returns to the course
    public void CloseEndPrompt()
    {
        Time.timeScale = 1;
        endprompt.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToLab(List<Photograph> finalPhotos)
    {
        Time.timeScale = 1;

        //load async next scene
        pics4grading = finalPhotos;
        SceneManager.LoadSceneAsync("Lab", LoadSceneMode.Single);

        //set loading icon to active
        if (loadingAnim!= null) loadingAnim.gameObject.SetActive(true);
    }
}
