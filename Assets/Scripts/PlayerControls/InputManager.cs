using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public PlayerInput Input;

    public Vector2 MoveVector { get; private set; }
    public Vector2 LookVector { get; private set; }
    public Vector2 NavigateVector { get; private set; }
    private Vector2 PreviousNavigateVector;

    private static float axisDownThreshold = .75f;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputValue value)
    {
        MoveVector = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        LookVector = value.Get<Vector2>();
    }

    public void OnNavigate(InputValue value)
    {
        PreviousNavigateVector = new Vector2(NavigateVector.x, NavigateVector.y);
        NavigateVector = value.Get<Vector2>();
    }

    public bool GetButtonDown(string buttonName)
    {
        return Input.actions[buttonName].WasPressedThisFrame();
    }

    public bool GetButton(string buttonName)
    {
        return Input.actions[buttonName].IsPressed();
    }

    public bool GetButtonUp(string buttonName)
    {
        return Input.actions[buttonName].WasReleasedThisFrame();
    }

    public void EnablePlayerActionMap()
    {
        Input.SwitchCurrentActionMap(Constants.PlayerActionMap);
    }

    public void EnableUIActionMap()
    {
        Input.SwitchCurrentActionMap(Constants.UIActionMap);
    }

    public bool AnyKeyDown()
    {
        return Keyboard.current.anyKey.wasPressedThisFrame;
    }

    /*
    public void OnTakePicture(InputValue value)
    {

    }

    public void OnReadyCamera(InputValue value)
    {

    }

    public void OnThrowObject(InputValue value)
    {

    }

    public void OnCrouch(InputValue value)
    {

    }

    public void OnRun(InputValue value)
    {

    }

    public void OnJump(InputValue value)
    {

    }

    public void OnNavigate(InputValue value)
    {

    }

    public void OnSubmit(InputValue value)
    {

    }

    public void OnCancel(InputValue value)
    {

    }

    public void OnPoint(InputValue value)
    {

    }

    public void OnClick(InputValue value)
    {

    }
    */

}
