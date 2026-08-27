using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRaycast : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f; //how far raycast stretches out
    [SerializeField] private LayerMask interactLayer;
    private InputAction interactAction;
    private InteractObject currentObject;
    private WeaponSwitcher weaponSwitcher;


    private void Awake()
    {
        weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        interactAction = FirstPersonController.playerInput.actions["Interact"];
        AssignInput();
    }

    private void AssignInput()
    {
        interactAction.performed += ctx => Interact();
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)); //raycast is casted at center of viewport, (0.5,0.5) = 50%x and 50%y

        if (Physics.Raycast(ray, out RaycastHit hitObject, interactDistance, interactLayer))
        {
            InteractObject interactableObject = hitObject.collider.GetComponentInParent<InteractObject>(); //get an object that has the open close script

            if (interactableObject != null)
            {
                currentObject = interactableObject; //set it the current object
  
                UIManager.interactText = interactableObject.GetInteractText();//turn on UI and show text
                UIManager.uiActive = true;

                return;
            }
        }

        currentObject = null; //reset object and UI after ray leaves the object
        UIManager.uiActive = false;
    }

    private void Interact()
    {
        if (currentObject == null)
            return;

        switch(currentObject.objectType)
        {
            case InteractObject.InteractObjectType.Door:
                currentObject.OpenClose(); //call objects open close function if its not null
                break;

            case InteractObject.InteractObjectType.Switch:
                currentObject.TurnOnOff();
                break;

            case InteractObject.InteractObjectType.PickableItem:
                currentObject.AddItemToInventory();
                weaponSwitcher.EquipWeapon();
                break;

            case InteractObject.InteractObjectType.AmmoRefill:
                currentObject.AmmoRefill();
                break;

            case InteractObject.InteractObjectType.Dialogue:
                currentObject.StartDialogue();
                break;

            default:
                break;
        }
    }

}
