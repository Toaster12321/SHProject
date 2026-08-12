using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController instance;
    public static PlayerInput playerInput;
    public bool canMove = true; //can the player move
    public bool isDashing = false;
    public bool dashCoolingDown = false;
    public bool MenuOpenInput { get; private set; }
    public bool MenuCloseInput { get; private set; }

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float dashCooldown = 4f;
    [SerializeField] private float dashDuration = 6f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private AudioSource walking;
    [SerializeField] private AudioSource outOfBreath;
    private Animator playerAnimator;

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
    private float dashTime = 0f;
    private float lastDashTime = -Mathf.Infinity;

    [SerializeField] private InputActionReference moveInput; //references for input system controls
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private InputActionReference weaponSwitchInput;
    [SerializeField] private InputActionReference _menuOpenAction;
    [SerializeField] private InputActionReference _menuCloseAction;
    private InputAction sprintAction;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>(); 
        playerCamera = GetComponentInChildren<Camera>(); //assign references
        characterController = GetComponent<CharacterController>();
        sprintAction = playerInput.actions["Sprint"];

        if (instance == null)
            instance = this;
        Cursor.lockState = CursorLockMode.Locked;//lock and hide cursor
        Cursor.visible = false;
    }

    
    void Update()
    {
        if (canMove) //if we can move run each function per frame
        {   
            HandleMovementInput();
            HandleMouseLook();
            ApplyFinalMovement();
            footstepsWalking();
        }
        
        if(isDashing) //start dash timer
            dashTime += Time.deltaTime;

        if (Time.time >= lastDashTime + dashCooldown)//reset cooldown for dash
            dashCoolingDown = false;

        MenuOpenInput = _menuOpenAction.action.WasPressedThisFrame(); //assign inputs
        MenuCloseInput = _menuCloseAction.action.WasPressedThisFrame();
    }

    private void HandleMovementInput()
    {
        currentInput = (moveInput.action.ReadValue<Vector2>()).normalized * walkSpeed; //reads up,down,left,right movement for walk speed (normalize so diag movement isnt faster)
        if (sprintAction.IsPressed() && isDashing == false && !dashCoolingDown && currentInput != Vector2.zero)//while shift is held, dash cooldown is over and we are moving a direction
        {
            walking.pitch = 1.6f; //speed up playback if running
            walkSpeed *= 2; //press shift to double move speed
            isDashing = true;
        }

        if ((!sprintAction.IsPressed() || currentInput == Vector2.zero) && isDashing) 
        {
            walking.pitch = 0.8f; //reset pitch to normal walking playback
            dashTime = 0f; //reset dash time and move speed
            walkSpeed /= 2;
            isDashing = false;
        }
        else if (dashTime >= dashDuration)
        {
            outOfBreath.Play(); //play SFX
            walking.pitch = 0.8f;
            dashCoolingDown = true;
            dashTime = 0f; //reset dash time and move speed
            walkSpeed /= 2;
            lastDashTime = Time.time; //store last dashed for 4s cooldown
            isDashing = false;
        }

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

    private void footstepsWalking()
    {
        bool isMoving = currentInput != Vector2.zero; //bool for movement 

        if (isMoving && !walking.isPlaying) //play footsteps when walking and not dashing
            walking.Play();
        else if (!isMoving && walking.isPlaying)
            walking.Stop();
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
