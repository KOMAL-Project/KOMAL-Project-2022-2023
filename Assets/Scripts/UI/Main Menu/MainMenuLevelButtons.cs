using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButtons : MonoBehaviour
{
    private static Color normal = new Color(0.62f,0.62f,0.62f,1); //grey button, no lightup
    private static Color completed = new Color(0.9f,0.9f,0.9f,1); //lightup after completion
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

        /*
        if ((furthestLevel + 1 >= level && furthestChapter == chapter) || furthestChapter > chapter) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
        */
        button.interactable = true; //button for now will always be usable

        if (ManageGame.finishedLevels.Contains(ManageGame.IDsToString(level, chapter))) {
            GetComponent<Image>().color = completed;
        }

        button.onClick.AddListener(ChangeLevel);
    }

    private void ChangeLevel() {
        string levelToLoad = (level > 12) ? "b" + (level - 12).ToString() : level.ToString();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Chapter " + chapter + "/Level " + levelToLoad);
    }

}
