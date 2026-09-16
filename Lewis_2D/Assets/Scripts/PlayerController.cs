using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10f;
    public float jumpBoost = 5f;
    public float jumpDetectDistance = .1f;

    public float jumpActivate = 5f;
    public float jumpBoostTimer = 0f;

    public bool jumpBoostActivated = false;

    PlayerInput playerInput;
    Rigidbody2D rb;

    public GameObject currentEquipment;

    Ray2D jumpRay;
    Vector2 moveInput;
    Vector2 dropOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializing component data
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        currentEquipment = null;

        // Setting up new move Vector
        moveInput = Vector2.zero;

        dropOffset = Vector2.zero;
        dropOffset.x += 5f;

        jumpRay = new Ray2D(transform.position, -transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        // Counts the amount of seconds that have passed since boost activation
        if(jumpBoostActivated)
        {
            if(jumpBoostTimer >= jumpActivate)
            {
                jumpHeight -= jumpBoost;
                jumpBoostActivated = false;
            }

            jumpBoostTimer += Time.deltaTime;
        }

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

    // Current equipment activation system
    public void ActivateEquipment()
    {
        if (currentEquipment != null)
        {
            if(currentEquipment.name == "Jump")
            {
                jumpHeight += jumpBoost;

                jumpBoostActivated = true;

                currentEquipment = null;
            }
        }
    }


    public void DropEquipment()
    {
        if(currentEquipment != null)
        {
            currentEquipment.SetActive(true);

            currentEquipment.transform.position = (Vector2) transform.position + dropOffset;

            if (currentEquipment.name == "Jump")
            {
                jumpHeight -= jumpBoost;
            }

            if (currentEquipment.name == "Speed")
            {
                // Reduce speed
            }

            currentEquipment = null;

            Debug.Log("Playing");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Equipment")
        {
            currentEquipment = collision.gameObject;

            collision.gameObject.SetActive(false);

            /* 
            Only uncomment if you want to enable powerup on pickup
            if (collision.gameObject.name == "Jump")
            {
                jumpHeight += jumpBoost;
            }
            */
        }
    }
}
