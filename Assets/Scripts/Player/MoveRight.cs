using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : Commands
{


   

    public override void Execute(Transform trans, CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio)
    {
        handler.animator.SetFloat("Speed", 1);
        
        Move(trans, layer,rb, animator);

        handler.commandList.Insert(0, trans.position);
        
    }

    public override void Move(Transform trans, LayerMask layer, Rigidbody2D rb, Animator animator)
    {
        
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);

    


      

        //flip character
        Vector3 characterScale = trans.localScale;
        if (Input.GetAxis("Horizontal") > 0) { characterScale.x = -1; }
        if (Input.GetAxis("Horizontal") < 0) { characterScale.x = 1; }
        trans.localScale = characterScale;


    }


}

