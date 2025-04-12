using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSnap : MonoBehaviour {

    //black bars that open and close
    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;
    //state of the animation
    enum ShutterState { Still, Closing, Opening};
    ShutterState snapState;
    //speed of the animation
    public float shutterSpeed;
    //canvas the animation is on
    CanvasScaler canvas;

	// Use this for initialization
	void Start () {
        //set initial positions
        canvas = this.gameObject.transform.parent.GetComponent<CanvasScaler>();
        topBar.localPosition = new Vector3(0, canvas.referenceResolution.y / 2,0);
        bottomBar.localPosition = new Vector3(0, -canvas.referenceResolution.y / 2, 0);
        topBar.sizeDelta = new Vector2(canvas.referenceResolution.x, 0);
        bottomBar.sizeDelta = new Vector2(canvas.referenceResolution.x, 0);
    }
	
	// Update is called once per frame
	void Update () {
        switch(snapState){
            //grow the shutters until they take up the full screen
            case ShutterState.Closing:
                topBar.sizeDelta = new Vector2(canvas.referenceResolution.x, topBar.rect.height + shutterSpeed);
                bottomBar.sizeDelta = new Vector2(canvas.referenceResolution.x, bottomBar.rect.height + shutterSpeed);
                if (topBar.rect.height >= canvas.referenceResolution.y || bottomBar.rect.height >= canvas.referenceResolution.y)
                {
                    snapState = ShutterState.Opening;
                }
                break;
            //shrink the shutters until they are no longer visible
            case ShutterState.Opening:
                topBar.sizeDelta = new Vector2(canvas.referenceResolution.x, topBar.rect.height - shutterSpeed);
                bottomBar.sizeDelta = new Vector2(canvas.referenceResolution.x, bottomBar.rect.height - shutterSpeed);
                if (topBar.rect.height <= 0 || bottomBar.rect.height <= 0)
                {
                    snapState = ShutterState.Still;
                }
                break;
            //do nothing
            case ShutterState.Still:
                break;
        }
	}

    //public method to set the animation in motion
    public void SnapShutter()
    {
        snapState = ShutterState.Closing;
    }
}
