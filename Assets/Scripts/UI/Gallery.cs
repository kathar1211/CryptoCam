using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

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
    public GameObject deleteConfirmationWindow;
    public PageDots pageNavigation;

    //all data
    private List<Texture2D> allGalleryPhotos;
    private List<Sprite> allPhotoSprites; //create all the sprites once to improve performance on redraws/page navigation

    private bool isInitialized = false;
    int pageIndex = 0;


    public CabinLab cabinLab;

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
            //if there's no save then this list hasnt been instantialized yet
            if (allGalleryPhotos == null) { allGalleryPhotos = new List<Texture2D>(); }

            allPhotoSprites = new List<Sprite>();
            foreach (Texture2D tex in allGalleryPhotos)
            {
                allPhotoSprites.Add(Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(.5f, .5f), 100,0, SpriteMeshType.FullRect));
            }

            // make a little page dots display 
            int pageIndexLimit = Mathf.FloorToInt((allGalleryPhotos.Count * 1f) / (thumbnails.Length * 1f));
            pageNavigation.Init(pageIndexLimit);

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
            pageNavigation.gameObject.SetActive(false);
            return;
        }
        else
        {
            emptyGalleryText.gameObject.SetActive(false);
            thumbnailHolder.SetActive(true);
        }

        //show left/right navigation only if we have more photos than can show on a screen
        LeftButton.gameObject.SetActive(allGalleryPhotos.Count > thumbnails.Length);
        RightButton.gameObject.SetActive(allGalleryPhotos.Count > thumbnails.Length);
        pageNavigation.gameObject.SetActive(allGalleryPhotos.Count > thumbnails.Length);

        //create and assign sprite 
        int startingIndex = pageIndex * thumbnails.Length;
        for (int i = 0; i < thumbnails.Length; i++)
        {
            int photoIndex = i + startingIndex;
            if (photoIndex >= allPhotoSprites.Count) 
            { 
                thumbnails[i].gameObject.SetActive(false); 
            }
            else
            {
                thumbnails[i].gameObject.SetActive(true);
                thumbnails[i].sprite = allPhotoSprites[photoIndex];
            }
        }

        //update pagedots
        pageNavigation.SelectPage(pageIndex);
    }


    // Update is called once per frame
    void Update()
    {
        HandleControllerNavigation();

        if (currentState == GalleryState.allThumbs)
        {
            //allow shoulder button navigation as well
            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.RTAxis) || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.RTAxisMac))
            {
                OnRightClick();
            }
            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.LTAxis) || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.LTAxisMac))
            {
                OnLeftClick();
            }
        }

        //handle closing windows based on state
        if (CrossPlatformInputManager.GetButtonDown(Constants.Cancel))
        {
            switch (currentState)
            {
                case GalleryState.allThumbs:
                    ReadyToClose = true;
                    if (CancelSFX != null) { CancelSFX.Play(); }
                    break;
                case GalleryState.bigThumb:
                    Delarge();
                    break;
                case GalleryState.confirm:
                    OnCancelDelete();
                    break;
            }
        }
    }

    void HandleControllerNavigation()
    {

    }

    public void OnLeftClick()
    {
        pageIndex--;
        int pageIndexLimit = Mathf.FloorToInt((allGalleryPhotos.Count - 1 * 1f) / (thumbnails.Length * 1f));
        if (pageIndex < 0) { pageIndex = pageIndexLimit; }

        DrawThumbnails();
    }

    public void OnRightClick()
    {
        pageIndex++;
        int pageIndexLimit = Mathf.FloorToInt(((allGalleryPhotos.Count - 1) * 1f) / (thumbnails.Length * 1f));
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
        if (CancelSFX != null) { CancelSFX.Play(); }
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
            allPhotoSprites.Add(Sprite.Create(photo.pic, new Rect(0f, 0f, photo.pic.width, photo.pic.height), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect));
        }

        DrawThumbnails();
        return allGalleryPhotos;
    }

    public List<Texture2D> GetGallery() {
        if (!isInitialized) { Start(); }
        return allGalleryPhotos; 
    }

    public void OnClickDelete()
    {
        deleteConfirmationWindow.SetActive(true);
        //bigThumbnail.gameObject.SetActive(false);
        currentState = GalleryState.confirm;
    }

    public void OnConfirmDelete()
    {
        if (ConfirmSFX != null) { ConfirmSFX.Play(); }
        deleteConfirmationWindow.SetActive(false);

        //which picture was this
        int thumbnailIndex = Array.IndexOf(thumbnails, selectedImage);
        int photoIndex = (pageIndex * thumbnails.Length) + thumbnailIndex;
        allGalleryPhotos.RemoveAt(photoIndex);
        allPhotoSprites.RemoveAt(photoIndex);

        Delarge();
        DrawThumbnails();
        cabinLab.SavePhotos(null);
    }

    public void OnCancelDelete()
    {
        deleteConfirmationWindow.SetActive(false);
        if (CancelSFX != null) { CancelSFX.Play(); }
        currentState = GalleryState.bigThumb;
        bigThumbnail.gameObject.SetActive(true);
    }

    public void OnClickSave()
    {
        Save.SavePhotoToPNG(bigThumbnail.sprite.texture);
    }
}
