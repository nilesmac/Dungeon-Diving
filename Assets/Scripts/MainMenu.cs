using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject TryAgainMenu;
    public GameObject pauseMenu;
    public AudioManager d_audio;
    public Scene scene;
    public Animator Transition;
    public float transitionTime = 1f;
    

    public void Update()
    {
        if (Time.timeScale == 0)
            Cursor.visible = true;
    }
    public void Start()
    {
        Cursor.visible = true;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        

    }

    public void PlayGame()
    {
        d_audio.Play("Button");
        StartCoroutine("PlayTransition");
        
    }

    IEnumerator PlayTransition()
    {
        Transition.SetTrigger("Transition");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        d_audio.Play("Button");
        Application.Quit();
        Debug.Log("Game Closed");
    }

    public void ExitToMenu()
    {
        d_audio.Play("Button");
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        d_audio.Play("Button");
        TryAgainMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnableTryAgainScreen()
    {
        
        
        TryAgainMenu.SetActive(true);
        
    }

    public void ResumeGame()
    {
        d_audio.Play("Button");
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
