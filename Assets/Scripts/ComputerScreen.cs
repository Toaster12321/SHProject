using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ComputerScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingTextBox;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera computerCamera;
    [SerializeField] private GameObject computerCanvas;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject passwordScreen;
    [SerializeField] private TMP_Text statusTextbox;
    private TMP_InputField passwordTextbox;
    private InputAction closeAction;

    private string loadingDots = "...";
    private float textSpeed = 1.2f;
    private MeshRenderer compRenderer;
    private bool passwordEntered;

    private const int LOADING_LOOP_TIMES = 3;

    private void Awake()
    {
        passwordTextbox = GetComponentInChildren<TMP_InputField>(true);
        compRenderer = GetComponent<MeshRenderer>();
        closeAction = FirstPersonController.playerInput.actions["CloseMenu"];
    }

    private void Start()
    {
        computerCanvas.SetActive(false);
        passwordScreen.SetActive(false);
        computerCamera.enabled = false;
        compRenderer.enabled = false;
    }

    private void Update()
    {
        if (computerCamera.enabled)
            closeAction.performed += ctx => CloseScreen();
        else
            closeAction.performed -= ctx => CloseScreen();

    }
    public void InteractScreen()
    {
        weaponHolder.SetActive(false);
        mainCanvas.enabled = false;
        pauseManager.PauseDuringText();
        playerCamera.enabled = false;
        computerCamera.enabled = true;
        StopAllCoroutines();
        if (!passwordEntered) 
            StartCoroutine(StartLoading());
        else
            StartCoroutine(ShowInterface());
    }

    private IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(1f);
        compRenderer.enabled = true;
        computerCanvas.SetActive(true);
        loadingScreen.SetActive(true);
        loadingTextBox.text = "";


        for (int i = 0; i < LOADING_LOOP_TIMES; i++)
        {
            foreach (char character in loadingDots)
            {
                loadingTextBox.text += character;
                yield return new WaitForSeconds(1f / textSpeed);
                if (loadingTextBox.text == "...")
                    loadingTextBox.text = "";
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loadingScreen.SetActive(false);
        passwordScreen.SetActive(true);
        yield return null;
        passwordTextbox.ActivateInputField();

    }

    public void ValidateInput(string passwordText) //password checker
    {
        if (passwordText == "test")
        {
            statusTextbox.text = "ACCESS GRANTED";
            statusTextbox.color = Color.green;
            passwordEntered = true;
            StartCoroutine(ShowInterface());
        }
        else
        {
            statusTextbox.text = "ACCESS DENIED";
            statusTextbox.color = Color.red;
        }
    }

    private IEnumerator ShowInterface()
    {
        yield return new WaitForSeconds(1f);
        passwordScreen.SetActive(false);

        for (int i = 0; i < LOADING_LOOP_TIMES; i++)
        {
            foreach (char character in loadingDots)
            {
                loadingTextBox.text += character;
                yield return new WaitForSeconds(1f / textSpeed);
                if (loadingTextBox.text == "...")
                    loadingTextBox.text = "";
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loadingScreen.SetActive(false);
        passwordScreen.SetActive(true);
        yield return null;
        passwordTextbox.ActivateInputField();

    }

    private void CloseScreen()
    {
        pauseManager.UnpauseDuringText();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weaponHolder.SetActive(true);
        passwordScreen.SetActive(false);
        loadingScreen.SetActive(false);
        computerCanvas.SetActive(false);
        computerCamera.enabled = false;
        compRenderer.enabled = false;
        playerCamera.enabled = true;
        mainCanvas.enabled = true;
    }

    private void OnEnable() //read password input when user submits with enter
    {
        passwordTextbox.onSubmit.AddListener(ValidateInput);
    }

    private void OnDisable()
    {
        passwordTextbox.onSubmit.RemoveListener(ValidateInput);
    }
}
