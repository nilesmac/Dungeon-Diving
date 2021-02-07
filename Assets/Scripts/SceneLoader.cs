using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    Scene scene;
    public int scenceNum;
    public bool goInDoor = false;
    private static int[] levelsLoaded = new int[4];
    private static int level=0;
    public bool gameOver=false;
    private GameObject Transition;
    public float transitionTime = 1f;





    // Start is called before the first frame update
    private SceneLoader instance = null;
    public SceneLoader Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
    
   
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
    

    void Start()
    {
        
        scene = SceneManager.GetActiveScene();

        scenceNum = SceneManager.GetActiveScene().buildIndex;

        if (scenceNum != 0)
            Cursor.visible = false;

    }


    public void ChangeScene()
    {
        int nextSceneIndex = UnityEngine.Random.Range(4, 8);

        
        while (levelsLoaded.Contains(nextSceneIndex))
        {
            nextSceneIndex = UnityEngine.Random.Range(4, 8);
            
        }
        //int sceneCheck = Array.Find(levelsLoaded, nextSceneIndex);

        Debug.Log("Selected"+nextSceneIndex);

        
        if (!levelsLoaded.Contains(nextSceneIndex))
        {
            //Debug.Log("random level selected" + nextSceneIndex);
            levelsLoaded[level] = nextSceneIndex;
            level++;
            if (level != 3)
                StartCoroutine("PlayTransition", nextSceneIndex);
            else
            {

                SceneManager.LoadScene(8);
            }
        }


    }
    IEnumerator PlayTransition(int nextSceneIndex)
    {
        Transition.GetComponent<Animator>().SetTrigger("Transition");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);
    }

    public void Update()
    {
        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        Transition = GameObject.Find("Transition");


        if (instance != null && instance != this || scenceNum < 1)
        {
            level = 0;
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

