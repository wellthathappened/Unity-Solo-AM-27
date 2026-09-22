using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI clipText;
    public TextMeshProUGUI ammoText;

    public Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        weaponName = GameObject.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        clipText = GameObject.Find("ClipText").GetComponent<TextMeshProUGUI>();
        ammoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();

        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float) player.health / (float) player.maxHealth;

        if (player.currentWeapon)
        {
            weaponName.text = player.currentWeapon.name;
            clipText.text = "Clip: " + player.currentWeapon.clip + '/' + player.currentWeapon.clipSize;
            ammoText.text = "Ammo: " + player.currentWeapon.ammo + '/' + player.currentWeapon.maxAmmo;
        }
        else
        {
            weaponName.text = "";
            clipText.text = "";
            ammoText.text = "";
        }

        /* 
        
        If you wanted to indicate some powerup in UI text do something like this
        if (player.jumpBoostActivated)
        {
            boostText.text = "Jump Boost Activated";

            healthBar.enabled = true;


        }
        else
        {
            boostText.text = "";
            healthBar.enabled = false;
        }

        */
    }
}
