using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    public PlayerHealth playerHealthObj;

    // Start is called before the first frame update
    void Start()
    {
        playerHealthObj = FindObjectOfType<PlayerHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Player")
        playerHealthObj.TakeDamage(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
            playerHealthObj.TakeDamage(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
