using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButtons : MonoBehaviour
{
    private int level;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        Button button = GetComponent<Button>();
        
        level = int.Parse(gameObject.name);
        int furthest = ManageGame.furthestLevel;
        int chapter = ManageGame.furthestChapter;

        if (furthest + 1 >= level) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }

        button.onClick.AddListener(changeLevel);
    }

    private void changeLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level " + level);
    }

}
