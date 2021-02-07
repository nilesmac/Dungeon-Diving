using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Commands
{
    //variables
    public float speed=5;
    public float jumpForce=10;
    public float extraJumps = 2;
    public float moveDistance=.5f;
    public bool isRewinding = false;

    //jumping extension
    public float jumpTimeExtension = .5f;
    public float jumpCondition = 0;

    //Ladder
    public bool isGrounded = false;
    public Vector2 direction = Vector2.down;
    public float distance = 1.0f;


    //attacking
    public float attackRange = .34f;
    public float attackDamage = .5f;
    public float attackRate = 2f;
    public float nextAttackTime = 0f;


    public abstract void Execute(Transform trans,CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer, LayerMask OneWay, LayerMask ladder, Animator animator, AudioManager audio);

    public virtual void Move(Transform trans, LayerMask layer, Rigidbody2D rb, Animator animator) { }

    public virtual void JumpMove(Transform trans, CommandHandler handler, 
        LayerMask layer,Rigidbody2D rb, LayerMask OneWay, LayerMask ladder, AudioManager audio, Animator animator) { }

    public virtual void LadderMovement(bool onLadder) { }

    public virtual void Undo(Transform trans, Rigidbody2D rb) { }

    public virtual void Rewind(Transform trans) { }

    public virtual void Record() { }

    public virtual void getPosition(Transform trans, CommandHandler handler, Commands command,
        Rigidbody2D rb, LayerMask layer) { }







}
