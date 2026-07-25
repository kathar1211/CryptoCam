using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityFileBrowser;

//contains only the aspects of a player's photo that need to be saved to file
//very similar to CryptidNomicon's PageContent struct
[System.Serializable]
public struct Photodata
{
    public int photoScore;
    public byte[] imageData;
    public string name;
}

//structure for gallery photos
[System.Serializable]
public struct PhotoOnlyData
{
    public byte[] imageData;
}

//structure for challenge photos
[System.Serializable]
public struct ChallengePhotoData
{
    public int photoScore;
    public byte[] imageData;
    public ChallengePhotographContent challenge;
}


[System.Serializable]
public class Save
{
    public List<Photodata> photos;
    public List<PhotoOnlyData> galleryPhotos;
    public List<ChallengePhotoData> challengePhotos;

    //create save data based on contents of the crytpidnomicon
    public void SaveFromCryptidNomicon(Dictionary<string, PageContent> contents)
    {
        if (contents == null) { return; }
        photos = new List<Photodata>();
        foreach (KeyValuePair<string, PageContent> content in contents)
        {
            if (content.Value == null) { continue; }
            Photodata data = new Photodata();
            data.photoScore = content.Value.photoScore;
            data.imageData = content.Value.image.GetRawTextureData();
            data.name = content.Value.name;
            photos.Add(data);
        }
    }

    public void SaveGalleryPhotos(List<Texture2D> gallery)
    {
        if (gallery == null) { return; }

        galleryPhotos = new List<PhotoOnlyData>();
        foreach (Texture2D photo in gallery)
        {
            PhotoOnlyData data = new PhotoOnlyData();
            data.imageData = photo.GetRawTextureData();
            galleryPhotos.Add(data);
        }
    }

    public void SaveChallengePhotos()
    {

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

    public static void DeleteSaveData()
    {
        File.Delete(Application.persistentDataPath + "/photos.save");
    }

    public static Save LoadSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/photos.save", FileMode.Open);
        Save loadedSave = (Save)bf.Deserialize(file);
        file.Close();

        return loadedSave;
    }

    //returns a cryptid nomicon spread created from the save file
    //https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity
    public static Dictionary<string, PageContent> LoadCryptidNomicon()
    {
        Dictionary<string, PageContent> loadedContents = new Dictionary<string, PageContent>();
        Save loadedSave = LoadSave();

        //start with a blank save and fill in data as we find it. keeps order consistent and allows for empty entries
        loadedContents = new Dictionary<string, PageContent>
         {
                { Constants.Jackalope, null },
                { Constants.Tsuchinoko, null },
                { Constants.Nessie, null },
                { Constants.Frogman, null },
                { Constants.Fresno, null },
                { Constants.Flatwoods, null },
                { Constants.Bigfoot, null },
                { Constants.Mothman, null },
        };

        foreach (Photodata photo in loadedSave.photos)
        {
            PageContent content = new PageContent();
            content.image = new Texture2D(Constants.CameraWidth, Constants.CameraHeight, TextureFormat.RGB24, true);
            content.image.LoadRawTextureData(photo.imageData);
            content.image.Apply();
            content.name = photo.name;
            content.photoScore = photo.photoScore;
            if (Constants.tedsWriting.ContainsKey(photo.name)) { content.flavorText = Constants.tedsWriting[photo.name]; }

            loadedContents[content.name] = content;
        }

        return loadedContents;
    }

    public static List<Texture2D> LoadGalleryPhotos()
    {
        List<Texture2D> gallery = new List<Texture2D>();
        Save loadedSave = Save.LoadSave();

        if (loadedSave != null && loadedSave.galleryPhotos != null)
        {
            foreach (PhotoOnlyData photo in loadedSave.galleryPhotos)
            {
                Texture2D constructedPhoto = new Texture2D(Constants.CameraWidth, Constants.CameraHeight, TextureFormat.RGB24, true);
                constructedPhoto.LoadRawTextureData(photo.imageData);
                constructedPhoto.Apply();
                gallery.Add(constructedPhoto);
            }
        }

        return gallery;
    }

    public static void SavePhotoToPNG(Texture2D photo)
    {
        byte[] png = photo.EncodeToPNG();
        string path = FileBrowser.SaveFileBrowser("", "", "CryptidCamPhoto", new string[] { ".png" });
        System.IO.File.WriteAllBytes(path, png);
    }
}
