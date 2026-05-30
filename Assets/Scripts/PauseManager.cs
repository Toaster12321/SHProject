using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool isPaused {  get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        FirstPersonController.instance.canMove = false;
        FirstPersonController.playerInput.SwitchCurrentActionMap("UI");
        print("pausing game");
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        FirstPersonController.instance.canMove = true;
        FirstPersonController.playerInput.SwitchCurrentActionMap("Player");
        print("unpausing game");
    }
}
