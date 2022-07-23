using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenScript : MonoBehaviour
{
    public void exitToMain() {
        SceneManager.LoadScene("Menu");
    }
    
}
