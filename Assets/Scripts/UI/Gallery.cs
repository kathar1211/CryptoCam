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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
