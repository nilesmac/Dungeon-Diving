using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    // Update is called once per frame
    public Transform castPoint;
    public float distance = .5f;
    public Rigidbody2D erb;
    public bool edge = false;
    public Transform enemyTransform;
    private Enemy enemyParent;
    //int layerMask = 1;
    RaycastHit2D hit;
    private bool facingLeft = true;
    //private Vector2 endPos;

    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy>();
    }


    void Update()
    {
        Vector3 rotation = enemyTransform.eulerAngles;
        Vector2 endPos = castPoint.position + Vector3.left * distance;

        if (rotation.y == 0f)
        {
            
            facingLeft = true;
        }
        else if (rotation.y == 180f)
        {
            facingLeft = false;
            
        }

        if (facingLeft)
        {
            
            endPos = castPoint.position + Vector3.left * distance;

            RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Edge"));
            Debug.DrawLine(castPoint.position, endPos, Color.blue);

            if (hit.collider != null)
            {
                //Debug.Log("Collided with" + hit.collider);
                if (hit.collider.gameObject.CompareTag("Edge"))
                {
                    edge = true;

                }
            }
            else
            {

                edge = false;
                // Debug.Log(edge);
            }
        }
        else
        {
            
            endPos = castPoint.position + Vector3.right * distance;

            RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Edge"));
            Debug.DrawLine(castPoint.position, endPos, Color.blue);

            if (hit.collider != null)
            {
                Debug.Log("Collided with" + hit.collider);
                if (hit.collider.gameObject.CompareTag("Edge"))
                {
                    edge = true;

                }
            }
            else
            {
                edge = false;
                // Debug.Log(edge);
            }
        }
        

    

    }
    
}
