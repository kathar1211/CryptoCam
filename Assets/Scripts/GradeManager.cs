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

    GameObject gameManager;

    enum GradeState { allThumbs, bigThumb};
    GradeState currentState;

	// Use this for initialization
	void Start () {
        //find the game manager, which has all the data we need from the previous level stored
        gameManager = GameObject.Find("GameManager");

        //display the pictures from the game manager
        gameManager.GetComponent<Photography>().ShowThumbnails(thumbnails);

        textbox.FeedText("Select pictures to show to Ted");
        textbox.DisplayText();

        bigThumbnail.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Enlarge(Image src)
    {
        bigThumbnail.sprite = src.sprite;
        bigThumbnail.gameObject.SetActive(true);
        textbox.FeedText("Select this picture?");
        textbox.DisplayText();
    }

    public void Delarge()
    {
        bigThumbnail.gameObject.SetActive(false);
        textbox.FeedText("Select pictures to show to Ted");
        textbox.DisplayText();
    }
}
