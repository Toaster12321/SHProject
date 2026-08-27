using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static string interactText;
    public static bool uiActive;

    [SerializeField] GameObject interactTextBox;
    [SerializeField] GameObject interactionCrosshair;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] CutsceneManager cutsceneManager;

    public bool inventoryOpen = false;
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
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
        if (FirstPersonController.instance.MenuOpenInput) //if open menu is pressed pause the game
        {
            if (!PauseManager.instance.isPaused && (cutsceneManager.cutsceneActive != true))
            {
                PauseGame();
            }
        }
        else if (FirstPersonController.instance.MenuCloseInput)//if the menu close input is pressed and we are paused unpause the game
        {
            if (PauseManager.instance.isPaused)
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        inventoryOpen = true;
        PauseManager.instance.PauseGame();
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;//unlock and show cursor
        Cursor.visible = true;
    }

    public void UnpauseGame()
    {
        inventoryOpen = false;
        PauseManager.instance.UnpauseGame();
        InventoryManager.instance.OnInventoryClosed();
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;//lock and hide cursor
        Cursor.visible = false;
    }
}
