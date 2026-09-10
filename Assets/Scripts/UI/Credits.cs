using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrantViewCreditsAchievement()
    {
        AchievementManager.GrantAchievement(AchievementManager.STEAM_ACHIEVEMENTS.WATCH_CREDITS);
    }
}
