using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CutsceneActivator : MonoBehaviour
{
    [SerializeField] private List<DialogueLines> dialogueLines;
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private bool playOnlyOnce = true;

    private bool hasPlayedOnce = false;

    private void OnTriggerEnter(Collider other)
    {
        Player _player = other.GetComponent<Player>();
        if (_player == null)
            return;

        if (playOnlyOnce && hasPlayedOnce)
            return;

        if (cutsceneManager != null )
        {
            hasPlayedOnce = true;
            cutsceneManager.StartCutscene(dialogueLines);
        }
    }
}
