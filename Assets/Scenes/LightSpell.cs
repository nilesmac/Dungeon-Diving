using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightSpell : MonoBehaviour
{
    public UnityEngine.Experimental.Rendering.Universal.Light2D SUN;
    public float countDown = 20.0f;
    public float downTime, upTime, pressTime = 0;
    public PlayerHealth playerHealth;
    public AudioManager b_audio;

    

    // Start is called before the first frame update
    void Start()
    {
        b_audio = FindObjectOfType<AudioManager>();
        playerHealth = GetComponent<PlayerHealth>();
        SUN.intensity = 0f;
        countDown = 10f;
    }

    public void SpellCast()
    {
        try
        {
            b_audio.Play("Nighteye");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        
            SUN.intensity = 2.5f;
        playerHealth.TakeMagic(1);
        
    }

    public void SpellExpired()
    {
        SUN.intensity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Q) && Time.timeScale != 0)
        {
            SpellCast();
            downTime = Time.time;
            pressTime = downTime + countDown;
        }

        if (Time.time >= pressTime)
        {
            SpellExpired();
            
        }
    }
}
