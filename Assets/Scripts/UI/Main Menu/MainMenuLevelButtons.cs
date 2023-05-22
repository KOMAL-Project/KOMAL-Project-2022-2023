using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButtons : MonoBehaviour
{
    private int level, chapter;
    private bool bonus;
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f; //keeps button shape the same as sprite

        Button button = GetComponent<Button>();

        bonus = gameObject.name.Contains("b");
        level = bonus ? int.Parse(gameObject.name.Substring(1)) : int.Parse(gameObject.name);
        chapter = int.Parse(this.transform.parent.parent.name.Split(" ")[1]);

        /*
        if ((furthestLevel + 1 >= level && furthestChapter == chapter) || furthestChapter > chapter) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
        */
        button.interactable = true; //button for now will always be usable

        if (ManageGame.finishedLevels.Contains(ManageGame.IDsToString(level, chapter, bonus))) {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        button.onClick.AddListener(ChangeLevel);
    }

    private void ChangeLevel() {
        ButtonScript.moving = false;
        string levelToLoad = (bonus) ? "b" + level.ToString() : level.ToString();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Chapter " + chapter + "/Level " + levelToLoad);
    }

}
