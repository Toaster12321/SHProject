using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private GameObject interfaceScreen;
    [SerializeField] private GameObject cameraScreen;
    [SerializeField] private GameObject cameraCloseButton;
    [SerializeField] private GameObject cameraRender;
    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    [SerializeField] private GameObject logScreen;
    [SerializeField] private GameObject logsCloseButton;
    [SerializeField] private GameObject logNote;
    [SerializeField] private TMP_Text logTextbox;
    [SerializeField] private TMP_Text statusTextbox;
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private RenderTexture cameraFeed;
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
        interfaceScreen.SetActive(false);
        cameraScreen.SetActive(false);
        logScreen.SetActive(false);
        logNote.SetActive(false);
        camera1.enabled = false;
        camera2.enabled = false;
        computerCamera.enabled = false;
        compRenderer.enabled = false;
        logTextbox.text = "";

        cameraDisplay.texture = cameraFeed;
        camera1.targetTexture = cameraFeed;
        camera2.targetTexture = cameraFeed;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        compRenderer.enabled = true;
        computerCanvas.SetActive(true);
        passwordScreen.SetActive(false);
        interfaceScreen.SetActive(true);
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
        cameraScreen.SetActive(false);
        logScreen.SetActive(false);
        computerCamera.enabled = false;
        playerCamera.enabled = true;
        mainCanvas.enabled = true;
        camera1.enabled = false;
        camera2.enabled = false;
    }

    public void OnLogsButtonClicked()
    {
        logScreen.SetActive(true);
        interfaceScreen.SetActive(false);
    }

    public void OnCamsButtonClicked()
    {
        cameraScreen.SetActive(true);
        interfaceScreen.SetActive(false);
    }

    public void OnCam1Clicked()
    {
        computerCamera.enabled = true;
        camera1.enabled = true;
        camera2.enabled = false;
        cameraCloseButton.SetActive(false);
        cameraRender.SetActive(true);
    }

    public void OnCam2Clicked()
    {
        computerCamera.enabled = true;
        camera2.enabled = true;
        camera1.enabled = false;
        cameraCloseButton.SetActive(false);
        cameraRender.SetActive(true);
    }


    public void HideLog()
    {
        logNote.SetActive(false);
        logsCloseButton.SetActive(true);
        logTextbox.text = "";
    }

    public void HideSecurityCamera()
    {
        camera1.enabled = false;
        camera2.enabled = false;
        computerCamera.enabled = true;
        cameraRender.SetActive(false);
        cameraCloseButton.SetActive(true);
    }


    public void HideCameraScreen()
    {
        cameraScreen.SetActive(false);
        interfaceScreen.SetActive(true);
    }

    public void HideLogsScreen()
    {
        logScreen.SetActive(false);
        interfaceScreen.SetActive(true);
    }


    private void OnEnable() //read password input when user submits with enter
    {
        passwordTextbox.onSubmit.AddListener(ValidateInput);
        closeAction.performed += ctx => CloseScreen();
    }

    private void OnDisable()
    {
        passwordTextbox.onSubmit.RemoveListener(ValidateInput);
        closeAction.performed -= ctx => CloseScreen();
    }

}
