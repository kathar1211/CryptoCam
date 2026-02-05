using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlWithHighlight : MonoBehaviour
{
    public GameObject Highlight;

    public void ShowHighlight()
    {
        Highlight.SetActive(true);
    }

    public void HideHighlight()
    {
        Highlight.SetActive(false);
    }
}
