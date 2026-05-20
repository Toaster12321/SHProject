using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CameraRaycast : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f; //how far raycast stretches out
    [SerializeField] private LayerMask interactLayer;
    private PlayerControls playerControls;
    private InteractObject currentObject;

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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)); //raycast is casted at center of viewport, (0.5,0.5) = 50%x and 50%y

        if (Physics.Raycast(ray, out RaycastHit hitObject, interactDistance, interactLayer))
        {
            Debug.Log(hitObject.collider.name);
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

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (currentObject == null)
            return;

        if (currentObject.objectType == InteractObject.InteractObjectType.Door)
            currentObject.OpenClose(); //call objects open close function if its not null
        else if (currentObject.objectType == InteractObject.InteractObjectType.Switch)
            currentObject.TurnOnOff();
        else
            return;
    }
}
