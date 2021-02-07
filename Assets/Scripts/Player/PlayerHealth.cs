using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    //Health information
    public int maxHealth = 4;
    public int currentHealth;

    //Magic information
    public int maxMagic = 3;
    public int currentMagic;

    public HealthBar healthBar;
    public MagicBar magicBar;
    public GameObject TryAgainMenu;
    public Animator ani;
    public AudioManager _audio;
    public bool PlayerDead=false;
    // Start is called before the first frame update
    public void Start()
    {
        //setting player health
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        

        //player magic
        currentMagic = maxMagic;
        magicBar.SetMaxMagic(maxMagic);

        Debug.Log(currentMagic);

        Time.timeScale = 1;
        PlayerDead = false;
    }

    // Update is called once per frame
    public void Update()
    {
        PlayerDead = false;
        if (currentHealth <= 0)
            PlayerDeath();

        
    }


    public void TakeDamage(int damage)
    {
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Player_hurt"))
            {
            ani.SetTrigger("isHurt");
            }


        _audio.Play("PlayerHurt");
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        
    }

    public void TakeMagic(int damage)
    {
        
        
        currentMagic -= damage;

        magicBar.SetMagic(currentMagic);

    }

    public void PlayerDeath()
    {
        ani.SetBool("isDead", true);
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("Player_death"))
        Time.timeScale = 0;
        TryAgainMenu.SetActive(true);
        PlayerDead = true;
}
}
