using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class ControllerNavigableConfirmationWindow : MonoBehaviour
{
    [SerializeField]
    UIControlWithHighlight LeftButton;
    [SerializeField]
    UIControlWithHighlight RightButton;

    private UIControlWithHighlight currentSelectedButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.gameObject.activeInHierarchy) { return; }

        //check for input to select a different button
        if (InputManager.Instance.GetButtonDown(Constants.Navigate))
        {
            ChangeSelectedButton();
        }

        //check for input to press the selected button
        if (InputManager.Instance.GetButtonDown(Constants.Submit) && !InputManager.Instance.GetButtonDown(Constants.Click)) //ignore clicks to avoid invoking buttons twice
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

    void ChangeSelectedButton()
    {
        if (currentSelectedButton == RightButton || currentSelectedButton == null)
        {
            currentSelectedButton = LeftButton;
            RightButton.HideHighlight();
            LeftButton.ShowHighlight();
        }
        else
        {
            currentSelectedButton = RightButton;
            RightButton.ShowHighlight();
            LeftButton.HideHighlight();
        }
    }
}
