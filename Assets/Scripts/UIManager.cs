using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static string interactText;
    public static bool uiActive;

    [SerializeField] GameObject interactTextBox;
    [SerializeField] GameObject interactionCrosshair;
    [SerializeField] GameObject pauseMenu;

    private void Start()
    {
        interactTextBox.SetActive(false);
        interactionCrosshair.SetActive(false);
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        //INTERACTIONS
        if (uiActive == true) //set in different scripts (camera raycast)
        {
            interactTextBox.SetActive(true); //show UI text
            interactionCrosshair.SetActive(true);
            interactTextBox.GetComponent<TMPro.TMP_Text>().text = interactText;
        }
        else
        {
            interactTextBox.SetActive(false);
            interactionCrosshair.SetActive(false);
        }

        //PAUSE MENU FUNCTIONS
        if (FirstPersonController.instance.MenuOpenInput)
        {
            if (!PauseManager.instance.isPaused)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        PauseManager.instance.PauseGame();
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;//unlock and show cursor
        Cursor.visible = true;
    }

    public void UnpauseGame()
    {
        PauseManager.instance.UnpauseGame();
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;//lock and hide cursor
        Cursor.visible = false;
    }
}
