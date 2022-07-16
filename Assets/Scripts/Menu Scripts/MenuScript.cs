using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    
    public void Quit() {
        Debug.Log("Game Quit");
        Application.Quit();
        
    }

    public void changeLevel(int level) {
        if (true) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(level, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
