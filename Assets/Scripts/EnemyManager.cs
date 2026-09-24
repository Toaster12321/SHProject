using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<string> killedEnemyList = new List<string>();
    public static EnemyManager instance;

    private void Awake()
    {
        GameManager.Instance.EnemyManager = this;

        if (instance == null)
            instance = this;
    }

    public void Save(ref EnemySaveData data)
    {
        data.killedEnemies.Clear();

        foreach(string enemyID in killedEnemyList)
        {
            data.killedEnemies.Add(enemyID);
        }
    }

    public void Load(EnemySaveData data)
    {
        foreach(string enemyID in data.killedEnemies)
        {
            if (!killedEnemyList.Contains(enemyID))
                killedEnemyList.Add(enemyID);
        }
    }

}

[System.Serializable]
public class EnemySaveData
{
    public List<string> killedEnemies = new List<string>();
}