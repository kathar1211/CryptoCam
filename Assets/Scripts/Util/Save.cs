using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//conatins only the aspects of a player's photo that need to be saved to file
//very similar to CryptidNomicon's PageContent struct
[System.Serializable]
public struct Photodata
{
    public int photoScore;
    public byte[] imageData;
    public string name;
}

[System.Serializable]
public class Save
{
    public List<Photodata> photos;

    //create save data based on contents of the crytpidnomicon
    public void CreateSaveFromCryptidNomicon(Dictionary<string, PageContent> contents)
    {
        photos = new List<Photodata>();
        foreach (KeyValuePair<string, PageContent> content in contents)
        {
            Photodata data = new Photodata();
            data.photoScore = content.Value.photoScore;
            data.imageData = content.Value.image.GetRawTextureData();
            data.name = content.Value.name;
            photos.Add(data);
        }
    }

    //write this save to file
    //https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/photos.save");
        bf.Serialize(file, this);
        file.Close();
    }

    //returns true if a save file is found
    public static bool SaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + "/photos.save");
    }

    //returns a cryptid nomicon spread created from the save file
    //https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity
    public static Dictionary<string, PageContent> LoadCryptidNomicon()
    {
        Dictionary<string, PageContent> loadedContents = new Dictionary<string, PageContent>();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/photos.save", FileMode.Open);
        Save loadedSave = (Save)bf.Deserialize(file);
        file.Close();

        foreach (Photodata photo in loadedSave.photos)
        {
            PageContent content = new PageContent();
            if (PlayerPrefs.HasKey(Constants.CameraHeight) && PlayerPrefs.HasKey(Constants.CameraWidth))
            {
                content.image = new Texture2D(PlayerPrefs.GetInt(Constants.CameraWidth), PlayerPrefs.GetInt(Constants.CameraHeight), TextureFormat.RGB24, true);
                content.image.LoadRawTextureData(photo.imageData);
                content.image.Apply();
            }
            //fallback case if we don't know what size these images should be
            else
            {
               // content.image = new Texture2D(800, 600, TextureFormat.RGB24, true);
               // content.image.LoadRawTextureData(photo.imageData);
            }
            content.name = photo.name;
            content.photoScore = photo.photoScore;
            if (Constants.tedsWriting.ContainsKey(photo.name)) { content.flavorText = Constants.tedsWriting[photo.name]; }

            loadedContents.Add(content.name, content);
        }

        return loadedContents;
    }
}
