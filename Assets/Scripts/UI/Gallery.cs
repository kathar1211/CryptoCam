using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    [SerializeField]
    SelectableImage[] thumbnails;
    [SerializeField]
    BigThumbnail bigThumbnail;
    SelectableImage selectedImage;

    [SerializeField]
    int columns; //need to know what the grid layout is to navigate thumbnails via controller
    UIControlWithHighlight highlightedUIcontrol;
    int activeThumbnailCount; //how many of these thumbnail objects are actually showing images

    enum GalleryState { allThumbs, bigThumb, confirm };
    GalleryState currentState;

    public UIControlWithHighlight DeleteButton;
    public UIControlWithHighlight SaveButton;
    public UIControlWithHighlight CloseButton;

    //sound effects
    [SerializeField]
    AudioSource ClickSFX;
    [SerializeField]
    AudioSource ConfirmSFX;
    [SerializeField]
    AudioSource CancelSFX;

    public bool ReadyToClose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
