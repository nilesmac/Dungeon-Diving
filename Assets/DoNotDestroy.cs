using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroy : MonoBehaviour
{


    private static DoNotDestroy instance = null;
    public static DoNotDestroy Instance
    {
        get { return instance; }
    }
    void Update()
    {
        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        //if (scenceNum >1)
        //  Destroy(gameObject);
        Debug.Log(scenceNum);
        if (instance != null && instance != this)
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
