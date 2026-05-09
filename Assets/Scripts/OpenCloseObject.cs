using GLTFast.Schema;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseObject : MonoBehaviour
{
    [SerializeField] private string screenText;
    [SerializeField] private Animator objectAnimator;

    private bool isOpen = false;

    public string GetInteractText()
    {
        return screenText;
    }

    public void OpenClose()
    {
        isOpen = !isOpen; //set is open to the opposite of what it previously was (always started closed -> false)
        objectAnimator.SetBool("openObject", isOpen); //play close or open animation based on the bool
    }
}
