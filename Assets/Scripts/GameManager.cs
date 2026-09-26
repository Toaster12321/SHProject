using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return null;
            }

            if (instance == null )
            {
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }

#endif
            return instance;

        }
    }

    public FirstPersonController FirstPersonController { get; set; }
    public InventoryManager InventoryManager { get; set; }
    public EnemyManager EnemyManager { get; set; }


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    
    public void SaveGame() //BUTTON EVENT
    {
        SaveManager.Save();
    }   
    
    public void LoadGame() //BUTTON EVENT
    {
        SaveManager.Load();
    }
}
