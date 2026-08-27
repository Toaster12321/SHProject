using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text characterNamePlate;
    [SerializeField] private TMP_Text dialogueText;

    private float charsPerSecond = 30f;

    private List<DialogueLines> currentLines;
    private int currentLineIndex;
    private Coroutine typingRoutine;

    private InputAction continueAction;
    private bool textActive = false;
    public bool cutsceneActive = false;
    private string fullLineText;


    private void Start()
    {
        continueAction = FirstPersonController.playerInput.actions["Continue"];
        dialogueUI.SetActive(false); //hide UI
    }

    private void Update()
    {
        if (!cutsceneActive)
            continueAction.performed -= ContinueDialogue;
        else
            continueAction.performed += ContinueDialogue;
    }

    public void StartCutscene(List<DialogueLines> dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Count == 0)
            return;

        PauseManager.instance.PauseDuringText();
        currentLines = dialogueLines;
        currentLineIndex = 0;
        cutsceneActive = true;

        dialogueUI.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLines currentDialogue = currentLines[currentLineIndex];

        characterNamePlate.text = currentDialogue.characterName.ToString();
        fullLineText = currentDialogue.lineText;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        textActive = true;
        dialogueText.text = "";

        foreach (char character in fullLineText)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1f / charsPerSecond);
        }

        textActive = false;
        typingRoutine = null;
    }

    private void ContinueDialogue(InputAction.CallbackContext ctx)
    {
        if(textActive)
        {
            FinishDialogueLine();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Count)
            EndCutscene();
        else
            ShowCurrentLine();

    }

    private void FinishDialogueLine()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        dialogueText.text = fullLineText;
        textActive = false;
        typingRoutine = null;
    }


    private void EndCutscene()
    {
        PauseManager.instance.UnpauseDuringText();
        cutsceneActive = false;
        currentLines = null;
        dialogueUI.SetActive(false);
    }
}
