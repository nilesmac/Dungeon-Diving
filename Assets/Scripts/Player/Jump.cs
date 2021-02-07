using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Commands
{
 

    public override void Execute(Transform trans, CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio)
    {
        
        JumpMove(trans, handler, layer, rb, OneWay, ladder, audio, animator);
        
        handler.commandList.Insert(0, trans.position);
    }

    public override void JumpMove(Transform trans, CommandHandler handler, LayerMask groundLayer, Rigidbody2D rb, LayerMask OneWay, LayerMask ladder, 
        AudioManager audio, Animator animator)
    {
        handler.animator.SetBool("IsJumping", false);
        //code for ground checking
        bool isGrounded = false;
        bool isOneWay = false;
        Vector2 position = trans.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;


        //OneWay Layer Raycast
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit2 = Physics2D.Raycast(position, direction, distance, OneWay);

        //Code for raycast for ground layer
        //debul shows raycast in editor mode
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);

        if (handler.onLadder)
        {
            extraJumps = 2;
            //animator.SetBool("OnLadder", true);
        }
        


        //resets amount of jumps if the player touches the ground
        if (hit.collider != null || hit2.collider != null)
        {
            //Debug.Log("you are grounded");
            isGrounded = true;
            isOneWay = true;

        }
    
        //Debug.Log(extraJumps);
        //resets jumps once player touches ground
        //saves the timer of when the player was considered grounded
        if (isGrounded || isOneWay)
        {
           
            extraJumps = 2;
        }
        //jump code
        if (extraJumps != 0)
        {
            try
            {
                audio.Play("Jump2");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            jumpForce = 10;
            //Debug.Log(jumpForce);
            if (extraJumps == 1)
            {
               
                jumpForce = 5;
            }

            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }

        
    }

}

  


       

