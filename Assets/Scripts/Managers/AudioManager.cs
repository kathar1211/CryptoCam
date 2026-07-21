using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public const float DEFAULT_VOL = -20f;

    /*List<AudioSource> AllSFX = new List<AudioSource>();
    List<AudioSource> AllBGM = new List<AudioSource>();*/
    float SFXVol = DEFAULT_VOL;
    float BGMVol = DEFAULT_VOL;

    public AudioMixer Mixer;
    private const string SFX_MIXER_GROUP = "SFX_VOL";
    private const string BGM_MIXER_GROUP = "BGM_VOL";
    private const string NON_UI_SFX_PITCH = "INGAME_SFX_PITCH";

    // Use this for initialization
    void Start () {

        LoadBGMVolume();
        LoadSFXVolume();

        UpdateSFXVolume(SFXVol);
        UpdateBGMVolume(BGMVol);
	}

    //get player pref values for bgm settings, if they exist
    void LoadBGMVolume()
    {
        if (PlayerPrefs.HasKey(Constants.BGMVolume))
        {
            BGMVol = PlayerPrefs.GetFloat(Constants.BGMVolume);
        }
    }

    //load saved player pref value for sfx volume, if it exists
    void LoadSFXVolume()
    {
        if (PlayerPrefs.HasKey(Constants.SFXVolume))
        {
            SFXVol = PlayerPrefs.GetFloat(Constants.SFXVolume);
        }
    }

    //set volume for sound effects and save to playerprefs
    //minimum is -80 db, max is 0
    public void UpdateSFXVolume(float volume)
    {
        if (volume < -80 || volume > 0) { return; }
        SFXVol = volume;
        PlayerPrefs.SetFloat(Constants.SFXVolume, volume);
        Mixer.SetFloat(SFX_MIXER_GROUP, SFXVol);
    }

    //set volume for bgm. works same as sfx
    public void UpdateBGMVolume(float volume)
    {
        if (volume < -80 || volume > 0) { return; }
        BGMVol = volume;
        PlayerPrefs.SetFloat(Constants.BGMVolume, volume);
        Mixer.SetFloat(BGM_MIXER_GROUP, BGMVol);
    }

    //return the current setting for sfx volume
    public float getSFXVolume()
    {
        LoadSFXVolume();
        return SFXVol;
    }

    //return the current setting for bgm volume
    public float getBGMVolume()
    {
        LoadBGMVolume();
        return BGMVol;
    }

}
