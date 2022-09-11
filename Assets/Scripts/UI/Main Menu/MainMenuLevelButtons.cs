using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButtons : MonoBehaviour
{
    private int level;
    private int chapter;
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f; //keeps button shape the same as sprite

        Button button = GetComponent<Button>();

        level = int.Parse(gameObject.name);
        chapter = int.Parse(this.transform.parent.parent.name.Split(" ")[1]);
        int furthestLevel = ManageGame.furthestLevel;
        int furthestChapter = ManageGame.furthestChapter;

        if (furthestLevel + 1 >= level && furthestChapter >= chapter) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }

        button.onClick.AddListener(changeLevel);
    }

    private void changeLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scenes/Chapter " + chapter + "/Level " + level);
    }

}
