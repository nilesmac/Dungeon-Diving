using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Commands
{
    private Enemy enemy;

    public override void Execute(Transform trans, CommandHandler handler, Commands command, Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio)
    {
        

        if (Time.time >= nextAttackTime)
        {
            Move(trans, layer, rb, animator);
            nextAttackTime = Time.time + .5f / attackRate;
        }
    }

    public override void Move(Transform trans, LayerMask layer, Rigidbody2D rb, Animator animator)
    {
        //play attack animation
        //detect enmesi in range
        
        animator.SetTrigger("Attack");

       Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(trans.position, attackRange, layer);

        foreach(Collider2D enemy in hitEnemies)
        {
                //Debug.Log("We hit" + enemy.name);
                //calling the take damage function from enemy script to effect health
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);  
        }
    }

    
    
    
}
