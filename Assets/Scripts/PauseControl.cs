using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public GameObject menu; // Assign in inspector
    public static bool gameIsPaused = false;

    private void Awake()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            gameIsPaused = !gameIsPaused;
            if (gameIsPaused)
                Pause();
            else
                Resume();
        }
    }

    private void Pause()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        menu.SetActive(gameIsPaused);
    }

    private void Resume()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        menu.SetActive(gameIsPaused);
    }

    public void ExitToDesktop()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
