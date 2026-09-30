using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int health = 5;
    public int maxHealth = 5;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float sprintCost = .1f;
    public float speed = 5.0f;

    public float sprintCooldown = 2;
    public float staminaRegen = 5;
    public float staminaCooldown = 2;

    public float sprintBoost = 2.0f;
    public float jumpHeight = 10f;
    public float jumpBoost = 5f;
    public float jumpDetectDistance = .1f;

    public float jumpActivate = 5f;
    public float jumpBoostTimer = 0f;

    public bool sprinting = false;
    public bool canSprint = true;
    public bool sprintStop = false;
    public bool staminaStop = false;
    public bool regenStamina = false;
    public bool toggleSprint = true;
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

        if(sprinting)
        {
            if ((moveInput.x == 1 || moveInput.x == -1) && stamina > 0)
            {
                tempMove.x *= sprintBoost;

                stamina -= sprintCost * Time.deltaTime;

                if (stamina < 0)
                    stamina = 0;

                StopCoroutine("staminaReset");
            }
            else
            {
                canSprint = false;
                sprinting = false;
            }
        }

        if (!sprinting)
        {
            if (!regenStamina && !staminaStop && stamina < maxStamina)
            {
                StartCoroutine("staminaReset");
            }
            if (!canSprint && !sprintStop)
            {
                StartCoroutine("sprintReset");
            }
            if (regenStamina)
            {
                stamina += staminaRegen * Time.deltaTime;

                if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                    regenStamina = false;
                }
            }
        }

        rb.linearVelocity = tempMove;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (canSprint)
        {
            if (toggleSprint)
            {
                sprinting = !sprinting;
            }
            else if (!toggleSprint)
            {
                sprinting = context.ReadValueAsButton();

                if (!sprinting)
                    canSprint = false;
            }
        }
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

    IEnumerator sprintReset()
    {
        sprintStop = true;

        yield return new WaitForSeconds(sprintCooldown);

        canSprint = true;
        sprintStop = false;
    }

    IEnumerator staminaReset()
    {
        staminaStop = true;

        yield return new WaitForSeconds(staminaCooldown);

        regenStamina = true;
        staminaStop = false;
    }
}
