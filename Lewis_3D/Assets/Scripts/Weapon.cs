using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerController player;

    public GameObject projectile;
    public Transform firePoint;
    public Camera firingDirection;

    [Header("Meta Attributes")]
    public bool canFire = true;
    public bool holdToAttack = true;
    public bool reloading = false;
    public int weaponID;
    public string weaponName;

    [Header("Weapon Stats")]
    public float projLifespan;
    public float projVelocity;
    public float reloadCooldown;
    public float rof;
    public int fireModes;
    public int currentFiremode;
    public int clip;
    public int clipSize;

    [Header("Ammo Stats")]
    public int ammo;
    public int maxAmmo;
    public int ammoRefill;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firePoint = transform.GetChild(0);
        firingDirection = Camera.main;
    }

    // Assign player to the weapon (think locked by thumbprint)
    public void equip(PlayerController p)
    {
        player = p;

        player.currentWeapon = this;

        // Set weapon to be same position and facing direction as weapon slot
        // If your weapon model is facing the wrong way, your model is built incorrectly
        transform.SetPositionAndRotation(player.weaponSlot.position, player.weaponSlot.rotation);
        transform.SetParent(player.weaponSlot);

        // Turn off physics and collision while we're holding the weapon
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    // Drop weapon
    public void unequip()
    {
        // Forget player weapon
        player.currentWeapon = null;

        // Forget parent
        transform.SetParent(null);

        // Forget Player
        this.player = null;

        // Become ungovernable (turn physics back on)
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
    }

    // Attack action
    public void fire()
    {
        // If you can shoot and you're not reloading and you have ammo, do the thing
        if(canFire && !reloading && clip > 0)
        {
            // Create the projectile at our fire point (at the right rotation)
            // Push the bullet with Rigidbody's AddForce in the camera's facing direction
            // Destroy the bullet after a specified amount of time (projLifespan)
            // Subtract the ammo
            // Start a countdown to be able to shoot again
            GameObject p = Instantiate(projectile, firePoint.position, firePoint.rotation);
            p.GetComponent<Rigidbody>().AddForce(firingDirection.transform.forward * projVelocity);
            Destroy(p, projLifespan);
            canFire = false;
            clip--;
            StartCoroutine("cooldownFire");
        }
    }

    public void reload()
    {
        // If our clip is already full or overfull, don't bother
        if (clip >= clipSize)
            return;

        // Otherwise, begin reload action
        reloading = true;
        canFire = false;

        int reloadCount = clipSize - clip;

        if (ammo < reloadCount)
        {
            clip += ammo;
            ammo = 0;
        }

        else
        {
            clip += reloadCount;
            ammo -= reloadCount;
        }

        StartCoroutine("reloadingCooldown");
    }

    public void recoil()
    {
        // TBD
    }

    // Wait for countdown to finish then re-enable firing if we have ammo
    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(rof);

        if(clip > 0)
            canFire = true;
    }

    // Wait for reloading time/animation to complete
    // NOTE FOR FUTURE!!!
    // Ensure you pass your reloading animation's time to this function to sync reload countdown with animation
    IEnumerator reloadingCooldown()
    {
        yield return new WaitForSeconds(reloadCooldown);

        reloading = false;
        canFire = true;
    }

    /*
    IEnumerator burstDuration()
    {

    }
    */
}
