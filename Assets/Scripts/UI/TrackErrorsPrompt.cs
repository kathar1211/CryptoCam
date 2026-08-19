using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrackErrorsPrompt : InitializationPrompt
{
    public override void OnYesButtonClick()
    {
        SentryUtils.EnableSentry();
        PlayerPrefs.SetInt(Constants.ErrorTrackingConsent, 1);
        base.OnYesButtonClick();
    }

    public override void OnNoButtonClick()
    {
        SentryUtils.DisableSentry();
        PlayerPrefs.SetInt(Constants.ErrorTrackingConsent, 0);
        base.OnNoButtonClick();
    }
}
