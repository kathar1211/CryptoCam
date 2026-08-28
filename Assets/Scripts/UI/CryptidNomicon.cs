using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

//holds all content needed for a given page of the cryptidnomicon
public class PageContent
{
   public int photoScore;
   public Texture2D image;
   public string name;
   public string flavorText;
}

public class CryptidNomicon : MonoBehaviour {

    //for cryptidnomicon
    protected GameObject page;
    public Sprite[] pages;
    protected int currentPage;

    public Image thumbnail;
    public TextMeshProUGUI scoreDesc;
    public TextMeshProUGUI imageDesc;
    public TextMeshProUGUI nameDesc;
    public Image aboutTheAuthor;
    public Image largeThumbnail;
    public Image largeThumbnailOverlay;
    public Image notUnlockedSilhouette;
    public TextMeshProUGUI totalScoreText;
    public StarIndicator starRatingDisplay;

    protected Dictionary<string, PageContent> pageContents;
    protected bool isInitialized = false;

    //"state" for when a photo is clicked and enlarged for viewing
    bool viewing = false;

    [SerializeField]
    AudioSource pageTurnSFX;

    public bool ReadyToClose = false;

    //sprites used for pages where cryptids are not unlocked yet
    public Sprite JackalopePreview;
    public Sprite NessiePreview;
    public Sprite FresnoPreview;
    public Sprite FrogmanPreview;
    public Sprite MothmanPreview;
    public Sprite BigfootPreview;
    public Sprite FlatwoodsPreview;
    public Sprite TsuchinokoPreview;

    protected Dictionary<string, Sprite> CryptidPreviewTable;

    // Use this for initialization
    protected void Start () {
        if (!isInitialized)
        {
            page = this.transform.GetChild(0).gameObject;
            currentPage = 0;
            pageContents = new Dictionary<string, PageContent>
            {
                { Constants.Jackalope, null },
                { Constants.Tsuchinoko, null },
                { Constants.Nessie, null },
                { Constants.Frogman, null },
                { Constants.Fresno, null },
                { Constants.Flatwoods, null },
                { Constants.Bigfoot, null },
                { Constants.Mothman, null },

            };
            aboutTheAuthor.gameObject.SetActive(false);

            //if we have save data, load it up on creating the cryptidnomicon
            if (Save.SaveFileExists())
            {
                pageContents = Save.LoadCryptidNomicon();
            }

            isInitialized = true;

            CryptidPreviewTable = new Dictionary<string, Sprite>
            {
                {Constants.Jackalope, JackalopePreview },
                {Constants.Fresno, FresnoPreview },
                {Constants.Tsuchinoko, TsuchinokoPreview },
                {Constants.Nessie, NessiePreview },
                {Constants.Flatwoods, FlatwoodsPreview },
                {Constants.Bigfoot, BigfootPreview },
                {Constants.Mothman, MothmanPreview },
                {Constants.Frogman, FrogmanPreview },
            };

            //get total score to show at the front
            UpdateScoreText();
        }
    }
	
	// Update is called once per frame
	protected void Update () {
        //handle page turning if not viewing a photo
        if (!viewing)
        {
            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Horizontal)){
                TurnPage(CrossPlatformInputManager.GetAxis(Constants.Horizontal) > 0);
            }

