using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public float countDown = 1.0f;
    public float downTime, upTime, pressTime = 0;
    public bool ready = false;
    public bool firstTouch = false;
    // Start is called before the first frame update
    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= pressTime && ready)
        {
            playerHealth.TakeDamage(1);
            //reset
            downTime = Time.time;
            pressTime = downTime + countDown;
        }

        if (firstTouch)
        {
            playerHealth.TakeDamage(1);
            firstTouch = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player" && ready)
        {
            Debug.Log(collision.gameObject);
            firstTouch = true;
            downTime = Time.time;
            pressTime = downTime + countDown;
            
        }
        else
            ready = false;

      
    }

}
