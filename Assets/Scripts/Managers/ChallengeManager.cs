using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChallengePhotographContent
{
    None,
    SleepingJackalope,
    SleepingTsuchinoko,
    LilypadFrogman,
    NessieWithFrogman,
    DancingFresno,
    SittingBigfoot,
    PeaceSignFlatwoods,
    CarryingMothman

}

public class ChallengeManager
{
    public static string GetTextForChallenge(ChallengePhotographContent content)
    {
        switch (content)
        {
            case ChallengePhotographContent.SleepingJackalope:
                return Constants.SleepingJackalope;
            case ChallengePhotographContent.SleepingTsuchinoko:
                return Constants.SleepingTsuchinoko;
            case ChallengePhotographContent.LilypadFrogman:
                return Constants.LilypadFrogman;
            case ChallengePhotographContent.NessieWithFrogman:
                return Constants.NessieWithFrogman;
            case ChallengePhotographContent.DancingFresno:
                return Constants.DancingFresno;
            case ChallengePhotographContent.SittingBigfoot:
                return Constants.SittingBigfoot;
            case ChallengePhotographContent.PeaceSignFlatwoods:
                return Constants.PeaceSignFlatwoods;
            case ChallengePhotographContent.CarryingMothman:
                return Constants.CarryingMothman;
            default:
            case ChallengePhotographContent.None:
                return "aint shit";
        }
    }

    public static bool ChallengeModeUnlocked()
    {
        return PlayerPrefs.GetInt(Constants.CryptidNomiconComplete) == 1;
    }

    public static ChallengePhotographContent CheckChallengePhotoRequirements(List<Cryptid> photoSubjects)
    {
        if (!ChallengeModeUnlocked()) { return ChallengePhotographContent.None; }

        foreach (Cryptid cryptid in photoSubjects)
        {
            //check for two possible jackalope related challenges
            if (cryptid is Jackalope)
            {
                Jackalope jackalope = cryptid as Jackalope;
                //jackalope sleeping
                if (jackalope.currentState == Jackalope.MoveState.sleep) {
                    return ChallengePhotographContent.SleepingJackalope; 
                }
                //mothman has this jackalope
                else if (jackalope.currentState == Jackalope.MoveState.caught)
                {
                    return ChallengePhotographContent.CarryingMothman;
                }
            }

            //check for sleeping tsuchinoko
            else if (cryptid is Tsuchinoko)
            {
                Tsuchinoko tsuchinoko = cryptid as Tsuchinoko;
                if (tsuchinoko.currentMovestate == Tsuchinoko.MoveState.Sleeping)
                {
                    return ChallengePhotographContent.SleepingTsuchinoko;
                }
            }

            //check for two possible frogman related challenges
            else if (cryptid is LovelandFrogman)
            {
                LovelandFrogman frogman = cryptid as LovelandFrogman;
                if (frogman.currentState == LovelandFrogman.MoveState.lilypadsit)
                {
                    //sitting on nessie
                    if (frogman.IsRidingNessie())
                    {
                        return ChallengePhotographContent.NessieWithFrogman;
                    }
                    //sitting on lilypad
                    else
                    {
                        return ChallengePhotographContent.LilypadFrogman;
                    }
                }
            }


            //check for bigfoot sitting
            else if (cryptid is Bigfoot)
            {
                Bigfoot bigfoot = cryptid as Bigfoot;
                if (bigfoot.currentState == Bigfoot.MoveState.sit)
                {
                    return ChallengePhotographContent.SittingBigfoot;
                }
            }

            //flatwoods doing peace
            else if (cryptid is FlatwoodsMonster)
            {
                FlatwoodsMonster flatwoods = cryptid as FlatwoodsMonster;
                if (flatwoods.currentState == FlatwoodsMonster.MoveState.pose)
                {
                    return ChallengePhotographContent.PeaceSignFlatwoods;
                }
            }

            //fresno dancing
            else if (cryptid is FresnoNightcrawler)
            {
                FresnoNightcrawler fresno = cryptid as FresnoNightcrawler;
                if (fresno.currentState == FresnoNightcrawler.MoveState.Dance)
                {
                    return ChallengePhotographContent.DancingFresno;
                }
            }
        }


        return ChallengePhotographContent.None;
    }
}