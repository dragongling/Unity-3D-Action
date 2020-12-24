using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public Canvas pauseMenu;
    public static bool gameIsPaused;

    private void Start()
    {
        gameIsPaused = false;
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
        pauseMenu.enabled = gameIsPaused;
    }

    private void Resume()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        pauseMenu.enabled = gameIsPaused;
    }

    public void QuitGame()
    {
        GameManager.QuitGame();
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("Main Screen");
    }
}
