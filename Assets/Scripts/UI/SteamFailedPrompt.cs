using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamFailedPrompt : InitializationPrompt
{
    public override void OnYesButtonClick()
    {
        base.OnYesButtonClick();
        SteamManager.RestartThroughSteam();
    }

}
