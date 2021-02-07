using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Insert fields
    public Transform enemyTransform;
    public Transform playerTransform;
    public GameObject healthDispayer;
    public Rigidbody2D erb;
    public LayerMask groundLayer;
    public Animator animator;
    public AudioManager e_audio;
    
   
    

    //enemy ai values
    public float agroRange = 4;
    public float enemyMoveSpeed = 2;
    public bool isDead = false;
    

    //taking damage function
    public PlayerHealth playerHealth;
    public HealthBar healthBar;
    public HealthDisplay healthD;

    //knockback funtions
    public CommandHandler player;
    

    //Health
    public float maxHealth = 2;
    public float currentHealth;


    //knockback
    //Knockback
    public float knockback = 0;
    public float knockTime = 0;
    public float knockbackCount = 0;
    public bool knockFromRight;

    //region code

    #region Public Variables
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public Transform leftLimit;
    public Transform rightLimit;
    [HideInInspector]public Transform target;
    [HideInInspector]public bool inRange;
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion

    #region Private Variables
    private Animator anim;
    private float distance;
    private bool attackMode = false;
    private bool cooling;
    private float intTimer;
    private bool facingRight;
  
    #endregion

    private RayCast raycast;

    private void Awake()
    {
        SelectTarget();
        intTimer = timer;
        anim = GetComponent<Animator>();
        healthD = FindObjectOfType<HealthDisplay>();


    }


    public void Start()
    {
        player = GameObject.Find("CommandHandler").GetComponent<CommandHandler>();
        erb = enemyTransform.GetComponent<Rigidbody2D>();
        raycast = GetComponentInChildren<RayCast>();

       
        //health
        currentHealth = maxHealth;
        
}



    public void Update()
    {
        /*if (GameObject.Find("raycast").GetComponent<RayCast>().edge)
         {
             inRange = false;
         }*/

        //Debug.Log(inRange);

        if (!isDead)
        {
            if (raycast.edge)
            {
                SelectTarget();
            }

            if (!attackMode && !raycast.edge)
            {
                Move();
            }

            if (!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
            {
                SelectTarget();

            }
            if (inRange)
            {
                EnemyLogic();
            }
        }

       
    }


    

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void EnemyLogic()
    {
        
        distance = Vector2.Distance(transform.position, target.position);

        if (!isDead)
        {

            if (distance > attackDistance)
            {

                StopAttack();
            }
            else if (attackDistance >= distance && cooling == false)
            {

                Attack();
            }
            if (cooling)
            {
                Cooldown();
                anim.SetBool("Attacking", false);
                //attackMode = false;
                //anim.SetBool("CanWalk", false);

            }
        }
    }

    void Move()
    {
        anim.SetBool("CanWalk", true);
        

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
        {
            
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
        
        }
    }

    void Attack()
    {
        
        timer = intTimer;
        attackMode = true;

        anim.SetBool("CanWalk", false);
        anim.SetBool("Attacking", true);
    }


    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attacking", false);

    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }
        Flip();
    }

    public void Flip()
    {
        if (!isDead)
        {
            Vector3 rotation = transform.eulerAngles;
            if (transform.position.x > target.position.x)
            {
                rotation.y = 0f;
            }
            else
            {
                rotation.y = 180f;
            }

            transform.eulerAngles = rotation;
        }


    }

   

    //aggro functions chasing player
    public void stopChasePlayer()
    {
        erb.velocity = new Vector2(0, 0);
        animator.SetBool("CanWalk", false);

    }

    private void ChasePlayer()
    {
        if (enemyTransform.position.x < playerTransform.position.x)
        {
            animator.SetBool("CanWalk", true);
            erb.velocity = new Vector2(enemyMoveSpeed, 0);
            //turn to face the player
            enemyTransform.localScale = new Vector2(-1, 1);
        }
        else if (enemyTransform.position.x > playerTransform.position.x)
        {
            animator.SetBool("CanWalk", true);
            erb.velocity = new Vector2(-enemyMoveSpeed, 0);
            //turning enemy to face player
            enemyTransform.localScale = new Vector2(1, 1);
        }

    }

  

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            if (healthDispayer.gameObject.activeInHierarchy == false)
            {
                healthDispayer.gameObject.SetActive(true);
            }

            currentHealth -= damage;
            animator.SetTrigger("Hurt");

            e_audio.PlayRandomCuts();


            if (currentHealth <= 0)
            {
                Die();
            }

        }
    }

    void Die()
    {
        e_audio.PlayRandomDeath();
        Debug.Log("enemy is dead!");
        animator.SetBool("IsDead", true);
        
        isDead = true;
        healthDispayer.SetActive(false);
        enemyTransform.transform.GetChild(5).gameObject.SetActive(false);

        //GetComponent<Collider2D>().enabled = false;
        //this.enabled = false;
    }
}
