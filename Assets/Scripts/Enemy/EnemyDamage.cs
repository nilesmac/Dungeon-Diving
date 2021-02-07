using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{

    private PlayerHealth player;
    private Enemy enemyobj;
    public Animator anim;
    public GameObject playerCollider;

    //damage
    public float timeleft = 2.0f;
    public bool ready = true;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerHealth>();
        enemyobj = FindObjectOfType<Enemy>();
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
     
           
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.tag == "Player"
                && anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
            {
                Debug.Log("Damage In Hit"+collision.gameObject);
                player.TakeDamage(1);
                
            }
        }
    }

}
