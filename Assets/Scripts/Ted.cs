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
    public Image tedImg;

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
                tedImg.sprite = normal;
                break;
            case TedMoods.LookDownHandUp:
                tedImg.sprite = lookDownHandUp;
                break;
            case TedMoods.LookDownHandUpBlush:
                tedImg.sprite = lookDownHandUpBlush;
                break;
            case TedMoods.LookUpHandUp:
                tedImg.sprite = lookUpHandUp;
                break;
            case TedMoods.LookUp:
                tedImg.sprite = lookUp;
                break;
            case TedMoods.LookUpBlush:
                tedImg.sprite = lookUpBlush;
                break;
            case TedMoods.SquintHandUp:
                tedImg.sprite = squintHandUp;
                break;
            case TedMoods.SquintHandUpBlush:
                tedImg.sprite = squintHandUpBlush;
                break;
            case TedMoods.Surprised:
                tedImg.sprite = surprised;
                break;
            case TedMoods.Disappointed:
                tedImg.sprite = disappointed;
                break;
            case TedMoods.Satisfied:
                tedImg.sprite = satisfied;
                break;
            case TedMoods.LeanForward:
                tedImg.sprite = leanForward;
                break;
            case TedMoods.Pleased:
                tedImg.sprite = pleased;
                break;
            case TedMoods.Uncertain:
                tedImg.sprite = uncertain;
                break;
            case TedMoods.Happy:
                tedImg.sprite = happy;
                break;
        }
    }
}
