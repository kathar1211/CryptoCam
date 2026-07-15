using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public Image Star1;
    public Image Star2;
    public Image Star3;
    public GameObject Container;

    public Color FilledStar;
    public Color EmptyStar;

    static int TwoStarScore = 250;
    static int ThreeStarScore = 500;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStars(int score)
    {
        int starCount;
        if (score < TwoStarScore) { starCount = 1; }
        else if (score < ThreeStarScore) { starCount = 2; }
        else { starCount = 3; }

        Container.SetActive(true);
        Star1.gameObject.SetActive(true);
        Star2.gameObject.SetActive(true);
        Star3.gameObject.SetActive(true);

        Star1.color = starCount < 1 ? EmptyStar : FilledStar;
        Star2.color = starCount < 2 ? EmptyStar : FilledStar;
        Star3.color = starCount < 3 ? EmptyStar : FilledStar;

    }

    public void HideStars()
    {
        Container.SetActive(false);
    }

}
