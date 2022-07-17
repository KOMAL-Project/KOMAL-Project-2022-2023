using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenDiceMoves : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = ("Total Dice Moves: "+ DieController.totalDiceMoves);
    }

    // Update is called once per frame
    
}
