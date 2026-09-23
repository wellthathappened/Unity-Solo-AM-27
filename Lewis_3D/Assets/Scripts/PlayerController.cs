using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool isAttacking = false;
    public bool hazardDamage = false;

    public int health = 5;
    public int maxHealth = 5;
    public float speed = 5;
    public float jumpHeight = 10;
    public float jumpDetectDistance = 1.1f;
    public float interactDistance = 6;
    public float hazardCooldown = 3;

    CinemachinePositionComposer cineCam;
    Camera playerCam;
    PlayerInput playerInput;
    Rigidbody rb;

    public Weapon currentWeapon;
    public Transform weaponSlot;
    public GameObject pickupObj;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;
    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializing component and camera data
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerCam = Camera.main;
        cineCam = GameObject.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();

        // Setting up new move Vector
        moveInput = Vector2.zero;

        // Initialize my rays for later raycast use
        jumpRay = new Ray(transform.position, -transform.up);
        interactRay = new Ray(playerCam.transform.position, playerCam.transform.forward);

        // Assign my weapon slot location
        weaponSlot = transform.GetChild(0);

        // Hide my mouse and prevent it from going off screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Camera rotation code made in fixed update to prevent physics desync
    private void FixedUpdate()
    {
        Quaternion playerRotation = Quaternion.identity;
        playerRotation.y = playerCam.transform.rotation.y;
        playerRotation.w = playerCam.transform.rotation.w;
        transform.rotation = playerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Die
        if(health <= 0)
        { }

        // Jump Ray update
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        // Interact Ray update
        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        // Check if interact ray hits an interactable objects.
        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.tag == "Weapon")
            {
                // Sets reference to interactable object to "pickupObj"
                pickupObj = interactHit.collider.gameObject;
            }
            else
                pickupObj = null;
        }
        else
            pickupObj = null;
        // If no obj hit or non-interactive obj hit, set pickupObj to null

        // If I have a full auto weapon and attack button is held, repeat attack.
        if (currentWeapon)
            if (currentWeapon.holdToAttack && isAttacking)
                currentWeapon.fire();

        // Move code
        Vector3 tempMove = rb.linearVelocity;

        // Normalize input vector to 3D worldspace direction
        tempMove.x = (moveInput.x * speed);
        tempMove.z = (moveInput.y * speed);

        // Normalize move vector to be relative to player's forward facing direction 
        rb.linearVelocity = (tempMove.x * transform.right) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.forward);
    }

    // Take in move input values
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Jump action
    public void Jump()
    {
        // If ray detects any collider, let player jump
        // ** NOTE **
        // If you are jumping on invisible collision, check your Physics settings
        // as you may have "Queries hit triggers" checked off (raycast hitting trigger colliders)
        if (Physics.Raycast(jumpRay, jumpDetectDistance))
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }

    // Simple shoulder swap that ONLY works with Cinemachine "Position Composer" component
    public void shoulderSwap()
    {
        cineCam.TargetOffset.x *= -1;
    }

    // If you have a weapon and the weapon isn't reloading, do the thing
    public void Reload()
    {
        if (currentWeapon)
            if (!currentWeapon.reloading)
                currentWeapon.reload();
    }

    // Attack action
    public void Attack(InputAction.CallbackContext context)
    {
        if(currentWeapon)
        {
            // If you have a full auto weapon, activate attacking flag while button is held
            // It is a pain to configure held down button to repeat an action in input settings
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    isAttacking = true;
                else
                    isAttacking = false;
            }

            // If it is semi-auto fire, just do the thing on button press.
            else if (context.ReadValueAsButton())
                currentWeapon.fire();
        }
    }

    // Interact action
    public void Interact(InputAction.CallbackContext context)
    {
        // If our interact button is active at all
        if(context.ReadValueAsButton())
        {
            // And we have a reference to an object with which to interact
            if (pickupObj)
            {
                // Check if it's a weapon and equip it ONLY if we do not already have a weapon
                if (pickupObj.tag == "Weapon")
                {
                    if (!currentWeapon)
                    {
                        pickupObj.GetComponent<Weapon>().equip(this);
                    }
                }

                // If the interact object is an ammo pickup and you want player to interact with ammo to acquire
                // uncomment the if statement below
                /*
                if (pickupObj.tag == "Ammo")
                {
                    Destroy(pickupObj);
                    if(currentWeapon && currentWeapon.ammo < currentWeapon.maxAmmo)
                    {
                        int ammoFill = currentWeapon.maxAmmo - currentWeapon.ammo;

                        if (ammoFill < currentWeapon.ammoRefill)
                        {
                            currentWeapon.ammo += ammoFill;
                        }
                        else
                        {
                            currentWeapon.ammo += currentWeapon.ammoRefill;
                        }
                    }
                }
                */
            }
            else if (currentWeapon)
                Reload();
            // Controller-concious context reload :)
        }
    }

    // Self-explanatory Drop Weapon action
    public void DropWeapon()
    {
        if (currentWeapon)
            currentWeapon.unequip();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If we collide with an ammo pickup, refill ammo as needed
        // Put below if-statement in comment block if you rather let players interact
        // with ammo to pick it up
        if(collision.gameObject.tag == "Ammo")
        {
            if(currentWeapon && currentWeapon.ammo < currentWeapon.maxAmmo)
            {
                int ammoFill = currentWeapon.maxAmmo - currentWeapon.ammo;

                if (ammoFill < currentWeapon.ammoRefill)
                {
                    currentWeapon.ammo += ammoFill;
                }
                else
                {
                    currentWeapon.ammo += currentWeapon.ammoRefill;
                }

                Destroy(collision.gameObject);
            }
        }

        // Take immediate damage on hazard
        if(collision.gameObject.tag == "Hazard")
        {
            health--;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Take progressive damage on hazard if we are not already but we are still touching hazard
        if (collision.gameObject.tag == "Hazard")
        {
            if(!hazardDamage)
                StartCoroutine("damageCooldown");
        }
    }

    // If we leave hazard zone, stop applying hazard damage and stop any active damageCooldown coroutine
    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            if (hazardDamage)
            {
                StopCoroutine("damageCooldown");
                hazardDamage = false;
            }
        }
    }

    // Throw flag that we are taking hazard damage and start countdown
    // Take damage once countdown is over.
    IEnumerator damageCooldown()
    {
        hazardDamage = true;

        yield return new WaitForSeconds(hazardCooldown);

        health--;
        hazardDamage = false;
    }
}