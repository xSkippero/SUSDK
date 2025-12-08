using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SUSDK.Domain;
using UnityEngine;

namespace SUSDK.Manager
{
  public class SaveManager<T>
  {

    public static string[] GetAllSaveNames()
    {
      if (!PlayerPrefs.HasKey("_saveList")) return null;
      var saveArray = JsonConvert.DeserializeObject<string[]>(PlayerPrefs.GetString("_saveList"));
      return (from saveName in saveArray let save = JsonConvert.DeserializeObject<GameSave<T>>(PlayerPrefs.GetString(saveName)) select saveName).ToArray();
    }

    [CanBeNull]
    public static GameSave<T> LoadGame(string saveName)
    {
      if(PlayerPrefs.HasKey(saveName)) {
        return JsonConvert.DeserializeObject<GameSave<T>>(PlayerPrefs.GetString(saveName));
      }

      Debug.Log("Save with Id \"" + saveName + "\" could not be loaded (not found)");
      return null;
    }

    public static void SaveGame(string saveName, GameSave<T> data)
    {
      PlayerPrefs.SetString(saveName, JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
      PlayerPrefs.Save();
      AddSaveToSaveList(saveName);
    }

    public static void DeleteGame(string saveGame)
    {
      PlayerPrefs.DeleteKey(saveGame);
      if (PlayerPrefs.HasKey("_saveList"))
      {
        var saveArray = JsonConvert.DeserializeObject<string[]>(PlayerPrefs.GetString("_saveList"));
        var saveList = new List<string>(saveArray);
        saveList.Remove(saveGame);
        PlayerPrefs.SetString("_saveList", JsonConvert.SerializeObject(saveList.ToArray(), Formatting.None,new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
      }
      PlayerPrefs.Save();
    }
    
    private static void AddSaveToSaveList(string saveGame)
    {
      if(PlayerPrefs.HasKey("_saveList")) {
        var saveArray = JsonConvert.DeserializeObject<string[]>(PlayerPrefs.GetString("_saveList"));
        var saveList = new List<string>(saveArray);
        if (!saveList.Contains(saveGame))
        {
          saveList.Add(saveGame);
        }
        PlayerPrefs.SetString("_saveList", JsonConvert.SerializeObject(saveList.ToArray(), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
      }
      else
      {
        PlayerPrefs.SetString("_saveList", JsonConvert.SerializeObject(new[] { saveGame }, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
      }
      PlayerPrefs.Save();
    }

  }
}
