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
        if (ManageGame.furthestLevel + 1 >= level) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level " + (level));
        }
    }
    
}
