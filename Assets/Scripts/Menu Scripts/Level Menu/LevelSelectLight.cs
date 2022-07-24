using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectLight : MonoBehaviour
{
    [SerializeField] private int level;
    private UnityEngine.UI.Button button;
    private UnityEngine.UI.Image image;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        image = GetComponent<UnityEngine.UI.Image>();
        if (ManageGame.furthestLevel + 1 >= level) {
            button.enabled = true;
            image.color = new Color(0.818f, 0.818f, 0.818f, 1);
        }
        else {
            button.enabled = false;
            image.color = new Color(0.2f, 0.2f, 0.2f, 1);
        }

    }

    // Update is called once per frame
    
}
