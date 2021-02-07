using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public void Update()
    {
        if (Input.GetKey(KeyCode.P))
            PauseGame();
            

        
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        
    }
    public void PauseGame()
    {
        Debug.Log("Called");
        Time.timeScale = 0;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
    }
}
