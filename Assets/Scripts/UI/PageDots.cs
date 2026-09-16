using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageDots : MonoBehaviour
{
    public Image dotPrefab;
    public Transform dotHolder;

    public Color selectedDotColor;
    public Color unselectedDotColor;

    private List<Image> allDots;
    private int selectedDotIndex;

    public void Init(int pageCount)
    {
        if (allDots != null)
        {
            foreach (Image dot in allDots)
            {
                Destroy(dot.gameObject);
            }
        }

        allDots = new List<Image>();
        for (int i = 0; i <= pageCount; i++)
        {
            Image dot = GameObject.Instantiate(dotPrefab, dotHolder);
            dot.color = unselectedDotColor;
            allDots.Add(dot);
        }
        SelectPage(0);
        selectedDotIndex = 0;
    }

    public void SelectPage(int pageIndex)
    {
        allDots[selectedDotIndex].color = unselectedDotColor;
        selectedDotIndex = pageIndex;
        allDots[selectedDotIndex].color = selectedDotColor;
    }
}
