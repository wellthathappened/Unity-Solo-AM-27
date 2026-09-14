using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10f;
    public float jumpDetectDistance = .1f;

    PlayerInput playerInput;
    Rigidbody2D rb;

    Ray2D jumpRay;
    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializing component data
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        // Setting up new move Vector
        moveInput = Vector2.zero;

        jumpRay = new Ray2D(transform.position, -transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        Vector2 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;

        rb.linearVelocity = tempMove;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
    }

    public void Jump()
    {
        if (Physics2D.Raycast(jumpRay.origin, jumpRay.direction, jumpDetectDistance))
            rb.AddForceY(jumpHeight,ForceMode2D.Impulse);
    }
}
