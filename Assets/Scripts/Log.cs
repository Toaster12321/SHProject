using TMPro;
using UnityEngine;

public class Log : MonoBehaviour
{
    [SerializeField] private GameObject logNote;
    [SerializeField] private TMP_Text logTextbox;
    [SerializeField] private string logText;
    [SerializeField] private GameObject logscreenCloseButton;

    public void ShowLog()
    {
        logNote.SetActive(true);
        logTextbox.text = logText;
        logscreenCloseButton.SetActive(false);
    }
}