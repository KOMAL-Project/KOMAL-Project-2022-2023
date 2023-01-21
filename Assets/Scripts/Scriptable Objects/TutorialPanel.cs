using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialPanel", order = 1)]
public class TutorialPanel : ScriptableObject
{
    public string levelID;
    public Texture2D panelImage;    
}
