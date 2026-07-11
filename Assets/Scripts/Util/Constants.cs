using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

    //main gameplay ui text
    public static string LeaveCoursePrompt = "Return to the lab with your photos and end the course?";
    public static string OutOfFilmPrompt = "You've run out of film. End course?";
    public static string PhotosRemaining = "Photos Remaining: ";
    public static string PhotoHelpPrompt1 = "Hold {0} to ready the camera; ";
    public static string PhotoHelpPrompt2 = "press {0} to take a picture";
    public static string ObjectHelpPrompt = "Press {0} to throw a carrot";

    //inputs (button names)
    public const string TakePicture = "TakePicture";
    public const string ReadyCamera = "ReadyCamera";
    public const string ThrowObject = "ThrowObject";
    public const string Pause = "Pause";
    public const string Submit = "Submit";
    public const string Cancel = "Cancel";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string CrouchButton = "CrouchButton";
    public const string RunButton = "RunButton";
    public const string JumpButton = "Jump";
    public const string LTAxis = "9th Axis";
    public const string RTAxis = "10th Axis";
    public const string LTAxisMac = "5th Axis";
    public const string RTAxisMac = "6th Axis";


    //tags
    public const string PlayerTag = "Player";
    public const string CryptidTag = "Cryptid";
    public const string ShoreTag = "Shore";
    public const string WaterTag = "Water";
    public const string AvoidTag = "AvoidanceZone";
    public const string DestroyTag = "DestroyZone";
    public const string CarrotTag = "Carrot";
    public const string TerrainTag = "lvl";
    public const string OptionalTag = "Optional";
    public const string DropShadowTag = "DropShadow";
    public const string BGMTag = "BGM";
    public const string CryptidContainerTag = "CryptidContainer";
    public const string SplashFXTag = "SplashFX";

    //navmesh areas
    public const string NavMeshLargeCryptids = "LargeCryptids";
    public const string NavMeshSmallCryptidsOnly = "SmallCryptidsOnly";
    public const string NavMeshWater = "Water";


    //grading page text
    public static string ShowTed = "Select pictures to show to Ted";
    public static string ConfirmSelectPhoto = "Select this picture?";
    public static string ProceedPhotos = "Proceed with {0} photos selected?";

    //used for insterting values into strings
    public static string ParameterSTR = "{0}";

    //ted dialogue
    public static string WelcomeBack = "Oh, welcome back! Shall we take a look at those \nphotos? I'm excited to see what you've got.";
    public static string DoneGrading = "Well done, thank you as always for sharing your \nphotos with me.";
    public static string NoSubject = "Oh, it's, uh... what is this supposed to be?";
    public static string NoPoints = "...Sorry, that's worth 0 points.";
    public static string FoundSubject = "Oh, it's {0}!";
    public static string FoundPoints = "That's worth {0} points.";
    public static string FacingAway = "Oh, but it's facing away from the camera. \nI'm afraid that's minus {0} points.";
    public static string GoodVisibility = "And you can see it perfectly clearly! \n100% visibility, well done.";
    public static string OKVisibility = "You can see most of it clearly, but some of it is \nblocked by obstacles in the shot.";
    public static string PoorVisibility = "You can't see it very clearly at all. A lot of it is blocked \nby obstacles in the shot.";
    public static string GoodCenter = "Very nice! It's perfectly centered in the shot. \nThat's plus {0} points!";
    public static string OKCenter = "It's a little off center, but not too bad. \nThat's plus {0} points.";
    public static string PoorCenter = "The cryptid is nowhere near the center of the shot. \nTry to frame it better next time. {0} points.";
    public static string GoodDistance = "Oho! This is a pretty close up shot! Well done, \nthat's plus {0} points.";
    public static string OKDistance = "It's a pretty good distance away. \nThat's plus {0} points.";
    public static string PoorDistance = "Hmm... it's pretty far away. I know it's asking a lot, but \nsee if you can get closer next time. {0} points.";
    public static string SpecialPose = "Ah, and a very nice pose, too. \nThat's {0} bonus points.";
    public static string DoubleCryptid = "Oh! And there's more than one cryptid in the shot! \nExcellent work, that's worth double.";
    public static string EndGrading = "Let's see, overall I give this photo... {0} points.";
    public static string ExistingEntry = "Oh, it looks like you already have an entry for this \ncryptid. Your last photo was worth {0} points.";
    public static string OverwritePrompt = "Which photo would you like to keep for the \nCryptid-Nomicon?";
    public static string NewPhotoSelected = "Got it, I'll use your new photo then.";
    public static string OldPhotoSelected = "Sure thing, we'll keep your old photo then.";

    //button options during grading
    public static string KeepPreviousText = "Keep Previous Photo";
    public static string UseNewText = "Use New Photo";

    //cabin ui
    public static string Score = "Score: ";
    public static string FinalScore = "Final Score: ";

    //ted intro
    public static string[] TedIntro =
    {
        "Ah, you must be that photographer I hired! Welcome, \nwelcome, do come in.",
        "I'm Dr. Ted Krupp, Ph.D. But you can just call me Ted. \nOr Dr. Ted, if you're feeling fancy.",
        "This cabin is my base for conducting my research. \nResearch on what? Why, cryptids of course!",
        "Shouldn't you already know that? I'm sure it was in the \nemail I sent. ...You are the photographer I hired, right?",
        "Well, it doesn't matter who you are, as long as you can \ntake pictures. That's your job, to take pictures of \ncryptids for me.",
        "I'll be using your photos for my upcoming book, the \nCryptid-Nomicon. Not the Cryptonomicon. That's \nsomething else.",
        "You shouldn't have much trouble finding cryptids in \nthe woods nearby. They seem to like it around here.",
        "When you're ready to head out, just select \"Embark\" \nor click on the camera. But no rush of course.",

        //not really part of the intro, just a sample line to give approximate max length
        //"What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over"
    };

    //sprites for ted intro - here out of convenience
    public static TedMoods[] IntroMoods =
    {
        TedMoods.Surprised, TedMoods.LookUp, TedMoods.Satisfied, TedMoods.LeanForward, TedMoods.Default, TedMoods.Uncertain, TedMoods.Pleased, TedMoods.Default
    };

    //player pref keys - used for saving data
    public static string FirstPlay = "FirstPlay"; //false if this is not the first time booting up the game
    public static string TextSpeed = "TextSpeed";
    public static string SFXVolume = "SFXVolume";
    public static string BGMVolume = "BGMVolume";
    public static string ErrorTrackingConsent = "ErrorTracking";
    public static string TedTalkIndex = "TedTalkIndex";
    public static string HasSeenAllTedDialogue = "HasTalkedToTed";
    //used for image data- what size are these images
    public static int CameraWidth = 1920;
    public static int CameraHeight = 1080;
    //camera settings
    public static string CameraSensitivity;
    public static string CameraFOV;
    public static string PostProcessingEnabled;

    //cryptid names
    public const string Tsuchinoko = "Tsuchinoko";
    public const string Nessie = "Loch Ness Monster";
    public const string Frogman = "Loveland Frogman";
    public const string Jackalope = "Jackalope";
    public const string Fresno = "Fresno Nightcrawler";
    public const string Flatwoods = "Flatwoods Monster";
    public const string Bigfoot = "Bigfoot";
    public const string Mothman = "Mothman";

    //holds description text for cryptids in the cryptid-nomicon
    public static Dictionary<string, string> tedsWriting = new Dictionary<string, string>()
    {
        {Tsuchinoko,"Tsuchinoko, the fat snake cryptid, is identifiable by his distinct round belly. He is Japanese in origin and rumoured to have a propensity for telling lies and drinking alcohol." },
        {Nessie,"Affectionately dubbed \"Nessie\" by her fans, the Loch Ness Monster is a famous and beloved aquatic creature. In 1933 she was put under an order of protection by the Scottish Government; serious legal consequences await anyone who would do her harm." },
        {Frogman,"These large bipedal frogs are named after Loveland Ohio, the region where they were first spotted lurking out of the water. Some reports claim they wield magic wands that shoot lightning, but that's just silly." },
        {Fresno, "Originally captured on security footage in Fresno, California, Fresno Nightcrawlers are recognizeable by their elongated legs and strange, meandering gait. These suspected extraterrestials seem to come in pairs, not unlike pants." },
        {Jackalope, "Two parts jackrabbit and one part antelope, the Jackalope may seem cute and friendly but is actually known for its incredible strength. Some hunters have been known to go as far as wearing stovepipes on their legs to avoid being impaled by its fearsome horns." },
        {Bigfoot, "The man, the myth, the legend, Bigfoot himself. Also sometimes known as Sasquatch, this large, hairy, humanoid creature is known to romp about in forests in the Pacific Northwest of North America. Despite his reputation as an elusive loner, he's the single most commonly sighted cryptid." },
        {Flatwoods, "Known for her signature skirt and hood, this stylish creature was first spotted in the town of Flatwoods in 1952 after a mysterious bright light crossed the night sky. Though there were several additional reports shortly after her first appearance, there have been no new sightings since the 50s." },
        {Mothman, "Despite the name, Mothman is usually described as a large birdlike creature. Originally spotted in Point Pleasant in 1966, he's alleged to be connected to the collapse of the Silver Bridge and the disappearance of several pets, but he was never found guilty in a court of law." }
    };

    public static readonly string[] DemoLockedCryptids = { Bigfoot, Mothman }; //cryptids that arent in the demo

    public const string defaultEntry = "Take a photo of this cryptid and submit it to Ted to learn more about them.";
    public const string demoLockedEntry = "This cryptid will be available in the full release of the game.";

    //feedback form
    public const string FeedbackFormURL = "https://forms.gle/TTNtVqNbBSVfdTrU7";

    public const int SteamAppID = 3772620;

}
