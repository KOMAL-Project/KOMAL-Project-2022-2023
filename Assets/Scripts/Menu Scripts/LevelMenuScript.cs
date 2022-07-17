using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject cam;
    private int menuType = 0;
    private DieController die;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            changeActiveMenu();
        }
    }

    public void restartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnToSelector() {
        SceneManager.LoadScene("Menu");
    }

    public void changeActiveMenu() {

        die = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DieController>();

        if (menuType == 0) {
            menuType++;
            cam.SetActive(true);
            pause.SetActive(true);
            die.canControl = false;
        }
        else if (menuType == 1) {
            menuType--;
            cam.SetActive(false);
            pause.SetActive(false);
            die.canControl = true;
            
        }
        else {
            menuType--;
            pause.SetActive(true);
            options.SetActive(false);
        }
    }

    
    
}
