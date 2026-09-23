using UnityEngine;
using System.IO;

public class SaveManager
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerData;
        public InventorySaveData InventoryData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".sav";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        if (_saveData.InventoryData == null)
            _saveData.InventoryData = new InventorySaveData();

        GameManager.Instance.FirstPersonController.Save(ref _saveData.PlayerData);
        GameManager.Instance.InventoryManager.Save(ref _saveData.InventoryData);
    }

    public static void Load()
    {
        string SaveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(SaveContent);
        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        GameManager.Instance.FirstPersonController.Load(_saveData.PlayerData);
        GameManager.Instance.InventoryManager.Load(_saveData.InventoryData);
    }
}
