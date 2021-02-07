using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPositions : Commands
{
    public override void Execute(Transform trans, CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio)
    {
        //code for ground checking
        bool isGrounded = false;
        Vector2 position = trans.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;



        //debul shows raycast in editor mode
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, layer);

        //resets amount of jumps if the player touches the ground
        if (hit.collider != null)
            isGrounded = true;

        if (!isGrounded)
        {
            handler.commandList.Insert(0, trans.position);
        }
    }
}
