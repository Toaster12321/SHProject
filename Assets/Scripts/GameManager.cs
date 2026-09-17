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

    private void Update()
    {
        if(Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            SaveManager.Save();
        }

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SaveManager.Load();
        }
    }
}
