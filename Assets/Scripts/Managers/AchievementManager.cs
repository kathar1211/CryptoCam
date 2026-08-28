using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager 
{
    public enum STEAM_ACHIEVEMENTS
    {
        TAKE_PICTURE_JACKALOPE,
        TAKE_PICTURE_NESSIE,
        TAKE_PICTURE_TSUCHINOKO,
        TAKE_PICTURE_FRESNO,
        TAKE_PICTURE_FROGMAN,
        TAKE_PICTURE_FLATWOODS,
        TAKE_PICTURE_BIGFOOT,
        TAKE_PICTURE_MOTHMAN,
        BONK_ALL_CRYPTIDS,
        CRYPTIDNOMICON_COMPLETE,
        CRYPTIDNOMICON_PERFECT,
        CRYPTID_POOF,
        SAVE_TO_GALLERY,
        TALK_TO_TED,
        WATCH_CREDITS,
        LAND_CARROT_ON_LILYPAD,
        CHALLENGES_COMPLETE_ALL
    };

    public enum STEAM_STATS
    {
        CRYPTIDNOMICON_ENTRIES = 3,
        CRYPTIDNOMICON_ENTRIES_PERFECT = 4,
        CHALLENGES_COMPLETE = 5,
        CRYPTIDS_BONKED = 6
    };

    public static void UpdateCryptidNomiconStats(Dictionary<string, PageContent> contents)
    {
        if (contents == null) { return; }

        int entryCount = 0;
        int threeStarCount = 0;

        foreach (KeyValuePair<string, PageContent> content in contents)
        {
           if (content.Value == null) { continue; }
           if (!string.IsNullOrEmpty(content.Value.name)) { entryCount++; }
           if (content.Value.photoScore >= StarIndicator.ThreeStarScore) { threeStarCount++; }
        }

        SetStat(STEAM_STATS.CRYPTIDNOMICON_ENTRIES, entryCount);
        SetStat(STEAM_STATS.CRYPTIDNOMICON_ENTRIES_PERFECT, threeStarCount);
    }

    public static void SetStat(STEAM_STATS statID, int newStatValue)
    {
        SteamManager.SetStat(statID.ToString(), newStatValue);
    }

    public static void GrantAchievement(STEAM_ACHIEVEMENTS achievementEnum)
    {
        SteamManager.GrantAchievement(achievementEnum.ToString());
    }

    public static void GrantPictureAchievement(string photoSubject)
    {
        switch (photoSubject)
        {
            case Constants.Jackalope:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_JACKALOPE.ToString());
                break;
            case Constants.Nessie:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_NESSIE.ToString());
                break;
            case Constants.Tsuchinoko:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_TSUCHINOKO.ToString());
                break;
            case Constants.Frogman:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_FROGMAN.ToString());
                break;
            case Constants.Flatwoods:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_FLATWOODS.ToString());
                break;
            case Constants.Fresno:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_FRESNO.ToString());
                break;
            case Constants.Bigfoot:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_BIGFOOT.ToString());
                break;
            case Constants.Mothman:
                SteamManager.GrantAchievement(STEAM_ACHIEVEMENTS.TAKE_PICTURE_MOTHMAN.ToString());
                break;
        }
    }
}
