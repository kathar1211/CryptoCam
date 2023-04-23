using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    List<AudioSource> AllSFX = new List<AudioSource>();
    List<AudioSource> AllBGM = new List<AudioSource>();
    float SFXVol = 1.0f;
    float BGMVol = .9f;

	// Use this for initialization
	void Start () {

        LoadBGMVolume();
        LoadSFXVolume();

        //grab all sounds

        //i dont think this will work- if we set sfx volume in one scene does it actually carry over?
        //consider switching to mixer groups
        AudioSource[] AllSounds = FindObjectsOfType<AudioSource>(); 
        //sort sounds into bgm and non bgm
        foreach (AudioSource sound in AllSounds)
        {
            if (sound.tag == Constants.BGMTag)
            {
                sound.volume = BGMVol;
                AllBGM.Add(sound);
            }
            else
            {
                sound.volume = SFXVol;
                AllSFX.Add(sound);
            }
        }
	}

    //get player pref values for bgm settings, if they exist
    void LoadBGMVolume()
    {
        if (PlayerPrefs.HasKey(Constants.BGMVolume))
        {
            BGMVol = PlayerPrefs.GetInt(Constants.BGMVolume) / 10.0f;
        }
    }

    //load saved player pref value for sfx volume, if it exists
    void LoadSFXVolume()
    {
        if (PlayerPrefs.HasKey(Constants.SFXVolume))
        {
            SFXVol = PlayerPrefs.GetInt(Constants.SFXVolume) / 10.0f;
        }
    }

    //set volume for sound effects and save to playerprefs
    //takes an int 0 to 10, maps to float from 0 to 1
    public void UpdateSFXVolume(int volume)
    {
        if (volume < 0 || volume > 10) { return; }
        SFXVol = volume / 10.0f;
        PlayerPrefs.SetInt(Constants.SFXVolume, volume);
        foreach (AudioSource sound in AllSFX) { sound.volume = SFXVol; }
    }

    //set volume for bgm. works same as sfx
    public void UpdateBGMVolume(int volume)
    {
        if (volume < 0 || volume > 10) { return; }
        BGMVol = volume / 10.0f;
        PlayerPrefs.SetInt(Constants.BGMVolume, volume);
        foreach (AudioSource sound in AllBGM) { sound.volume = BGMVol; }
    }

    //return the current setting for sfx volume
    public int getSFXVolume()
    {
        LoadSFXVolume();
        int value = (int)((SFXVol * 10)+.5f);
        return value;
    }

    //return the current setting for bgm volume
    public int getBGMVolume()
    {
        LoadBGMVolume();
        int value = (int)((BGMVol * 10f)+.5f);
        return value;
    }
}
