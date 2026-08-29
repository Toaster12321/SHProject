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
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject passwordScreen;
    private TMP_InputField passwordTextbox;
    private InputAction closeAction;

    private string loadingDots = "...";
    private float textSpeed = 1.2f;
    private MeshRenderer compRenderer;

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
        mainCanvas.enabled = false;
        pauseManager.PauseDuringText();
        playerCamera.enabled = false;
        computerCamera.enabled = true;
        StopAllCoroutines();
        StartCoroutine(StartLoading());
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
        
        loadingScreen.SetActive(false);
        passwordScreen.SetActive(true);
        yield return null;
        passwordTextbox.ActivateInputField();
    }

    private void CloseScreen()
    {
        pauseManager.UnpauseDuringText();
        passwordScreen.SetActive(false);
        loadingScreen.SetActive(false);
        computerCanvas.SetActive(false);
        computerCamera.enabled = false;
        compRenderer.enabled = false;
        playerCamera.enabled = true;
        mainCanvas.enabled = true;
    }
}
