using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//keep track of ted's many emotions
public enum TedMoods
{
    Default,
    LookDownHandUp,
    LookDownHandUpBlush,
    LookUpHandUp,
    LookUp,
    LookUpBlush,
    SquintHandUp,
    SquintHandUpBlush,
    Surprised,
    Disappointed,
    Satisfied,
    LeanForward,
    Pleased,
    Uncertain,
    Happy
}




public class Ted : MonoBehaviour {

    
    //ted sprites
    public Sprite normal;
    public Sprite lookDownHandUp;
    public Sprite lookDownHandUpBlush;
    public Sprite lookUpHandUp;
    public Sprite lookUp;
    public Sprite lookUpBlush;
    public Sprite squintHandUp;
    public Sprite squintHandUpBlush;
    public Sprite surprised;
    public Sprite disappointed;
    public Sprite satisfied;
    public Sprite leanForward;
    public Sprite pleased;
    public Sprite uncertain;
    public Sprite happy;

    //teds actual image, not his container
    public SpriteWithShadow tedImg;
    public Animator tedAnimator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //change out ted sprite to match mood
    public void SetTedSprite(TedMoods mood)
    {
        switch (mood)
        {
            case TedMoods.Default:
                tedImg.SetSprite(normal);
                break;
            case TedMoods.LookDownHandUp:
                tedImg.SetSprite(lookDownHandUp);
                break;
            case TedMoods.LookDownHandUpBlush:
                tedImg.SetSprite(lookDownHandUpBlush);
                break;
            case TedMoods.LookUpHandUp:
                tedImg.SetSprite(lookUpHandUp);
                break;
            case TedMoods.LookUp:
                tedImg.SetSprite(lookUp);
                break;
            case TedMoods.LookUpBlush:
                tedImg.SetSprite(lookUpBlush);
                break;
            case TedMoods.SquintHandUp:
                tedImg.SetSprite(squintHandUp);
                break;
            case TedMoods.SquintHandUpBlush:
                tedImg.SetSprite(squintHandUpBlush);
                break;
            case TedMoods.Surprised:
                tedImg.SetSprite(surprised);
                break;
            case TedMoods.Disappointed:
                tedImg.SetSprite(disappointed);
                break;
            case TedMoods.Satisfied:
                tedImg.SetSprite(satisfied);
                break;
            case TedMoods.LeanForward:
                tedImg.SetSprite(leanForward);
                break;
            case TedMoods.Pleased:
                tedImg.SetSprite(pleased);
                break;
            case TedMoods.Uncertain:
                tedImg.SetSprite(uncertain);
                break;
            case TedMoods.Happy:
                tedImg.SetSprite(happy);
                break;
        }
    }

    public void SlideTed()
    {
        tedAnimator.SetTrigger("Slide");
    }
}
