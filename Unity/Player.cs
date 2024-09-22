using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
  
    public int maxHealth = 100;
    public int maxBullets = 6;
    public int maxBombs = 2;
    public int maxShields = 3;
    public int maxShieldHealth = 30;


    public int hpBullet = 5;    
    public int hpAI = 10;       
    public int hpBomb = 5;      
    public int hpRain = 5;      

    // Current player state
    public int currentHealth;
    public int currentBullets;
    public int currentBombs;
    public int currentShields;
    public int currentShieldHealth;
    public int numDeaths = 0;   
    public bool isShield = false;

    public TMP_Text healthtext;
    public TMP_Text shieldtext;
    public TMP_Text shieldCount;
    public TMP_Text ammotext;
    public TMP_Text rainBombtext;
    public TMP_Text deathstext;

    public InputAction playerControls;

    

    public HealthBar healthBar;
    public HealthBar shieldBar;
    public GameObject Shield;

    public float timerDuration = 3f;
    public Slider timerSlider;

    float timer;

    void Start()
    {
        // Initialize player's state
        currentHealth = 60;
        currentBullets = maxBullets;
        currentBombs = maxBombs;
        currentShields = maxShields;
        currentShieldHealth = 0; 
        healthBar.SetMaxHealth(maxHealth);
        shieldBar.SetMaxHealth(maxShieldHealth);
        timerSlider.value = 0;

    }
    void Update()
    {
        healthBar.SetHealth(currentHealth);
        shieldBar.SetHealth(currentShieldHealth);
        healthtext.text = currentHealth.ToString();
        shieldtext.text = currentShieldHealth.ToString();
        shieldCount.text = currentShields.ToString();
        ammotext.text = currentBullets.ToString();
        rainBombtext.text = currentBombs.ToString();
        deathstext.text = numDeaths.ToString();
        CheckShield();
    }

    // Function to handle taking damage
    public void TakeDamage(int damage)
    {
  
        int playerDamage = Math.Max(0, (damage - currentShieldHealth));
        currentShieldHealth = Math.Max(0, currentShieldHealth -  damage);
        currentHealth -= playerDamage;

        if (currentHealth <= 0)
        {
            Die(); 
        }
    }
    public void ActivateShield()
    {
        if (currentShields > 0)
        {
            currentShields--;
            currentShieldHealth = maxShieldHealth;
        }

    }

    private void CheckShield()
    {
        if ( currentShieldHealth > 0 )
        {
            Shield.SetActive(true);
        }
        else
        {
            Shield.SetActive(false);
        }
    }

    public void StartReload()
    {
        StartCoroutine(ReloadTimer());
    }

    private IEnumerator ReloadTimer()
    {
        timer = 0;

        
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;

            
            float normalizedTime = Mathf.Clamp01(timer / timerDuration);

            
            timerSlider.value = normalizedTime;

            yield return null; 
        }

        // Reset slider and timer
        timerSlider.value = 0;
        timer = 0;
        if (currentBullets <= 0)
        {
            currentBullets = maxBullets;
        }
    }

    // Function to handle player death
    void Die()
    {
        numDeaths++;  
        currentHealth = maxHealth; 
        healthBar.SetMaxHealth(maxHealth);
        Debug.Log("Player has died " + numDeaths + " times.");
    }
}
