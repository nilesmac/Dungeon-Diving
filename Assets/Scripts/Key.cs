using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int Keycount;
    public AudioManager audioMan;
    // Start is called before the first frame update
    void Start()
    {
        audioMan = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Player")
        {
            try
            {
                audioMan.Play("KeySound");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            Keycount++;
            Destroy(gameObject);
            Debug.Log(Keycount);
        }
    }
}
