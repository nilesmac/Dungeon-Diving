using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusic2 : MonoBehaviour
{

    private static LevelMusic2 instance = null;
    public static LevelMusic2 Instance
    {
        get { return instance; }
    }
    void Update()
    {
        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        //if (scenceNum >1)
        //  Destroy(gameObject);
        if (instance != null && instance != this || scenceNum < 1)
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
}

