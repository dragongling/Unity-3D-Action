using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
