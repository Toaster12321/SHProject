using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<string> killedEnemyList = new List<string>(); //stores which enemies have been killed in game
    public static EnemyManager instance;

    private void Awake()
    {
        GameManager.Instance.EnemyManager = this;

        if (instance == null)
            instance = this;
    }

    public void Save(ref EnemySaveData data)
    {
        data.killedEnemies.Clear(); //clear previous list then append to save data

        foreach(string enemyID in killedEnemyList)
        {
            data.killedEnemies.Add(enemyID);
        }
    }

    public void Load(EnemySaveData data)
    {
        foreach(string enemyID in data.killedEnemies) //store data from saved enemy list to in-game kill list
        {
            if (!killedEnemyList.Contains(enemyID))
                killedEnemyList.Add(enemyID);
        }
    }

}

[System.Serializable] //class that stores killedEnemies list to be translated to JSON
public class EnemySaveData 
{
    public List<string> killedEnemies = new List<string>();
}