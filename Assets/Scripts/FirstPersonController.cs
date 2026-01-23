using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove {  get; private set; } = true; //can the player move

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(0.1f, 10)] private float lookSpeedX = 2.0f;//speed at which the camera moves in x and y directions
    [SerializeField, Range(0.1f, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float UpperLockLimit = 80.0f;//how many degrees we can look directly up/down before the camera stops moving
    [SerializeField, Range(1, 180)] private float LowerLockLimit = 80.0f;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;
    private Vector2 cameraInput;

    private float rotationX = 0;

    [SerializeField] private InputActionReference moveInput; //references for input system controls
    [SerializeField] private InputActionReference lookInput;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>(); //assign references
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;//lock and hide cursor
        Cursor.visible = false;
    }

    
    void Update()
    {
        //Vector2 mouseRelativeToCenter = new Vector2(

        //    Input.mousePosition.x - Screen.width / 2,

        //    Input.mousePosition.y - Screen.height / 2

        //);

        if (CanMove) //if we can move run each function per frame
        {   
            HandleMovementInput();
            HandleMouseLook();

            ApplyFinalMovement();
        }
        
    }

    private void HandleMovementInput()
    {
        currentInput = moveInput.action.ReadValue<Vector2>() * walkSpeed; //reads up,down,left,right movement for walk speed

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.forward * currentInput.y) + (transform.right * currentInput.x); //sets movedirection.y to our forward direction based on left/right movement and the right direction based on the up/down movement
        moveDirection.y = moveDirectionY;//assign new value of move direction
    }

    private void HandleMouseLook()
    {
        cameraInput = lookInput.action.ReadValue<Vector2>();//reads mouse movement as vector2 for camera speed

        rotationX -= cameraInput.y * lookSpeedY; //rotates in the x direction along the y axis
        rotationX = Mathf.Clamp(rotationX, -UpperLockLimit, LowerLockLimit); //locks rotation x to -80 and 80 degrees

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); //transform player camera for up and down direction
        transform.rotation *= Quaternion.Euler(0, cameraInput.x * lookSpeedX, 0); //transforms game object for left and right direction

    }

    private void ApplyFinalMovement()
    {
        if(!characterController.isGrounded) 
            moveDirection.y -= gravity * Time.deltaTime; //if we are in the sky appply gravity each frame

        characterController.Move(moveDirection * Time.deltaTime);//move the character controller in a direction each frame
    }

    private void OnEnable() //enable controls
    {
        moveInput.action.Enable();
        lookInput.action.Enable();
    }

    private void OnDisable() //disable controls
    {
        moveInput.action.Disable();
        lookInput.action.Disable();
    }
}
