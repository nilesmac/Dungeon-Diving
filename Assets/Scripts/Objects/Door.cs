using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isPlayerInZone = false;
    //public AudioManager doorSound;
    public Key keyObj;

    private SceneLoader sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        keyObj = FindObjectOfType<Key>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(watchForKeyPRess());
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerInZone = false;
        }
    }

    IEnumerator watchForKeyPRess()
    {
       
        if (isPlayerInZone)
        {
            
            if (Input.GetKeyDown(KeyCode.C) && keyObj.Keycount == 1)
            {
                //doorSound.Play("Door");
                sceneLoader.ChangeScene();
            }
            yield return null;
        }
    }
}
