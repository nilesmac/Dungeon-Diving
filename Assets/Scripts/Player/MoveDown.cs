using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public Transform trans;
    public LayerMask layerOneWay;
    public LayerMask groundLayer;
    public CommandHandler handler;

    public void Start()
    {
        handler = FindObjectOfType<CommandHandler>();

    }

    public void Update()
    {
        //code for ground checking
        bool isGrounded = false;
        bool isOneWay = false;
        Vector2 position = trans.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        //debug shows raycast in editor mode
        //for oneone layer check
        Debug.DrawRay(position, direction, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, layerOneWay);


        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit2 = Physics2D.Raycast(position, direction, distance, groundLayer);

        if (hit.collider != null)
        {
            isOneWay = true;
        }

        if (hit2.collider != null)
        {
            isGrounded = true;
        }

      
        if ((isGrounded && Input.GetKey(KeyCode.S) || (isOneWay && Input.GetKey(KeyCode.S))))
        {
            Debug.Log("Ducked");
            handler.animator.SetBool("Ducking", true);
        }
        else
        {
            handler.animator.SetBool("Ducking", false);
        }


        if (isOneWay && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            trans.GetComponent<Collider2D>().enabled = false;
            Invoke("JumpDownTimer", 0.4f);
        }
        

       
    }
    void JumpDownTimer()
    {
        Debug.Log("JumpDown Called");
        trans.GetComponent<Collider2D>().enabled = true;
    }

    }

