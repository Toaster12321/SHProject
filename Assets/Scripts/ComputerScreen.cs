using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingTextBox;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera computerCamera;
    [SerializeField] private GameObject computerScreen;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private Canvas mainCanvas;
    private string loadingDots = "...";
    private float textSpeed = 1f;
    private MeshRenderer compRenderer;

    private const int LOADING_LOOP_TIMES = 4;

    private void Awake()
    {
        compRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        compRenderer.enabled = false;
    }

    public void InteractScreen()
    {
        mainCanvas.enabled = false;
        compRenderer.enabled = true;
        pauseManager.PauseDuringText();
        computerScreen.SetActive(true);
        playerCamera.enabled = false;
        computerCamera.enabled = true;
        StopAllCoroutines();
        StartCoroutine(StartLoading());
    }

    private IEnumerator StartLoading()
    {
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
            
    }
}
