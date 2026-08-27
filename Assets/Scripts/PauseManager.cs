using Unity.VisualScripting;
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
        Time.timeScale = 0f; //set time to 0 = game time paused
        FirstPersonController.instance.canMove = false; //prevent all movement in input script
        FirstPersonController.playerInput.SwitchCurrentActionMap("UI"); //switch to UI controls
        AudioListener.pause = true; //pause all audio playing
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        FirstPersonController.instance.canMove = true;
        FirstPersonController.playerInput.SwitchCurrentActionMap("Player");//switch back to overworld controls
        AudioListener.pause = false;
    }

    public void PauseDuringText()
    {
        isPaused = true;
        FirstPersonController.instance.canMove = false; //prevent all movement in input script
        FirstPersonController.playerInput.SwitchCurrentActionMap("UI"); //switch to UI controls
    }


    public void UnpauseDuringText()
    {
        isPaused = false;
        FirstPersonController.instance.canMove = true; //prevent all movement in input script
        FirstPersonController.playerInput.SwitchCurrentActionMap("Player"); //switch to UI controls
    }
}