            //allow shoulder button navigation as well
            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.RTAxis) || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.RTAxisMac))
            {
                TurnPage(true);
            }
            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.LTAxis) || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.LTAxisMac))
            {
                TurnPage(false);
            }

            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Submit))
            {
                if (currentPage != 0 && currentPage != pageContents.Count + 1) //not applicable on front and back cover
                {
                    EnlargePhoto();
                }
            }

            if (CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Cancel))
            {
                Close();
            }
        }
        //close out of viewing a photo on any input
        else
        {
            if (Input.anyKeyDown || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Submit) || CrossPlatformInputManager.GetButtonOrAxisDown(Constants.Cancel))
            {
                DelargePhoto();
            }
        }

    }

    //turn pages of the cryptidnomicon. true for forward false for back
    public void TurnPage(bool forward)
    {
        //play sound effect
        if (pageTurnSFX != null) pageTurnSFX.Play();

        /*if (forward && currentPage >= pageContents.Count + 1)
        {
            ReadyToClose = true;
            return;
        }
        else*/
        if (forward && currentPage < pageContents.Count + 1)
        {
            currentPage++;
        }
        else if (!forward && currentPage > 0)
        {
            currentPage--;
        }
        //close book if player turns pages beyond back cover

        
        //current page will now keep track of content, but only 3 sprites are used: beginning middle and end
        if (currentPage == 0 )
        {
            page.GetComponent<Image>().sprite = pages[0];
            aboutTheAuthor.gameObject.SetActive(false);
            totalScoreText.gameObject.SetActive(true);
        }
        else if (currentPage > pageContents.Count)
        {
            page.GetComponent<Image>().sprite = pages[2];
            aboutTheAuthor.gameObject.SetActive(true);
            totalScoreText.gameObject.SetActive(false);
        }
        else
        {
            page.GetComponent<Image>().sprite = pages[1];
            aboutTheAuthor.gameObject.SetActive(false);
            totalScoreText.gameObject.SetActive(false);
        }

        //if we are still in the middle sprite update the content
        if (currentPage > 0 && currentPage <= pageContents.Count )
        {
            KeyValuePair<string, PageContent> entry = pageContents.ElementAt(currentPage - 1);
            PageContent content = entry.Value;
            if (content == null) //no entry/ picture not taken
            {
                DisplayLockedPage(entry.Key);
            }
            else //use player's saved image
            {
                DisplayUnlockedPage(content);
            }
           
        }

        //otherwise hide it
        else
        {
            thumbnail.gameObject.SetActive(false);
            scoreDesc.gameObject.SetActive(false);
            imageDesc.gameObject.SetActive(false);
            nameDesc.gameObject.SetActive(false);
            notUnlockedSilhouette.gameObject.SetActive(false);
            starRatingDisplay.gameObject.SetActive(false);
        }
    }

    protected virtual void DisplayUnlockedPage(PageContent content)
    {
        thumbnail.gameObject.SetActive(true);
        scoreDesc.gameObject.SetActive(true);
        imageDesc.gameObject.SetActive(true);
        nameDesc.gameObject.SetActive(true);
        starRatingDisplay.gameObject.SetActive(true);

        notUnlockedSilhouette.gameObject.SetActive(false);

        thumbnail.sprite = Sprite.Create(content.image, new Rect(0f, 0f, content.image.width, content.image.height), new Vector2(.5f, .5f));
        //thumbnail.rectTransform.sizeDelta = new Vector2(content.image.width/5, content.image.height/5);
        scoreDesc.text = "Score: " + content.photoScore;
        starRatingDisplay.ShowStars(content.photoScore);
        imageDesc.text = content.flavorText;
        nameDesc.text = content.name;
    }

    protected virtual void DisplayLockedPage(string contentKey)
    {
        thumbnail.gameObject.SetActive(false);
        scoreDesc.gameObject.SetActive(false);
        starRatingDisplay.gameObject.SetActive(false);

        imageDesc.gameObject.SetActive(true);
        nameDesc.gameObject.SetActive(false);
        notUnlockedSilhouette.gameObject.SetActive(true);

        //demo stuff
        /* if (Constants.DemoLockedCryptids.Contains(entry.Key))
         {
             imageDesc.text = Constants.demoLockedEntry;
         }
         else
         {*/
        imageDesc.text = Constants.defaultEntry;
        //}

        notUnlockedSilhouette.sprite = CryptidPreviewTable[contentKey];
        //nameDesc.text = entry.Key;
    }

    //convert photo taken in game to content to display on page
    public virtual PageContent PhotoToPage(Photograph photo)
    {
        PageContent content = new PageContent();
        content.image = photo.pic;
        content.name = photo.subjectName;
        content.photoScore = photo.finalScore;
        if (Constants.tedsWriting.ContainsKey(photo.subjectName)) { content.flavorText = Constants.tedsWriting[photo.subjectName]; }

        return content;
    }

    public Photograph PageToPhoto(PageContent content)
    {
        Photograph photo = new Photograph();
        photo.pic = content.image;
        photo.subjectName = content.name;
        photo.finalScore = content.photoScore;
        return photo;
    }

    //accept photos from grading to display in the crytpidnomicon
    public virtual Dictionary<string, PageContent> RecievePhotos(List<Photograph> finalPhotos)
    {
        if (!isInitialized) { Start(); }
        foreach (Photograph photo in finalPhotos)
        {
            if (photo.finalScore <= 0) { continue; }

            if (!pageContents.ContainsKey(photo.subjectName))
            {
                pageContents.Add(photo.subjectName, PhotoToPage(photo));
            }
            else
            {
                pageContents[photo.subjectName] = PhotoToPage(photo);
            }

        }

        UpdateScoreText();

        //return photo data structure for saving
        return pageContents;
    }

    protected void UpdateScoreText()
    {
        int totalScore = CalculateTotalScore(pageContents);
        if (totalScore == 0) { totalScoreText.text = "Total Score:\n-"; }
        else { totalScoreText.text = "Total Score:\n" + totalScore.ToString(); }
    }

    private int CalculateTotalScore(Dictionary<string, PageContent> pagecontents)
    {
        int sum = 0;
        foreach (KeyValuePair<string, PageContent> content in pagecontents)
        {
            if (content.Value == null) { continue; }
            sum += content.Value.photoScore;
        }
        return sum;
    }

    //select a photo to view it up close
    public void EnlargePhoto()
    {
        largeThumbnailOverlay.gameObject.SetActive(true);
        largeThumbnail.sprite = thumbnail.sprite;
        viewing = true;
    }

    //return to default state after viewing a photo
    public void DelargePhoto()
    {
        largeThumbnailOverlay.gameObject.SetActive(false);
        viewing = false;
    }

    //returns true if this cryptidnomicon has an entry for a given cryptid
    public bool HasEntry(string key)
    {
        if (!isInitialized) { Start(); }
        return pageContents.ContainsKey(key);
    }

    //returns the entry for a given cryptid. returns an empty pagecontent object if no entry is found
    public PageContent GetEntry(string key)
    {
        if (!isInitialized) { Start(); }
        if (HasEntry(key))
        {
            return pageContents[key];
        }

        PageContent empty = new PageContent();
        return empty;
    }

    public void Close()
    {
        ReadyToClose = true;
    }

    public Dictionary<string, PageContent> GetPageContents() { 
        if (!isInitialized) { Start(); }
        return pageContents;
    }
}
