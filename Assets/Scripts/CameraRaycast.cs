using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CameraRaycast : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f; //how far raycast stretches out
    private PlayerControls playerControls;
    private OpenCloseObject currentObject;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Interact.performed += Interact;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Interact.performed -= Interact;
        playerControls.Disable();
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitObject, interactDistance))
        {
            OpenCloseObject interactableObject = hitObject.collider.GetComponent<OpenCloseObject>(); //get an object that has the open close script

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

    private void Interact(InputAction.CallbackContext ctx)
    {
        currentObject?.Interact(); //call objects interact function if its not null
    }
}
