using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

    //main gameplay ui text
    public static string LeaveCoursePrompt = "Return to the lab with your photos and end the course?";
    public static string PhotosRemaining = "Photos Remaining: ";
    public static string PhotoHelpPrompt = "Hold right click to ready the camera and press right click to take a picture";
    public static string ObjectHelpPrompt = "Press Q to throw a carrot";

    //inputs (button names)
    public static string TakePicture = "TakePicture";
    public static string ReadyCamera = "ReadyCamera";
    public static string ThrowObject = "ThrowObject";
    public static string Submit = "Submit";
    public static string Horizontal = "Horizontal";
    public static string Vertical = "Vertical";

    //grading page text
    public static string ShowTed = "Select pictures to show to Ted";
    public static string ConfirmSelectPhoto = "Select this picture?";
    public static string ProceedPhotos = "Proceed with {0} photos selected?";
    // public static string ProceedPart2 = " photos selected?";

    //used for insterting values into strings
    public static string ParameterSTR = "{0}";

    //ted dialogue
    public static string WelcomeBack = "Oh, welcome back! Shall we take a look at those photos? I'm excited to see what you've got.";
    public static string DoneGrading = "Well done, thank you as always for sharing your photos with me.";
    public static string NoSubject = "Oh, it's, uh... what is this supposed to be?";
    public static string NoPoints = "...Sorry, that's worth 0 points.";
    public static string FoundSubject = "Oh, it's {0}!";
    public static string FoundPoints = "That's worth {0} points.";
    public static string FacingAway = "Oh, but it's facing away from the camera. I'm afraid that's minus {0} points.";
    public static string GoodVisibility = "And you can see it perfectly clearly! 100% visibility, well done.";
    public static string OKVisibility = "You can see most of it clearly, but overall it's only about {0}% visible.";
    public static string PoorVisibility = "You can't see it very clearly at all. Only about {0}% of it isn't blocked by obstacles in the shot.";
    public static string GoodCenter = "Very nice! It's perfectly centered in the shot. That's plus {0} points!";
    public static string OKCenter = "It's a little off center, but not too bad. That's plus {0} points.";
    public static string PoorCenter = "The cryptid is nowhere near the center of the shot. Try to frame it better next time. {0} points.";
    public static string GoodDistance = "Oho! This is a pretty close up shot! Well done, that's plus {0} points.";
    public static string OKDistance = "It's a pretty good distance away. That's plus {0} points.";
    public static string PoorDistance = "Hmm... it's pretty far away. I know it's asking a lot, but see if you can get closer next time. {0} points.";
    public static string DoubleCryptid = "Oh! And there's more than one cryptid in the shot! Excellent work, that's worth double.";
    public static string EndGrading = "Let's see, overall I give this photo... {0} points.";

    //cabin ui
    public static string Score = "Score: ";
    public static string FinalScore = "Final Score: ";

    //ted intro
    public static string[] TedIntro =
    {
        "Ah, you must be that photographer I hired! Welcome, welcome, do come in.",
        "I'm Dr. Ted Krupp, PhD. But you can just call me Ted. Or Dr. Ted if you're feeling fancy.",
        "This cabin is my base for conducting research on cryptids. I'm compiling information for my upcoming book, the Cryptid-Nomicon.",
        "As you know, I'm in need of some photos for my book. That's your job, to take pictures of cryptids for me.",
        "You shouldn't have much trouble finding them in the woods nearby. They seem to like it around here.",
        "When you're ready to head out, just select \"Embark\". But no rush of course.",
    };

    //sprites for ted intro - here out of convenience
    public static TedMoods[] IntroMoods =
    {
        TedMoods.Surprised, TedMoods.LookUp, TedMoods.Satisfied, TedMoods.Uncertain, TedMoods.Pleased, TedMoods.Default
    };

    //player pref keys - used for saving data
    public static string FirstPlay = "FirstPlay";




}
