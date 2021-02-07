using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undue : Commands
{


    public override void Execute(Transform trans, CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio)
    {
        if (handler.commandList.Count > 0)
        {
                
                trans.position = handler.commandList[0];
                handler.commandList.RemoveAt(0);
           
        }
        else
        {
            handler.StopRewind();
        }
    }

}
