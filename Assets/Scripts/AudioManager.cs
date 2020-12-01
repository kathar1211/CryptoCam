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
        //get prefab values for audio settings, if they exist
        if (PlayerPrefs.HasKey(Constants.BGMVolume)){
            BGMVol = PlayerPrefs.GetInt(Constants.BGMVolume) / 10.0f;
        }
        if (PlayerPrefs.HasKey(Constants.SFXVolume))
        {
            SFXVol = PlayerPrefs.GetInt(Constants.SFXVolume) / 10.0f;
        }

        //grab all sounds
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
        int value = (int)((SFXVol * 10)+.5f);
        return value;
    }

    //return the current setting for bgm volume
    public int getBGMVolume()
    {
        int value = (int)((BGMVol * 10f)+.5f);
        return value;
    }
}
