using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static string interactText;
    public static bool uiActive;

    [SerializeField] GameObject interactTextBox;
    [SerializeField] GameObject interactionCrosshair;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame

    void Update()
    {
        if (uiActive == true)
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
    }
}
