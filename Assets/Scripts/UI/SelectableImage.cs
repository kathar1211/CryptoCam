using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableImage : UIControlWithHighlight
{
    public Image Image;

    public Sprite sprite { get { return Image.sprite; } set { Image.sprite = value; } }
    public GameObject SelectedIndicator;


}
