using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

//holds all content needed for a given page of the cryptidnomicon
public struct PageContent
{
   public int photoScore;
   public Texture2D image;
   public string name;
   public string flavorText;
}

public class CryptidNomicon : MonoBehaviour {

    //holds description text for cryptids
    public static Dictionary<string, string> tedsWriting = new Dictionary<string, string>()
    {
        {"Tsuchinoko","Tsuchinoko, the fat snake cryptid, is identifiable by is distinct round belly. He is Japanese in origin and rumoured to have a propensity for telling lies and drinking alcohol." },
        {"Loch Ness Monster","Affectionately dubbed \"Nessie\" by her fans, the Loch Ness Monster is a popular and beloved aquatic creature. In 1933 she was put under an order of protection by the Scottish Government; serious legal consequences await anyone who would do her harm." },
        {"Loveland Frogman","These large bipedal frogs are named after Loveland Ohio, the region where they were first spotted lurking out of the water. Some reports claim they weild magic wands, but that's just silly." },
        {"Fresno Nightcrawler", "Originally captured on security footage in Fresno, California, Fresno Nightcrawlers are recognizeable by their elongated legs and strange, meandering gait. These suspected extraterrestial seem to come in pairs, not unlike pants." },
        {"Jackalope", "Two parts jackrabbit and one part antelope, the Jackalope may seem cute and friendly but is actually known for its incredible strength. Some hunters have been known to go as far as wearing stovepipes on their legs to avoid being impaled by its fearsome horns." }
    };

    //for cryptidnomicon
    GameObject page;
    public Sprite[] pages;
    int currentPage;

    public Image thumbnail;
    public Text scoreDesc;
    public Text imageDesc;
    public Text nameDesc;
    public Image aboutTheAuthor;
    public Image largeThumbnail;
    public Image largeThumbnailOverlay;

    List<PageContent> pageContents;

    //"state" for when a photo is clicked and enlarged for viewing
    bool viewing = false;

    [SerializeField]
    AudioSource pageTurnSFX;

    // Use this for initialization
    void Start () {
        page = this.transform.GetChild(0).gameObject;
        currentPage = 0;
        pageContents = new List<PageContent>();
        aboutTheAuthor.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        //handle page turning if not viewing a photo
        if (!viewing)
        {
            /*if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TurnPage(false);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TurnPage(true);
            }*/
            if (CrossPlatformInputManager.GetButtonDown(Constants.Horizontal)){
                TurnPage(CrossPlatformInputManager.GetAxis(Constants.Horizontal) > 0);
            }
        }
        //close out of viewing a photo on any input
        else
        {
            if (Input.anyKeyDown)
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

        if (forward && currentPage < pageContents.Count + 1)
        {
            currentPage++;
        }
        else if (!forward && currentPage > 0)
        {
            currentPage--;
        }
        
        //current page will now keep track of content, but only 3 sprites are used: beginning middle and end
        if (currentPage == 0 )
        {
            page.GetComponent<Image>().sprite = pages[0];
            aboutTheAuthor.gameObject.SetActive(false);
        }
        else if (currentPage > pageContents.Count)
        {
            page.GetComponent<Image>().sprite = pages[2];
            aboutTheAuthor.gameObject.SetActive(true);
        }
        else
        {
            page.GetComponent<Image>().sprite = pages[1];
            aboutTheAuthor.gameObject.SetActive(false);
        }

        //if we are still in the middle sprite update the content
        if (currentPage > 0 && currentPage <= pageContents.Count )
        {
            thumbnail.gameObject.SetActive(true);
            scoreDesc.gameObject.SetActive(true);
            imageDesc.gameObject.SetActive(true);
            nameDesc.gameObject.SetActive(true);
            PageContent content = pageContents[currentPage-1];

            thumbnail.sprite = Sprite.Create(content.image, new Rect(0f, 0f, content.image.width, content.image.height), new Vector2(.5f, .5f));
            thumbnail.rectTransform.sizeDelta = new Vector2(content.image.width/5, content.image.height/5);
            scoreDesc.text = "Score: " + content.photoScore;
            imageDesc.text = content.flavorText;
            nameDesc.text = content.name;
        }

        //otherwise hide it
        else
        {
            thumbnail.gameObject.SetActive(false);
            scoreDesc.gameObject.SetActive(false);
            imageDesc.gameObject.SetActive(false);
            nameDesc.gameObject.SetActive(false);
        }
    }

    //convert photo taken in game to content to display on page
    public PageContent PhotoToPage(Photograph photo)
    {
        PageContent content = new PageContent();
        content.image = photo.pic;
        content.name = photo.subjectName;
        content.photoScore = photo.finalScore;
        if (tedsWriting.ContainsKey(photo.subjectName)) { content.flavorText = tedsWriting[photo.subjectName]; }

        return content;
    }

    //accept photos from grading to display in the crytpidnomicon
    public void RecievePhotos(List<Photograph> finalPhotos)
    {
        foreach (Photograph photo in finalPhotos)
        {
            if (photo.finalScore > 0)
            pageContents.Add(PhotoToPage(photo));
        }
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
}
