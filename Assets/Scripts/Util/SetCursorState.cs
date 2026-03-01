using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//hide cursor if user is using gamepad, show cursor if player is using mouse/keyboard
public class SetCursorState : MonoBehaviour
{

    private void Start()
    {
        Object.DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //mouse input - show the mouse cursor if user is using the mouse
        if (CustomController.HasMouseMoved())
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.visible = true;
            }
            CustomController.UsingController = false;
        }

        //gamepad input - im a little worried about checking every button every frame forever but let's see how it goes
        if (CustomController.IsControllerConnected())
        {
            //gamepad buttons dont have an event like keypresses, we have to check them all ourselves
            //https://discussions.unity.com/t/find-out-if-any-button-on-any-gamepad-has-been-pressed-and-which-one/65089
            foreach (KeyCode button in CustomController.AllGamepadButtons)
            {
                if (Input.GetKeyDown(button))
                {
                    CustomController.UsingController = true;
                    Cursor.visible = false;
                }
            }

            //allowing axis input from gamepad as well
            foreach (string axis in CustomController.AllGamepadAxesAsButtons)
            {
                if (CrossPlatformInputManager.GetButtonOrAxisDown(axis))
                {
                    CustomController.UsingController = true;
                    Cursor.visible = false;
                }
            }
        }
    }

    public static void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void UnlockCursor()
    {
        if (!CustomController.UsingController)
        {
            Cursor.visible = true;
        }
        Cursor.lockState = CursorLockMode.None;
    }

   
}
