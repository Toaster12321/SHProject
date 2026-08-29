using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static string interactText;
    public static bool uiActive;

    [SerializeField] GameObject interactTextBox;
    [SerializeField] GameObject interactionCrosshair;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] CutsceneManager cutsceneManager;

    public bool inventoryOpen = false;
    private InputAction _menuOpenAction;
    private InputAction _menuCloseAction;
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        _menuOpenAction = FirstPersonController.playerInput.actions["OpenMenu"];
        _menuCloseAction = FirstPersonController.playerInput.actions["CloseMenu"];
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
        if (!PauseManager.instance.isPaused && (cutsceneManager.cutsceneActive != true))
            _menuOpenAction.performed += ctx => PauseGame();
        else
            _menuOpenAction.performed -= ctx => PauseGame();

        if (PauseManager.instance.isPaused)
            _menuCloseAction.performed += ctx => UnpauseGame();
        else
            _menuCloseAction.performed -= ctx => UnpauseGame();

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
