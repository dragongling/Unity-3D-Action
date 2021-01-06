using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
        GameManager.QuitGame();
    }
}
