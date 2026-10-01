using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject PauseMenu;

    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI clipText;
    public TextMeshProUGUI ammoText;

    public Image healthBar;

    public bool paused = false;
    public bool enemiesGone = false;
    public int enemyCount = 0;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            PauseMenu = GameObject.FindGameObjectWithTag("Pause");

            weaponName = GameObject.Find("WeaponName").GetComponent<TextMeshProUGUI>();
            clipText = GameObject.Find("ClipText").GetComponent<TextMeshProUGUI>();
            ammoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            PauseMenu.SetActive(false);

            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (paused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Time.timeScale = 0;

                PauseMenu.SetActive(true);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Time.timeScale = 1;

                PauseMenu.SetActive(false);
            }

            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

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

            if(enemyCount <= 0)
            {
                enemiesGone = true;
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

    public void Pause()
    {
        paused = !paused;

        Cursor.visible = paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

        PauseMenu.SetActive(paused);
    }

    public void LoadLevel(int levelID)
    {
        if (levelID >= SceneManager.sceneCount)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
    }

    public void MainMenu()
    {
        LoadLevel(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
