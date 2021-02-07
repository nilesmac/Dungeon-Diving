using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderZone : MonoBehaviour
{
    private CommandHandler thePlayer;
  
  
    // Start is called before the first frame update
    void Start()
    {
        //makes it so the value of thePlayer isn't null!
        //this avoids NullReferenceExcoception: Object reference not set to an instance of an object
        thePlayer = FindObjectOfType<CommandHandler>();
        
    }

    private void Update()
    {
        //Debug.Log(thePlayer.onLadder);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            thePlayer.onLadder = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        thePlayer.onLadder = false;
    }


}
