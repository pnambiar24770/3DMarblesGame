using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    public KeyCode resetHotkey = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(resetHotkey))
        {
            ReloadCurrentScene();
        }
    }

    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }
}