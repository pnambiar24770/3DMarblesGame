using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject PauseMenuUI;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

           

            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }


    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;

    }
}
