using UnityEngine;
using System.IO;

public class SaveManager
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".sav";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
    }

    private static void HandleSaveData()
    {

    }
}
