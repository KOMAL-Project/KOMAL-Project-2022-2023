using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButtons : MonoBehaviour
{
    private int level;
    private int furthest;

    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        button = GetComponent<Button>();
        level = int.Parse(gameObject.name);
        furthest = ManageGame.furthestLevel;

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
