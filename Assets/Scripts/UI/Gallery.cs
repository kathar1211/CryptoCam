using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    [SerializeField]
    SelectableImage[] thumbnails;
    [SerializeField]
    Image bigThumbnail;
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
    public UIControlWithHighlight LeftButton;
    public UIControlWithHighlight RightButton;

    //sound effects
    [SerializeField]
    AudioSource ClickSFX;
    [SerializeField]
    AudioSource ConfirmSFX;
    [SerializeField]
    AudioSource CancelSFX;

    public bool ReadyToClose;

    //store photograph information that corresponds with each thumbnail
    Dictionary<SelectableImage, Texture2D> displayedPhotos = new Dictionary<SelectableImage, Texture2D>();

    public GameObject emptyGalleryText;
    public GameObject thumbnailHolder;

    //all data
    private List<Texture2D> allGalleryPhotos;

    private bool isInitialized = false;
    int pageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!isInitialized)
        {
            //if we have save data, load it up on creating the cryptidnomicon
            if (Save.SaveFileExists())
            {
                allGalleryPhotos = Save.LoadGalleryPhotos();
            }

            DrawThumbnails();

            isInitialized = true;
        }
    }

    private void OnEnable()
    {
        if (!isInitialized) { Start(); }
        else { DrawThumbnails(); }
    }

    //draw thumbnails to page
    private void DrawThumbnails()
    {
        //show explanatory text if theres no gallery photos
        if (allGalleryPhotos == null || allGalleryPhotos.Count == 0)
        {
            emptyGalleryText.gameObject.SetActive(true);
            thumbnailHolder.SetActive(false);
        }
        else
        {
            emptyGalleryText.gameObject.SetActive(false);
            thumbnailHolder.SetActive(true);
        }

        //show left/right navigation only if we have more photos than can show on a screen
        LeftButton.gameObject.SetActive(allGalleryPhotos.Count > thumbnails.Length);
        RightButton.gameObject.SetActive(allGalleryPhotos.Count > thumbnails.Length);

        //create and assign sprite 
        int startingIndex = pageIndex * thumbnails.Length;
        for (int i = 0; i < thumbnails.Length; i++)
        {
            int photoIndex = i + startingIndex;
            if (photoIndex >= allGalleryPhotos.Count) 
            { 
                thumbnails[i].gameObject.SetActive(false); 
            }
            else
            {
                thumbnails[i].gameObject.SetActive(true);
                thumbnails[i].sprite = Sprite.Create(allGalleryPhotos[photoIndex], new Rect(0f, 0f, allGalleryPhotos[photoIndex].width, allGalleryPhotos[photoIndex].height), new Vector2(.5f, .5f));
            }
        }

        //todo: we'll make a little page dots display 
    }


    // Update is called once per frame
    void Update()
    {
        HandleControllerNavigation();
    }

    void HandleControllerNavigation()
    {

    }

    public void OnLeftClick()
    {
        pageIndex--;
        int pageIndexLimit = Mathf.FloorToInt((allGalleryPhotos.Count * 1f) / (thumbnails.Length * 1f));
        if (pageIndex < 0) { pageIndex = pageIndexLimit; }

        DrawThumbnails();
    }

    public void OnRightClick()
    {
        pageIndex++;
        int pageIndexLimit = Mathf.FloorToInt((allGalleryPhotos.Count * 1f) / (thumbnails.Length * 1f));
        if (pageIndex > pageIndexLimit) { pageIndex = 0; }

        DrawThumbnails();
    }

    //bring up image on click
    public void Enlarge(SelectableImage src)
    {
        if (!bigThumbnail.gameObject.activeInHierarchy && currentState == GalleryState.allThumbs)
        {
            //play sfx if applicable
            if (ClickSFX != null) { ClickSFX.Play(); }

            selectedImage = src;
            //bigThumbnail.sprite = src.sprite;
            bigThumbnail.gameObject.SetActive(true);
            bigThumbnail.sprite = src.Image.sprite;

            currentState = GalleryState.bigThumb;
        }
    }

    public void Delarge()
    {
        bigThumbnail.gameObject.SetActive(false);
        currentState = GalleryState.allThumbs;
    }

    //accept new photos, return the final list of all photos together
    public List<Texture2D> MergePhotos(List<Photograph> newPhotos)
    {
        if (!isInitialized) { Start(); }

        if (allGalleryPhotos == null)
        {
            allGalleryPhotos = new List<Texture2D>();
        }
      
        foreach (Photograph photo in newPhotos)
        {
            allGalleryPhotos.Add(photo.pic);
        }


        return allGalleryPhotos;
    }

    public List<Texture2D> GetGallery() { return allGalleryPhotos; }
}
