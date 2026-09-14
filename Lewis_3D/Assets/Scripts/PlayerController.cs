using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10f;
    public float jumpDetectDistance = 1.1f;

    PlayerInput playerInput;
    Rigidbody rb;
    Camera playerCam;

    Ray jumpRay;
    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializing component data
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Setting up new move Vector
        moveInput = Vector2.zero;

        playerCam = Camera.main;

        jumpRay = new Ray(transform.position, -transform.up);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = (moveInput.x * speed);
        tempMove.z = (moveInput.y * speed);

        rb.linearVelocity = tempMove;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump()
    {
        if(Physics.Raycast(jumpRay, jumpDetectDistance))
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }
}