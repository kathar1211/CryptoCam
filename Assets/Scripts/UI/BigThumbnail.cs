using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

//ui for when a photo has been selected on the grading scene and player needs to decide what to do with it
public class BigThumbnail : MonoBehaviour
{
    public GradeManager GradeManager;
    public Image DisplayedImage;

    [SerializeField]
    UIControlWithHighlight YesButton;
    [SerializeField]
    UIControlWithHighlight NoButton;

    [SerializeField]
    UIControlWithHighlight GalleryButton;
    [SerializeField]
    UIControlWithHighlight ChallengeButton;

    [SerializeField]
    TextMeshProUGUI GalleryLabel;
    [SerializeField]
    TextMeshProUGUI ChallengeLabel;

    private int photoIndex;
    private bool picSelectedForGallery;
    private bool picQualifiesForChallenge;
    private bool picSelectedForChallenge;

    private UIControlWithHighlight currentSelectedButton;

    public void Init(Image image, bool selectedForGallery, bool qualifiesForChallenge, bool selectedForChallenge)
    {
        DisplayedImage.sprite = image.sprite;
        picSelectedForGallery = selectedForGallery;
        picQualifiesForChallenge = qualifiesForChallenge;
        picSelectedForChallenge = selectedForChallenge;

        if (picSelectedForGallery) { GalleryLabel.text = Constants.UndoSaveToGallery; }
        else { GalleryLabel.text = Constants.SaveToGallery; }

        ChallengeButton.gameObject.SetActive(picQualifiesForChallenge);
        ChallengeLabel.text = picSelectedForChallenge ? Constants.UnsubmitChallenge : Constants.SubmitChallenge;
    }

    public void Update()
    {
        //UI navigation with controller
        HandleControllerNavigation();

        //check for input to press the selected button
        if (CrossPlatformInputManager.GetButtonDown(Constants.Submit) && !Input.GetMouseButtonDown(0)) //ignore clicks to avoid invoking buttons twice
        {
            if (currentSelectedButton != null)
            {
                Button buttonSelect = currentSelectedButton.GetComponent<Button>();
                if (buttonSelect != null && buttonSelect.enabled)
                {
                    buttonSelect.onClick.Invoke();
                }
            }
        }
    }

    public void HandleControllerNavigation()
    {
        HandleHorizontalControllerNavigation();
        HandleVerticalControllerNavigation();
    }

    private void HandleVerticalControllerNavigation()
    {
        if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Vertical))
        {
            float verticalDir = CrossPlatformInputManager.GetAxis(Constants.Vertical);
            //positive value is up, negative value is down 
        }
    }

    private void HandleHorizontalControllerNavigation()
    {
        if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Horizontal))
        {
            if (currentSelectedButton == null) { MoveHighlight(YesButton); return; } //default button if we dont have one yet

            float horizontalDir = CrossPlatformInputManager.GetAxis(Constants.Horizontal);

            //positive value is right
            if (horizontalDir > 0)
            {
                if (currentSelectedButton == YesButton) { MoveHighlight(NoButton); return; }
                if (currentSelectedButton == NoButton)
                {
                    if (picQualifiesForChallenge) { MoveHighlight(ChallengeButton); return; }
                    else { MoveHighlight(GalleryButton); return; }
                }
                if (currentSelectedButton == GalleryButton || currentSelectedButton == ChallengeButton) { MoveHighlight(YesButton); return; }
            }
            // negative value is left
            else
            {
                if (currentSelectedButton == NoButton) { MoveHighlight(YesButton); return; }
                if (currentSelectedButton == YesButton)
                {
                    if (picQualifiesForChallenge) { MoveHighlight(ChallengeButton); return; }
                    else { MoveHighlight(GalleryButton); return; }
                }
                if (currentSelectedButton == GalleryButton || currentSelectedButton == ChallengeButton) { MoveHighlight(NoButton); return; }
            }
        }
    }

    public void MoveHighlight(UIControlWithHighlight button)
    {
        UIControlWithHighlight prevSelectedButton = currentSelectedButton;
        currentSelectedButton = button;

        //set hover to match dimensions of selected button
        if (prevSelectedButton != null) { prevSelectedButton.HideHighlight(); }
        if (currentSelectedButton != null) { currentSelectedButton.ShowHighlight(); }
    }

    public void OnGalleryButtonClick()
    {

    }

    public void OnChallengeButtonClick()
    {

    }

}
