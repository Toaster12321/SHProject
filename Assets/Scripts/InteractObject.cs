using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractObject : MonoBehaviour
{
    public enum InteractObjectType
    {
        Door,
        Switch
    }

    [SerializeField] public InteractObjectType objectType;
    [SerializeField] private string screenText;
    [SerializeField] private Transform player;
    [SerializeField] private Animator objectAnimator;
    [SerializeField] private bool XComparison; //choose x or z based on arrows in editor on open/close object to compare player position to 
    [SerializeField] private bool ZComparison;

    [Header("Light Switch")]
    [SerializeField] private GameObject switchObject;
    [SerializeField] private Material emissiveMaterial;

    private bool isOpen = false;
    private bool isOn = false;

    public string GetInteractText()
    {
        return screenText;
    }

    public void OpenClose()
    {

        isOpen = !isOpen; //set is open to the opposite of what it previously was (always started closed -> false)

        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position); //gets the players transform in local space

        if (XComparison) //compare x axis
        {
            if ((localPlayerPos.x > 0 || objectAnimator.GetBool("openObject")) && objectAnimator.GetBool("openObjectInside") != true) //if the player is above 0 we are outside, if we triggered a bool play "close" in the animation state tree regardless of comparison
            {
                objectAnimator.SetBool("openObject", isOpen);
            }
            else //else we are inside so play inside open/close animations
            {
                objectAnimator.SetBool("openObjectInside", isOpen); //play close or open animation based on the bool
            }
        }
        else if (ZComparison) //compare z axis
        {
            if ((localPlayerPos.z > 0 || objectAnimator.GetBool("openObject")) && objectAnimator.GetBool("openObjectInside") != true)
            {
                objectAnimator.SetBool("openObject", isOpen);
            }
            else
            {
                objectAnimator.SetBool("openObjectInside", isOpen);
                
            }
        }
        else
        {
            objectAnimator.SetBool("openObject", isOpen);
        }    
    }

    public void TurnOnOff()
    {
        isOn = !isOn; //set is on to the opposite of what it previously was (always started off -> on(true))
        var light = switchObject.GetComponent<Light>();
        light.enabled = isOn;

        if (light.enabled)
            emissiveMaterial.EnableKeyword("_EMISSION");
        else
            emissiveMaterial.DisableKeyword("_EMISSION");
    }    
}