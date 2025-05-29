using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//convenience script for images with a faked drop shadow
public class SpriteWithShadow : MonoBehaviour
{
    public Image MainSprite;
    public Image ShadowSprite;

    //set the sprite for both the image and the drop shadow at once
    public void SetSprite(Sprite sprite)
    {
        MainSprite.sprite = sprite;
        ShadowSprite.sprite = sprite;
    }
}
