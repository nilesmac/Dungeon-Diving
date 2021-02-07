using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusic : MonoBehaviour
{

    private static LevelMusic instance = null;
    public static LevelMusic Instance
    {
        get { return instance; }
    }
    void Update()
    {
        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        //if (scenceNum >1)
        //  Destroy(gameObject);
        //Debug.Log(scenceNum);
        if (instance != null && instance != this || scenceNum >2)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /*private static GameObject instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance = null)
            instance = gameObject;
        else
            Destroy(gameObject);

        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        //if (scenceNum >1)
          //  Destroy(gameObject);

      

    }*/

}
