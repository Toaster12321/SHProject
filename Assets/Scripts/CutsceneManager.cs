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

    public bool cutsceneActive = false;


    [Header("Note UI Elements")]
    [SerializeField] private GameObject noteUI;

    private float charsPerSecond = 30f;

    private List<DialogueLines> currentLines;
    private int currentLineIndex;
    private Coroutine typingRoutine;
    private InputAction continueAction;
    private InputAction clickAction;
    private bool textActive = false;
    private bool showingNote = false;
    private string fullLineText;


    private void Start()
    {
        continueAction = FirstPersonController.playerInput.actions["Continue"];
        clickAction = FirstPersonController.playerInput.actions["Click"];
        dialogueUI.SetActive(false); //hide UI
        noteUI.SetActive(false);
    }

    private void Update()
    {
        if (!cutsceneActive)
        {
            continueAction.performed -= ContinueDialogue;
            clickAction.performed -= ContinueDialogue;
        }
        else
        {
            if (showingNote)
                return;

            continueAction.performed += ContinueDialogue;
            clickAction.performed += ContinueDialogue;
        }

        if (showingNote)
        {
            continueAction.performed += HideNoteUI;
            clickAction.performed += HideNoteUI;
        }
        else if(!showingNote)
        {
            continueAction.performed -= HideNoteUI;
            clickAction.performed -= HideNoteUI;
        }
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

    public void ShowNoteUI()
    {
        noteUI.SetActive(true);
        showingNote = true;
        PauseManager.instance.PauseDuringText();
    }

    public void HideNoteUI(InputAction.CallbackContext ctx)
    {
        noteUI.SetActive(false);
        showingNote = false;
        PauseManager.instance.UnpauseDuringText();
    }
}
