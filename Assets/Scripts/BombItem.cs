using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : MonoBehaviour
{
    public AudioManager c_audio;
    public int bombCount;
    public GameObject bombWall;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bombCount++;
        c_audio.Play("KeySound");
        Destroy(gameObject);
        Destroy(bombWall);
    }
}
