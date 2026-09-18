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

    public void equip(PlayerController p)
    {
        player = p;

        player.currentWeapon = this;

        transform.SetPositionAndRotation(player.weaponSlot.position, player.weaponSlot.rotation);
        transform.SetParent(player.weaponSlot);

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    public void unequip()
    {
        player.currentWeapon = null;

        transform.SetParent(null);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;

        this.player = null;
    }

    public void fire()
    {
        if(canFire && !reloading && clip > 0)
        {
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
        if (clip >= clipSize)
            return;

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

    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(rof);

        if(clip > 0)
            canFire = true;
    }

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
