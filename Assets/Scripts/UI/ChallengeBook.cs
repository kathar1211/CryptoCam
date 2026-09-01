using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeBook : CryptidNomicon
{
    //sprites where we show concept art for unlocked entries
    public Sprite JackalopeConceptArt;
    public Sprite NessieConceptArt;
    public Sprite FresnoConceptArt;
    public Sprite FrogmanConceptArt;
    public Sprite MothmanConceptArt;
    public Sprite BigfootConceptArt;
    public Sprite FlatwoodsConceptArt;
    public Sprite TsuchinokoConceptArt;

    public Image unlockedConceptArt;

    private Dictionary<string, Sprite> ConceptArtLookupTable;

    protected override void Initialize()
    {
        page = this.transform.GetChild(0).gameObject;
        currentPage = 0;
        pageContents = new Dictionary<string, PageContent>
            {
                {ChallengePhotographContent.SleepingJackalope.ToString(), null },
                {ChallengePhotographContent.DancingFresno.ToString(), null },
                {ChallengePhotographContent.SleepingTsuchinoko.ToString(), null },
                {ChallengePhotographContent.LilypadFrogman.ToString(), null },
                {ChallengePhotographContent.PeaceSignFlatwoods.ToString(), null },
                {ChallengePhotographContent.NessieWithFrogman.ToString(), null },
                {ChallengePhotographContent.SittingBigfoot.ToString(), null },
                {ChallengePhotographContent.CarryingMothman.ToString(), null },


            };
        aboutTheAuthor.gameObject.SetActive(false);

        //if we have save data, load it up on creating the cryptidnomicon
        if (Save.SaveFileExists())
        {
            pageContents = Save.LoadChallengeBook();
        }

        isInitialized = true;

        CryptidPreviewTable = new Dictionary<string, Sprite>
            {
                {ChallengePhotographContent.SleepingJackalope.ToString(), JackalopePreview },
                {ChallengePhotographContent.DancingFresno.ToString(), FresnoPreview },
                {ChallengePhotographContent.SleepingTsuchinoko.ToString(), TsuchinokoPreview },
                {ChallengePhotographContent.NessieWithFrogman.ToString(), NessiePreview },
                {ChallengePhotographContent.PeaceSignFlatwoods.ToString(), FlatwoodsPreview },
                {ChallengePhotographContent.SittingBigfoot.ToString(), BigfootPreview },
                {ChallengePhotographContent.CarryingMothman.ToString(), MothmanPreview },
                {ChallengePhotographContent.LilypadFrogman.ToString(), FrogmanPreview },
            };

        ConceptArtLookupTable = new Dictionary<string, Sprite>
            {
                {Constants.SleepingJackalope.ToString(), JackalopeConceptArt },
                {Constants.DancingFresno.ToString(), FresnoConceptArt },
                {Constants.SleepingTsuchinoko.ToString(), TsuchinokoConceptArt },
                {Constants.NessieWithFrogman.ToString(), NessieConceptArt },
                {Constants.PeaceSignFlatwoods.ToString(), FlatwoodsConceptArt },
                {Constants.SittingBigfoot.ToString(), BigfootConceptArt },
                {Constants.CarryingMothman.ToString(), MothmanConceptArt },
                {Constants.LilypadFrogman.ToString(), FrogmanConceptArt },
            };

        //get total score to show at the front
        UpdateScoreText();
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
    }

    //accept photos from grading to display in the crytpidnomicon
    public override Dictionary<string, PageContent> RecievePhotos(List<Photograph> finalPhotos)
    {
        if (finalPhotos == null || finalPhotos.Count == 0) { return null; }

        if (!isInitialized) { Initialize(); }
        foreach (Photograph photo in finalPhotos)
        {
            if (photo.finalScore <= 0) { continue; }
            if (photo.challenge == ChallengePhotographContent.None) { continue; }


            if (!pageContents.ContainsKey(photo.challenge.ToString()))
            {
                pageContents.Add(photo.challenge.ToString(), PhotoToPage(photo));
            }
            else
            {
                pageContents[photo.challenge.ToString()] = PhotoToPage(photo);
            }

        }

        UpdateScoreText();

        //return photo data structure for saving
        return pageContents;
    }

    //instead of the cryptid's name, use the challenge description
    public override PageContent PhotoToPage(Photograph photo)
    {
        PageContent content = base.PhotoToPage(photo);
        content.name = ChallengeManager.GetTextForChallenge(photo.challenge);

        return content;
    }

    protected override void HideInnerContent()
    {
        base.HideInnerContent();
        unlockedConceptArt.gameObject.SetActive(false);
    }

    protected override void DisplayUnlockedPage(PageContent content)
    {
        thumbnail.gameObject.SetActive(true);
        scoreDesc.gameObject.SetActive(true);
        imageDesc.gameObject.SetActive(false);
        unlockedConceptArt.gameObject.SetActive(true);
        nameDesc.gameObject.SetActive(true);
        starRatingDisplay.gameObject.SetActive(true);

        notUnlockedSilhouette.gameObject.SetActive(false);

        thumbnail.sprite = Sprite.Create(content.image, new Rect(0f, 0f, content.image.width, content.image.height), new Vector2(.5f, .5f));
        //thumbnail.rectTransform.sizeDelta = new Vector2(content.image.width/5, content.image.height/5);
        scoreDesc.text = "Score: " + content.photoScore;
        starRatingDisplay.ShowStars(content.photoScore);
        nameDesc.text = content.name;
        unlockedConceptArt.sprite = ConceptArtLookupTable[content.name];
    }

    protected override void DisplayLockedPage(string contentKey)
    {
        thumbnail.gameObject.SetActive(false);
        scoreDesc.gameObject.SetActive(false);
        starRatingDisplay.gameObject.SetActive(false);
        unlockedConceptArt.gameObject.SetActive(false);

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
        imageDesc.text = Constants.challengesLockedEntry;
        //}

        notUnlockedSilhouette.sprite = CryptidPreviewTable[contentKey];
        //nameDesc.text = entry.Key;
    }
}
